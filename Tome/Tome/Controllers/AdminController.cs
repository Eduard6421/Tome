﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

                String roleId = (from role in db.Roles
                                 where role.Name == "Moderator"
                                 select role.Id).Single();

                var rol = (from roles in db.UserRoles
                           where roles.UserId == id
                           select roles).FirstOrDefault();

                Debug.WriteLine(rol.UserId);

                db.UserRoles.Remove(rol);

                IdentityUserRole identity = new IdentityUserRole { UserId = id, RoleId = roleId };

                db.UserRoles.Add(identity);

                db.SaveChanges();


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

                String roleId = (from role in db.Roles
                                 where role.Name == "User"
                                 select role.Id).Single();

                var rol = (from roles in db.UserRoles
                           where roles.UserId == id
                           select roles).FirstOrDefault();

                Debug.WriteLine(rol.UserId);

                db.UserRoles.Remove(rol);

                IdentityUserRole identity = new IdentityUserRole { UserId = id, RoleId = roleId };

                db.UserRoles.Add(identity);

                db.SaveChanges();


            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);

            }

            return RedirectToAction("ListUsers");

        }

        [HttpGet]
        public ActionResult ListTags()
        {
            List<Tag> tagList = new List<Tag>();

            tagList = db.Tags.Select(x => x).ToList();

            return View(tagList);
        }


        [HttpGet]
        public ActionResult CreateTag()
        {

            Tag newTag = new Tag();

            return View(newTag);
        }

        [HttpPost]
        public ActionResult CreateTag(Tag newTag)
        {
            try
            {
                newTag.TagTitle = newTag.TagTitle.ToLower();

                db.Tags.Add(newTag);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured: " + e);
            }

            return RedirectToAction("ListTags");
        }


        public ActionResult DeleteTag(String id)
        {
            try
            {
                var tag = db.Tags.Where(x=> x.TagId == Int32.Parse(id)).Select(x => x).SingleOrDefault();
                db.Tags.Remove(tag);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured: " + e);
            }

            return RedirectToAction("ListTags");
        }



    }
}