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
    public class MalzemeController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Malzeme
        public ActionResult Index()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //Bir hata oluştuğundan dolayı direkt index kullanılmıyor olsa da sitede yazma mecburiyetinde kaldım.

            return View();
        }
        //MALZEME STOK KISMI
        //Malzeme stokları için sayfalama, filtreleme işlemi
        public ActionResult MalzemeStok(string arama, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Malzeme stok tablosundan verileri işlenebilir bir halde almak için LINQ sorgusu
            var malzemeListesi = db.Malzeme_Stok.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                malzemeListesi = malzemeListesi.Where(m => m.malzeme_ad.Contains(arama));
            }
            //Veritabanından alınıp filtrelenen veriler için sayfalama ve view'e aktarma işlemi
            ViewBag.arama = arama;
            var malzemeler = malzemeListesi.ToList().ToPagedList(sayfa, 15);

            return View(malzemeler);
        }

        [HttpGet]
        public ActionResult YeniMalzemeStok()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View();
        }
        //Yeni malzeme ekleme işlemi
        [HttpPost]
        public ActionResult YeniMalzemeStok(Malzeme_Stok malzeme)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Malzme ad kontrolü sağlanması ve aynı isimde başka bir malzeme eklenmemesi için
            //veritabanından stok tablosunu çeken LINQ sorgusu
            var malzemeler = db.Malzeme_Stok.ToList();

            if (malzemeler.Any(m => m.malzeme_ad == malzeme.malzeme_ad))
            {
                ViewBag.ErrorMessage = "Bu isimde başka bir malzeme mevcut. Lütfen başka bir isim deneyin.";
                return View();
            }
            //Malzeme stok olarak veriyi ekleyip, kaydedip, malzeme stok indexine yönlendirme işlemi
            db.Malzeme_Stok.Add(malzeme);
            db.SaveChanges();
            return RedirectToAction("MalzemeStok", "Malzeme");
        }
        //Olası bir hatalı ekleme durumunda ilgili malzeme silme işlemi
        public ActionResult MalzemeStokSil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var malzeme = db.Malzeme_Stok.Find(id);
            db.Malzeme_Stok.Remove(malzeme);
            db.SaveChanges();
            return RedirectToAction("MalzemeStok");
        }
        //İlgili malzeme düzenleme işlemi için sayfaya veri iletme methodu
        public ActionResult MalzemeStokGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var malzeme = db.Malzeme_Stok.Find(id);
            return View(malzeme);
        }
        //MalzemeStokGetir sayfasındaki güncelleme işlemini yapan method
        public ActionResult MalzemeStokGüncelle(Malzeme_Stok _malzeme)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Kontrollerin sağlanması adına yazdığım LINQ sorguları ve if yapıları
            var malzemeler = db.Malzeme_Stok.ToList();
            var malzeme = db.Malzeme_Stok.Find(_malzeme.malzeme_id);

            if (malzemeler.Any(m => m.malzeme_ad == _malzeme.malzeme_ad && m.malzeme_id != _malzeme.malzeme_id))
            {
                ViewBag.ErrorMessage = "Bu isimde başka bir malzeme mevcut. Lütfen başka bir isim deneyin.";
                return View("MalzemeStokGetir", malzeme);
            }
            //Kullanıcının boş geçme durumunda otomatik olarak eski ismi kullanan if yapısı
            if (string.IsNullOrEmpty(_malzeme.malzeme_ad))
            {
                _malzeme.malzeme_ad = malzeme.malzeme_ad;
            }
            //Güncelleme, kaydetme ve sonrasında malzeme stok indexine yönlendirme işlemi
            malzeme.malzeme_ad = _malzeme.malzeme_ad;
            db.SaveChanges();
            return RedirectToAction("MalzemeStok");
        }

        //MALZEME KULLANIM KISMI
        public ActionResult MalzemeKullanim(string arama, int? departman_id, int? malzeme_id, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Malzeme kullanım listesi filtreleme ve arama işlemleri için LINQ sorguları
            var malzemeKullanimListesi = db.Malzeme_Kullanim.AsQueryable();
            var hastaListesi = db.Hasta.ToList();
            var personelListesi = db.Personel.ToList();
            var acilListesi = db.Acil_Kayit.ToList();
            var randevuListesi = db.Randevu.ToList();
            var departmanListesi = db.Departman.ToList();
            var malzemeListesi = db.Malzeme_Stok.ToList();

            if (departman_id != null)
            {
                if (departman_id == 1)
                {
                    malzemeKullanimListesi = malzemeKullanimListesi.Where(mkl => mkl.randevu_id == null);
                }
                else
                {
                    malzemeKullanimListesi = malzemeKullanimListesi.Where(mkl => mkl.Randevu.departman_id == departman_id);
                }
            }

            if (malzeme_id != null)
            {
                malzemeKullanimListesi = malzemeKullanimListesi.Where(mkl => mkl.malzeme_id == malzeme_id);
            }
            //Arama işlemini hasta ad-soyad/tcno veya personel ad-soyad/tcno veya acil veya randevu kayıt idsine göre yaptım
            if (!string.IsNullOrEmpty(arama))
            {
                int ID;
                bool isInteger = int.TryParse(arama, out ID);

                malzemeKullanimListesi = malzemeKullanimListesi.Where(aak =>
                    (aak.Acil_Kayit.Hasta.hasta_ad + " " + aak.Acil_Kayit.Hasta.hasta_soyad).Contains(arama) ||
                    aak.Acil_Kayit.Hasta.hasta_tcno.Contains(arama) ||
                    (aak.Randevu.Hasta.hasta_ad + " " + aak.Randevu.Hasta.hasta_soyad).Contains(arama) ||
                    aak.Randevu.Hasta.hasta_tcno.Contains(arama) ||
                    (aak.Acil_Kayit.Personel.personel_ad + " " + aak.Acil_Kayit.Personel.personel_soyad).Contains(arama) ||
                    aak.Acil_Kayit.Personel.personel_tcno.Contains(arama) ||
                    (aak.Randevu.Personel.personel_ad + " " + aak.Randevu.Personel.personel_soyad).Contains(arama) ||
                    aak.Randevu.Personel.personel_tcno.Contains(arama) ||
                    (isInteger && aak.acil_id == ID) || (isInteger && aak.randevu_id == ID));
            }
            //View'e aktarılacak olan veriler
            ViewBag.arama = arama;
            ViewBag.departmanListesi = new SelectList(departmanListesi, "departman_id", "departman_ad");
            ViewBag.malzemeListesi = new SelectList(malzemeListesi, "malzeme_id", "malzeme_ad");
            //Filtrelenmiş ve sayfalanmış veriyi view'e aktarma işlemi
            var malzemeKullanimlar = malzemeKullanimListesi.ToList().ToPagedList(sayfa, 15);

            return View(malzemeKullanimlar);
        }
        [HttpGet]
        public ActionResult YeniMalzemeKullanim()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni malzeme kullanım sayfasına sayfa yüklendiğinde aktarılacak olan veriler
            var malzemeler = db.Malzeme_Stok.ToList();
            ViewBag.malzemeler = new SelectList(malzemeler, "malzeme_id", "malzeme_ad");

            return View();
        }

        [HttpPost]
        public ActionResult YeniMalzemeKullanim(Malzeme_Kullanim malzemeKullanim, string kayit_tipi, int acil_randevuID)
        {
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni malzeme kullanım sayfası için aktarılacak viewler
            ViewBag.KayitTipi = kayit_tipi;
            var malzemeler = db.Malzeme_Stok.ToList();
            ViewBag.malzemeler = new SelectList(malzemeler, "malzeme_id", "malzeme_ad");

            var acilKayitListesi = db.Acil_Kayit.ToList();
            var randevuListesi = db.Randevu.ToList();
            var malzemeListesi = db.Malzeme_Stok.ToList();
            //Yine sayfa için kontroller if yapıları
            if (kayit_tipi == "acil")
            {
                malzemeKullanim.acil_id = acil_randevuID;
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
                malzemeKullanim.randevu_id = acil_randevuID;
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
            //Stok kontrol işlemi
            var malzeme = db.Malzeme_Stok.FirstOrDefault(m => m.malzeme_id == malzemeKullanim.malzeme_id);
            if (malzemeKullanim.kullanim_adet > malzeme.malzeme_adet)
            {
                ViewBag.ErrorMessage = "Belirtilen malzeme miktarı için stok yetersizdir.";
                return View();
            }
            //Veri ekleme, kaydetme ve malzeme kullanım indexine yönlendirme işlemi
            db.Malzeme_Kullanim.Add(malzemeKullanim);
            db.SaveChanges();
            return RedirectToAction("MalzemeKullanim", "Malzeme");
        }

        public ActionResult MalzemeKullanimSil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Olası bir yanlış malzeme kullanım kaydı girilme durumunda silme, veritabanını kaydetme ve malzeme kullanım indexine yönlendirme işlemi
            var malzemeKullanim = db.Malzeme_Kullanim.Find(id);
            db.Malzeme_Kullanim.Remove(malzemeKullanim);
            db.SaveChanges();
            return RedirectToAction("MalzemeKullanim");
        }

        //MALZEME ALIM KISMI
        public ActionResult MalzemeAlim(string arama, int? malzeme_id, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Malzeme alım sayfası için LINQ sorguları ve arama/filtrleme işlemleri
            var malzemeAlimListesi = db.Malzeme_Alim.AsQueryable();
            var malzemeListesi = db.Malzeme_Stok.ToList();

            if (malzeme_id != null)
            {
                malzemeAlimListesi = malzemeAlimListesi.Where(mal => mal.malzeme_id == malzeme_id);
            }

            if (!string.IsNullOrEmpty(arama))
            {
                malzemeAlimListesi = malzemeAlimListesi.Where(mal => mal.alim_firma.Contains(arama));
            }

            //MalzemeAlim viewine viewbag olarak veri gönderme, filtrelenmiş verileri sayfalama ve indexe parametre olarak iletme işlemi
            ViewBag.arama = arama;
            ViewBag.malzemeListesi = new SelectList(malzemeListesi, "malzeme_id", "malzeme_ad");

            var malzemeAlimlar = malzemeAlimListesi.ToList().ToPagedList(sayfa, 15);

            return View(malzemeAlimlar);
        }

        [HttpGet]
        public ActionResult YeniMalzemeAlim()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni malzeme alım kaydı sayfası açıldığında aktarılması gereken malzemelerin seçilmesi için gerekli olan LINQ sorgusu
            var malzemeler = db.Malzeme_Stok.ToList();
            ViewBag.malzemeler = new SelectList(malzemeler, "malzeme_id", "malzeme_ad");

            return View();
        }

        [HttpPost]
        public ActionResult YeniMalzemeAlim(Malzeme_Alim malzemeAlim)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni malzeme alım sayfasında post işlemi yapıldıktan sonra veritabanına veri ekleme işlemi
            var malzemeler = db.Malzeme_Stok.ToList();
            ViewBag.malzemeler = new SelectList(malzemeler, "malzeme_id", "malzeme_ad");

            var malzemeListesi = db.Malzeme_Stok.ToList();

            var malzeme = db.Malzeme_Stok.FirstOrDefault(m => m.malzeme_id == malzemeAlim.malzeme_id);

            db.Malzeme_Alim.Add(malzemeAlim);
            db.SaveChanges();
            return RedirectToAction("MalzemeAlim", "Malzeme");
        }

        public ActionResult MalzemeAlimSil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Olası bir hatalı kayıt ekleme durumunda veritabanından veri sildikten sonra Malzeme Alım indexine yönelndirme işlemi
            var malzemeAlim = db.Malzeme_Alim.Find(id);
            db.Malzeme_Alim.Remove(malzemeAlim);
            db.SaveChanges();
            return RedirectToAction("MalzemeAlim");
        }
    }
}