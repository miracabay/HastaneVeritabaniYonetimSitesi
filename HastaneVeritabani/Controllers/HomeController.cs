using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HastaneVeritabani.Models.Entity;

namespace HastaneVeritabani.Controllers
{
    public class HomeController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        public ActionResult Index()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Ana sayfada gösterilmek üzere giriş yap ekranından alınan başarılı olma durumunda yönetici adı
            var yönetici_adi = Session["Username"]?.ToString();
            ViewBag.yönetici_adi = yönetici_adi;
            //Veritabanından STORED PROCEDURE kullanarak çekilen verileri view'e iletme işlemi
            var sql = "EXEC sp_AnaSayfa";
            var anaSayfaVeri = db.Database.SqlQuery<sp_AnaSayfa_Result>(sql).ToList();

            return View(anaSayfaVeri);
        }
    }
}