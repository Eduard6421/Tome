using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.ApplicationInsights.Web;
using Microsoft.AspNet.Identity;
using Tome.Models;

namespace Tome.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //var tomes = db.Tomes.OrderBy(elem => Guid.NewGuid()).Take(1);

            String currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            try
            {
                // anonymous user session
                if (Request.AnonymousID == "")
                {
                    var tomes = (from tome in db.Tomes
                                 where tome.IsPrivate == false
                                 orderby tome.CreationDate
                                 select tome).OrderBy(r => Guid.NewGuid()).Take(5).AsEnumerable();
                    ViewBag.Tomes = tomes;
                }
                else  // successfully auth user
                {
                    var roleName = (from userroles in db.UserRoles
                                    join roles in db.Roles on userroles.RoleId equals roles.Id
                                    where userroles.UserId == currentUserId
                                    select roles.Name).FirstOrDefault();

                    if (roleName == "Administrator")
                    {
                        // administrator query for get all tomes
                        var tomes = (from tome in db.Tomes
                                     orderby tome.CreationDate
                                     select tome).OrderBy(r => Guid.NewGuid()).Take(5).AsEnumerable();
                        ViewBag.Tomes = tomes;
                    }
                    else
                    {

                        // regular user or moderator query to get only tome where has access
                        var tomes = (from tome in db.Tomes
                                     where tome.IsPrivate == false || tome.ApplicationUser.Id == currentUserId
                                     orderby tome.CreationDate
                                     select tome).OrderBy(r => Guid.NewGuid()).Take(5).AsEnumerable();
                        Console.WriteLine(tomes);
                        Console.WriteLine(currentUser);
                        ViewBag.Tomes = tomes;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            ViewBag.anonymous = "anonymous-" + Request.AnonymousID;


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