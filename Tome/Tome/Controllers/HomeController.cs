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
                                 select tome).OrderBy(r => Guid.NewGuid()).AsEnumerable();

                    var tags = (from tag in db.Tags
                        select tag);

                    var map = (from tome in db.Tomes
                        join refs in db.TagReferences on tome.TomeId equals refs.TomeId into newjoin
                        from tomesnref in newjoin.DefaultIfEmpty()
                        join tag in db.Tags on tomesnref.TagId equals tag.TagId into result
                        from ended in result.DefaultIfEmpty()
                        select new { newTome = tome, newTag = (ended.TagTitle == null ? "No category" : ended.TagTitle) }).ToDictionary(x => x.newTome, x => x.newTag);
                    

                    ViewBag.Tomes = tomes.ToList();
                    ViewBag.Count = tomes.Count();
                    ViewBag.Tags = tags.ToList();
                    ViewBag.Map = map;
                }
                else  // successfully auth user
                {
                    var roleName = (from userroles in db.UserRoles
                                    join roles in db.Roles on userroles.RoleId equals roles.Id
                                    where userroles.UserId == currentUserId
                                    select roles.Name).FirstOrDefault();

                    var tags = (from tag in db.Tags
                        select tag);

                    var map = (from tome in db.Tomes
                        join refs in db.TagReferences on tome.TomeId equals refs.TomeId into newjoin
                        from tomesnref in newjoin.DefaultIfEmpty()
                        join tag in db.Tags on tomesnref.TagId equals tag.TagId into result
                        from ended in result.DefaultIfEmpty()
                        select new { newTome = tome, newTag = (ended.TagTitle == null ? "No category" : ended.TagTitle) }).ToDictionary(x => x.newTome, x => x.newTag);

                    ViewBag.roleAccount = roleName;
                    ViewBag.Tags = tags.ToList();
                    ViewBag.Map = map;
                    if (roleName == "Administrator")
                    {
                        // administrator query for get all tomes
                        var tomes = (from tome in db.Tomes
                                     orderby tome.CreationDate
                                     select tome).OrderBy(r => Guid.NewGuid()).AsEnumerable();
                        

                        ViewBag.Tomes = tomes.ToList();
                        ViewBag.Count = tomes.Count();
                        ViewBag.Tags = tags.ToList();
                    }
                    else
                    {

                        // regular user or moderator query to get only tome where has access
                        var tomes = (from tome in db.Tomes
                                     where tome.IsPrivate == false || tome.ApplicationUser.Id == currentUserId
                                     orderby tome.CreationDate
                                     select tome).OrderBy(r => Guid.NewGuid()).AsEnumerable();
                        
                        Console.WriteLine(tomes);
                        Console.WriteLine(currentUser);
                        ViewBag.Tomes = tomes.ToList();
                        ViewBag.Count = tomes.Count();
                        ViewBag.Tags = tags.ToList();
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