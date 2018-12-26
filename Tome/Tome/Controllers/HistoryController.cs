using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tome.Filters;
using Tome.Models;

namespace Tome.Controllers
{
    public class HistoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: History
        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator,Moderator,User")]
        public ActionResult Index(int id)
        {
            try
            {
                var TomeHistoryList = (from tomeHistory in db.TomeHistories
                                       where tomeHistory.TomeId == id
                                       orderby tomeHistory.ModificationDate descending
                                       select tomeHistory);

                int curentVersion = (from version in db.CurrentVersions
                                        where version.TomeId == id
                                        select version.TomeHistoryId).FirstOrDefault();


                if (!Request.IsAuthenticated && (db.Tomes.Find(id)).IsPrivate)
                {
                    return RedirectToAction("AccessDenied", "Error");
                }
                String currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var roleName = (from userroles in db.UserRoles
                    join roles in db.Roles on userroles.RoleId equals roles.Id
                    where userroles.UserId == currentUserId
                    select roles.Name).FirstOrDefault();
                ViewBag.roleAccount = roleName;
                ViewBag.curentVersion = curentVersion;
                ViewBag.TomeHistoryList = TomeHistoryList.ToList();

                return View(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator,Moderator,User")]
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

            return RedirectToAction("Index","History",new { id = id });
        }

        [HttpGet]
        [AccessDeniedAuthorize(Roles = "Administrator,Moderator,User")]
        public ActionResult DeleteVersion(int id, int idHistory)
        {
            try
            {
                var currentTomeHistory = (from currentVersion in db.CurrentVersions
                    where currentVersion.TomeId == id
                    select currentVersion).SingleOrDefault();
                

                int curentVersion = (from version in db.CurrentVersions
                    where version.TomeId == id
                    select version.TomeHistoryId).FirstOrDefault();

                if(idHistory == curentVersion)
                {
                    TempData["Alert"] = "Current version cannot be deleted.";
                    return RedirectToAction("Index", new{id=id});
                }

                var deletedHistory = db.TomeHistories.Where(x => x.Id == idHistory).SingleOrDefault();
                db.TomeHistories.Remove(deletedHistory);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                Debug.WriteLine("An error has occured: " + e);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "History", new { id = id });
        }


    }
}