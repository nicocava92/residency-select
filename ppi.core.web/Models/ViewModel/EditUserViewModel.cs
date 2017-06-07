using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public string Password { get; set; }


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
            foreach(IdentityRole r in LstRoles)
            {
                if (u.Roles.Any(m => m.RoleId.Equals(r.Id)))
                    auxLstRoles.Add(r);

            }
            UserInRoles = new List<string>();
            foreach(IdentityRole r in auxLstRoles)
            {
                UserInRoles.Add(r.Id);
            }

        }

        //Save changes to the user
        internal static void saveChanges(List<string> selectedRoles, List<string> currentRoles, string email, string userId,string usersite,string password)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser u = db.Users.Find(userId);
            if(password == passwordRepeat) { 
                u.PasswordHash = UserManager.PasswordHasher;
            }
            u.Email = email;
            removeAllRoles(u);
            addSelectedRoles(selectedRoles, db,u);
            db.SaveChanges();
        }

        //Add new roles to the user
        private static void addSelectedRoles(List<string> lstNewUserRoles,ApplicationDbContext db,ApplicationUser u)
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