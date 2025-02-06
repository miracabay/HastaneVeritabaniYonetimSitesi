using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HastaneVeritabani.Models;
using HastaneVeritabani.Models.Entity;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;

namespace HastaneVeritabani.Controllers
{
    public class ReceteController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Recete

        public void ReceteDurumGüncelle()
        {//Reçete tarihlerini kontrol edip tarihi geçen kayıtların durumunu false yapması için oluşturduğum method
            var tarih=DateTime.Now;

            var gecersizReceteler = db.Recete.Where(r => r.recete_son_kullanim_tarihi < tarih &&
                                                    r.recete_durum == true).ToList();

            foreach (var recete in gecersizReceteler)
            {
                recete.recete_durum = false;
                db.SaveChanges();
            }
        }

        public ActionResult Index(string arama, string durum, int? departman_id, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İndex sayfası her açıldığında aktif olmayan reçetelerin tespiti ve durumlarının false yapılması için methodumuzu çağırıyoruz
            ReceteDurumGüncelle();
            //Filtreleme ve arama işlemleri için yazdığım LINQ sorgularım
            var receteListesi = db.Recete.AsQueryable();

            var departmanListesi = db.Departman.ToList();
            ViewBag.departmanListesi = new SelectList(departmanListesi, "departman_id", "departman_ad");
            //Arama ve filtreleme işlemlerim
            if (!string.IsNullOrEmpty(arama))
            {
                int ID;
                bool isInteger = int.TryParse(arama, out ID);

                receteListesi = receteListesi.Where(rl =>
                    (rl.Acil_Kayit.Hasta.hasta_ad + " " + rl.Acil_Kayit.Hasta.hasta_soyad).Contains(arama) ||
                    rl.Acil_Kayit.Hasta.hasta_tcno.Contains(arama) ||
                    (rl.Randevu.Hasta.hasta_ad + " " + rl.Randevu.Hasta.hasta_soyad).Contains(arama) ||
                    rl.Randevu.Hasta.hasta_tcno.Contains(arama) ||
                    (isInteger && rl.acil_id == ID) || (isInteger && rl.randevu_id == ID));
            }

            if (!string.IsNullOrEmpty(durum))
            {
                if (durum == "gecerli")
                {
                    receteListesi = receteListesi.Where(rl => rl.recete_durum == true);
                }
                else if (durum == "gecersiz")
                {
                    receteListesi = receteListesi.Where(rl => rl.recete_durum == false);
                }
            }

            if (departman_id.HasValue)
            {
                if (departman_id == 1)
                {
                    receteListesi = receteListesi.Where(rl => rl.Acil_Kayit != null);
                }
                else
                {
                    receteListesi = receteListesi.Where(rl => rl.Randevu != null && rl.Randevu.departman_id == departman_id);
                }
            }
            //İlgili verileri ve işlenmiş, sayfalandırılmış reçeteler listesini view'e aktaran yapı
            var receteler = receteListesi.ToList().ToPagedList(sayfa, 15);

            ViewBag.arama = arama;
            ViewBag.durum = durum;
            return View(receteler);
        }

        public ActionResult ReceteBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İlgili reçetenin bilgi sayfasında veritabanından n:n ilişkisi sebebiyle oluşturduğum
            //ilac_recete tablosundan veri çeken LINQ sorgusu
            var recete = db.Recete
                .Include(r => r.Ilac_Recete)
                .FirstOrDefault(r => r.recete_id == id);

            if (recete.acil_id != null)
            {
                ViewBag.Tip = "Acil";
            }

            if (recete.randevu_id != null)
            {
                ViewBag.Tip = "Randevu";
            }

            return View(recete);
        }

        [HttpGet]
        public ActionResult YeniRecete()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni reçete oluşturma sayfasında get işlemi ile gelmesi gereken kullanıcının seçmesi için ilaç tablosuna ait LINQ sorgusu
            var ilaclar = db.Ilac.ToList();
            ViewBag.ilaclar = new SelectList(ilaclar, "ilac_id", "ilac_ad");

            return View();
        }

        [HttpPost]
        public ActionResult YeniRecete(Recete recete, string kayit_tipi, int acil_randevuID)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni reçete sayfasında post işlemi yapıldığında gereken kontrollerin yapılması için LINQ sorguları
            ViewBag.KayitTipi = kayit_tipi;
            var acilKayitListesi = db.Acil_Kayit.ToList();
            var randevuListesi = db.Randevu.ToList();
            var receteListesi = db.Recete.ToList();
            //ilgili kontroller için if yapıları
            if (kayit_tipi == "acil")
            {
                recete.acil_id = acil_randevuID;
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

                if (db.Recete.Any(r=>r.acil_id==acil_randevuID))
                {
                    ViewBag.ErrorMessage = "Belirtilen acil kaydına ait reçete bilgisi bulunmaktadır. Lütfen başka bir acil kaydı deneyin.";
                    return View();
                }
            }

            if (kayit_tipi == "randevu")
            {
                recete.randevu_id = acil_randevuID;
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

                if (db.Recete.Any(r => r.randevu_id == acil_randevuID))
                {
                    ViewBag.ErrorMessage = "Belirtilen randevu kaydına ait reçete bilgisi bulunmaktadır. Lütfen başka bir randevu kaydı deneyin.";
                    return View();
                }
            }
            //Tüm kontrollerden geçen bir reçete kaydı için veriyi veritabanında oluşturup
            //ilgili reçetenin bilgi sayfasına yönlendirme yapan yapı
            recete.recete_durum = true;
            db.Recete.Add(recete);
            db.SaveChanges();
            return RedirectToAction("ReceteBilgi", "Recete", new { id=recete.recete_id });
        }

        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Olası bir yanlış reçete ekleme durumunda silme işlemi için oluşturduğum yapı
            var recete = db.Recete.Find(id);
            db.Recete.Remove(recete);
            db.SaveChanges();
            return RedirectToAction("Index", "Recete");
        }
        [HttpGet]
        public ActionResult IlacEkle(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Reçete bilgi sayfasında ilaç ekleme butonuna basıldığında viewe iletilmesi gereken veriler.
            var ilaclar = db.Ilac.ToList();
            ViewBag.ilaclar = new SelectList(ilaclar, "ilac_id", "ilac_ad");
            return View();
        }

        [HttpPost]
        public ActionResult IlacEkle(int id, int ilac_id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Reçete bilgi>ilaç ekleme sayfasında post işlemi yapıldığında gerekli kontrolleri yapması
            //ve sonrasında ilgili reçete için ilac_recete tablosuna veri eklemesi için yazdığım LINQ sorgusu
            var ilaclar = db.Ilac.ToList();
            ViewBag.ilaclar = new SelectList(ilaclar, "ilac_id", "ilac_ad");

            if (db.Recete.Any(r => r.recete_id == id && r.recete_durum == false))
            {
                ViewBag.ErrorMessage = "Geçersiz durumda olan reçete kayıtlarına ilaç ekleme işlemi yapılmamaktadır.";
                return View();
            }
            //Kontrolden geçen durumda (reçetenin aktif olma durumunda) reçeteye ilaç ekleme yapısı
            var ilacRecete = new Ilac_Recete();
            ilacRecete.recete_id = id;
            ilacRecete.ilac_id = ilac_id;
            db.Ilac_Recete.Add(ilacRecete);
            db.SaveChanges();

            return RedirectToAction("ReceteBilgi", new { id = id });
        }

        public ActionResult IlacSil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Reçeteye hatalı bir ilaç eklenmesi durumunda ilacı veritabanı ilac_recete tablosundan silip ilgili reçetenin bilgi sayfasına yönlendiren yapı
            var ilac_recete = db.Ilac_Recete.Find(id);
            db.Ilac_Recete.Remove(ilac_recete);
            db.SaveChanges();

            return RedirectToAction("ReceteBilgi", new { id = ilac_recete.recete_id });
        }

    }
}