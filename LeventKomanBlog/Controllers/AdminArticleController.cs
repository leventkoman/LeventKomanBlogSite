using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LeventKomanBlog.Models;
using System.Web.Helpers;
using System.IO;

namespace LeventKomanBlog.Controllers
{
    public class AdminArticleController : Controller
    {
        private LeventKomanBlogDB db = new LeventKomanBlogDB();

        public ActionResult Index()
        {
            var article = db.Article.Include(a => a.Category).Include(a => a.User);
            return View(article.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "CategoryName");
            ViewBag.UserId = new SelectList(db.User, "Id", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article, string stickers, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    WebImage image = new WebImage(ImageFile.InputStream);
                    FileInfo photoinfo = new FileInfo(ImageFile.FileName);

                    string newphoto = Guid.NewGuid().ToString() + photoinfo.Extension;
                    image.Resize(800, 350);
                    image.Save("~/Uploads/ArticlePhoto/" + newphoto);
                    article.Photo = "/Uploads/ArticlePhoto/" + newphoto;

                }

                if (stickers!=null)
                {
                    string[] stickerarray = stickers.Split(',');
                    foreach (var item in stickerarray)
                    {
                        Sticker newsticker = new Sticker { StickerName = item };
                        db.Sticker.Add(newsticker);
                        article.Sticker.Add(newsticker);
                    }
                }

                article.UserId = Convert.ToInt32(Session["userid"]);
                db.Article.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Category, "Id", "CategoryName", article.CategoryId);
            return View(article);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "CategoryName", article.CategoryId);
           // ViewBag.UserId = new SelectList(db.User, "Id", "Username", article.UserId);
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Article article, int id ,HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var editarticle = db.Article.Find(id);
                if (ImageFile != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(editarticle.Photo)))
                    {
                        System.IO.File.Delete(Server.MapPath(editarticle.Photo));
                    }

                    WebImage image = new WebImage(ImageFile.InputStream);
                    FileInfo photoinfo = new FileInfo(ImageFile.FileName);

                    string newphoto = Guid.NewGuid().ToString() + photoinfo.Extension;
                    image.Resize(800, 350);
                    image.Save("~/Uploads/ArticlePhoto/" + newphoto);
                    editarticle.Photo = "/Uploads/ArticlePhoto/" + newphoto;
                }

                editarticle.Title = article.Title;
                editarticle.CategoryId = article.CategoryId;
                editarticle.Contents = article.Contents;
                editarticle.Date = article.Date;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "CategoryName", article.CategoryId);
            //ViewBag.UserId = new SelectList(db.User, "Id", "Username", article.UserId);
            return View(article);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection fc)
        {
            var articles = db.Article.Find(id);

            if (articles==null)
            {
                return HttpNotFound();
            }
            if (System.IO.File.Exists(Server.MapPath(articles.Photo)))
            {
                System.IO.File.Delete(Server.MapPath(articles.Photo));
            }

            foreach (var item in articles.Comment.ToList())
            {
                db.Comment.Remove(item);
            }
            foreach (var item in articles.Sticker.ToList())
            {
                db.Sticker.Remove(item);
            }
            db.Article.Remove(articles);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
