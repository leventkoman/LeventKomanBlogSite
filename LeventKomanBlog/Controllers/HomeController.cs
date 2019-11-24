using LeventKomanBlog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;


namespace LeventKomanBlog.Controllers
{
    public class HomeController : Controller
    {
        LeventKomanBlogDB db = new LeventKomanBlogDB();

        public ActionResult Index(int Page=1)
        {
            var article = db.Article.OrderByDescending(x => x.Date).ToPagedList(Page,5);
            return View(article);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Category()
        {
            return View(db.Category.ToList());
        }

        public ActionResult SearchBlog(string search = null)
        {

            //var article = db.Article.Include(a => a.Category).Include(a => a.User);
            //return View(article.ToList());
            var searchs = db.Article.Where(x => x.Title.Contains(search)).ToList();
            return View(searchs.OrderByDescending(x=>x.Date));
        }

        public ActionResult ArticleDetails(int id)
        {
            var articledetails = db.Article.Where(x => x.Id == id).SingleOrDefault();

            if (articledetails == null)
            {
                return HttpNotFound();
            }

            return View(articledetails);
        }

        public JsonResult CreateComment(string yorum, int Makaleid)
        {
            var userid = Session["userid"];
            if (yorum == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.Comment.Add(new Comment
            {
                UserId = Convert.ToInt32(userid),
                ArticleId = Makaleid,
                CommentOfContent = yorum,
                Date = DateTime.Now
            });
            db.SaveChanges();
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteComment(int id)
        {
            var userid = Session["userid"];
            var comment = db.Comment.Where(x => x.Id == id).SingleOrDefault();
            var article = db.Article.Where(x => x.Id == comment.ArticleId).SingleOrDefault();
            if (comment.UserId == Convert.ToInt32(userid))
            {
                db.Comment.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("ArticleDetails", "Home", new {id = article.Id});
            }

            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult ReadsCounter(int Makaleid)
        {
            var article = db.Article.Where(x => x.Id == Makaleid).SingleOrDefault();
            article.ReadsCount += 1;
            db.SaveChanges();
            return View();
        }

    }
}