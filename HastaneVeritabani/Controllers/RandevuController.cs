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
using System.Data.Entity;

namespace HastaneVeritabani.Controllers
{
    public class RandevuController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Randevu

        public void RandevuDurumGüncelle()
        {//Randevu tarihi geçmiş olan kayıtların randevu durumunu değiştirmek için oluşturduğum method
            //Kendi kendine işletilme şansı yok bu sebeple her index açıldığında çağıracağız.
            var tarih = DateTime.Now;

            var tarihiGecmisRandevular = db.Randevu.Where(r => r.randevu_tarih < tarih &&
                                                    r.randevu_durum == true).ToList();
            
            foreach (var randevu in tarihiGecmisRandevular)
            {
                randevu.randevu_durum = false;
                db.SaveChanges();
            }
        }

        public ActionResult Index()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //Yine mvc kaynaklı bir problem sebebiyle yazdım, kullanılmıyor.

            return View();
        }

        [HttpGet]
        public ActionResult YeniRandevu()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni randevu sayfası açıldığında post işlemi yapılmadan önce gönderilmesi gereken veriler
            var departmanlar = db.Departman.Where(d => d.departman_id != 1).ToList();
            ViewBag.departmanlar = new SelectList(departmanlar, "departman_id", "departman_ad");

            return View();
        }
        [HttpPost]
        public ActionResult YeniRandevu(Randevu randevu, string hasta_tcno, int departman_id, DateTime tarih)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni randevu sayfasında post işlemi yapıldıktan sonra yapılacak işlemler için LINQ sorgularım
            var departmanlar = db.Departman.Where(d => d.departman_id != 1).ToList();
            ViewBag.departmanlar = new SelectList(departmanlar, "departman_id", "departman_ad");

            var hasta = db.Hasta.SingleOrDefault(h => h.hasta_tcno == hasta_tcno);
            //Kullanıcı tarafından girilecek olan departmana ait aktif doktorları personel listesinden alan  LINQ sorgusu
            var departmanAktifDoktorlari = db.Personel.Where(p => p.departman_id == departman_id &&
                                        p.personel_durum == true &&
                                        db.Meslek.Any(m => m.meslek_id == p.meslek_id &&
                                        m.meslek_ad.ToLower().Contains("doktor"))).ToList();
            //Hata kontrolleri
            if (hasta == null)
            {
                ViewBag.ErrorMessage = "Girilen TC Numarası ile eşleşen bir hasta kaydı bulunamadı. Lütfen önce Hasta Kayıt işlemini yapın.";
                return View();
            }

            if (!departmanAktifDoktorlari.Any())
            {
                ViewBag.ErrorMessage = "Seçilen departmanda atanabilecek aktif bir doktor bulunamadı.";
                return View();
            }

            var hastaAktifRandevu = db.Randevu.Any(r => r.hasta_id == hasta.hasta_id && r.departman_id == departman_id && r.randevu_durum == true);

            if (hastaAktifRandevu)
            {
                ViewBag.ErrorMessage = "Hastanın bu departmanda zaten aktif bir randevu kaydı bulunmaktadır.";
                return View();
            }

            if (tarih < DateTime.Now)
            {
                ViewBag.ErrorMessage = "Geçmiş tarihe ait bir randevu kaydı oluşturulamaz. Lütfen geçerli bir tarih giriniz.";
                return View();
            }

            if (tarih > DateTime.Now.AddYears(1))
            {
                ViewBag.ErrorMessage = "Bir yıl ve sonrasına ait bir randevu kaydı oluşturulamaz. Lütfen geçerli bir tarih giriniz.";
                return View();
            }

            var departman = db.Departman.SingleOrDefault(d => d.departman_id == departman_id);
            if (departman == null || !departman.departman_durum)
            {
                ViewBag.ErrorMessage = "Seçilen departman pasif durumdadır. Lütfen başka bir departman seçiniz.";
                return View();
            }


            if (tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday)
            {
                ViewBag.ErrorMessage = "Hafta sonuna ait bir randevu kaydı oluşturulamaz. Lütfen hafta içi bir gün seçiniz.";
                return View();
            }
            //Randevu saatlerinin otomatik olarak atanması için oluşturduğum yapı
            //08.00 ve 17.00 aralarında atama yapmak istiyoruz
            //Eğer ilgili saat doluysa 10 dakika sonrası için randevu atama işlemi yapıyoruz
            DateTime baslangicSaati = tarih.Date.AddHours(8);
            DateTime bitisSaati = tarih.Date.AddHours(17);

            foreach (var doktor in departmanAktifDoktorlari)
            {
                DateTime uygunSaat = baslangicSaati;
                //Doktorların randevularını çekmek için oluşturduğum bir LINQ sorgusu
                //Veritabanındaki tablolara uygun şekilde hazırladım.
                var doktorRandevulari = db.Randevu
                    .Where(r => r.personel_id == doktor.personel_id &&
                                DbFunctions.TruncateTime(r.randevu_tarih) == DbFunctions.TruncateTime(tarih) &&
                                db.Meslek.Any(m => m.meslek_id == r.Personel.meslek_id && m.meslek_ad.ToLower().Contains("doktor")))
                    .OrderBy(r => r.randevu_tarih);
                //Uygun randevu saati bulunması için yazdığım döngü, bulduğu durumda döngü kırılır.
                foreach (var mevcutRandevu in doktorRandevulari)
                {
                    if (uygunSaat.AddMinutes(10) <= mevcutRandevu.randevu_tarih)
                    {
                        break;
                    }
                    uygunSaat = mevcutRandevu.randevu_tarih.AddMinutes(10);
                }

                if (uygunSaat < bitisSaati)
                {//Uygun bir saat bulunması durumunda randevu kaydını ekleyerek aktif randevular indexine yönlendirme işlemi
                    randevu.hasta_id = hasta.hasta_id;
                    randevu.personel_id = doktor.personel_id;
                    randevu.departman_id = departman_id;
                    randevu.randevu_tarih = uygunSaat;
                    randevu.randevu_durum = true;

                    db.Randevu.Add(randevu);
                    db.SaveChanges();

                    return RedirectToAction("AktifRandevular");
                }
            }
            //İlgili tarihte tüm randevu saatleri doluysa randevu eklemeyen ve kullanıcıya hata veren yapı
            ViewBag.ErrorMessage = "Belirtilen tarihte uygun bir randevu zamanı bulunmamaktadır.";
            return View();
        }


        public ActionResult AktifRandevular(string arama, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Öncelikle randevu tarihler kontrolü
            RandevuDurumGüncelle();
            //Ardından veritabanından tüm aktif olan randevuları çeken LINQ sorgusu
            var randevular = db.Randevu.AsQueryable();
            var aktifRandevular = randevular.Where(ar => ar.randevu_durum == true);
            //Filtreleme işlemi
            if (!string.IsNullOrEmpty(arama))
            {
                aktifRandevular = aktifRandevular.Where(ar =>
                    (ar.Hasta.hasta_ad + " " + ar.Hasta.hasta_soyad).Contains(arama) ||
                    ar.Hasta.hasta_tcno.Contains(arama) ||
                    (ar.Personel.personel_ad + " " + ar.Personel.personel_soyad).Contains(arama) ||
                    ar.Personel.personel_tcno.Contains(arama));
            }

            //View'e filtrelenmiş ve sayfalanmış veriyi iletmek için oluşturduğum yapı
            ViewBag.arama = arama;
            var kayitlar = aktifRandevular.ToList().ToPagedList(sayfa, 15);

            return View(kayitlar);
        }

        public ActionResult GecmisRandevular(string arama, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanından randevu durumu false olan verileri çeken LINQ sorgusu
            var randevular = db.Randevu.AsQueryable();
            var gecmisRandevular = randevular.Where(gr => gr.randevu_durum == false);
            //Filtreleme işlemi
            if (!string.IsNullOrEmpty(arama))
            {
                gecmisRandevular = gecmisRandevular.Where(gr =>
                    (gr.Hasta.hasta_ad + " " + gr.Hasta.hasta_soyad).Contains(arama) ||
                    gr.Hasta.hasta_tcno.Contains(arama) ||
                    (gr.Personel.personel_ad + " " + gr.Personel.personel_soyad).Contains(arama) ||
                    gr.Personel.personel_tcno.Contains(arama));
            }
            //View'e filtrelenmiş ve sayfalanmış veriyi iletme işlemi
            ViewBag.arama = arama;
            var kayitlar = gecmisRandevular.ToList().ToPagedList(sayfa, 15);

            return View(kayitlar);
        }

        public ActionResult RandevuIptal(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İlgili randevunun iptal edilme işlemi
            //View'den gelen id değeri ile ilgili randevu durumunu false yapar ve veritabanını kaydedip aktif randevular indexine yönlendirir.
            var randevu = db.Randevu.Find(id);
            randevu.randevu_durum = false;
            db.SaveChanges();

            return RedirectToAction("AktifRandevular");
        }
        public ActionResult RandevuBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İlgili randevunun bilgilerini veritabanından id değerine göre çeken ve view'e ileten yapı
            var randevu = db.Randevu.Find(id);

            return View(randevu);
        }

        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Hatalı bir randevu eklenme durumunda parametre olarak gelen id değerine göre veritabanından silme ve kaydetme işlemi
            var randevu = db.Randevu.Find(id);
            bool tip = randevu.randevu_durum;
            db.Randevu.Remove(randevu);
            db.SaveChanges();
            //Silme işleminin ardından randevu kaydının durumuna göre aktif veya geçmiş randevular indexine yönlendirme işlemi
            if (tip)
            {
                return RedirectToAction("AktifRandevular");
            }

            else
            {
                return RedirectToAction("GecmisRandevular");
            }
        }
    }
}