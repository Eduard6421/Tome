using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Tome.Filters;
using Tome.Models;

namespace Tome.Controllers
{
    public class AdminController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator")]
        public ActionResult ListUsers()
        {
            try
            {

                var usersWithRoles = (from user in db.Users
                                      from userRole in user.Roles
                                      join role in db.Roles on userRole.RoleId equals
                                          role.Id
                                      select new UserViewModel()
                                      {
                                          Id = user.Id,
                                          Username = user.UserName,
                                          Email = user.Email,
                                          Role = role.Name
                                      }).ToList();

                String currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var roleName = (from userroles in db.UserRoles
                    join roles in db.Roles on userroles.RoleId equals roles.Id
                    where userroles.UserId == currentUserId
                    select roles.Name).FirstOrDefault();
                ViewBag.roleAccount = roleName;
                ViewBag.usersWithRoles = usersWithRoles;
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);
            }


            return View();

        }


        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator")]
        public ActionResult Promote(String id)
        {

            try
            {
                String roleIdAdmin = (from role in db.Roles
                    where role.Name == "Administrator"
                    select role.Id).SingleOrDefault();


                String roleId = (from role in db.Roles
                                 where role.Name == "Moderator"
                                 select role.Id).Single();

                var rol = (from roles in db.UserRoles
                           where roles.UserId == id
                           select roles).FirstOrDefault();

                if (roleIdAdmin == rol.RoleId)
                {
                    TempData["errorDemote"] = "true";
                    return RedirectToAction("ListUsers");
                }


                Debug.WriteLine(rol.UserId);

                db.UserRoles.Remove(rol);

                IdentityUserRole identity = new IdentityUserRole { UserId = id, RoleId = roleId };

                db.UserRoles.Add(identity);

                db.SaveChanges();
                String currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var roleName = (from userroles in db.UserRoles
                    join roles in db.Roles on userroles.RoleId equals roles.Id
                    where userroles.UserId == currentUserId
                    select roles.Name).FirstOrDefault();
                ViewBag.roleAccount = roleName;

            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);

            }

            return RedirectToAction("ListUsers");
        }

        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator")]
        public ActionResult Demote(String id)
        {
            try
            {

                String roleIdAdmin = (from role in db.Roles
                    where role.Name == "Administrator"
                    select role.Id).SingleOrDefault();

                String roleId = (from role in db.Roles
                                 where role.Name == "User"
                                 select role.Id).Single();

                var rol = (from roles in db.UserRoles
                           where roles.UserId == id
                           select roles).FirstOrDefault();


                if (roleIdAdmin == rol.RoleId)
                {
                    TempData["errorDemote"] = "true";
                    return RedirectToAction("ListUsers");
                }


                Debug.WriteLine(rol.UserId);
                db.UserRoles.Remove(rol);
                IdentityUserRole identity = new IdentityUserRole { UserId = id, RoleId = roleId };
                db.UserRoles.Add(identity);
                db.SaveChanges();

                String currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var roleName = (from userroles in db.UserRoles
                    join roles in db.Roles on userroles.RoleId equals roles.Id
                    where userroles.UserId == currentUserId
                    select roles.Name).FirstOrDefault();
                ViewBag.roleAccount = roleName;

            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);

            }

            return RedirectToAction("ListUsers");

        }

        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator")]
        public ActionResult ListTags()
        {   
            List<Tag> tagList = new List<Tag>();

            tagList = db.Tags.Select(x => x).ToList();
            String currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            var roleName = (from userroles in db.UserRoles
                join roles in db.Roles on userroles.RoleId equals roles.Id
                where userroles.UserId == currentUserId
                select roles.Name).FirstOrDefault();
            ViewBag.roleAccount = roleName;
            return View(tagList);
        }


        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator")]
        public ActionResult CreateTag()
        {

            Tag newTag = new Tag();
            String currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            var roleName = (from userroles in db.UserRoles
                join roles in db.Roles on userroles.RoleId equals roles.Id
                where userroles.UserId == currentUserId
                select roles.Name).FirstOrDefault();
            ViewBag.roleAccount = roleName;
            return View(newTag);
        }

        [HttpPost]
        [AccessDeniedAuthorize(Roles = "Administrator")]
        public ActionResult CreateTag(Tag newTag)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    newTag.TagTitle = newTag.TagTitle.ToLower();

                    db.Tags.Add(newTag);
                    db.SaveChanges();

                    String currentUserId = User.Identity.GetUserId();
                    ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                    var roleName = (from userroles in db.UserRoles
                        join roles in db.Roles on userroles.RoleId equals roles.Id
                        where userroles.UserId == currentUserId
                        select roles.Name).FirstOrDefault();
                    ViewBag.roleAccount = roleName;
                }
                else
                {
                    String currentUserId = User.Identity.GetUserId();
                    ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                    var roleName = (from userroles in db.UserRoles
                        join roles in db.Roles on userroles.RoleId equals roles.Id
                        where userroles.UserId == currentUserId
                        select roles.Name).FirstOrDefault();
                    ViewBag.roleAccount = roleName;
                    return View(newTag);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured: " + e);
            }

            return RedirectToAction("ListTags");
        }

        [AccessDeniedAuthorize(Roles = "Administrator")]
        public ActionResult DeleteTag(String id)
        {
            try
            {
                Int32 Id = Int32.Parse(id);
                var tag = db.Tags.Where(x=> x.TagId == Id).Select(x => x).SingleOrDefault();
                db.Tags.Remove(tag);
                db.SaveChanges();

                String currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var roleName = (from userroles in db.UserRoles
                    join roles in db.Roles on userroles.RoleId equals roles.Id
                    where userroles.UserId == currentUserId
                    select roles.Name).FirstOrDefault();
                ViewBag.roleAccount = roleName;
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured: " + e);
            }

            return RedirectToAction("ListTags");
        }



    }
}