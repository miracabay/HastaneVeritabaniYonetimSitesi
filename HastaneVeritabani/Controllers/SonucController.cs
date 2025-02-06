using HastaneVeritabani.Models.Entity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using PagedList;
using PagedList.Mvc;
using System.Windows.Forms;

namespace HastaneVeritabani.Controllers
{
    public class SonucController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Sonuc
        public ActionResult Index()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //MVC'de oluşan bir hata sebebiyle oluşturduğum index, kullanılmıyor.

            return View();
        }

        public ActionResult CikmisSonuclar(string arama, string test_tipi, int? departman_id, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanında sonuc tablosundan sonuç durumu true, olan yani çıkmış tahlil sonuçlarını listeleyen sayfa
            //ve bu sayfaya ait filtreleme işlemlerinin yapılması için oluşturduğum LINQ yapıalrı.
            var sonucListesi = db.Sonuc.AsQueryable();
            var cikmisSonucListesi = sonucListesi.Where(csl => csl.sonuc_durum == true);

            var departmanListesi = db.Departman.ToList();
            ViewBag.departmanListesi = new SelectList(departmanListesi, "departman_id", "departman_ad");

            var testTipleri = db.Sonuc
                .Select(tt => tt.sonuc_test_tipi)
                .Distinct()
                .ToList();
            ViewBag.testTipleri = testTipleri;
            ViewBag.secilenTestTipi = test_tipi;
            //Filtreleme işlemleri (hasta/personel ad-soyad/tcno veya acil/randevu id)
            if (!string.IsNullOrEmpty(arama))
            {
                int ID;
                bool isInteger = int.TryParse(arama, out ID);

                cikmisSonucListesi = cikmisSonucListesi.Where(csl =>
                    (csl.Acil_Kayit.Hasta.hasta_ad + " " + csl.Acil_Kayit.Hasta.hasta_soyad).Contains(arama) ||
                    csl.Acil_Kayit.Hasta.hasta_tcno.Contains(arama) ||
                    (csl.Randevu.Hasta.hasta_ad + " " + csl.Randevu.Hasta.hasta_soyad).Contains(arama) ||
                    csl.Randevu.Hasta.hasta_tcno.Contains(arama) ||
                    (isInteger && csl.acil_id == ID) || (isInteger && csl.randevu_id == ID));
            }
            //Departman için filtreleme işlemi
            if (departman_id.HasValue)
            {
                if (departman_id == 1)
                {
                    cikmisSonucListesi = cikmisSonucListesi.Where(csl => csl.Acil_Kayit != null);
                }
                else
                {
                    cikmisSonucListesi = cikmisSonucListesi.Where(csl => csl.Randevu != null && csl.Randevu.departman_id == departman_id);
                }
            }

            //Test tipi için filtreleme işlemi
            if (!string.IsNullOrEmpty(test_tipi))
            {
                cikmisSonucListesi = cikmisSonucListesi.Where(csl => csl.sonuc_test_tipi == test_tipi);
            }
            //İşlenmiş veriyi view'e aktaran ve sayfalayan yapı
            var sonuclar = cikmisSonucListesi.ToList().ToPagedList(sayfa, 15);

            ViewBag.arama = arama;
            return View(sonuclar);
        }
        public ActionResult CikmisSonucSil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Hatalı eklenen bir true durumundaki tahlil sonucu için silme işlemi
            var sonuc = db.Sonuc.Find(id);
            db.Sonuc.Remove(sonuc);
            db.SaveChanges();
            return RedirectToAction("CikmisSonuclar","Sonuc");
        }
        public ActionResult BekleyenSonucSil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Hatalı eklenen bir false durumundaki sonuç içi silme işlemi
            var sonuc = db.Sonuc.Find(id);
            db.Sonuc.Remove(sonuc);
            db.SaveChanges();
            return RedirectToAction("BekleyenSonuclar", "Sonuc");
        }

        [HttpGet]
        public ActionResult YeniSonuc()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult YeniSonuc(Sonuc sonuc, string kayit_tipi, int acil_randevuID)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni tahlil ekleme ekranında kontrollerin yapılması için oluşturduğum LINQ sorguları
            ViewBag.KayitTipi = kayit_tipi;
            var acilKayitListesi = db.Acil_Kayit.ToList();
            var randevuListesi = db.Randevu.ToList();

            if (kayit_tipi == "acil")
            {
                sonuc.acil_id = acil_randevuID;
                var acilKayit = db.Acil_Kayit.FirstOrDefault(a => a.acil_id == acil_randevuID);

                if (acilKayit == null)
                {
                    ViewBag.ErrorMessage = "Veritabanında belirtilen acil kayıt numarası bulunmuyor. Lütfen başka bir acil kayıt numarası deneyin.";
                    return View();
                }

                if (acilKayit.acil_kayit_durum == false)
                {
                    ViewBag.ErrorMessage = "Belirtilen acil kaydı aktif değil. Lütfen başka bir acil kaydı deneyin.";
                    return View();
                }
            }

            if (kayit_tipi == "randevu")
            {
                sonuc.randevu_id = acil_randevuID;
                var randevu = db.Randevu.FirstOrDefault(r => r.randevu_id == acil_randevuID);

                if (randevu == null)
                {
                    ViewBag.ErrorMessage = "Veritabanında belirtilen randevu kayıt numarası bulunmuyor. Lütfen başka bir randevu kayıt numarası deneyin.";
                    return View();
                }

                if (randevu.randevu_durum == false)
                {
                    ViewBag.ErrorMessage = "Belirtilen randevu kaydı aktif değil. Lütfen başka bir randevu kaydı deneyin.";
                    return View();
                }
            }
            //Yeni tahlil sonucunun durumunu false yaptıktan sonra veritabanına ekleyen yapı
            sonuc.sonuc_durum = false;
            db.Sonuc.Add(sonuc);
            db.SaveChanges();
            return RedirectToAction("BekleyenSonuclar", "Sonuc");
        }
        public ActionResult BekleyenSonuclar(string arama, string test_tipi, int? departman_id, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Durumu false olan yani çıkmamış tahlil sonuçları için index sayfası ve LINQ sorguları
            var sonucListesi = db.Sonuc.AsQueryable();
            var cikmisSonucListesi = sonucListesi.Where(csl => csl.sonuc_durum == false);
            //Filtreleme işlemleri için departman ve test tipleri LINQ sorguları
            var departmanListesi = db.Departman.ToList();
            ViewBag.departmanListesi = new SelectList(departmanListesi, "departman_id", "departman_ad");

            var testTipleri = db.Sonuc
                .Select(tt => tt.sonuc_test_tipi)
                .Distinct()
                .ToList();
            ViewBag.testTipleri = testTipleri;
            ViewBag.secilenTestTipi = test_tipi;
            //Filtreleme işlemleri (personel/hasta tcno-ad-soyad veya acil/randevu id)
            if (!string.IsNullOrEmpty(arama))
            {
                int ID;
                bool isInteger = int.TryParse(arama, out ID);

                cikmisSonucListesi = cikmisSonucListesi.Where(csl =>
                    (csl.Acil_Kayit.Hasta.hasta_ad + " " + csl.Acil_Kayit.Hasta.hasta_soyad).Contains(arama) ||
                    csl.Acil_Kayit.Hasta.hasta_tcno.Contains(arama) ||
                    (csl.Randevu.Hasta.hasta_ad + " " + csl.Randevu.Hasta.hasta_soyad).Contains(arama) ||
                    csl.Randevu.Hasta.hasta_tcno.Contains(arama) ||
                    (isInteger && csl.acil_id == ID) || (isInteger && csl.randevu_id == ID));
            }
            //Departman filtreleme işlemleri
            if (departman_id.HasValue)
            {
                if (departman_id == 1)
                {
                    cikmisSonucListesi = cikmisSonucListesi.Where(csl => csl.Acil_Kayit != null);
                }
                else
                {
                    cikmisSonucListesi = cikmisSonucListesi.Where(csl => csl.Randevu != null && csl.Randevu.departman_id == departman_id);
                }
            }

            //Test tipi filtreleme işlemleri
            if (!string.IsNullOrEmpty(test_tipi))
            {
                cikmisSonucListesi = cikmisSonucListesi.Where(csl => csl.sonuc_test_tipi == test_tipi);
            }
            //Filtrelenmiş verileri sayfalayarak view'e ileten yapı
            var sonuclar = cikmisSonucListesi.ToList().ToPagedList(sayfa, 15);

            ViewBag.arama = arama;
            return View(sonuclar);
        }

        public ActionResult SonucBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İlgili sonucun bilgilerini alarak view'e ileten yapı
            var sonuc = db.Sonuc.Find(id);

            if (sonuc.acil_id != null)
            {
                string tip = "Acil";
                ViewBag.Tip = tip;
            }

            if (sonuc.randevu_id != null)
            {
                string tip = "Randevu";
                ViewBag.Tip = tip;
            }

            return View(sonuc);
        }
        [HttpGet]
        public ActionResult Sonuclandir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Bekleyen tahliller sayfasındaki sonuçlandır sayfası için oluşturulan method
            var sonuc = db.Sonuc.Find(id);

            ViewBag.TahlilTürü = sonuc.sonuc_test_tipi;
            if (sonuc.acil_id != null)
            {
                ViewBag.AcilRandevuID = sonuc.acil_id;
            }
            else
            {
                ViewBag.AcilRandevuID = sonuc.randevu_id;
            }

            return View(id);
        }
        [HttpPost]
        public ActionResult Sonuclandir(int id, string sonuc_aciklama)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Tahlil sonuçlandırma ekranında kullanıcıdan veri alarak ilgili tahlil kaydını
            //veritabanında bulup durumunu true yaptıktan sonra kullanıcıdna gelen veriyi
            //açıklama olarak ekleyen yapı
            var sonuc = db.Sonuc.Find(id);

            sonuc.sonuc_aciklama = sonuc_aciklama;
            sonuc.sonuc_durum = true;
            db.SaveChanges();

            return RedirectToAction("SonucBilgi", new { id = id });
        }

        public ActionResult SonucGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Tahlil güncelleme işlemi için oluşturulan view'e veri aktaran yapı
            var sonuc = db.Sonuc.Find(id);

            ViewBag.TahlilTürü = sonuc.sonuc_test_tipi;
            if (sonuc.acil_id != null)
            {
                ViewBag.AcilRandevuID = sonuc.acil_id;
            }
            else
            {
                ViewBag.AcilRandevuID = sonuc.randevu_id;
            }

            ViewBag.SonucAciklama = sonuc.sonuc_aciklama;
            return View(sonuc);
        }

        public ActionResult Güncelle(Sonuc _sonuc)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Tahlil sonucu güncellenmesi için sonucgetir indexinden veri alarak işleyen veri güncelleme işemi yapan yapı
            var sonuc = db.Sonuc.Find(_sonuc.sonuc_id);
            sonuc.sonuc_aciklama = _sonuc.sonuc_aciklama;
            db.SaveChanges();

            return RedirectToAction("SonucBilgi", new { id = _sonuc.sonuc_id });
        }
    }
}