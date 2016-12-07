using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace PPI.Core.Web.Models.ViewModel
{
    public class AdministerUsersViewModel
    {
        public List<ApplicationUser> LstUsers { get; set; }
        public SelectList Roles { get; set; }
        public List<IdentityRole> LstRoles { get; set; }
        public int idSelectedRole { get; set; }

        public AdministerUsersViewModel()
        {
            loadRolesAndUsers();
        }
        
        private void loadRolesAndUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            LstRoles = db.Roles.ToList();
            LstUsers = db.Users.ToList();
            LstRoles.Insert(0, new IdentityRole { Id = "-1", Name = "-- Show All --" });
            Roles = new SelectList(LstRoles, "Id", "Name");
            db.Dispose();
        }

        //Changes the list of users to only show the users in a specific role
        internal void changeSelectedRole(int idRole)
        {
            //Get role that has the id
            ApplicationDbContext db = new ApplicationDbContext();
            IdentityRole r = db.Roles.Find(idRole);
            List<ApplicationUser> lstAuxUsers = db.Users.ToList();
            List<ApplicationUser> lstUsersInRole = new List<ApplicationUser>();
            foreach(ApplicationUser u in lstAuxUsers)
            {
                if(u.Roles.Any(m => m.RoleId.Equals(r.Id)))
                    lstUsersInRole.Add(u);
            }
            //When we have all of the users that pertain to the role we change the user listing
            LstUsers = lstUsersInRole;
            db.Dispose();
        }
    }
}