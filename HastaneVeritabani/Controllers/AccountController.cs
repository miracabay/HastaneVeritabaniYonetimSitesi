using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HastaneVeritabani.Models.Entity;

namespace HastaneVeritabani.Controllers
{
    public class AccountController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();

        public ActionResult Login()
        {
            return View();
        }

        //View kısmında kullanıcıdan alınan kullanıcı ad ıve şifre ile giriş kontrolü
        [HttpPost]
        public ActionResult Login(string girilenUsername, string girilenPassword)
        {

            var yoneticiler = db.Yönetici.ToList();
            var girisYapanYonetici = yoneticiler.FirstOrDefault(
                y => y.kullanici_adi == girilenUsername &&
                     y.sifre == girilenPassword);

            if (girisYapanYonetici != null)
            {
                Session["UserLoggedIn"] = "true";
                Session["Username"]=girisYapanYonetici.kullanici_adi;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı. Lütfen tekrar deneyin.";
                return View();
            }
            
        }

        //MainLayout kısmında sağ üst kısımda bulunan Çıkış Yap methodu.
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("UserLoggedIn");
            return RedirectToAction("Login");
        }
    }
}
