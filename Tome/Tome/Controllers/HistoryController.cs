using System;
using System.Collections.Generic;
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
            var currentTomeHistory = (from tomeHistory in db.TomeHistories
                where tomeHistory.TomeId == id
                orderby tomeHistory.ModificationDate descending 
                select tomeHistory);
            ViewBag.currentTomeHistory = currentTomeHistory;
            return View();
        }

        [HttpPost]
        public ActionResult RevertHistory(int id,int idHistory)
        {
            


            return RedirectToAction("Index");
        }



    }
}