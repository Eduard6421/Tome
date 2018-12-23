using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tome.Models;

namespace Tome.Controllers
{
    public class HistoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: History
        public ActionResult Index(int id)
        {
            try
            {
                var TomeHistoryList = (from tomeHistory in db.TomeHistories
                                       where tomeHistory.TomeId == id
                                       orderby tomeHistory.ModificationDate descending
                                       select tomeHistory);
                ViewBag.TomeHistoryList = TomeHistoryList;
                return View(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult ChangeVersion(int id, int idHistory)
        {

            try
            {
                var currentTomeHistory = (from currentVersion in db.CurrentVersions
                                          where currentVersion.TomeId == id
                                          select currentVersion).SingleOrDefault();

                currentTomeHistory.TomeHistoryId = idHistory;

                db.SaveChanges();

            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured: " + e);
                return RedirectToAction("Index");

            }

            return RedirectToAction("Index");
        }



    }
}