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

        private readonly string BASE_PATH = @"C:\Users\Eduard\Desktop\tomes\tomes";
        // GET: Tome
        public ActionResult Index()
        {
            var tomes = from tome in db.Tomes
                        orderby tome.CreationDate
                        select tome;
            

            ViewBag.Tomes = tomes;
            
            return View();
        }


        /// <summary>
        ///  Receives an id an return the proper tome
        /// </summary>
        /// <param name="id">The id of the tome</param>
        /// <returns>The tome object</returns>
        [HttpGet]
        public ActionResult Show(int id)
        {
            Models.Tome tome = db.Tomes.Find(id);
            var currentTomeHistory = (from tomeHistory in db.TomeHistories
                                        where tomeHistory.TomeId == id
                                        orderby tomeHistory.ModificationDate
                                        select tomeHistory).Take(1);
            ViewBag.currentTomeHistory = currentTomeHistory;
            return View();
        }


        [HttpGet]
        public ActionResult Add()
        {
            Editor Editor1 = new Editor(System.Web.HttpContext.Current, "Editor1");
            Editor1.LoadFormData("nalapuladetigqan");
            Editor1.MvcInit();
            ViewBag.Editor = Editor1.MvcGetString();


            return View();
        }

        [HttpPost]
        public ActionResult Add(Models.Tome tome)
        {
            try
            {

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);


                tome.CreationDate = DateTime.Now;
                tome.ApplicationUser = currentUser;
                db.Tomes.Add(tome);
                db.SaveChanges();

                // create init history

                TomeHistory tomeHistory = new TomeHistory
                {
                    Tome = tome,
                    FilePath = BASE_PATH + "-" +
                               (User.Identity.GetUserName().IsEmpty() ? "anonymous" : User.Identity.GetUserName()) +
                               "-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ModificationDate = DateTime.Now,
                    ApplicationUser = currentUser
                };
                db.TomeHistories.Add(tomeHistory);
                db.SaveChanges();


                CurrentVersion currentVersion = new CurrentVersion {TomeHistory = tomeHistory, Tome = tome};
                db.CurrentVersions.Add(currentVersion);
                db.SaveChanges();



                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured: " + e);
                return View();
            }

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Models.Tome tome = db.Tomes.Find(id);
            ViewBag.tome = tome;
            return View();
        }

        public ActionResult Edit(int id,TomeContent tomeContent)
        {
            try
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                Models.Tome tome = db.Tomes.Find(id);
                Models.TomeHistory tomeHistory = new TomeHistory();
                if (TryUpdateModel(tome))
                {
                    tome.CreationDate = DateTime.Now;
                    tome.IsPrivate = tomeContent.requestTome.IsPrivate;

                    // register history
                    tomeHistory.Tome = tome;
                    tomeHistory.FilePath = BASE_PATH + "-" + (User.Identity.GetUserName().IsEmpty() ? "anonymous" : User.Identity.GetUserName()) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    
                    // create file and fill with content
                    Debug.WriteLine(tomeContent.Content);
                    Debug.WriteLine(tomeHistory.FilePath);
                    System.IO.File.WriteAllText(tomeHistory.FilePath, tomeContent.Content);

                    // insert into db
                    tomeHistory.ModificationDate = DateTime.Now;
                    tomeHistory.ApplicationUser = currentUser;
                    db.TomeHistories.Add(tomeHistory);
                    db.SaveChanges();


                    // update curent version

                    var currentVersion = db.CurrentVersions.SingleOrDefault(m => m.Tome.TomeId == id);
                    currentVersion.TomeHistory = tomeHistory;
                    db.SaveChanges();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return View();
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