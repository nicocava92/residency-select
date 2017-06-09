using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PPI.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PPI.Core.Domain.Abstract;

namespace PPI.Core.Web.Models.ViewModel
{
    public class EditUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<IdentityRole> LstRoles { get; set; }
        //Stores the Roles the user is in to show them selected on the view
        public List<string> UserInRoles { get; set; }
        //List roles for the user to become a part of
        public List<string> LstNewUserRoles { get; set; }
        //used to edit password
        [DataType(DataType.Password)]
        [Display(Name = "Insert new password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Repeat password")]
        public string PasswordRepeat { get; set; }



        public EditUserViewModel() { }

        public EditUserViewModel(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            LstRoles = db.Roles.ToList();
            ApplicationUser u = db.Users.Where(r => r.Id.Equals(id)).FirstOrDefault();
            User = u;
            //Get id for roles the user is in
            List<IdentityRole> auxLstRoles = new List<IdentityRole>();
            //First check the user against each role and see which ones he is in
            foreach (IdentityRole r in LstRoles)
            {
                if (u.Roles.Any(m => m.RoleId.Equals(r.Id)))
                    auxLstRoles.Add(r);

            }
            UserInRoles = new List<string>();
            foreach (IdentityRole r in auxLstRoles)
            {
                UserInRoles.Add(r.Id);
            }

        }


        /// <summary>
        /// Receives information to perform changes to the user and stores it.
        /// </summary>
        /// <param name="selectedRoles"></param>
        /// <param name="currentRoles"></param>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <param name="usersite"></param>
        /// <param name="Password"></param>
        /// <param name="PasswordRepeat"></param>
        /// <returns>
        /// Returns an error string if the amount is greater than 0 then there are errors and the user changes were not stored.
        /// </returns>
        internal static string saveChanges(List<string> selectedRoles, List<string> currentRoles, string email, string userid, string usersite, string Password, string PasswordRepeat, IUnitOfWork unitOfWork)
        {
            {
                bool passwordBeingChanged = false;
                bool emailBeingchanged = false;
                //Run validations first
                System.Text.StringBuilder errors = new System.Text.StringBuilder();
                if (Password.Length > 0 && PasswordRepeat.Length > 0)
                {
                    //Password needs validation
                    if (!Password.Equals(PasswordRepeat))
                    {
                        errors.Append("<p>Inserted passwords do not appear to be the same.</p>");
                    }
                    else if (Password.Length < 6)
                    {
                        errors.Append("<p>Password must have more than 6 characters.</p>");
                    }
                    passwordBeingChanged = true;
                }
                if (usersite == "")
                {
                    errors.Append("<p>A site needs to be selected.</p>");
                }
                if (selectedRoles.Count == 0)
                {
                    errors.Append("<p>At least 1 Role needs to be selected.</p>");
                }
                if (email.Length > 0)
                {
                    if (IsValidEmail(email))
                        errors.Append("<p>Please revise the inserted e-mail address, it appears to be in an invalid format.</p>");
                    emailBeingchanged = true;
                }

                if (errors.Length == 0)
                {
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    ApplicationDbContext db = new ApplicationDbContext();
                    ApplicationUser u = db.Users.Find(userid);
                    //if(password == passwordRepeat) { 
                    //    u.PasswordHash = UserManager.PasswordHasher;
                    //}
                    if (emailBeingchanged) //if e-mail is being changed then change e-mail
                        u.Email = email;
                    if (passwordBeingChanged) //if password is being changed then change password
                    {
                        u.PasswordHash = UserManager.PasswordHasher.HashPassword(Password);
                    }

                    removeAllRoles(u);
                    addSelectedRoles(selectedRoles, db, u);


                    SiteUser site_user = new SiteUser();
                    site_user.AspNetUsersId = userid;
                    site_user.SiteId = Convert.ToInt32(usersite);
                    unitOfWork.ISiteUserRepository.Add(site_user);


                    //Save changes to both contexts
                    db.SaveChanges();
                    unitOfWork.Commit();
                }





                return errors.ToString(); //Return errors it will either return none or a more than 1
            }
        }

        public static bool IsValidEmail(string strIn)
        {
            var invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        //Add new roles to the user
        private static void addSelectedRoles(List<string> lstNewUserRoles, ApplicationDbContext db, ApplicationUser u)
        {
            foreach (var s in lstNewUserRoles)
            {
                IdentityRole r = db.Roles.Where(m => m.Id.Equals(s)).FirstOrDefault();
                if (r != null)
                    u.Roles.Add(new IdentityUserRole { RoleId = s });
            }
        }
        //Remove all roles the user currenlty has
        public static void removeAllRoles(IdentityUser u)
        {
            u.Roles.Clear();
        }

        public string[] getArrayCurrentRoles()
        {
            return UserInRoles.ToArray();
        }


    }
}