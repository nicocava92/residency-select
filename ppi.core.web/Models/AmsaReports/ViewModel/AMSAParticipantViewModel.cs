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
            dbr.SaveChanges();

            /*
            ModelState.Remove("AMSAParticipant.AMSA_Password");
            ModelState.Remove("AMSAParticipant.Title");
            ModelState.Remove("AMSAParticipant.AAMCNumber");
            ModelState.Remove("AMSAParticipant.AMSACode");
            */

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

            //If password is not empty then assign the one that was set
            if (this.AMSAParticipant.AMSA_Password != null)
            {
                //assign the password that was generated
                IPasswordHasher passwordHasher = new PasswordHasher();
                p.AMSA_Password = passwordHasher.HashPassword(this.AMSAParticipant.AMSA_Password);
            }
            else
            {
                this.assignAMSAPassword();
                p.AMSA_Password = this.AMSAParticipant.AMSA_Password;
            }
            //If AAMC Number
            if (this.AMSAParticipant.AAMCNumber != null)
                p.AAMCNumber = this.AMSAParticipant.AAMCNumber;
            else
            {
                this.assignAAMCNumer();
                p.AAMCNumber = this.AMSAParticipant.AAMCNumber;
            }


            if (this.AMSAParticipant.AMSACode != null)
            {
                p.AMSACode = this.AMSAParticipant.AMSACode;
                this.markCodeAsChecked(p.AMSACode);
            }
            else
            {
                this.assignAMSACode();
                p.AMSACode = this.AMSAParticipant.AMSACode;
            }
            p.FirstName = this.AMSAParticipant.FirstName;
            p.LastName = this.AMSAParticipant.LastName;
            p.PrimaryEmail = this.AMSAParticipant.PrimaryEmail;
            p.Title = this.AMSAParticipant.Title;
            p.Gender = this.stringSelectedGender;
            if (p.Gender == null)
                p.Gender = this.AMSAParticipant.Gender;
            checkIfStatusExists(p);
            //Check if the are results in the database for the participant if there are update status to the participant status
            dbr.AMSAParticipant.Add(p);
            dbr.SaveChanges();
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


        //Verify if the password is correct
        public bool checkPassword(string emailOrAMSACode, string insertedPassword)
        {
            //Check with AMSACode and E-mail just in case we want to use either
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAParticipant p = dbr.AMSAParticipant.Where(m => m.PrimaryEmail.ToUpper().Equals(emailOrAMSACode.ToUpper()) || m.AMSACode.ToUpper().Equals(emailOrAMSACode.ToUpper())).FirstOrDefault();
            if(p != null) { 
                IPasswordHasher passwordHasher = new PasswordHasher();
                PasswordVerificationResult res = passwordHasher.VerifyHashedPassword(p.AMSA_Password,insertedPassword);
                //if 0 then its not if 1 then it is
                if (res == PasswordVerificationResult.Success) { 
                    dbr.Dispose();
                    return true;
                }
                else if (res == PasswordVerificationResult.Failed) { 
                    dbr.Dispose();
                    return false;
                }
            }
            dbr.Dispose();
            return false;
        }
        //Change password
        public bool changePassword(string newPassword)
        {
            try
            {
                AMSAReportContext dbr = new AMSAReportContext();
                AMSAParticipant p = dbr.AMSAParticipant.Find(this.AMSAParticipant.Id);
                IPasswordHasher passwordHasher = new PasswordHasher();
                p.AMSA_Password = passwordHasher.HashPassword(newPassword);
                dbr.SaveChanges();
                dbr.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
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
        public void assignAMSACode()
        {
            //Get the first amsa code 
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(m => !m.Used && m.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if(c != null) {
                this.AMSAParticipant.AMSACode = c.Code;
                c.markAsUsed();
                dbr.SaveChanges();
            }
            dbr.Dispose();
        }

        //Generate AMSA Password
        public void assignAMSAPassword()
        {
            string name = this.AMSAParticipant.FirstName;
            string last = this.AMSAParticipant.LastName;
            string number = "";
            number += name.Substring(0, 2);
            number += DateTime.UtcNow.ToString("yyMMddHHmmssfff");
            number += last.Substring(0, 2);
            IPasswordHasher passwordHasher = new PasswordHasher();
            this.AMSAParticipant.AMSA_Password = passwordHasher.HashPassword(number);
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