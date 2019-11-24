using LeventKomanBlog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace LeventKomanBlog.Controllers
{
    public class AccountController : Controller
    {
        LeventKomanBlogDB db = new LeventKomanBlogDB();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            var login = db.User.Where(x => x.Username == user.Username).SingleOrDefault();

            if (login.Username == user.Username && login.Password == user.Password)
            {
                Session["userid"] = login.Id;
                Session["username"] = login.Username;
                Session["roleid"] = login.RoleId;
                Session["userphoto"] = user.UserPhoto;

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Kullanıcı adı vaya şifre yanlış.";
                return View();
            }
        }

        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Register(User user, HttpPostedFileBase UserPhoto)
        {
            if (ModelState.IsValid)
            {
                //if (UserPhoto!=null)
                //{
                //    WebImage image = new WebImage(UserPhoto.InputStream);
                //    FileInfo photoinfo = new FileInfo(UserPhoto.FileName);

                //    string newphoto = Guid.NewGuid().ToString() + photoinfo.Extension;
                //    image.Resize(150, 150);
                //    image.Save("~/Uploads/UyePhoto" + newphoto);
                //    user.UserPhoto = "~/Uploads/UyePhoto" + newphoto;
                user.RoleId = 2;
                db.User.Add(user);
                db.SaveChanges();
                Session["roleid"] = user.RoleId;
                Session["username"] = user.Username;
                Session["userphoto"] = user.UserPhoto;
                return RedirectToAction("Index", "Home");

                //}
                //else
                //{
                //    ModelState.AddModelError("Fotoğraf","Fotoğraf seçiniz...");
                //}

            }
            return View(user);
        }

        public ActionResult ShowProfile(int id)
        {
            var user = db.User.Where(x => x.Id == id).SingleOrDefault();
            if (Convert.ToInt32(Session["userid"]) != user.Id)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        public ActionResult EditProfile(int id)
        {
            var user = db.User.Where(x => x.Id == id).SingleOrDefault();
            if (Convert.ToInt32(Session["userid"]) != user.Id)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(User user, int id, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var users = db.User.Where(x => x.Id == id).SingleOrDefault();

                if (ImageFile != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(users.UserPhoto)))
                    {
                        System.IO.File.Delete(Server.MapPath(users.UserPhoto));
                    }

                    WebImage image = new WebImage(ImageFile.InputStream);
                    FileInfo photoinfo = new FileInfo(ImageFile.FileName);

                    string newphoto = Guid.NewGuid().ToString() + photoinfo.Extension;
                    image.Resize(128, 128);
                    image.Save("~/Uploads/UserPhoto/" + newphoto);
                    users.UserPhoto = "/Uploads/UserPhoto/" + newphoto;
                }

                users.Name = user.Name;
                users.Surname = user.Surname;
                users.Username = user.Username;
                users.Email = user.Email;
                users.Password = user.Password;

                db.SaveChanges();
                Session["username"] = user.Username;
                Session["userphoto"] = user.UserPhoto;

                return RedirectToAction("ShowProfile", "Account", new { id = users.Id });
            }

            return View();
        }

        public ActionResult DeleteProfile(User user, int id)
        {
            User deleteuser;
            deleteuser = db.User.Find(user.Id);
            if (user != null)
            {
                db.User.Remove(deleteuser);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}