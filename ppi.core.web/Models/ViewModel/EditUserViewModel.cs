using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
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

        public EditUserViewModel(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            LstRoles = db.Roles.ToList();
            ApplicationUser u = db.Users.Find(id);
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
        internal void saveChanges()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser u = db.Users.Find(this.User.Id);
            removeAllRoles(u);
            addSelectedRoles(LstNewUserRoles,db,u);
            db.SaveChanges();
        }

        //Add new roles to the user
        private void addSelectedRoles(List<string> lstNewUserRoles,ApplicationDbContext db,ApplicationUser u)
        {
            foreach (var s in lstNewUserRoles)
            {
                IdentityRole r = db.Roles.Where(m => m.Id.Equals(s)).FirstOrDefault();
                if (r != null)
                    User.Roles.Add(new IdentityUserRole { RoleId = s });
            }
        }
        //Remove all roles the user currenlty has
        public void removeAllRoles(IdentityUser u)
        {
            User.Roles.Clear();
        }
    }
}