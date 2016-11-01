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

        public AMSAParticipantViewModel()
        {
            loadGenders();
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvents = new SelectList(dbr.AMSAEvent.ToList(), "id", "Name");
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
            p.AMSACode = this.AMSAParticipant.AMSACode;
            p.AMSAEvent = dbr.AMSAEvent.Find(this.idSelectedEvent);
            //Password -- Need to check if this is necesary and check for the password compared to how mvc identity takes care of passwords
            //p.AMSA_Password = this.AMSAParticipant.AMSA_Password;
            IPasswordHasher passwordHasher = new PasswordHasher();
            p.AMSA_Password = passwordHasher.HashPassword(this.AMSAParticipant.AMSA_Password);
            p.AAMCNumber = this.AMSAParticipant.AAMCNumber;
            p.FirstName = this.AMSAParticipant.FirstName;
            p.LastName = this.AMSAParticipant.LastName;
            p.PrimaryEmail = this.AMSAParticipant.PrimaryEmail;
            p.Title = this.AMSAParticipant.Title;
            p.Gender = this.stringSelectedGender;
            dbr.SaveChanges();
            dbr.Dispose();
        }

        //Save new participant inside the database
        public void saveNewParticipant()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Get selected Event
            AMSAParticipant p = new AMSAParticipant();
            p.AMSACode = this.AMSAParticipant.AMSACode;
            p.AMSAEvent = dbr.AMSAEvent.Find(this.idSelectedEvent);
            //Password -- Need to check if this is necesary and check for the password compared to how mvc identity takes care of passwords
            //p.AMSA_Password = this.AMSAParticipant.AMSA_Password;
            IPasswordHasher passwordHasher = new PasswordHasher();
            p.AMSA_Password = passwordHasher.HashPassword(this.AMSAParticipant.AMSA_Password);
            p.AAMCNumber = this.AMSAParticipant.AAMCNumber;
            p.FirstName = this.AMSAParticipant.FirstName;
            p.LastName = this.AMSAParticipant.LastName;
            p.PrimaryEmail = this.AMSAParticipant.PrimaryEmail;
            p.Title = this.AMSAParticipant.Title;
            p.Gender = this.stringSelectedGender;
            dbr.AMSAParticipant.Add(p);
            dbr.SaveChanges();
            dbr.Dispose();
        }

        //Need a random password generator
        public string generatePassword()
        {
            throw new NotImplementedException();
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
    }

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