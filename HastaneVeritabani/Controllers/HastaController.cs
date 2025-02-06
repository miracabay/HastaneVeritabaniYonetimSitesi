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

namespace HastaneVeritabani.Controllers
{
    public class HastaController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Hasta
        //Veritabanından hasta tablosundan alınan tüm hastalar için listeleme sayfası methodu
        public ActionResult Index(string arama, string aktifAcilKayit, string aktifRandevuKayit, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Tüm hastaları veritabanından çekme ve arama işlemine göre filtreleme işlemi
            var hastaListesi = db.Hasta.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {//Filtreleme işlemi için küçük bir LINQ sorgusu
                hastaListesi = hastaListesi.Where(h => (h.hasta_ad + " " + h.hasta_soyad).Contains(arama) ||
                                                                h.hasta_tcno.Contains(arama));
            }
            //Son olarak sayfalama, view'e gerekli, filtrelenemiş verileri iletme işlemi
            ViewBag.arama = arama;

            var hastalar = hastaListesi.ToList().ToPagedList(sayfa, 15);

            return View(hastalar);
        }
        //Veritabanındaki Hasta Bilgi STORED PROCEDURE ü kullanarak ilgili hastanın bilgi sayfasına ileten method
        public ActionResult HastaBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //SP kullanımı için parametre, ve query (sql) tanımı
            var sql = "exec sp_HastaBilgi @HastaID = @id";
            var parameters = new[] { new SqlParameter("@id", id) };
            var hasta_bilgi = db.Database.SqlQuery<sp_HastaBilgi_Result>(sql, parameters).ToList();
            //Sayfada gösterilmek için hastanın yaş bilgisini hesaplama
            var hasta = db.Hasta.Find(id);
            var dogum_tarihi = hasta.hasta_dogum_tarihi;
            var tarih = DateTime.Now;
            var yas = tarih.Year - dogum_tarihi.Year;
            if (tarih.Date < dogum_tarihi.AddYears(yas))
            {
                yas--;
            }
            ViewBag.yas = yas;

            return View(hasta_bilgi);
        }

        [HttpGet]
        public ActionResult YeniHasta()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        //Yeni hasta kaydı oluşturmak için yazdığım method
        [HttpPost]
        public ActionResult YeniHasta(Hasta hasta, Adres adres)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Hasta tablosundaki kayıtlarla kontrol yapılması için veritabanından veri alıyoruz
            var hastalar = db.Hasta.ToList();
            //Diğer hastalarla aynı olamayacağından, tc, ad-soyad, telefon kontrolleri
            if (hastalar.Any(h => h.hasta_tcno == hasta.hasta_tcno))
            {
                ViewBag.ErrorMessage = "Bu TC Numarası başka bir hastaya ait. Lütfen başka bir TC Numarası deneyin.";
                return View();
            }

            if (hastalar.Any(h => h.hasta_ad == hasta.hasta_ad && h.hasta_soyad == hasta.hasta_soyad))
            {
                ViewBag.ErrorMessage = "Bu isim ve soyisim başka bir hastaya ait. Lütfen başka bir isim ve soyisim deneyin.";
                return View();
            }

            if (hastalar.Any(h => h.hasta_telefon.Replace(" ", "") == hasta.hasta_telefon.Trim().Replace(" ", "")))
            {
                ViewBag.ErrorMessage = "Bu telefon numarası başka bir hastaya ait. Lütfen başka bir telefon numarası deneyin.";
                return View();
            }
            //Sayfa üzerinden oluşturulan adres bilgisi
            db.Adres.Add(adres);
            db.SaveChanges();
            //Oluşturulan adresi hastaya atama işlemi
            hasta.adres_id = adres.adres_id;
            //Veritabanına hsatayı ekleme, kaydetme ve indexe yönlendirme işlemi
            db.Hasta.Add(hasta);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        //Hasta güncellemesi için Hasta bilgilerini veritabanından alıp view'e ileten yapı ve sayfa
        public ActionResult HastaGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var hasta = db.Hasta.Find(id);
            return View(hasta);
        }
        //Gerekli kontrolleri yaparak hasta güncelleme işlemi için method
        public ActionResult Güncelle(Hasta _hasta)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanındaki diğer hastaları veritabanından alıyoruz
            var hastalar = db.Hasta.ToList();
            var hasta = db.Hasta.Find(_hasta.hasta_id);
            //Kontrol işlemleri
            if (hastalar.Any(h => h.hasta_tcno == _hasta.hasta_tcno && h.hasta_id != _hasta.hasta_id))
            {
                ViewBag.ErrorMessage = "Bu TC No başka bir hastaya ait. Lütfen başka bir TC No deneyin.";
                return View("HastaGetir", hasta);
            }

            if (hastalar.Any(h => h.hasta_ad == _hasta.hasta_ad && h.hasta_soyad == _hasta.hasta_soyad &&
                                    h.hasta_id != _hasta.hasta_id))
            {
                ViewBag.ErrorMessage = "Bu isim ve soyisim başka bir hastaya ait. Lütfen başka bir isim ve soyisim deneyin.";
                return View("HastaGetir", hasta);
            }

            if (hastalar.Any(h => h.hasta_telefon == _hasta.hasta_telefon && h.hasta_id != _hasta.hasta_id))
            {
                ViewBag.ErrorMessage = "Bu telefon numarası başka bir hastaya ait. Lütfen başka bir telefon numarası deneyin.";
                return View("HastaGetir", hasta);
            }
            //Kullanıcının güncelleme (hastagetir) sayfasında bilgileri boş bırakma durumunda bilgilerin korunmasını sağlayan
            //if yapıları
            if (string.IsNullOrEmpty(_hasta.hasta_tcno))
            {
                _hasta.hasta_tcno = hasta.hasta_tcno;
            }

            if (string.IsNullOrEmpty(_hasta.hasta_ad))
            {
                _hasta.hasta_ad = hasta.hasta_ad;
            }

            if (string.IsNullOrEmpty(_hasta.hasta_soyad))
            {
                _hasta.hasta_soyad = hasta.hasta_soyad;
            }

            if (string.IsNullOrEmpty(_hasta.hasta_telefon))
            {
                _hasta.hasta_telefon = hasta.hasta_telefon;
            }

            if (hasta.hasta_cinsiyet == _hasta.hasta_cinsiyet)
            {
                _hasta.hasta_cinsiyet = hasta.hasta_cinsiyet;
            }

            if (_hasta.hasta_dogum_tarihi == null)
            {
                _hasta.hasta_dogum_tarihi = hasta.hasta_dogum_tarihi;
            }
            //Hasta güncelleme işlemleir
            hasta.hasta_tcno = _hasta.hasta_tcno;
            hasta.hasta_ad = _hasta.hasta_ad;
            hasta.hasta_soyad = _hasta.hasta_soyad;
            hasta.hasta_telefon = _hasta.hasta_telefon;
            hasta.hasta_cinsiyet = _hasta.hasta_cinsiyet;
            hasta.hasta_cinsiyet = _hasta.hasta_cinsiyet;
            //Veritabanını kaydeteme ve hasta indexine yönlendirme işlemi
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Olası bir hatalı ekleme durumunda kullanıcı silme işlemi
        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var hasta = db.Hasta.Find(id);
            db.Hasta.Remove(hasta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}