﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;
using Tome.Models;

namespace Tome.Controllers
{
    public class TomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly string BASE_PATH = "\\uploads\\";

        private readonly string TOME_IDENTIFIER = "tome-";

        //Restrict the acces to these
        // GET: Tome
        public ActionResult Index()
        {
            var tomes = (from tome in db.Tomes
                         orderby tome.CreationDate
                         select tome).OrderBy(r => r.CreationDate);
            ViewBag.Tomes = tomes;

            return View();
        }

        public ActionResult Search(String searchedText)
        {
            var tomes = (from tome in db.Tomes
                         where tome.Name.Contains(searchedText)
                         orderby tome.CreationDate
                         select tome);

            ViewBag.Tomes = tomes;

            return View();
        }

        public ActionResult IndexByName(bool sortByNameAsc)
        {

            List<Models.Tome> tomes;

            if (sortByNameAsc)
            {
                tomes = (from tome in db.Tomes
                         orderby tome.Name
                         select tome).ToList();
            }
            else
            {
                tomes = (from tome in db.Tomes
                         orderby tome.Name descending
                         select tome).ToList();
            }

            ViewBag.Tomes = tomes;

            return View();
        }

        public ActionResult IndexByDate(bool sortByDateAsc)
        {

            List<Models.Tome> tomes;

            if (sortByDateAsc)
            {
                tomes = (from tome in db.Tomes
                         orderby tome.CreationDate
                         select tome).ToList();
            }
            else
            {
                tomes = (from tome in db.Tomes
                         orderby tome.CreationDate descending
                         select tome).ToList();
            }

            ViewBag.Tomes = tomes;

            return View();
        }


        public ActionResult SearchByTag(int TagId)
        {
            List<Models.Tome> tomes;

            tomes = (from tome in db.Tomes
                join tagRef in db.TagReferences on tome.TomeId equals tagRef.TomeId
                join tag in db.Tags on tagRef.TagId equals tag.TagId
                orderby tome.CreationDate
                select tome).ToList();

            ViewBag.Tomes = tomes;

            return View();


        }

        public ActionResult SearchByTagName(int TagId, bool sortByNameAsc)
        {
            List<Models.Tome> tomes;


            if(sortByNameAsc)
            { tomes = (from tome in db.Tomes
                join tagRef in db.TagReferences on tome.TomeId equals tagRef.TomeId
                join tag in db.Tags on tagRef.TagId equals tag.TagId
                orderby tome.Name
                select tome).ToList();
            }
            else
            {
                tomes = (from tome in db.Tomes
                    join tagRef in db.TagReferences on tome.TomeId equals tagRef.TomeId
                    join tag in db.Tags on tagRef.TagId equals tag.TagId
                    orderby tome.Name descending
                    select tome).ToList();

            }
            ViewBag.Tomes = tomes;

            return View();


        }

        public ActionResult SearchByTagDate(int TagId, bool sortByDateAsc)
        {
            List<Models.Tome> tomes;


            if (sortByDateAsc)
            {
                tomes = (from tome in db.Tomes
                    join tagRef in db.TagReferences on tome.TomeId equals tagRef.TomeId
                    join tag in db.Tags on tagRef.TagId equals tag.TagId
                    orderby tome.CreationDate
                    select tome).ToList();
            }
            else
            {
                tomes = (from tome in db.Tomes
                    join tagRef in db.TagReferences on tome.TomeId equals tagRef.TomeId
                    join tag in db.Tags on tagRef.TagId equals tag.TagId
                    orderby tome.CreationDate descending
                    select tome).ToList();

            }
            ViewBag.Tomes = tomes;

            return View();


        }

        [HttpGet]
        public ActionResult Show(int id)
        {

            try
            {
                String currentUserId = User.Identity.GetUserId();

                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                var roleName = (from userroles in db.UserRoles
                                join roles in db.Roles on userroles.RoleId equals roles.Id
                                where userroles.UserId == currentUserId
                                select roles.Name).FirstOrDefault();

                var currentTome = (from tome in db.Tomes
                                   where tome.TomeId == id
                                   select tome).SingleOrDefault();

                if (currentTome.IsPrivate == true && (currentUserId == null ||
                                                      (currentUser != currentTome.ApplicationUser &&
                                                       roleName != "Moderator" && roleName != "Administrator")))
                {
                    //Denied Access ( bc ori nu e logat ori nu e detinatorul ori nu e moderator / administrator)
                    return RedirectToAction("Index");
                }


                int currentHistory = (from version in db.CurrentVersions
                                      where version.TomeId == id
                                      select version.TomeHistoryId).SingleOrDefault();


                var currentTomeHistory = (from tomeHistory in db.TomeHistories
                                          where tomeHistory.Id == currentHistory
                                          select tomeHistory).SingleOrDefault();

                TomeViewModel currentTomeViewModel = new TomeViewModel();

                currentTomeViewModel.ReferredTome = db.Tomes.Find(id);
                currentTomeViewModel.TomeContent = new TomeContent();

                currentTomeViewModel.TomeContent.Content = currentTomeHistory.FilePath;

                return View(currentTomeViewModel);

            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        [ValidateInput(true)]
        public ActionResult Add()
        {
            Models.TomeViewModel newTomeViewModel = new TomeViewModel();


            var SelectListItems = db.Tags.Select(x => new SelectListItem { Value = x.TagId.ToString(), Text = x.TagTitle });

            var TagList = new List<SelectListItem>(SelectListItems);


            newTomeViewModel.TagList = TagList;

            return View(newTomeViewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Models.TomeViewModel tome)
        {
            string path = Server.MapPath("..");
            try
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                tome.ReferredTome.CreationDate = DateTime.Now;
                tome.ReferredTome.Name = tome.ReferredTome.Name.ToLower();

                if (Request.IsAuthenticated)
                {
                    tome.ReferredTome.ApplicationUser = currentUser;
                    tome.ReferredTome.IsPrivate = false;
                }
                else
                {
                    // need to be null for anonymous users
                    tome.ReferredTome.ApplicationUser = null;
                }

                db.Tomes.Add(tome.ReferredTome);
                db.SaveChanges();


                // check if tag is non empty

                if (tome.SelectedTag != 0)
                {
                    Tag chosenTag = db.Tags.Find(tome.SelectedTag);
                    TagReference newReference = new TagReference();

                    newReference.Tag = chosenTag;
                    newReference.Tome = tome.ReferredTome;

                    db.TagReferences.Add(newReference);
                }
                // create init history

                TomeHistory tomeHistory = new TomeHistory
                {
                    Tome = tome.ReferredTome,
                    FilePath = path + BASE_PATH + TOME_IDENTIFIER +
                               (User.Identity.GetUserName().IsEmpty() ? ("anonymous" + Request.AnonymousID) : User.Identity.GetUserName()) +
                               "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html",
                    ModificationDate = DateTime.Now,
                    ApplicationUser = currentUser
                };


                db.TomeHistories.Add(tomeHistory);
                db.SaveChanges();
                //Write content to file
                string content = tome.TomeContent.Content.Replace("\"../uploads/", "\"../../uploads/");

                System.IO.File.WriteAllText(tomeHistory.FilePath, content);

                CurrentVersion currentVersion = new CurrentVersion
                { TomeHistory = tomeHistory, Tome = tome.ReferredTome };
                db.CurrentVersions.Add(currentVersion);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);
                return RedirectToAction("Index");
            }

        }



        /// <summary>
        /// Saves the contents of an uploaded image file.
        /// </summary>
        /// <param name="targetFolder">Location where to save the image file.</param>
        /// <param name="file">The uploaded image file.</param>
        /// <exception cref="InvalidOperationException">Invalid MIME content type.</exception>
        /// <exception cref="InvalidOperationException">Invalid file extension.</exception>
        /// <exception cref="InvalidOperationException">File size limit exceeded.</exception>
        /// <returns>The relative path where the file is stored.</returns>
        private static string SaveFile(string targetFolder, HttpPostedFileBase file)
        {
            const int megabyte = 1024 * 1024;

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidOperationException("Invalid MIME content type.");
            }

            var extension = Path.GetExtension(file.FileName.ToLowerInvariant());
            string[] extensions = { ".gif", ".jpg", ".png", ".svg", ".webp" };
            if (!extensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file extension.");
            }

            if (file.ContentLength > (8 * megabyte))
            {
                throw new InvalidOperationException("File size limit exceeded.");
            }
            Debug.Write(file.FileName);
            var fileName = Guid.NewGuid() + extension;
            var path = Path.Combine(targetFolder, fileName);
            Debug.Write(file.FileName);
            file.SaveAs(path);

            return Path.Combine("/uploads", fileName).Replace('\\', '/');
        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            //Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var location = SaveFile(Server.MapPath("~/uploads/"), file);

            return Json(new { location }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                String filePath;

                Models.Tome tome = db.Tomes.Find(id);
                Models.TomeViewModel editTomeViewModel = new TomeViewModel();


                Tag selectedTag = (from tag in db.TagReferences
                                   where tag.TomeId == id
                                   select tag.Tag).SingleOrDefault();


                var SelectListItems = db.Tags.Select(x => new SelectListItem { Value = x.TagId.ToString(), Text = x.TagTitle });

                var TagList = new List<SelectListItem>(SelectListItems);

                editTomeViewModel.TagList = TagList;

                if (selectedTag != null)
                {
                    editTomeViewModel.SelectedTag = selectedTag.TagId;
                }

                // Find current version and get the file path
                int currentVersionId = (from version in db.CurrentVersions
                                        where version.TomeId == id
                                        select version.TomeHistoryId).SingleOrDefault();

                filePath = (from history in db.TomeHistories
                            where history.Id == currentVersionId
                            select history.FilePath).SingleOrDefault();


                String tomeContent = System.IO.File.ReadAllText(filePath);
                editTomeViewModel.ReferredTome = tome;
                Debug.WriteLine(filePath);
                Debug.WriteLine(tomeContent);

                editTomeViewModel.ReferredTome = tome;
                TomeContent content = new TomeContent();
                content.Content = tomeContent;
                editTomeViewModel.TomeContent = content;

                return View(editTomeViewModel);
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TomeViewModel editedTome)
        {
            string path = Server.MapPath("../..");

            try
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                Models.Tome tome = db.Tomes.Find(editedTome.ReferredTome.TomeId);
                tome.CreationDate = DateTime.Now;
                tome.IsPrivate = editedTome.ReferredTome.IsPrivate;

                TomeHistory tomeHistory = new TomeHistory
                {
                    Tome = tome,
                    FilePath = path + BASE_PATH + TOME_IDENTIFIER +
                               (User.Identity.GetUserName().IsEmpty() ? ("anonymous" + Request.AnonymousID) : User.Identity.GetUserName()) +
                               "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html",
                    ModificationDate = DateTime.Now,
                    ApplicationUser = currentUser
                };

                // insert into db
                db.TomeHistories.Add(tomeHistory);
                db.SaveChanges();


                // create file and fill with content
                string content = editedTome.TomeContent.Content.Replace("\"../uploads/", "\"../../uploads/"); ;
                System.IO.File.WriteAllText(tomeHistory.FilePath, content);


                // update curent version

                var currentVersion = db.CurrentVersions.SingleOrDefault(m => m.Tome.TomeId == editedTome.ReferredTome.TomeId);
                currentVersion.TomeHistory = tomeHistory;
                db.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {

            db.CurrentVersions.RemoveRange(db.CurrentVersions.Where(version => version.TomeId == id));
            db.TomeHistories.RemoveRange(db.TomeHistories.Where(history => history.TomeId == id));
            db.Tomes.Remove(db.Tomes.Find(id));

            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}