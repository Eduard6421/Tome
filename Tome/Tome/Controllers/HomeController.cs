using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tome.Models;

namespace Tome.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var tomes = db.Tomes.OrderBy(elem => Guid.NewGuid()).Take(1);

            ViewBag.Tomes = tomes;

            return View();
        }

        public ActionResult SearchTitle(string pattern)
        {
            var tomes = from tome in db.Tomes
                        where tome.Name.Contains(pattern)
                        select tome;
            
            ViewBag.Tomes = tomes;
            return View();
        }
        public ActionResult SearchTag(string pattern)
        {
            // DE FACUT CU JOIN MUIE CE ESTI ... :))
            var tomes = from tome in db.Tomes
                where tome.Name.Contains(pattern)
                select tome;

            ViewBag.Tomes = tomes;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}