using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Ionic.Zip;
//Import HiqPdf
using HiQPdf;
using PPI.Core.Web.Models.AmsaReports;
//Zip files
using System.IO.Compression;
using PPI.Core.Web.Models.AmsaReports.ViewModel;


namespace PPI.Core.Web.Controllers
{
    using PPI.Core.Web.Models;
    using PPI.Core.Web.Infrastructure;
    using PPI.Core.Domain.Abstract;
    using PPI.Core.Domain.Concrete;
    using PPI.Core.Domain.Entities;
    using System.Security.AccessControl;
    using System.Net.Mail;
    using System.Net;


    public class ReportsController : BaseController
    {               
        [Log]
        public ReportsController(IUnitOfWork unitOfWork)
            : base(unitOfWork) { }        
        //
        // GET: /Reports/
        [Log]
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }        
        [Log]
        [Authorize(Roles = "Admin")]
        public ActionResult ManageText()
        {
            var model = new ManageTextViewModel();
            model.Programs = UnitOfWork.IProgramRepository.AsQueryable();
            model.Cultures = UnitOfWork.ICultureRepository.AsQueryable();
            model.CultureId = 1;
                        
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult SelectProgram(int? programId)
        {
            var reportslist = programId.HasValue ? UnitOfWork.IProgramPracticeReportRepository.AsQueryable().Where(m => m.ProgramId == programId) : null;
            List<PracticeReport> reports = new List<PracticeReport>();
            foreach (var item in reportslist)
            {
                reports.Add(item.PracticeReport);
            }

            var model = new ManageTextViewModel();
            //model.Programs = UnitOfWork.IProgramRepository.AsQueryable();
            model.CultureId = 1;
            model.ProgramId = programId.GetValueOrDefault(0);
            model.Reports = reports;
            return PartialView("_PartialPracticeReports", model);
            
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult SelectReport(int? reportId,int? programId,int? cultureId)
        {
            var scaleslist = reportId.HasValue ? UnitOfWork.IPracticeScaleReportRepository.AsQueryable().Where(m => m.PracticeReportId == reportId) : null;
            List<HoganField> scales = new List<HoganField>();
            foreach (var item in scaleslist)
            {
                if (!scales.Exists(m => m.Id == item.PracticeScale.HoganFieldId))
                    scales.Add(item.PracticeScale.HoganField);
            }
            
            var model = new ManageTextViewModel();
            model.ReportId = reportId.GetValueOrDefault(0);
            model.ProgramId = programId.GetValueOrDefault(0);            
            model.CultureId = cultureId.GetValueOrDefault(1); 
            model.Scales = scales;

            
                return PartialView("_PartialPracticeScales", model);
            
        }
        [HttpPost, ValidateInput(false)]        
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateText(ResxValue textUpdate)
        {
            UnitOfWork.IResxValueRepository.Update(textUpdate);
            UnitOfWork.Commit();
            return PartialView("_PartialManageResxValue", textUpdate);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ModifyText(int programId,int reportId,int scaleId, int cultureId)
        { 
            var model = new ManageTextViewModel();
            model.ProgramId = programId;
            model.ReportId = reportId;
            model.ScaleId = scaleId;
            model.CultureId = cultureId;
            var ReportTextList = new List<ReportTextModel>();
            //Get all the Scales for the Report
            var ReportData = UnitOfWork.IPracticeScaleReportRepository.Find(m => m.PracticeReportId == model.ReportId && m.PracticeScale.HoganFieldId == model.ScaleId);
            foreach (var item in ReportData)
            {
                ReportTextModel TextModel = new ReportTextModel();
                TextModel.Category = item.PracticeScale.PracticeCategory;
                TextModel.ReportScale = item;
                string egtext = item.PracticeText.TextResx.ResxValues.FirstOrDefault(m => m.Culture.Id == 1).Value; //Hard coded English
                TranslationText TranText = new Models.TranslationText();
                TranText.EnglishText = egtext;
                TranText.Text = item.PracticeText;                
                //Get intro statement pieces
                List<TranslationText> ChildText = new List<TranslationText>();
                if (item.PracticeText.IsIntroduction.GetValueOrDefault(false))
                {
                    foreach (var introitem in item.PracticeText.PracticeTextChildren)
                    {
                        TranslationText ChildTranText = new Models.TranslationText();
                        ChildTranText.Text = introitem;
                        ChildTranText.EnglishText = introitem.TextResx.ResxValues.FirstOrDefault(m => m.Culture.Id == 1).Value; //Hard coded English;
                        ChildText.Add(ChildTranText);
                    }
                }
                TranText.ChildText = ChildText;                
                TextModel.Text = TranText;
                List<TranslationText> AlternativeText = new List<TranslationText>();
                // Get the Alteratives
                foreach (var Altitem in item.PracticeText.PracticeTextOptions)
                {
                    TranslationText AltTranText = new Models.TranslationText();
                    if (Altitem.PracticeTextAlternative != null)
                    {
                        AltTranText.Text = Altitem.PracticeTextAlternative;
                        AltTranText.EnglishText = Altitem.PracticeTextAlternative.TextResx.ResxValues.FirstOrDefault(m => m.Culture.Id == 1).Value; //Hard coded English;                        

                        //Get intro statement pieces
                        List<TranslationText> ChildText2 = new List<TranslationText>();
                        if (Altitem.PracticeText.IsIntroduction.GetValueOrDefault(false))
                        {
                            foreach (var introitem2 in Altitem.PracticeTextAlternative.PracticeTextChildren)
                            {
                                TranslationText ChildTranText2 = new Models.TranslationText();
                                ChildTranText2.Text = introitem2;
                                ChildTranText2.EnglishText = introitem2.TextResx.ResxValues.FirstOrDefault(m => m.Culture.Id == 1).Value; //Hard coded English;
                                ChildText2.Add(ChildTranText2);
                            }
                        }
                        AltTranText.ChildText = ChildText2;
                        AlternativeText.Add(AltTranText);
                    }
                }
                
                TextModel.AlternativeText = AlternativeText;
                ReportTextList.Add(TextModel);
            }
            model.ReportText = ReportTextList;
            

           //var AvailibleCategories = 

            return View(model);
        }
        #region Reports        
        [Log]
        [Authorize(Roles = "Admin")]
        public ActionResult Reports()
        {
            return View();
        }
        [Log]
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        public ActionResult PreviewProfessionReport(string hoganId, int language, int report, int siteId, int programId,int eventId)
        {
            var model = new PracticeReportsViewModel(UnitOfWork.IUserPracticeTextRepository, hoganId, language, report);
            var PracticeReport = new PracticeReportModel();                      
            var Candidate = UnitOfWork.IPersonRepository.First(m => m.Hogan_Id == hoganId);            
            //var Report = UnitOfWork.IPracticeReportRepository.GetById(1, "en-US");
            var Report = UnitOfWork.IPracticeReportRepository.First(m => m.Id == report);
            var ReportSet = UnitOfWork.IUserPracticeTextRepository.GetUserPracticeText(hoganId, language, report);
            // TODO: refactor this mess ..
            // Need to do the replacements
            var Replacements = UnitOfWork.IReplacementExpressionRepository.AsQueryable();
            foreach (var item in ReportSet)
            {

                foreach (var replitem in Replacements)
                {
                    string pattern = replitem.FindValue;
                    
                        switch (pattern)
                        {
                            case "<Dr.>":
                                item.Text = Regex.Replace(item.Text, pattern, "Dr.");
                                break;
                            case "<His>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "His");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "Her");
                                break;
                            case "<his>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "his");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "her");
                                break;
                            case "<He>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "He");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "She");
                                break;
                            case "<he>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "he");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "she");
                                break;
                            case "<Himself>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "Himself");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "Herself");
                                break;
                            case "<himself>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "himself");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "herself");
                                break;
                            case "<him>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "him");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "her");
                                break;
                            case "<Him>":
                                if (Candidate.Gender == "Male")
                                    item.Text = Regex.Replace(item.Text, pattern, "Him");
                                else if (Candidate.Gender == "Female")
                                    item.Text = Regex.Replace(item.Text, pattern, "Her");
                                break;
                            case "<Name>":
                                item.Text = Regex.Replace(item.Text, pattern, Candidate.FirstName);
                                break;
                            case "<Title>":
                                if (string.IsNullOrEmpty(Candidate.Title))
                                {
                                    item.Text = Regex.Replace(item.Text, pattern, "Dr.");
                                }
                                else
                                {
                                    item.Text = Regex.Replace(item.Text, pattern, Candidate.Title);
                                }
                                
                                break;
                            case "<Lastname>":
                                item.Text = Regex.Replace(item.Text, pattern, Candidate.LastName);
                                break;
                            
                            
                            
                    }
                    
                }
                
                //string pattern = "(<Dr.>)";
                //var currentText = item.Text;
                //currentText = Regex.Replace(currentText, pattern, Replacements.FirstOrDefault().Expression);
                //item.Text = currentText;
                
            }


            var Site = UnitOfWork.ISiteRepository.First(h => h.Id == siteId);
                
            var ReportFor = Candidate.FirstName + " " + Candidate.LastName;

           

            PracticeReport.HoganId = hoganId;
            PracticeReport.ReportFor = ReportFor;                                   

            var jPersonicaLogo = "";

            if (Site.BrandingLogo != null)
            {
                PracticeReport.Color = Site.BrandingColor;
                PracticeReport.BackgroundMimeType = Site.BrandingBackgroundMimeType;
                PracticeReport.Background = Site.BrandingBackground;
                PracticeReport.Logo = Site.BrandingLogo;
                PracticeReport.LogoMimeType = Site.BrandingLogoMimeType;
                //RS LOGO DOTS'
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed_dots.png");               
            }
            else
            {
                PracticeReport.Color = Report.DefaultColor;
                PracticeReport.BackgroundMimeType = Report.DefaultBackgroundMimeType;
                PracticeReport.Background = Report.DefaultBackground;
                PracticeReport.Logo = Report.DefaultLogo;
                PracticeReport.LogoMimeType = Report.DefaultLogoMimeType;
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed.png");
            }

            var noTranslation = "No Translation Available";
            PracticeReport.Title = noTranslation;
            PracticeReport.Introduction = noTranslation;
            PracticeReport.IntroductionTwo = noTranslation;
            PracticeReport.IntroductionThree = noTranslation;

            if (Report.ReportTitleResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Title = Report.ReportTitleResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Introduction = Report.IntroductionResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionTwoResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionTwo = Report.IntroductionTwoResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionThreeResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionThree = Report.IntroductionThreeResx.ResxValues.First(rx => rx.CultureId == language).Value;

            

            List<PracticeReportModel> ReportBarandingData = new List<PracticeReportModel>();
            ReportBarandingData.Add(PracticeReport);
                
                
            
            
            model.localReport.EnableExternalImages = true;
            switch (report)
            {
                case 1:
                    model.FileName = "~/Reports/ProfessionPractice.rdlc";                    
                    break;
                case 2:
                    model.FileName = "~/Reports/TransistionPractice.rdlc";                    
                    break;
                case 3:
                    model.FileName = "~/Reports/ManagePractice.rdlc";                    
                    break;
                case 11:
                    model.FileName = "~/Reports/GraphPractice.rdlc";
                    break;
            }
            model.ReportTitle = Candidate.LastName.Replace(' ', '_') + "_" + Candidate.FirstName.Replace(' ', '_') + "_" + PracticeReport.Title.Replace(' ', '_') + "_" + hoganId;           
            model.localReport.DataSources.Add(new ReportDataSource("PracticeReports", ReportSet));
            model.localReport.DataSources.Add(new ReportDataSource("PracticeReportBranding", ReportBarandingData));
            model.Format = Models.Base.ReportViewModel.ReportFormat.PDF;
            model.localReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath(model.FileName);            
            model.ViewAsAttachment = true;
            model.localReport.SetParameters(new ReportParameter("jPersonicaLogo", jPersonicaLogo));
            
                       
            
            var renderbytes = model.RenderReport();
            
            //Record the Report Request
            PersonPracticeReport ReportRequest = new PersonPracticeReport();
            ReportRequest.AspNetUsersId = User.Identity.GetUserId();
            ReportRequest.PersonId = Candidate.Id;
            ReportRequest.PracticeReportId = report;
            ReportRequest.RunDate = DateTime.Now;
            ReportRequest.EventId = eventId;
            //ReportRequest.EventId = 
            UnitOfWork.IPersonPracticeReportRepository.Add(ReportRequest);
            UnitOfWork.Commit();
            // ------------------            
                return File(renderbytes, model.LastmimeType, model.ReportExportFileName);                            
        }
        [Log]
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        public ActionResult PreviewMatchReport(string hoganId, int language, int report, int siteId, int programId, int eventId)
        {

            var PracticeReport = new PracticeReportModel();
            var Candidate = UnitOfWork.IPersonRepository.First(m => m.Hogan_Id == hoganId);
            var Report = UnitOfWork.IPracticeReportRepository.First(m => m.Id == report);
            var Site = UnitOfWork.ISiteRepository.First(h => h.Id == siteId);
            var programID = programId;
            var model = new PracticeMatchReportsViewModel(UnitOfWork.IUserPracticeCategoryTextRepository, hoganId, language, report, programID);
            var ReportSet = UnitOfWork.IUserPracticeCategoryTextRepository.GetUserPracticeCategoryText(hoganId, language, report, programID);
            var ReportFor = Candidate.FirstName + " " + Candidate.LastName;
            var ValidRating = UnitOfWork.IManual_Hogan_ImportRepository.AsQueryable().FirstOrDefault(m => m.Hogan_User_ID == hoganId).Valid;



            PracticeReport.HoganId = hoganId;
            PracticeReport.ReportFor = ReportFor;

            var jPersonicaLogo = "";

            if (Site.BrandingLogo != null)
            {
                PracticeReport.Color = Site.BrandingColor;
                PracticeReport.BackgroundMimeType = Site.BrandingBackgroundMimeType;
                PracticeReport.Background = Site.BrandingBackground;
                PracticeReport.Logo = Site.BrandingLogo;
                PracticeReport.LogoMimeType = Site.BrandingLogoMimeType;
                //RS LOGO DOTS'
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed_dots.png");

            }
            else
            {
                PracticeReport.Color = Report.DefaultColor;
                PracticeReport.BackgroundMimeType = Report.DefaultBackgroundMimeType;
                PracticeReport.Background = Report.DefaultBackground;
                PracticeReport.Logo = Report.DefaultLogo;
                PracticeReport.LogoMimeType = Report.DefaultLogoMimeType;
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed.png");
            }

            var noTranslation = "No Translation Available";
            PracticeReport.Title = noTranslation;
            PracticeReport.Introduction = noTranslation;
            PracticeReport.IntroductionTwo = noTranslation;
            PracticeReport.IntroductionThree = noTranslation;


            //TODO: FIX THIS DUPLICATION
            if (Report.ReportTitleResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Title = Report.ReportTitleResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Introduction = Report.IntroductionResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionTwoResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionTwo = Report.IntroductionTwoResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionThreeResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionThree = Report.IntroductionThreeResx.ResxValues.First(rx => rx.CultureId == language).Value;



            List<PracticeReportModel> ReportBarandingData = new List<PracticeReportModel>();
            ReportBarandingData.Add(PracticeReport);




            model.localReport.EnableExternalImages = true;

            //TODO need to fix these two hard codes some how?  Look from db first i guess
            if (report == 8)
            {
                model.FileName = "~/Reports/RangeReport.rdlc";
            }
            else if (report == 7)
            {
                model.FileName = "~/Reports/MatchReport.rdlc";
            }
            else if (report == 9)
            {
                model.FileName = "~/Reports/CoachReport.rdlc";
            }
            else if (report == 10)
            {
                model.FileName = "~/Reports/CoachReport.rdlc";
            }
            else if (report == 12)
            {
                model.FileName = "~/Reports/ScalesReport.rdlc";
            }
            else if (report == 13)
            {
                model.FileName = "~/Reports/ScalesReport.rdlc";
            }
            else if (report == 14)
            {
                model.FileName = "~/Reports/ScalesReport.rdlc";
            }
            //model.ReportTitle = hoganId + "_" + Candidate.FirstName.Replace(' ', '_') + "_" + Candidate.LastName.Replace(' ', '_') + "_" + PracticeReport.Title.Replace(' ', '_');
            model.ReportTitle = Candidate.LastName.Replace(' ', '_') + "_" + Candidate.FirstName.Replace(' ', '_') + "_" + PracticeReport.Title.Replace(' ', '_') + "_" + hoganId;
            model.localReport.DataSources.Add(new ReportDataSource("MatchPracticeReports", ReportSet));
            model.localReport.DataSources.Add(new ReportDataSource("PracticeReportBranding", ReportBarandingData));
            model.Format = Models.Base.ReportViewModel.ReportFormat.PDF;
            model.localReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath(model.FileName);
            model.ViewAsAttachment = true;
            //TODO need to fix these two hard codes some how?  Look from db first i guess
            int CountOFIN = ReportSet.Count(m => m.CategoryId == 1002 || m.CategoryId == 1003);
            int TotalCount = ReportSet.Count(m => m != null);
            model.localReport.SetParameters(new ReportParameter("CountOFIN", CountOFIN.ToString()));
            model.localReport.SetParameters(new ReportParameter("TotalCount", TotalCount.ToString()));
            model.localReport.SetParameters(new ReportParameter("jPersonicaLogo", jPersonicaLogo));
            if (report == 7)
            {

                string AssessmentValid = "Assessment Not Valid";
                if (ValidRating.HasValue)
                {
                    if (ValidRating.Value >= 10)
                        AssessmentValid = "Assessment Valid";
                }
                model.localReport.SetParameters(new ReportParameter("Valid", AssessmentValid));
            }

            var renderbytes = model.RenderReport();
            //Record the Report Request

            PersonPracticeReport ReportRequest = new PersonPracticeReport();
            ReportRequest.AspNetUsersId = User.Identity.GetUserId();
            ReportRequest.PersonId = Candidate.Id;
            ReportRequest.PracticeReportId = report;
            ReportRequest.RunDate = DateTime.Now;
            ReportRequest.EventId = eventId;
            UnitOfWork.IPersonPracticeReportRepository.Add(ReportRequest);
            UnitOfWork.Commit();
            // ------------------            
                return File(renderbytes, model.LastmimeType, model.ReportExportFileName);
            
        }
        [Log]
        [Authorize(Roles = "Admin")]
        public ActionResult UploadFiles()
        {
            int i = 0;
            
                var r = new List<UploadFileModel>();
                string savedFileName = "";
                int ErrorCount = 0;
                int SuccessCount = 0;
                foreach (string key in Request.Files)
                {
                   ErrorCount = 0;
                   SuccessCount = 0;
                
                    HttpPostedFileBase hpf = Request.Files[i] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;
                    savedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.UploadFilesFolderName, Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName);

                   
                    FileInfo fileInfo = new FileInfo(savedFileName);
                    if (fileInfo.Exists)
                    {
                        using (var reader = new CsvHelper.CsvReader(new StreamReader(savedFileName)))
                        {
                            reader.Configuration.RegisterClassMap<PPI.Core.Web.Infrastructure.ImportMaps.Hogan_File_Map>();
                            reader.Configuration.IgnoreReadingExceptions = true;
                            reader.Configuration.ReadingExceptionCallback = (exception, row) =>
                                {
                                    Trace.TraceInformation("Row Exception {0}: Row Data {1}", exception.Message, row.Parser.RawRecord);
                                    ErrorCount++;
                                };
                            var records = reader.GetRecords<PPI.Core.Domain.Entities.Manual_Hogan_Import>();
                            
                            //Should be an Add / Update base on Hogin ID
                            foreach (var item in records)
	                        {
                                var Hogan = UnitOfWork.IManual_Hogan_ImportRepository.AsQueryable().FirstOrDefault(m => m.Hogan_User_ID == item.Hogan_User_ID);
                                if (Hogan == null)
                                {
                                    item.LastUpdated = DateTime.Now;
                                    UnitOfWork.IManual_Hogan_ImportRepository.Add(item);
                                }
                                else
                                {
                                    item.LastUpdated = DateTime.Now;                                                                        
                                    UnitOfWork.IManual_Hogan_ImportRepository.Update(Utility.Update(Hogan, item));
                                }
                                SuccessCount++;
                                UnitOfWork.Commit();
                            }
                            
                            
                            
                        }
                    }
                     r.Add(new UploadFileModel()
                    {
                        Name = savedFileName,
                        Length = hpf.ContentLength,
                        FailedCount = ErrorCount,
                        SuccessfullCount = SuccessCount
                    });
                    i++;
                }
                
                return View("UploadFiles", r);
          
        }
        [Log]
        [Authorize(Roles = "Admin")]
        [ActionName("DeleteAll")]
        public ActionResult DeleteReports()
        {
            var concreate = UnitOfWork.IManual_Hogan_ImportRepository.AsQueryable();
            UnitOfWork.IManual_Hogan_ImportRepository.Delete(concreate);
            UnitOfWork.Commit();
            return View("Delete");
        }
        #endregion        
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        public ActionResult GetReports(int? eventId, int? page)
        {
            var model = new GetReportViewModel();
            SelectList Events = null;
            Events = this.Events;            
            int totalRecords = 0;
            int? FirstEventId = this.CurrentEvent;
            if (Events.Count() > 0)
            {
                if (eventId.HasValue)
                {
                    FirstEventId = eventId.Value;
                    this.CurrentEvent = FirstEventId.Value;
                }
            }
            model.EventId = FirstEventId;            
            model.Reports = UnitOfWork.IEventPracticeReportRepository.AsQueryable().Where(m => m.EventId == FirstEventId);
            
            totalRecords = UnitOfWork.IPersonEventRepository.AsQueryable().Where(m => m.EventId == FirstEventId).Select(t => t.Person).Count();
            PagingInfo pagingInfo = new PagingInfo { CurrentPage = page.GetValueOrDefault(1), PageCount = 5, TotalRecords = totalRecords };
            pagingInfo.PageSize = int.Parse(PPI.Core.Web.Infrastructure.Utility.GetCookie("pageSize"));
            pagingInfo.LastPage = totalRecords / pagingInfo.PageSize;
            model.PagingInfo = pagingInfo;                                

            
            model.Participants = UnitOfWork.IPersonEventRepository.AsQueryable().Where(m => m.EventId == FirstEventId)
                .Select(t => t.Person)
                .ToList().OrderBy(s => s.LastName)
                .Skip((pagingInfo.CurrentPage - 1) * pagingInfo.PageSize)
                .Take(pagingInfo.PageSize);

            model.PeopleAvailibleReports = new List<PeopleAvailibleReports>();
            foreach (var item in model.Participants)
            {
                var newList = new PeopleAvailibleReports();
                newList.PersonId = item.Id;
                newList.ReportDataAvailible = PPI.Core.Web.Infrastructure.Utility.ReportDataAvailible(item.Hogan_Id, UnitOfWork);
                model.PeopleAvailibleReports.Add(newList);
            }

            ViewBag.EventId = new SelectList(Events, "Value", "Text", model.EventId);

            return View(model);
        }
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        [HttpPost]
        public ActionResult GetReports(GetReportsPostViewModel model)
        {            
            var CurrentEvent = UnitOfWork.IEventRepository.AsQueryable().FirstOrDefault(m => m.Id == model.EventId);
            if (CurrentEvent == null)
                return new HttpNotFoundResult();
            var uniqueFolder = Guid.NewGuid().ToString();
            var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.UploadFilesFolderName,uniqueFolder);
            System.IO.DirectoryInfo di = new DirectoryInfo(FilePath);
            di.Create();
            var zipFIleName = FilePath + "\\" + uniqueFolder + ".zip";
            var zip = new ZipFile(zipFIleName);
            zip.TempFileFolder = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var item in model.SelectedReports)
                {
                
                    if (!item.Equals("on"))
                    {
                        var userId = int.Parse(item.Split('_')[1].ToString());
                        var reportId = int.Parse(item.Split('_')[0].ToString());
                        var CurrentUser = UnitOfWork.IPersonRepository.AsQueryable().FirstOrDefault(m => m.Id == userId);
                        var CurrentReport = UnitOfWork.IPracticeReportRepository.AsQueryable().FirstOrDefault(m => m.Id == reportId);
                        if (CurrentReport.PracticeGroup == "Match")
                        {
                            DownloadMatchReport(CurrentUser.Hogan_Id, CurrentCulture, CurrentReport.Id, CurrentEvent.ProgramSite.SiteId, CurrentEvent.ProgramSite.ProgramId, CurrentEvent.Id, uniqueFolder);
                        }
                        else
                        {
                            DownloadProfessionReport(CurrentUser.Hogan_Id, CurrentCulture, CurrentReport.Id, CurrentEvent.ProgramSite.SiteId, CurrentEvent.ProgramSite.ProgramId, CurrentEvent.Id, uniqueFolder);
                        }              
                    }
                    
                }
                zip.AddSelectedFiles("*.*", FilePath,"",false);
                zip.Save();                
                return File(zipFIleName, "application/zip", uniqueFolder + ".zip");
        }
        public ActionResult ReportEventSwitch(int eventId)
        {
            return RedirectToAction("GetReports", new { eventId = eventId });
        }
        public string DownloadProfessionReport(string hoganId, int language, int report, int siteId, int programId, int eventId, string uniqueFolder)
        {
            var model = new PracticeReportsViewModel(UnitOfWork.IUserPracticeTextRepository, hoganId, language, report);
            var PracticeReport = new PracticeReportModel();
            var Candidate = UnitOfWork.IPersonRepository.First(m => m.Hogan_Id == hoganId);
            //var Report = UnitOfWork.IPracticeReportRepository.GetById(1, "en-US");
            var Report = UnitOfWork.IPracticeReportRepository.First(m => m.Id == report);
            var ReportSet = UnitOfWork.IUserPracticeTextRepository.GetUserPracticeText(hoganId, language, report);
            // TODO: refactor this mess ..
            // Need to do the replacements
            var Replacements = UnitOfWork.IReplacementExpressionRepository.AsQueryable();
            foreach (var item in ReportSet)
            {

                foreach (var replitem in Replacements)
                {
                    string pattern = replitem.FindValue;

                    switch (pattern)
                    {
                        case "<Dr.>":
                            item.Text = Regex.Replace(item.Text, pattern, "Dr.");
                            break;
                        case "<His>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "His");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "Her");
                            break;
                        case "<his>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "his");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "her");
                            break;
                        case "<He>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "He");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "She");
                            break;
                        case "<he>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "he");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "she");
                            break;
                        case "<Himself>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "Himself");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "Herself");
                            break;
                        case "<himself>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "himself");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "herself");
                            break;
                        case "<him>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "him");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "her");
                            break;
                        case "<Him>":
                            if (Candidate.Gender == "Male")
                                item.Text = Regex.Replace(item.Text, pattern, "Him");
                            else if (Candidate.Gender == "Female")
                                item.Text = Regex.Replace(item.Text, pattern, "Her");
                            break;
                        case "<Name>":
                            item.Text = Regex.Replace(item.Text, pattern, Candidate.FirstName);
                            break;
                        case "<Title>":
                            if (string.IsNullOrEmpty(Candidate.Title))
                            {
                                item.Text = Regex.Replace(item.Text, pattern, "Dr.");
                            }
                            else
                            {
                                item.Text = Regex.Replace(item.Text, pattern, Candidate.Title);
                            }

                            break;
                        case "<Lastname>":
                            item.Text = Regex.Replace(item.Text, pattern, Candidate.LastName);
                            break;



                    }

                }

                //string pattern = "(<Dr.>)";
                //var currentText = item.Text;
                //currentText = Regex.Replace(currentText, pattern, Replacements.FirstOrDefault().Expression);
                //item.Text = currentText;

            }

            //if siteId = -1 were going to use default branding on the report and not Any Site Branding
            Site Site;
            if (siteId == -1)
            {
                Site = new Site();
                Site.BrandingLogo = null;
            }
            else
            {
                Site = UnitOfWork.ISiteRepository.First(h => h.Id == siteId);
            }
            
            var ReportFor = Candidate.FirstName + " " + Candidate.LastName;

            PracticeReport.HoganId = hoganId;
            PracticeReport.ReportFor = ReportFor;

            var jPersonicaLogo = "";

            if (Site.BrandingLogo != null)
            {
                PracticeReport.Color = Site.BrandingColor;
                PracticeReport.BackgroundMimeType = Site.BrandingBackgroundMimeType;
                PracticeReport.Background = Site.BrandingBackground;
                PracticeReport.Logo = Site.BrandingLogo;
                PracticeReport.LogoMimeType = Site.BrandingLogoMimeType;
                //RS LOGO DOTS'
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed_dots.png");
            }
            else
            {
                PracticeReport.Color = Report.DefaultColor;
                PracticeReport.BackgroundMimeType = Report.DefaultBackgroundMimeType;
                PracticeReport.Background = Report.DefaultBackground;
                PracticeReport.Logo = Report.DefaultLogo;
                PracticeReport.LogoMimeType = Report.DefaultLogoMimeType;
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed.png");
            }

            var noTranslation = "No Translation Available";
            PracticeReport.Title = noTranslation;
            PracticeReport.Introduction = noTranslation;
            PracticeReport.IntroductionTwo = noTranslation;
            PracticeReport.IntroductionThree = noTranslation;

            if (Report.ReportTitleResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Title = Report.ReportTitleResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Introduction = Report.IntroductionResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionTwoResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionTwo = Report.IntroductionTwoResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionThreeResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionThree = Report.IntroductionThreeResx.ResxValues.First(rx => rx.CultureId == language).Value;



            List<PracticeReportModel> ReportBarandingData = new List<PracticeReportModel>();
            ReportBarandingData.Add(PracticeReport);




            model.localReport.EnableExternalImages = true;
            switch (report)
            {
                case 1:
                    model.FileName = "~/Reports/ProfessionPractice.rdlc";
                    break;
                case 2:
                    model.FileName = "~/Reports/TransistionPractice.rdlc";
                    break;
                case 3:
                    model.FileName = "~/Reports/ManagePractice.rdlc";
                    break;
                case 11:
                    model.FileName = "~/Reports/GraphPractice.rdlc";
                    break;
            }
            //model.ReportTitle = hoganId + "_" + Candidate.FirstName.Replace(' ', '_') + "_" + Candidate.LastName.Replace(' ', '_') + "_" + PracticeReport.Title.Replace(' ', '_');
            model.ReportTitle = Candidate.LastName.Replace(' ', '_') + "_" + Candidate.FirstName.Replace(' ', '_') + "_" + PracticeReport.Title.Replace(' ', '_') + "_" + hoganId;           
            model.localReport.DataSources.Add(new ReportDataSource("PracticeReports", ReportSet));
            model.localReport.DataSources.Add(new ReportDataSource("PracticeReportBranding", ReportBarandingData));
            model.Format = Models.Base.ReportViewModel.ReportFormat.PDF;
            model.localReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath(model.FileName);
            model.ViewAsAttachment = true;
            model.localReport.SetParameters(new ReportParameter("jPersonicaLogo", jPersonicaLogo));



            var renderbytes = model.RenderReport();

            //Record the Report Request
            PersonPracticeReport ReportRequest = new PersonPracticeReport();
            ReportRequest.AspNetUsersId = User.Identity.GetUserId();
            ReportRequest.PersonId = Candidate.Id;
            ReportRequest.PracticeReportId = report;
            ReportRequest.RunDate = DateTime.Now;
            ReportRequest.EventId = eventId;
            //ReportRequest.EventId = 
            UnitOfWork.IPersonPracticeReportRepository.Add(ReportRequest);
            UnitOfWork.Commit();
            // ------------------          
            var savedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.UploadFilesFolderName, uniqueFolder);
            System.IO.Directory.CreateDirectory(savedFileName);
            savedFileName = savedFileName + "\\" + model.ReportTitle + ".pdf";
            System.IO.File.WriteAllBytes(savedFileName, renderbytes);

            return savedFileName;
        }
        public string DownloadMatchReport(string hoganId, int language, int report, int siteId, int programId, int eventId, string uniqueFolder)
        {

            var PracticeReport = new PracticeReportModel();
            var Candidate = UnitOfWork.IPersonRepository.First(m => m.Hogan_Id == hoganId);
            var Report = UnitOfWork.IPracticeReportRepository.First(m => m.Id == report);            
            var programID = programId;
            var model = new PracticeMatchReportsViewModel(UnitOfWork.IUserPracticeCategoryTextRepository, hoganId, language, report, programID);
            var ReportSet = UnitOfWork.IUserPracticeCategoryTextRepository.GetUserPracticeCategoryText(hoganId, language, report, programID);
            var ReportFor = Candidate.FirstName + " " + Candidate.LastName;
            var ValidRating = UnitOfWork.IManual_Hogan_ImportRepository.AsQueryable().FirstOrDefault(m => m.Hogan_User_ID == hoganId).Valid;



            PracticeReport.HoganId = hoganId;
            PracticeReport.ReportFor = ReportFor;

            var jPersonicaLogo = "";

            Site Site;
            if (siteId == -1)
            {
                Site = new Site();
                Site.BrandingLogo = null;
            }
            else
            {
                Site = UnitOfWork.ISiteRepository.First(h => h.Id == siteId);
            }

            if (Site.BrandingLogo != null)
            {
                PracticeReport.Color = Site.BrandingColor;
                PracticeReport.BackgroundMimeType = Site.BrandingBackgroundMimeType;
                PracticeReport.Background = Site.BrandingBackground;
                PracticeReport.Logo = Site.BrandingLogo;
                PracticeReport.LogoMimeType = Site.BrandingLogoMimeType;
                //RS LOGO DOTS'
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed_dots.png");

            }
            else
            {
                PracticeReport.Color = Report.DefaultColor;
                PracticeReport.BackgroundMimeType = Report.DefaultBackgroundMimeType;
                PracticeReport.Background = Report.DefaultBackground;
                PracticeReport.Logo = Report.DefaultLogo;
                PracticeReport.LogoMimeType = Report.DefaultLogoMimeType;
                jPersonicaLogo = Server.MapPath("~\\Reports\\images\\j3p_Logo_Distributed.png");
            }

            var noTranslation = "No Translation Available";
            PracticeReport.Title = noTranslation;
            PracticeReport.Introduction = noTranslation;
            PracticeReport.IntroductionTwo = noTranslation;
            PracticeReport.IntroductionThree = noTranslation;


            //TODO: FIX THIS DUPLICATION
            if (Report.ReportTitleResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Title = Report.ReportTitleResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.Introduction = Report.IntroductionResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionTwoResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionTwo = Report.IntroductionTwoResx.ResxValues.First(rx => rx.CultureId == language).Value;
            if (Report.IntroductionThreeResx.ResxValues.FirstOrDefault(rx => rx.CultureId == language) != null)
                PracticeReport.IntroductionThree = Report.IntroductionThreeResx.ResxValues.First(rx => rx.CultureId == language).Value;



            List<PracticeReportModel> ReportBarandingData = new List<PracticeReportModel>();
            ReportBarandingData.Add(PracticeReport);




            model.localReport.EnableExternalImages = true;

            //TODO need to fix these two hard codes some how?  Look from db first i guess
            if (report == 8)
            {
                model.FileName = "~/Reports/RangeReport.rdlc";
            }
            else if (report == 7)
            {
                model.FileName = "~/Reports/MatchReport.rdlc";                
            }
            else if (report == 9)
            {
                model.FileName = "~/Reports/CoachReport.rdlc";
            }
            else if (report == 10)
            {
                model.FileName = "~/Reports/CoachReport.rdlc";
            }
            else if (report == 12)
            {
                model.FileName = "~/Reports/ScalesReport.rdlc";
            }
            else if (report == 13)
            {
                model.FileName = "~/Reports/ScalesReport.rdlc";
            }
            else if (report == 14)
            {
                model.FileName = "~/Reports/ScalesReport.rdlc";
            }
            //model.ReportTitle = hoganId + "_" + Candidate.FirstName.Replace(' ', '_') + "_" + Candidate.LastName.Replace(' ', '_') + "_" + PracticeReport.Title.Replace(' ', '_');
            model.ReportTitle = Candidate.LastName.Replace(' ', '_') + "_" + Candidate.FirstName.Replace(' ', '_') + "_" + PracticeReport.Title.Replace(' ', '_') + "_" + hoganId;
            model.localReport.DataSources.Add(new ReportDataSource("MatchPracticeReports", ReportSet));
            model.localReport.DataSources.Add(new ReportDataSource("PracticeReportBranding", ReportBarandingData));
            model.Format = Models.Base.ReportViewModel.ReportFormat.PDF;
            model.localReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath(model.FileName);
            model.ViewAsAttachment = true;
            //TODO need to fix these two hard codes some how?  Look from db first i guess
            int CountOFIN = ReportSet.Count(m => m.CategoryId == 1002 || m.CategoryId == 1003);
            int TotalCount = ReportSet.Count(m => m != null);
            model.localReport.SetParameters(new ReportParameter("CountOFIN", CountOFIN.ToString()));
            model.localReport.SetParameters(new ReportParameter("TotalCount", TotalCount.ToString()));
            model.localReport.SetParameters(new ReportParameter("jPersonicaLogo", jPersonicaLogo));
            if (report == 7)
            {
                string AssessmentValid = "Assessment Not Valid";
                if (ValidRating.HasValue)
                {
                    if (ValidRating.Value >= 10)
                        AssessmentValid = "Assessment Valid";
                }
                model.localReport.SetParameters(new ReportParameter("Valid", AssessmentValid));
            }

            var renderbytes = model.RenderReport();
            //Record the Report Request

            PersonPracticeReport ReportRequest = new PersonPracticeReport();
            ReportRequest.AspNetUsersId = User.Identity.GetUserId();
            ReportRequest.PersonId = Candidate.Id;
            ReportRequest.PracticeReportId = report;
            ReportRequest.RunDate = DateTime.Now;
            ReportRequest.EventId = eventId;
            UnitOfWork.IPersonPracticeReportRepository.Add(ReportRequest);
            UnitOfWork.Commit();
            // ------------------            
            var savedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.UploadFilesFolderName, uniqueFolder, model.ReportTitle + ".pdf");
            System.IO.File.WriteAllBytes(savedFileName, renderbytes);
            return savedFileName;
        }
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        [HttpGet]
        public ActionResult AMSAReportToPDF(ViewDataDictionary data)
        {
            UserAnswersViewModel userWAnswers = (UserAnswersViewModel)data.Model;
            return View(userWAnswers);
        }
        //Used to check design for pdf report generation, comment out before deploy
        [HttpGet]
        public ActionResult AMSAReportToPDFTestDesign()
        {
            AMSAReportContext db = new AMSAReportContext();
            //Instead of getting all users we will only get the ints that are passed through
            //When needed to buid reports only for the users they send through
            List<AmsaReportStudentData> lstStudents = db.lstStudentsForReport.ToList();
            AmsaReportsItems items = new AmsaReportsItems();
            List<string> lstFiles = new List<string>();
            UserAnswersViewModel participantWAnswers = new UserAnswersViewModel(lstStudents[0], items);
            return View(participantWAnswers);
        }

        
        
        //Getting reports for users and triggering download to pdf
        //Getting reports for users and triggering download to pdf
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        public void getPdfReports(List<int> lstParticipantIds)
        {
            //Get e-mail of the user that is performing the task right away, just in case 
            //he or she decides to close the application and we can't reach this data later on


            //Get all users for reports
            ApplicationDbContext db = new ApplicationDbContext();

            string to = System.Web.HttpContext.Current.User.Identity.Name;
            to = db.Users.Where(m => m.UserName == to).FirstOrDefault().Email;


            if (lstParticipantIds.Count > 0) { 
                var FilePath = Server.MapPath("~/Reports");
                string timeStamp = DateTime.Now.ToString("yyyyMMdd");
                Guid fileName = Guid.NewGuid();
                string fName = fileName.ToString() + timeStamp;

                var FilePathZip = System.IO.Path.Combine(FilePath, fName + ".zip");
                ZipFile _Zip = new ZipFile(FilePathZip);
           
                //Find users that are selected for report generation
                List<AmsaReportStudentData> lstStudents = new List<AmsaReportStudentData>();
                //Loop through users and generate the final list (remove non selected users from the list)
                lstStudents = getFinalListOfParticipantsForReport(lstParticipantIds);

                AmsaReportsItems items = new AmsaReportsItems();
                List<string> lstFiles = new List<string>();

                List<FileResult> lstResult = new List<FileResult>();
                //Get Report for only 1 studen for now            
                Guid g;
                foreach (AmsaReportStudentData s in lstStudents)
                {

                    UserAnswersViewModel participantWAnswers = new UserAnswersViewModel(s, items);
                    ViewDataDictionary sendData = new ViewDataDictionary();
                    sendData.Model = participantWAnswers;
                    // get the About view HTML code
                    string htmlToConvert = RenderViewAsString("AMSAReportToPDF", sendData);
                
                    // the base URL to resolve relative images and css
                    String thisViewUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                    String baseUrl = thisViewUrl;

                    // instantiate the HiQPdf HTML to PDF converter
                    HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
                    //Set to the highest quality of images possible
                    htmlToPdfConverter.Document.ImagesCompression = 0;
                    htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
                    htmlToPdfConverter.Document.PageSize = PdfPageSize.Letter;
                    htmlToPdfConverter.SerialNumber = @"35e2jo+7-uZO2va2+-rabn8e//-7v/s/+bq-6P/s7vHu-7fHm5ubm";
                    // render the HTML code as PDF in memory
                    byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

                    // send the PDF document to browser
                    FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                    g = Guid.NewGuid();
                    string gN = g.ToString();
                    string name = s.LastName + "-" + s.FirstName + "-AmsaReport-" + s.PersonId + "--" + gN + ".pdf";
                    //fileResult.FileDownloadName = name;
                    //Store the file to folder on disc
                    MemoryStream ms = new MemoryStream();
                    System.IO.File.WriteAllBytes(FilePath + "/" + name, pdfBuffer);
                    _Zip.AddFile(FilePath + "/" + name, "");
                    lstFiles.Add(FilePath + "/" + name);
                }
                _Zip.Save();
                //Delete pdfs outside the zip file (doing it this way so we dont use up all the memory if a lot of pdfs are happening from different)
                //users at the same time
                foreach (string s in lstFiles)
                {
                    System.IO.File.Delete(s);
                }

                string fileNameEmail = "";

                fileNameEmail = this.uri(FilePathZip);
                //string requestUrl = this.uri(Request.Url.ToString());
                string requestUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);

               
                //After pdf is created we send over the e-mail

                var emailmessage = new System.Net.Mail.MailMessage();
                emailmessage.From = new System.Net.Mail.MailAddress("noreply@j3personica.com");
                emailmessage.Subject = "Your AMSA Reports are ready";
                emailmessage.IsBodyHtml = true;
                var filename = fileName.ToString();
                System.Text.StringBuilder body = new System.Text.StringBuilder();
                body.Append("<p>Your AMSA reports have been generated successfully!</p>");
                body.Append("<p>Please click the 'Requested Reports' link to view and download the reports.</p>");
                body.Append("<p><a href='" + requestUrl + "/Reports/" + fileNameEmail + "'>Requested Reports</a></p>");
                emailmessage.Body = body.ToString();
                //MailClass.SendEmail(emailmessage.Subject, emailmessage.Body, "noreply@j3personica.com", "nicocava92@live.com");


                //Send Grid example code
                var Credentials = new NetworkCredential(
                        PPI.Core.Web.Properties.Settings.Default.SMTPUSER,
                        PPI.Core.Web.Properties.Settings.Default.SMTPPASSWORD
                        );

                var transportWeb = new SendGrid.Web(Credentials);

                var Mail = new SendGrid.SendGridMessage();

                MailAddress from = new MailAddress("noreply@j3personica.com");
           
                Mail.AddTo(to);
                Mail.From = from;


                Mail.Subject = emailmessage.Subject;
                Mail.Html = emailmessage.Body;
                try { 
                //We use a try catch here just in case the user doesn't have an e-mail in the system, making sure 
                //reports are still completely generated and app continues working correctly even tho this issue is present.
                transportWeb.Deliver(Mail);
                }
                catch
                {
                    Console.WriteLine("Error producing message to that needs to be sent");
                }
            }
        }

        //Receives list of students and returns them
        private List<AmsaReportStudentData> getFinalListOfParticipantsForReport(List<int> lstParticipantIds)
        {
            List<AmsaReportStudentData> lstS = new List<AmsaReportStudentData>();
            AMSAReportContext dbr = new AMSAReportContext();
            foreach(int i in lstParticipantIds)
            {
                AmsaReportStudentData s = dbr.lstStudentsForReport.Find(i);
                lstS.Add(s);
            }
            //Order students by lastname for reports to be ordered alphabetifcally
            lstS.OrderBy(m => m.LastName);
            dbr.Dispose();
            return lstS;
        }
        

        public string uri(string s)
        {
            string result = "";
            Uri uri = new Uri(s);
            if (uri.IsFile)
            {
                result = System.IO.Path.GetFileName(uri.LocalPath);
            }
            return result;
        }

        public string RenderViewAsString(string viewName, ViewDataDictionary viewData)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();

            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(),
                    stringWriter
                    );

            // render the view to a HTML code
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }

        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        [HttpGet]
        public ActionResult CreateAmsaStudents()
        {
            return View(new AMSAParticipantStudentDataViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAmsaStudents(AMSAParticipantStudentDataViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                //If all is good then save the model to the database
                pvm.SaveNew();
                return RedirectToAction("IndexAmsaStudents");
            }

            return View(pvm);
            
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        public ActionResult EditAmsaStudents(int id) {
            AMSAParticipantStudentDataViewModel svm = new AMSAParticipantStudentDataViewModel();
            svm.loadStudentData(id);
            return View(svm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditAmsaStudents(AMSAParticipantStudentDataViewModel svm)
        {
            if (ModelState.IsValid)
            {
                svm.updateStudentData();
                return RedirectToAction("IndexAmsaStudents");
            }

            return View(svm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SiteCordinator,J3PAdmin")]
        public ActionResult IndexAmsaStudents()
        {
            return View(new ReportStudentDataListViewModel());
        }

        //Return participant list by the id that is selected
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetParticipantsByEvent(ReportStudentDataListViewModel pvm)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            try
            {
                List<AmsaReportStudentData> participantsInEvent = new List<AmsaReportStudentData>();
                //if the id != to 0 then filter
                if (pvm.idSelectedEvent != 0)
                {
                    //Get the listing of information that needs to be shown on the view
                    participantsInEvent = dbr.lstStudentsForReport.Where(r => r.AMSAEvent.id == pvm.idSelectedEvent && (r.Status.ToUpper().Equals("COMPLETED") || r.Status.ToUpper().Equals("COMPLETED PASS") ||
                    r.Status.ToUpper().Equals("COMPLETE") || r.Status.ToUpper().Equals("COMPLETE PASS")
                    )).ToList();
                }
                else
                {
                    //If it is == 0 then what we are need to show is all of the participants
                    participantsInEvent = dbr.lstStudentsForReport.Where(r => (r.Status.ToUpper().Equals("COMPLETED") || r.Status.ToUpper().Equals("COMPLETED PASS") ||
                    r.Status.ToUpper().Equals("COMPLETE") || r.Status.ToUpper().Equals("COMPLETE PASS")
                    )).ToList();
                }
                pvm.LstStudentData = participantsInEvent;
                return View("IndexAmsaStudents", pvm);
            }
            catch
            {
                return View("IndexAmsaStudents", pvm);
            }
        }

        //Used to get list from Dashboard
        public ActionResult GetParticipantsByEvent(int? eventId)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            ReportStudentDataListViewModel pvm = new ReportStudentDataListViewModel();
            pvm.idSelectedEvent = eventId ?? 0;
            List<AmsaReportStudentData> participantsInEvent = new List<AmsaReportStudentData>();
            try
            {
                //if the id != to 0 then filter
                if (eventId != 0)
                {
                    //Get the listing of information that needs to be shown on the view
                    participantsInEvent = dbr.lstStudentsForReport.Where(r => r.AMSAEvent.id == eventId && (r.Status.ToUpper().Equals("COMPLETED") || r.Status.ToUpper().Equals("COMPLETED PASS") 
                    ||
                    r.Status.ToUpper().Equals("COMPLETE") || r.Status.ToUpper().Equals("COMPLETE PASS")
                    )).ToList();
                }
                else
                {
                    //If it is == 0 then what we are need to show is all of the participants
                    participantsInEvent = dbr.lstStudentsForReport.Where(r => (r.Status.ToUpper().Equals("COMPLETED") || r.Status.ToUpper().Equals("COMPLETED PASS")
                    ||
                    r.Status.ToUpper().Equals("COMPLETE") || r.Status.ToUpper().Equals("COMPLETE PASS")
                    )).ToList();
                }
                pvm.LstStudentData = participantsInEvent;
                return View("IndexAmsaStudents", pvm);
            }
            catch
            {
                return View("IndexAmsaStudents", pvm);
            }

        }

        //Get view to add csv for datafeed
        [HttpGet]
        public ActionResult UploadAMSADataFeed(int? id)
        {
            AMSAReportStudentDataUploadViewModel avm = new AMSAReportStudentDataUploadViewModel();
            int eventId = id ?? 0;
            if(eventId > 0)
            {
                avm.idSelectedEvent = eventId;
            }
            return View(avm);
        }


        //Load uploaded data
        [HttpPost]
        public ActionResult Upload(AMSAReportStudentDataUploadViewModel pvm)
        {
            if(pvm.idSelectedEvent != 0) { 

                ReportUtilities.checkUploadCSVandExcel(Request, ModelState);
                
                if (ModelState.IsValid)
                {
                    pvm.PerformStudentDataInertions(Request, ModelState);
                    //After data insertions are done we update the event date update
                    pvm.updateEventUpdate();
                    if (ModelState.IsValid)
                    {
                        return View("IndexAmsaStudents", new ReportStudentDataListViewModel()); //Retun from when file is actually correct
                    }
                    else
                        return View("UploadAMSADataFeed", pvm);
                }
                return View("UploadAMSADataFeed", pvm);
            }
            else
            {
                ModelState.AddModelError("Participant", "Please select an event");
                return View("UploadAMSADataFeed",pvm);
            }
        }

        //Load data from a specific data feed
        public ActionResult UploadAMSADataFeedByEvent(int? eventId)
        {
            int id = eventId ?? 0;
            if (id == -1)
                id = 0;
            return View("UploadAMSADataFeed", new AMSAReportStudentDataUploadViewModel { idSelectedEvent = id});
        }



    }

    
}