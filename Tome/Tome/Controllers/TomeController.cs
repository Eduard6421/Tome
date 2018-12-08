using System;
using System.Collections.Generic;
using System.Linq;
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

        private string BASE_PATH = @"E:\Facultate\Anul III\Tome\Tome\Tome\App_Data\tomes\tomes";
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

                TomeHistory tomeHistory = new TomeHistory();
                tomeHistory.Tome = tome;
                tomeHistory.FilePath = BASE_PATH + "-" + (User.Identity.GetUserName().IsEmpty() ? "anonymous" : User.Identity.GetUserName()) + "-" + DateTime.Now;
                tomeHistory.ModificationDate = DateTime.Now;
                tomeHistory.ApplicationUser = currentUser;
                db.TomeHistories.Add(tomeHistory);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
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

        public ActionResult Edit(int id,Models.Tome requestTome)
        {
            try
            {
                Models.Tome tome = db.Tomes.Find(id);
                if (TryUpdateModel(tome))
                {
                    tome.CreationDate = DateTime.Now;
                    tome.IsPrivate = requestTome.IsPrivate;
                    tome.Name = requestTome.Name;
                    
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