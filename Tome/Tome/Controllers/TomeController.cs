using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;
using RTE;
using Tome.Models;

namespace Tome.Controllers
{
    public class TomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly string BASE_PATH = @"tomes\";

        private readonly string TOME_IDENTIFIER = "tome-";


        // GET: Tome
        public ActionResult Index()
        {
            var tomes = from tome in db.Tomes
                        orderby tome.CreationDate
                        select tome;


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

                if (currentTome.IsPrivate == true && (currentUserId == null || (currentUser != currentTome.ApplicationUser && roleName != "Moderator" && roleName != "Administrator")))
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
                currentTomeViewModel.TomeContent.Content = System.IO.File.ReadAllText(currentTomeHistory.FilePath.Replace(@"\\",@"\"));

                return View(currentTomeViewModel);

            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        [ValidateInput(true)]
        public ActionResult Add()
        {
            Models.TomeViewModel newTomeViewModel = new TomeViewModel();
            Editor TextEditor = new Editor(System.Web.HttpContext.Current, "Editor");

            TextEditor.LoadFormData("\n A new tome has been created.\n");
            TextEditor.MvcInit();
            ViewBag.Editor = TextEditor.MvcGetString();


            return View(newTomeViewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Models.TomeViewModel tome, String Editor)
        {
            try
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);


                tome.ReferredTome.CreationDate = DateTime.Now;
                tome.ReferredTome.ApplicationUser = currentUser;
                db.Tomes.Add(tome.ReferredTome);
                db.SaveChanges();

                // create init history

                TomeHistory tomeHistory = new TomeHistory
                {
                    Tome = tome.ReferredTome,
                    FilePath = BASE_PATH + TOME_IDENTIFIER + (User.Identity.GetUserName().IsEmpty() ? "anonymous" : User.Identity.GetUserName()) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ModificationDate = DateTime.Now,
                    ApplicationUser = currentUser
                };


                db.TomeHistories.Add(tomeHistory);
                db.SaveChanges();


                //Write content to file
                System.IO.File.WriteAllText(tomeHistory.FilePath, Editor);


                CurrentVersion currentVersion = new CurrentVersion { TomeHistory = tomeHistory, Tome = tome.ReferredTome };
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

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                String filePath;

                Models.Tome tome = db.Tomes.Find(id);

                Models.TomeViewModel editTomeViewModel = new TomeViewModel();

                Editor TextEditor = new Editor(System.Web.HttpContext.Current, "Editor");

                editTomeViewModel.ReferredTome = tome;


                // Find current version and get the file path
                int currentVersion = (from version in db.CurrentVersions
                                      where version.TomeId == id
                                      select version.TomeHistoryId).SingleOrDefault();

                filePath = (from history in db.TomeHistories
                            where history.Id == id
                            select history.FilePath).SingleOrDefault();


                String tomeContent = System.IO.File.ReadAllText(filePath);

                //Load the contents into the editor

                TextEditor.LoadFormData(tomeContent);
                TextEditor.MvcInit();
                ViewBag.Editor = TextEditor.MvcGetString();

                editTomeViewModel.ReferredTome = tome;


                return View(editTomeViewModel);
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult Edit(TomeViewModel editedTome, String Editor)
        {
            try
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                Models.Tome tome = db.Tomes.Find(editedTome.ReferredTome.TomeId);
                Models.TomeHistory tomeHistory = new TomeHistory();

                tome.CreationDate = DateTime.Now;
                tome.IsPrivate = editedTome.ReferredTome.IsPrivate;

                // register history
                tomeHistory.Tome = tome;
                tomeHistory.FilePath = BASE_PATH + TOME_IDENTIFIER + (User.Identity.GetUserName().IsEmpty() ? "anonymous" : User.Identity.GetUserName()) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                // create file and fill with content
                System.IO.File.WriteAllText(tomeHistory.FilePath, Editor);

                // insert into db
                tomeHistory.ModificationDate = DateTime.Now;
                tomeHistory.ApplicationUser = currentUser;
                db.TomeHistories.Add(tomeHistory);
                db.SaveChanges();


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
            Models.Tome tome = db.Tomes.Find(id);
            db.Tomes.Remove(tome);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}