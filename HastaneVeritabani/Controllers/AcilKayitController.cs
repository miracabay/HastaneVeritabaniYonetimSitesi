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
    public class AcilKayitController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: AcilKayit

        public ActionResult Index()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public ActionResult YeniAcilKayit()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        [HttpPost]
        public ActionResult YeniAcilKayit(Acil_Kayit acil_kayit, string hasta_tcno, string tani)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni bir acil kaydı yapmak ve kontrolleri sağlamak için veritabanı işlemleri LINQ sorgusu
            var hasta = db.Hasta.SingleOrDefault(h => h.hasta_tcno == hasta_tcno);
            var hastalar = db.Hasta.ToList();
            var personeller = db.Personel.ToList();

            //Departmanı acil olan, aktif olan ve doktor olan personeller listesi LINQ sorgusu
            var acilDoktorlar = db.Personel.Where(ad => ad.departman_id == 1 &&
                                ad.personel_durum==true &&
                                db.Meslek.Any(m => m.meslek_id == ad.meslek_id &&
                                m.meslek_ad.ToLower().Contains("doktor"))).ToList();
            //Aktif acil kaydı olan hasta kontrolü için veritabanı işlemi LINQ sorgusu
            var hastaAktifAcilKayit = db.Acil_Kayit.Where(hak => hak.hasta_id == hasta.hasta_id &&
                                                       hak.acil_kayit_durum == true).ToList();

            //Hasta bulunmama durumu
            if (hasta == null)
            {
                ViewBag.ErrorMessage = "Girilen TC Numarası ile eşleşen bir hasta kaydı bulunamadı. " +
                                    "Lütfen önce Hasta İşlemleri>Hasta Kayıt yolu ile yeni hasta kaydı oluşturun.";

                return View(acil_kayit);
            }
            //Hastanın aktif acil kaydı olma durumu
            if (hastaAktifAcilKayit.Any())
            {
                ViewBag.ErrorMessage = "Hastanın aktif bir acil kaydı bulunmaktadır.";
                return View(acil_kayit);
            }
            //Extreme bir durum olsa da kontrolünü yazmak istedim
            //Acil departmanında herhangi bir doktor bulunmama durumu için LINQ sorgusu ve kontrolü
            if (acilDoktorlar.Any())
            {
                Random random = new Random();
                int randomIndex = random.Next(acilDoktorlar.Count);
                var atanacakPersonel = acilDoktorlar[randomIndex];
                acil_kayit.personel_id = atanacakPersonel.personel_id;
            }
            else
            {
                ViewBag.ErrorMessage = "Acil departmanında atanabilecek durumda aktif doktor kaydı bulunamadı.";
                return View(acil_kayit);
            }
            //Kayıt ve veritabanına işleme işlemi, işlem yapıldıktan sonra aktif acil kayıtlar viewine yönlendirilir.
            acil_kayit.hasta_id = hasta.hasta_id;
            acil_kayit.acil_tani = tani;
            acil_kayit.acil_kayit_durum = true;

            db.Acil_Kayit.Add(acil_kayit);
            db.SaveChanges();
            return RedirectToAction("AktifAcilKayitlar");

        }
        //Aktif olan acil kayıtları listeleme sayfası
        public ActionResult AktifAcilKayitlar(string arama, int sayfa= 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanı Acil_Kayit tablosundan aktif olan kayıtları çekmek için LINQ sorgusu
            var acilKayitlar = db.Acil_Kayit.AsQueryable();
            var acilAktifKayitlar = acilKayitlar.Where(aak => aak.acil_kayit_durum == true);
            //Sayfa içerisindeki arama işlemi, viewden gelen arama parametresine göre filtreleme işlemi
            if (!string.IsNullOrEmpty(arama))
            {
                acilAktifKayitlar = acilAktifKayitlar.Where(aak =>
                    (aak.Hasta.hasta_ad + " " + aak.Hasta.hasta_soyad).Contains(arama) ||
                    aak.Hasta.hasta_tcno.Contains(arama) ||
                    (aak.Personel.personel_ad + " " + aak.Personel.personel_soyad).Contains(arama) ||
                    aak.Personel.personel_tcno.Contains(arama));
            }

            //Son olarak sayfalama işlemi ve view'e aktarma
            ViewBag.arama = arama;
            var kayitlar = acilAktifKayitlar.ToList().ToPagedList(sayfa, 15);

            return View(kayitlar);
        }
        //Geçmiş olan acil kayıtları listeleme sayfası
        public ActionResult GecmisAcilKayitlar(string arama, int sayfa = 1)
        {//Giriş kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanı üzerinden pasif durumda olan acil kayıtları çekmek için LINQ sorgusu
            var acilKayitlar = db.Acil_Kayit.AsQueryable();
            var acilGecmisKayitlar = acilKayitlar.Where(aak => aak.acil_kayit_durum == false);
            //View'den gelen arama parametresiyle arama işlemi için LINQ sorgusu ve filtreleme işlemi
            if (!string.IsNullOrEmpty(arama))
            {
                acilGecmisKayitlar = acilGecmisKayitlar.Where(aak =>
                    (aak.Hasta.hasta_ad + " " + aak.Hasta.hasta_soyad).Contains(arama) ||
                    aak.Hasta.hasta_tcno.Contains(arama) ||
                    (aak.Personel.personel_ad + " " + aak.Personel.personel_soyad).Contains(arama) ||
                    aak.Personel.personel_tcno.Contains(arama));
            }
            //İşlenmiş ve filtrelenmiş son kayıtları sayfalama ve view'e aktarma işlemi
            ViewBag.arama = arama;
            var kayitlar = acilGecmisKayitlar.ToList().ToPagedList(sayfa, 15);

            return View(kayitlar);
        }
        //Aktif acil kayıt sayfasında taburcu işlemi
        public ActionResult AcilKayitTaburcu(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Tıklanan aktif acil kaydın id'sini alarak acil kayıt durumu false olarak ayarlama
            //ve ardından kullanıcıyı aktif acil kayıtlar viewine iletme
            var acilKayit = db.Acil_Kayit.Find(id);
            acilKayit.acil_kayit_durum = false;
            db.SaveChanges();

            return RedirectToAction("AktifAcilKayitlar");
        }
        //Tıklanan acil kayıttan id değeri alarak acil kayıt bilgi sayfasına ileten method
        public ActionResult AcilKayitBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var acil_kayit = db.Acil_Kayit.Find(id);

            return View(acil_kayit);
        }
        //Hatalı bir oluşturma işlemi için acil kayıt silme işlemi
        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var acil_kayit = db.Acil_Kayit.Find(id);
            bool tip = acil_kayit.acil_kayit_durum;
            db.Acil_Kayit.Remove(acil_kayit);
            db.SaveChanges();
            //Acil kayıt silindikten sonra silinen acil kaydın türüne göre aktif veya geçmiş
            //acil kayıt sayfasına yönlendiren işlemler
            if (tip)
            {
                return RedirectToAction("AktifAcilKayitlar");
            }

            else
            {
                return RedirectToAction("GecmisAcilKayitlar");
            }
        }
    }
}