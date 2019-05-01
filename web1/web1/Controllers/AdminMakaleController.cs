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
    public class AdminMakaleController : Controller
    {
        BlogDB db = new BlogDB();
        // GET: AdminMakale
        public ActionResult Index()
        {
            var makales = db.Makales.ToList();
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoris, "Kategoriid", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale,string etiketler, HttpPostedFileBase Foto)
        {

            if (ModelState.IsValid)
            {

                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makale.Foto = "/Uploads/MakaleFoto/" + newfoto;

                }
                if (etiketler != null)
                {
                    string[] etiketDizi = etiketler.Split(',');
                    foreach (var i in etiketDizi)
                    {
                        var yeniEtiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yeniEtiket);
                        makale.Etikets.Add(yeniEtiket);


                    }
                }
                makale.Uyeid = Convert.ToInt32(Session["uyeid"]);
                makale.Okunma = 1;
                db.Makales.Add(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(makale);
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
            ViewBag.kategoriid = new SelectList(db.Kategoris, "Kategoriid", "KategoriAdi", makale.Kategoriid);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit (int id , HttpPostedFileBase Foto,Makale makale)
        {
            try
            {

                var makales = db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
                if (Foto !=null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Foto)); 
                    }

                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makales.Foto = "/Uploads/MakaleFoto/" + newfoto;
                    makales.Baslik = makale.Baslik;
                    makales.Icerik = makale.Icerik;
                    makales.Kategoriid = makale.Kategoriid;
                    db.SaveChanges();
                    Console.WriteLine(makales.ToString());
                }
                return RedirectToAction("Index");



                // TODO: Add update logic here


            }
            catch
            {
                ViewBag.kategoriid = new SelectList(db.Kategoris, "Kategoriid", "KategoriAdi", makale.Kategoriid);
                return View(makale);
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makales = db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
            if (makales==null)
            {
                return HttpNotFound();
            }
            return View(makales);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var makales = db.Makales.Where(u => u.Makaleid == id).SingleOrDefault();
                if (makales == null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Foto));
                    }

                foreach (var i in makales.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in makales.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makales);
                db.SaveChanges();


                    // TODO: Add delete logic here

                    return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
