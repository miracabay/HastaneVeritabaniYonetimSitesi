using HastaneVeritabani.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;
using System.Data.Entity.Validation;

namespace HastaneVeritabani.Controllers
{
    public class AdresController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Adres
        public ActionResult AdresGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İlgili personel veya hastanın adres id'sini parametre olarak alan ve veritabanından
            //ilgili adres id'yi bularak view'e ileten method
            var adres = db.Adres.Find(id);

            return View(adres);
        }

        public ActionResult Güncelle(Adres _adres)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İlgili personel veya hastanın adres id'si ile ulaşılan adresi değiştirme ve güncelleme işlemi
            var adres = db.Adres.Find(_adres.adres_id);

            if (string.IsNullOrEmpty(_adres.sehir))
            {
                _adres.sehir = adres.sehir;
            }

            if (string.IsNullOrEmpty(_adres.ilce))
            {
                _adres.ilce = adres.ilce;
            }

            if (string.IsNullOrEmpty(_adres.mahalle))
            {
                _adres.mahalle = adres.mahalle;
            }

            if (string.IsNullOrEmpty(_adres.sokak))
            {
                _adres.sokak = adres.sokak;
            }

            if (string.IsNullOrEmpty(_adres.bina))
            {
                _adres.bina = adres.bina;
            }

            if (string.IsNullOrEmpty(_adres.daire))
            {
                _adres.daire = adres.daire;
            }
            //Kullanıcının verileri boş girme durumunda eski verilerin korunması için if yapıalrı

            adres.sehir = _adres.sehir;
            adres.ilce = _adres.ilce;
            adres.mahalle = _adres.mahalle;
            adres.sokak = _adres.sokak;
            adres.bina = _adres.bina;
            adres.daire = _adres.daire;
            db.SaveChanges();
            //Son olarak ilgili acil kaydını kaydeden ve ana sayfaya yönelndiren redirect to action işlemi
            return RedirectToAction("Index","Home");
        }
        //İlgili personel veya hastanın adres id bilgisi ile Adres tablosundan veriyi bularak
        //view'e ileten method.
        public ActionResult AdresBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var adres=db.Adres.Find(id);
            var hastalar = db.Hasta.ToList();
            var personeller = db.Personel.ToList();
            //Adresin personele mi hastaya mı ait olduğunu bulduktan sonra view'e iletme işlemi
            var hasta=db.Hasta.SingleOrDefault(h=>h.adres_id==id);
            var personel = db.Personel.SingleOrDefault(p => p.adres_id == id);

            if (hasta != null) { ViewBag.Tip = "Hasta"; }
            if (personel != null) { ViewBag.Tip = "Personel"; }

            ViewBag.hasta = hasta;
            ViewBag.personel = personel;

            return View(adres);
        }
    }
}