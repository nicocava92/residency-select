using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PPI.Core.Web.Models.AmsaReports;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class AMSAParticipantViewModel
    {
        public AMSAParticipant AMSAParticipant { get; set; }
        public SelectList AMSAEvents { get; set; }
        public int idSelectedEvent { get; set; }

        public SelectList Genders { get; set; }
        public List<Gender> LstGender { get; set; }
        public string stringSelectedGender { get; set; }
        

        //Event list
        public List<AMSAEvent> LstEvents { get; set; }

        public AMSAParticipantViewModel()
        {
            loadGenders();
            AMSAReportContext dbr = new AMSAReportContext();
            //List of events is used later on and can't get the data from select this, to 
            //stop double loading the events are loaded into a list and then used for different things such as assigning them to select lists
            LstEvents = dbr.AMSAEvent.ToList();

            AMSAEvents = new SelectList(LstEvents, "id", "Name");
            dbr.Dispose();
        }

        //Add in the possible genders
        public void loadGenders()
        {
            LstGender = new List<Gender>();
            LstGender.Add(new Gender("Male", "Male"));
            LstGender.Add(new Gender("Female", "Female"));
            Genders = new SelectList(LstGender, "Value", "Name");
        }

        public void loadSelectedData(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            this.AMSAParticipant = dbr.AMSAParticipant.Find(id);
            this.idSelectedEvent = this.AMSAParticipant.AMSAEvent.id;
            this.stringSelectedGender = this.AMSAParticipant.Gender;
            dbr.Dispose();
        }

        public void saveChanges()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Get selected Event
            AMSAParticipant p = dbr.AMSAParticipant.Find(this.AMSAParticipant.Id);
            p.AMSAEvent = dbr.AMSAEvent.Find(this.idSelectedEvent);
            //Password -- Need to check if this is necesary and check for the password compared to how mvc identity takes care of passwords
            //IPasswordHasher passwordHasher = new PasswordHasher();
            //p.AMSA_Password = passwordHasher.HashPassword(this.AMSAParticipant.AMSA_Password);
            p.FirstName = this.AMSAParticipant.FirstName;
            p.LastName = this.AMSAParticipant.LastName;
            p.Gender = this.stringSelectedGender;
            p.PrimaryEmail = this.AMSAParticipant.PrimaryEmail;
            dbr.SaveChanges();
            dbr.Dispose();
        }

        //Save new participant inside the database
        public void saveNewParticipant()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Get selected Event
            AMSAParticipant p = new AMSAParticipant();
            
            p.AMSAEvent = dbr.AMSAEvent.Find(this.idSelectedEvent);
            //Password -- Need to check if this is necesary and check for the password compared to how mvc identity takes care of passwords
            //p.AMSA_Password = this.AMSAParticipant.AMSA_Password;

            //If AAMC Number
            if (this.AMSAParticipant.AAMCNumber != null && !this.AMSAParticipant.AAMCNumber.Equals(""))
                p.AAMCNumber = this.AMSAParticipant.AAMCNumber;
            else
            {
                this.assignAAMCNumer();
                p.AAMCNumber = this.AMSAParticipant.AAMCNumber;
            }

            //Assign amsa code and amsa PIN (Password)
            if (this.AMSAParticipant.AMSACode != null && !this.AMSAParticipant.AMSACode.Equals("") &&  amsaCodeExists())
            {
                assignASMACodeWithPreExistingCode();
                p.AMSACode = this.AMSAParticipant.AMSACode;
                p.AMSA_Password = this.AMSAParticipant.AMSA_Password;
            }
            else
            {
                this.assignAMSACodeAndPassword();
                p.AMSACode = this.AMSAParticipant.AMSACode;
                p.AMSA_Password = this.AMSAParticipant.AMSA_Password;
            }
            
            p.FirstName = this.AMSAParticipant.FirstName;
            p.LastName = this.AMSAParticipant.LastName;
            p.PrimaryEmail = this.AMSAParticipant.PrimaryEmail;
            p.Title = this.AMSAParticipant.Title;
            p.Gender = this.stringSelectedGender;
            if (this.AMSAParticipant.Gender == null && !this.AMSAParticipant.Gender.Equals(""))
                p.Gender = this.AMSAParticipant.Gender;
            checkIfStatusExists(p);
            //Check if the are results in the database for the participant if there are update status to the participant status
            dbr.AMSAParticipant.Add(p);
            dbr.SaveChanges();
            this.markCodeAsChecked(p.AMSACode);
            dbr.Dispose();
        }

        private void checkIfStatusExists(AMSAParticipant p)
        {
            AMSAReportContext dbo = new AMSAReportContext();
            //Check if there are results already for this participant
            AmsaReportStudentData auxData = dbo.lstStudentsForReport.Where(m => m.PersonId.ToUpper().Equals(this.AMSAParticipant.AMSACode) && m.AMSAEvent.id == p.AMSAEvent.id).FirstOrDefault();
            if (auxData != null)
                p.Status = auxData.Status;
            dbo.Dispose();
        }

        
        //Generate AMSA Number
        public void assignAAMCNumer()
        {
            string name = this.AMSAParticipant.FirstName;
            string last = this.AMSAParticipant.LastName;
            string number = "";
            number += name.Substring(0,2);
            number += DateTime.UtcNow.ToString("yyMMddHHmmssfff");
            number += last.Substring(0, 2);
            this.AMSAParticipant.AAMCNumber = number;
        }

        //Generate AMSA Code (need to setup amsa codes first to be able to get them from the database)
        public void assignAMSACodeAndPassword()
        {
            //Get the first amsa code 
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(m => !m.Used && m.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if(c != null) {
                this.AMSAParticipant.AMSACode = c.Code;
                this.AMSAParticipant.AMSA_Password = c.Pin;
                dbr.SaveChanges();
            }
            dbr.Dispose();
        }
        //Assign amsa code if the participant is added to the application with an already existent amsa code
        public void assignASMACodeWithPreExistingCode() {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(m => m.AMSAEvent.id == this.idSelectedEvent && m.Code.Equals(this.AMSAParticipant.AMSACode)).FirstOrDefault();
            if(c != null)
            {
                this.AMSAParticipant.AMSACode = c.Code;
                this.AMSAParticipant.AMSA_Password = c.Pin;
                dbr.SaveChanges();
            }
            dbr.Dispose();
        }

        //Check if the amsa code that the user wants to add in exists or not in the database
        public bool amsaCodeExists()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(m => m.Code.Equals(this.AMSAParticipant.AMSACode) && m.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            dbr.Dispose();
            return c != null;
        }

        public int AMASCodeCount()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Get the the event on first 
            int eventId = LstEvents[0].id;
            List<AMSACode> lstCodes = dbr.AMSACodes.Where(m => !m.Used && m.AMSAEvent.id == eventId).ToList();
            int ammountOfCodes = lstCodes.Count();
            dbr.Dispose();
            return ammountOfCodes;
        }

        //Check if the amsa code exists and if it isn't assigned to something else
        public void checkAMSACode(ModelStateDictionary m) { 
            AMSAReportContext dbr = new AMSAReportContext();
            //Check if inserted code exists and is not used
            AMSACode c = dbr.AMSACodes.Where(r => r.Code.Equals(this.AMSAParticipant.AMSACode) && !r.Used).FirstOrDefault();
            //If value is not null then the code will be assigned to the user, do so and mark the code as used
            if (c == null)
            {
                m.AddModelError("AMSACode", "AMSA Code does not exist or is already assigned to another participant");
            }
            dbr.Dispose();
        }

        //Check if the user is not already 
        //Check by email and amsa event id
        public void checkIfUserAlreadyAssignedtoEvent(ModelStateDictionary m)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAParticipant p = dbr.AMSAParticipant.Where(r => r.PrimaryEmail.Equals(this.AMSAParticipant.PrimaryEmail) && r.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if(p != null) {
                m.AddModelError("AMSAParticipantExists", "ERROR! Participant with this e-mail address is already assigned to the selected event");
            }
            dbr.Dispose();
        }



        //Receives Code and markes it as used, used when the amsa code is not automatically assigned
        public void markCodeAsChecked(string code)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(m => m.Code.Equals(code)).FirstOrDefault();
            if(c != null)
            {
                c.markAsUsed();
            }
        }
      
    }

    //Class used for gender selection in Participant insertion view
    public class Gender
    {
        public string Value { get; set; }
        public string Name { get; set; }
        
        public Gender(string n, string f)
        {
            Name = n;
            Value = f;
        }
    }

    




}