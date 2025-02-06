using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HastaneVeritabani.Models.Entity;
using PagedList;
using PagedList.Mvc;

namespace HastaneVeritabani.Controllers
{
    public class YöneticiController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Yönetici

        public ActionResult Index(string arama, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanından yönetici tablosundaki verileri alıp filtreleme işlemleri için
            //listeleyen LINQ sorgusu
            var yöneticiListesi=db.Yönetici.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                yöneticiListesi=yöneticiListesi.Where(y=>y.kullanici_adi.Contains(arama));
            }

            ViewBag.arama = arama;
            //Filtrelenmiş yapıyı sayfalayarak view'e ileten yapı
            var yöneticiler = yöneticiListesi.ToList().ToPagedList(sayfa, 15);

            return View(yöneticiler);
        }

        [HttpGet]
        public ActionResult YeniYönetici()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View();
        }
        [HttpPost]
        public ActionResult YeniYönetici(Yönetici yönetici, string sifre_dogrulama)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni yönetici oluşturma ekranında kontrollerin yapılması için oluşturduğum LINQ sorgusu
            //Veritabanından yönetici tablosundan verileri alır
            var yöneticiler=db.Yönetici.ToList();
            //Kontrol işlemleri
            if (yöneticiler.Any(y => y.kullanici_adi == yönetici.kullanici_adi)){
                ViewBag.ErrorMessage = "Bu isimde başka bir yönetici mevcut. Lütfen başka bir isim deneyin.";
                return View();
            }

            if (yönetici.sifre != sifre_dogrulama)
            {
                ViewBag.ErrorMessage = "Şifre doğrulama başarısız. Lütfen tekrar deneyin.";
                return View();
            }
            //Kontrolleri geçen bir yönetici girişi için yeni yönetici oluşturma ve indexe yönlendirme yapısı
            db.Yönetici.Add(yönetici);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Hatalı bir yönetici eklenmesi durumunda silme işlemi için oluşturduğum method
            //Veritabanında id 1 olan (admin) yöneticisinin her zaman kalması için oluşturduğum if yapısı
            if (id == 1)
            {
                ViewBag.ErrorMessage = "Bu yönetici değiştirilemez ve silinemez.";
                return View("YöneticiGetir");
            }

            //Veritabanında ilgili idyle gelen yöneticiyi silme ve indexe yönlendirme işlemi
            var yönetici=db.Yönetici.Find(id);
            db.Yönetici.Remove(yönetici);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult YöneticiGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yönetici güncelleme butonuna tıklandığında ilgili yöneticinin bilgilerini view olarak aktaran yapı
            var yönetici = db.Yönetici.Find(id);
            return View(yönetici);
        }

        public ActionResult Güncelle(Yönetici _yönetici, string sifre_dogrulama)
        {
            //Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //YöneticiGetir indexinde post işlemi yapıldığı durumda çağrılan güncelleme methodu
            //Kontrollerin yapılması adına oluşturduğum veritabanından bilgi alan LINQ sorgusu
            var yöneticiler = db.Yönetici.ToList();
            var yönetici = db.Yönetici.Find(_yönetici.id);
            //Yönetici ID 1 kontrolü (veritabanında her zaman giriş yapılabilmesi için en az bir yönetici bulunması için)
            if (_yönetici.id == 1)
            {
                ViewBag.ErrorMessage = "Bu yönetici değiştirilemez ve silinemez.";
                return View("YöneticiGetir", yönetici);
            }
            //Diğer kontroller
            if (yöneticiler.Any(y => y.kullanici_adi == _yönetici.kullanici_adi && y.id != _yönetici.id))
            {
                ViewBag.ErrorMessage = "Bu isimde başka bir yönetici mevcut. Lütfen başka bir isim deneyin.";
                return View("YöneticiGetir", yönetici);
            }

            if (_yönetici.sifre != sifre_dogrulama)
            {
                ViewBag.ErrorMessage = "Şifreler uyuşmuyor. Lütfen tekrar deneyin.";
                return View("YöneticiGetir", yönetici);
            }
            //Yönetici adı girilmediği durumda yönetici adını eski adı olarak ayarlayan yapı
            if (string.IsNullOrEmpty(_yönetici.kullanici_adi))
            {
                _yönetici.kullanici_adi = yönetici.kullanici_adi;
            }
            //Yeni yöneticiyi veritabanına kaydettikten indexe yönlendirme yapan yapı
            yönetici.kullanici_adi = _yönetici.kullanici_adi;
            yönetici.sifre = _yönetici.sifre;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}