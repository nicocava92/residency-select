using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Excel;
using System.Data;
using PPI.Core.Web.Models.AmsaReports.Email;

namespace PPI.Core.Web.Models.AmsaReports
{
    //Singleton class for report utilities
    public class ReportUtilities
    {
        //Matrix used to get data for Scale paragraphs
        public static string[,] gateFieldMatrix { get; set; }
        private static ReportUtilities getReportUtilities;
        //Used to check if the reminder has been sent today
        public static DateTime DateReminderSent { get; set; }

        public static ReportUtilities GetReportUtilities
        {
            get {
                if(getReportUtilities == null)
                    getReportUtilities = new ReportUtilities();
                return getReportUtilities; }
            set { getReportUtilities = value; }
        }
        


        //Set values in the matrix to get scale values
        private ReportUtilities()
        {
            gateFieldMatrix = new string[4,4];
            gateFieldMatrix[1, 1] = "LM";
            gateFieldMatrix[1, 2] = "LM";
            gateFieldMatrix[1, 3] = "LM";
            gateFieldMatrix[2, 1] = "ML";
            gateFieldMatrix[2, 2] = "MM";
            gateFieldMatrix[2, 3] = "MH";
            gateFieldMatrix[3, 1] = "HM";
            gateFieldMatrix[3, 2] = "HM";
            gateFieldMatrix[3, 3] = "HM";
        }
        

        //Receives values for low, med, high and scale. Returns paragraph for summary
        public string getParagraph(string scale, string gateFieldDominant, string gateFieldSecondary)
        {
            //Get int values for the gateFields
            int dominant = this.intForGateField(gateFieldDominant);
            int secondary = this.intForGateField(gateFieldSecondary);
            //Get gateField value for the paragaraph related to the scale and dominante and secondary scales
            string gateFieldP = gateFieldMatrix[dominant,secondary];
            //Search for values in string loaded from database
            AMSAReportContext dbo = new AMSAReportContext();
            AonParagraphs a = dbo.lstAonParagraphs.Where(m => m.GATEField.Equals(gateFieldP) && m.scale.Equals(scale)).FirstOrDefault();
            return a.paragraph;
        }

        public int intForGateField(string s)
        {
            int ret = 1;
            if (s.Equals("Low"))
                ret = 1;
            if (s.Equals("Med"))
                ret = 2;
            if (s.Equals("High"))
                ret = 3;
            return ret;
        }

        //Used to check when only CSV files can be uploaded
        internal static void checkUploadCSV(HttpRequestBase request, ModelStateDictionary m)
        {
            int i = 0;
            while (i < request.Files.Count)
            {
                var n = i + 1;
                HttpPostedFileBase hpf = request.Files[i] as HttpPostedFileBase;
                if (hpf.ContentLength != 0)
                {
                   
                    string name_without_extention = Path.GetFileNameWithoutExtension(hpf.FileName);
                    string get_extention = Path.GetExtension(hpf.FileName);
                    if (!get_extention.Equals(".csv"))
                    {
                        m.AddModelError("Upload", "ERROR! File needs to be .csv. Please make sure you are uploading the correct file type. | File number: " + n);
                    }
                    string file_name = name_without_extention + DateTime.Now.ToString("yyyyMMddHHmmssfff") + get_extention;
                }
                else
                {
                    m.AddModelError("Upload", "ERROR! Upload of file not correct File number:" + n);
                }
                i++;
            }
        }

        internal static List<string[]> getDataFromCsvXlsxORXLAMSACodes(HttpPostedFileBase httpPostedFileBase)
        {
            return getDataFromCsvXlsxORXL(httpPostedFileBase, "AMSACODE");
        }

        internal static List<string[]> getDataFromCsvXlsxORXLAMSAParticipants(HttpPostedFileBase httpPostedFileBase)
        {
            return getDataFromCsvXlsxORXL(httpPostedFileBase, "AMSAPARTICIPANTS");
        }

        //Returns a list of AMSAparticipants
        internal static List<AMSAParticipant> arrayToAMSAParticipants(List<string[]> lstValues)
        {
            List<AMSAParticipant> lstParticipants = new List<AMSAParticipant>();
            int c = 0;
            foreach(string[] values in lstValues) { 
                if (c > 0)
                {
                    //Try to save if there are errors let user know the participants that where not uploaded.. store the participants e-mail address and line number to let the user know
                    AMSAParticipant p = new AMSAParticipant();
                    p.FirstName = values[0];
                    p.LastName = values[1];
                    p.PrimaryEmail = values[2];
                    p.AMSACode = values[3];
                    p.AAMCNumber = values[4];
                    p.Gender = values[5];
                    p.Title = values[6];
                    lstParticipants.Add(p);
                }
                c++;
            }
            return lstParticipants;
        }

        //Converts string generated from excel to list of AMSA Codes to be inserted in the database
        internal static List<AMSACode> arrayToAMSACodes(List<string[]> values)
        {
            List<AMSACode> lstCodes = new List<AMSACode>();
            int c = 0;
            foreach (string[] v in values)
            {
                if (c > 0)
                {
                    //Get the amsa code present on each line (skip the first since the tytles for the rows are there)
                    AMSACode auxCode = new AMSACode();
                    auxCode.Code = v[0];
                    auxCode.Pin = v[1];
                    lstCodes.Add(auxCode);
                }
                c++;
            }
            return lstCodes;
        }
        

        //Used to check when CSV and Excel files can be uploaded
        internal static void checkUploadCSVandExcel(HttpRequestBase request, ModelStateDictionary m)
        {
            int i = 0;
            while (i < request.Files.Count)
            {
                var n = i + 1;
                HttpPostedFileBase hpf = request.Files[i] as HttpPostedFileBase;
                if (hpf.ContentLength != 0)
                {

                    string name_without_extention = Path.GetFileNameWithoutExtension(hpf.FileName);
                    string get_extention = Path.GetExtension(hpf.FileName);
                    if (!get_extention.Equals(".csv") && !get_extention.Equals(".xlsx") && !get_extention.Equals(".xls"))
                    {
                        m.AddModelError("Upload", "ERROR! File needs to be .csv, .xlsx or .xls. Please make sure you are uploading the correct file type. | File number: " + n);
                    }
                    string file_name = name_without_extention + DateTime.Now.ToString("yyyyMMddHHmmssfff") + get_extention;
                }
                else
                {
                    m.AddModelError("Upload", "ERROR! Upload of file not correct File number:" + n);
                }
                i++;
            }
        }


        //Gets the data from string[] and stores it in a list of participants
        /// <summary>
        /// Turns string[] read from csv,xls, xlsx to List of participants data, ignores first line since titles do not get stored
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static List<AmsaReportStudentData> arrayToParticipantResultsList(List<string[]> lstValues)
        {
            List<AmsaReportStudentData> lstParticipants = new List<AmsaReportStudentData>();
            var c = 0;
            foreach (string[] values in lstValues)
            {
                //Ingore first line
                if (c > 0)
                {
                    //Try to save if there are errors let the user know
                    AmsaReportStudentData p = new AmsaReportStudentData();
                    p.FirstName = values[0];
                    p.LastName = values[1];
                    p.PersonId = values[2];
                    //Only do date conversions if the dates exist 
                    if (values[3] != "")
                        p.RegistrationDate = Convert.ToDateTime(values[3]);
                    if (values[4] != "")
                        p.CompletionDate = Convert.ToDateTime(values[4]);
                    //Depending on the status we are going to either save the result or update the participant status
                    p.Status = values[5].Trim();
                    if (p.Status != "")
                    {
                        if (p.Status.ToUpper().Equals("COMPLETED") || p.Status.ToUpper().Equals("COMPLETED PASS"))
                        {
                            p.Stanine_Ambition = Convert.ToInt32(values[7]);
                            p.Stanine_Assertiveness = Convert.ToInt32(values[8]);
                            p.Stanine_Awareness = Convert.ToInt32(values[9]);
                            p.Stanine_Composure = Convert.ToInt32(values[10]);
                            p.Stanine_Conceptual = Convert.ToInt32(values[11]);
                            p.Stanine_Cooperativeness = Convert.ToInt32(values[12]);
                            p.Stanine_Drive = Convert.ToInt32(values[13]);
                            p.Stanine_Flexibility = Convert.ToInt32(values[14]);
                            p.Stanine_Humility = Convert.ToInt32(values[15]);
                            p.Stanine_Liveliness = Convert.ToInt32(values[16]);
                            p.Stanine_Mastery = Convert.ToInt32(values[17]);
                            p.Stanine_Positivity = Convert.ToInt32(values[18]);
                            p.Stanine_Power = Convert.ToInt32(values[19]);
                            p.Stanine_Sensitivity = Convert.ToInt32(values[20]);
                            p.Stanine_Structure = Convert.ToInt32(values[21]);
                        }
                    }
                    lstParticipants.Add(p);
                }
                c++;
            }
            return lstParticipants;
        }

        internal static List<string[]> getDataFromCsvXlsxORXLReportData(HttpPostedFileBase httpPostedFileBase)
        {
            return getDataFromCsvXlsxORXL(httpPostedFileBase, "REPORTDATA");
        }

        internal static List<string[]> getDataFromCsvXlsxORXL(HttpPostedFileBase httpPostedFileBase, string type)
        {
            //if file is CSV then read csv
            if (checkIfCSV(httpPostedFileBase))
            {
                return getDataFromCSVReportData(httpPostedFileBase);
            }
            else if (checkIfXLSX(httpPostedFileBase))
            {
                return getDataFromXLSX(httpPostedFileBase,type);
            }
            else if (checkIfXLS(httpPostedFileBase))
            {
                return getDataFromXLS(httpPostedFileBase,type);
            }
            //If none of them work return a list with empty strings
            return new List<string[]>();
        }

        //Type = tells the method the type of data feed we are dealing with so we can deal with dates accordingly
        private static List<string[]> getDataFromXLS(HttpPostedFileBase httpPostedFileBase, string type)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(httpPostedFileBase.InputStream, false, ReadOption.Loose);
            return readAnyExcel(excelReader, type);
        }

        /// <summary>
        /// Returns List of string for any excel, dates have to be dealt with in specific manner, because of this Type is added (tell the method the type and adapt it to deal with dates for the specific type of file)
        /// </summary>
        /// <param name="excelReader"></param>
        /// <param name="Type">Gives the oportunity to deal with files in different ways to developers</param>
        /// <returns></returns>
        private static List<string[]> readAnyExcel(IExcelDataReader excelReader, string Type)
        {
            List<string[]> ret = new List<string[]>();
            DataSet result = excelReader.AsDataSet();

            while (excelReader.Read())
            {
                List<string> auxList = new List<string>();
                if (Type.ToUpper().Equals("REPORTDATA")) { 
                    //Add values for columns into the file
                    //Loop through the columns in the row 
                    for (int i = 0; i <= 21; i++)
                    {

                        //For .xls reading dates is different, because of this we try for each column to read a date
                        //if there is a date present then we read it and convert it to a string to store it on the list
                        //this lets us read dats from .xls files

                        //specific for upload of data feed, will need to change this 
                        //depending on what we are reading (dealing with dates in .xls makes us have this extra step).
                        if (i == 3 || i == 4) { 
                            try
                            {
                                DateTime d = excelReader.GetDateTime(i);
                                string date = d.ToString();
                                auxList.Add(date);
                            }
                            catch
                            {
                                auxList.Add(excelReader.GetString(i));
                            }
                        }
                        else
                        {
                            auxList.Add(excelReader.GetString(i));
                        }
                    }
                }
                if (Type.ToUpper().Equals("AMSACODE"))
                {
                    //Simply add strings, no dates to worry about
                    for (int i = 0; i <= 1; i++)
                    {
                        auxList.Add(excelReader.GetString(i)); 
                    }
                }
                if (Type.ToUpper().Equals("AMSAPARTICIPANTS")) {
                    //Simply add strings, no dates to worry about
                    for (int i = 0; i <= 7; i++)
                    {
                        auxList.Add(excelReader.GetString(i));
                    }
                }
                //Convert list to string array and add it in to the list
                string[] s = auxList.ToArray();
                ret.Add(s);
            }
            return ret;
        }

        //Read xlsx files (Microsoft Excel 2007 and above) and return a list of string arrays
        //If our excel has more than 21 columns then we should change the capacity of the ammount of columns that are checked
        //Type == tells the method they type of excel we are reading so we can deal with dates accordingly
        private static List<string[]> getDataFromXLSX(HttpPostedFileBase httpPostedFileBase, string type)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(httpPostedFileBase.InputStream);
            return readAnyExcel(excelReader, type);
        }

        //No need for type since CSV doesn't have problems storing dates from CSV to an array (split happens correctly without any special requirements). 
        private static List<string[]> getDataFromCSVReportData(HttpPostedFileBase httpPostedFileBase)
        {
            // Use the InputStream to get the actual stream sent.
            StreamReader csvreader = new StreamReader(httpPostedFileBase.InputStream, Encoding.Default, true);
            var c = 0;
            List <string[]> values = new List<string[]>();
            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                values.Add(line.Split(','));
            }
            return values;
        }

        //Check if the file is XLS
        private static bool checkIfXLS(HttpPostedFileBase uploadedFile)
        {
            string get_extention = Path.GetExtension(uploadedFile.FileName);
            return get_extention.Equals(".xls");
        }

        //Check if the file is XLSX
        private static bool checkIfXLSX(HttpPostedFileBase uploadedFile)
        {
            string get_extention = Path.GetExtension(uploadedFile.FileName);
            return get_extention.Equals(".xlsx");
        }

        //Check if the file is CSV
        internal static bool checkIfCSV(HttpPostedFileBase uploadedFile)
        {
            string get_extention = Path.GetExtension(uploadedFile.FileName);
            return get_extention.Equals(".csv");
        }

        //Check if the reminder has been sent today
        internal static bool reminderSentToday()
        {
            if (DateReminderSent != null)
                return DateReminderSent.Date == DateTime.Now.Date;
            else
                return false;
        }

        //Set todays date as reminder sent date
        internal static void setReminderSentToday()
        {
            DateReminderSent = DateTime.Now;
        }
        
    }
}