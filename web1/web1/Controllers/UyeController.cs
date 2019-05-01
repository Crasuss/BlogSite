using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using web1.Models;

namespace web1.Controllers
{


    public class UyeController : Controller
    {
        BlogDB db = new BlogDB();

        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(u => u.Uyeid == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"])!=uye.Uyeid)
            {
                return HttpNotFound();
            }

            return View(uye);
        }
       public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Uye uye,string Sifre)
        {


            var login = db.Uyes.SingleOrDefault(u => u.KullanıcıAdi == uye.KullanıcıAdi && u.Email == uye.Email && u.Sifre == uye.Sifre);
            if (login != null)
            {
                Session["uyeid"] = login.Uyeid;
                Session["kullanıcıadi"] = login.KullanıcıAdi;
                Session["yetkiid"] = login.Yetkiid;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Uyarı = "BİLGİLERİNİZİ KONTROL EDİNİZ!!!";
                return View();
            }
        }



        public ActionResult Logout()
        {
            Session["uyeid"] = null;
            Session.Abandon();
            return RedirectToAction("Index","Home");

        }
                                 
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Uye uye , HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uye.Foto = ("/Uploads/UyeFoto/" + newfoto);
                    uye.Yetkiid = 2;
                    db.Uyes.Add(uye);
                    db.SaveChanges();
                    Session["uyeid"] = uye.Uyeid;
                    Session["kullanıcıadi"] = uye.KullanıcıAdi;
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("Fotoğraf","Fotoğraf  yükleyiniz!!!!");
                }
            }
            return View(uye);
        }

        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(u => u.Uyeid == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.Uyeid)
            {
                return HttpNotFound();
            }
            return View(uye);
        }

        [HttpPost]
        public ActionResult Edit(Uye uye ,int id,HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                var uyes = db.Uyes.Where(u => u.Uyeid == id).SingleOrDefault();
                if (Foto != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(uyes.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(uyes.Foto));
                    }

                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uyes.Foto = ("/Uploads/UyeFoto/" + newfoto);
                }
                uyes.AdSoyad = uye.AdSoyad;
                uyes.KullanıcıAdi = uye.KullanıcıAdi;
                uyes.Sifre = uye.Sifre;
                uyes.Email = uye.Email;
                    db.SaveChanges();
                    Session["kullanıcıadi"] = uye.KullanıcıAdi;
                return RedirectToAction("Index", "Home", new { id = uyes.Uyeid });       
                                
            }
            return View();
        }


    }
}