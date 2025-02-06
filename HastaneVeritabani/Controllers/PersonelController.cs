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
    public class PersonelController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Personel
        public ActionResult Index(string arama, string durum, int? departman_id, int? meslek_id, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İndex sayfasında arama ve filtreleme işlemleri için LINQ sorguları ve filtreleme yapıları
            var personelListesi = db.Personel.AsQueryable();
            var departmanlar = db.Departman.ToList();
            var meslekler = db.Meslek.ToList();
            //personel ad soyad veya tcnoya göre arama yapılmaktadır.
            if (!string.IsNullOrEmpty(arama))
            {
                personelListesi = personelListesi.Where(p => (p.personel_ad + " " + p.personel_soyad).Contains(arama) ||
                                                                p.personel_tcno.Contains(arama));
            }
            //güncelleme: personelin aktif olup olmama durumuna göre arama işlemi
            if (!string.IsNullOrEmpty(durum))
            {
                if (durum == "aktif")
                {
                    personelListesi = personelListesi.Where(p => p.personel_durum == true);
                }
                else if (durum == "pasif")
                {
                    personelListesi = personelListesi.Where(p => p.personel_durum == false);
                }
            }

            if (departman_id != null)
            {
                personelListesi = personelListesi.Where(p => p.departman_id == departman_id);
            }

            if (meslek_id != null)
            {
                personelListesi = personelListesi.Where(p => p.meslek_id == meslek_id);
            }
            //view'e gerekli yapıları aktarmak için oluşturduğum yapı
            ViewBag.arama = arama;
            ViewBag.durum = durum;
            ViewBag.departmanlar = new SelectList(departmanlar, "departman_id", "departman_ad");
            ViewBag.meslekler = new SelectList(meslekler, "meslek_id", "meslek_ad");


            var personeller = personelListesi.ToList().ToPagedList(sayfa, 15);

            return View(personeller);
        }

        public ActionResult PersonelGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Personel güncelleme işlemi için PersonelGetir viewine gerekli bilgileri sağlayan yapı
            var personel = db.Personel.Find(id);
            var departmanlar = db.Departman.ToList();
            var meslekler=db.Meslek.ToList();
            ViewBag.departmanlar = new SelectList(departmanlar, "departman_id", "departman_ad");
            ViewBag.meslekler = new SelectList(meslekler, "meslek_id", "meslek_ad");
            return View(personel);
        }

        [HttpGet]
        public ActionResult YeniPersonel()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni personel sayfasına post işlemi yapılmadan aktarılması gereken veriler
            var departmanlar = db.Departman.ToList();
            var meslekler = db.Meslek.ToList();
            ViewBag.departmanlar = new SelectList(departmanlar, "departman_id", "departman_ad");
            ViewBag.meslekler = new SelectList(meslekler, "meslek_id", "meslek_ad");

            return View();
        }
        [HttpPost]
        public ActionResult YeniPersonel(Personel personel, Adres adres)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Post işlemi yapıldıktan sonra gerekli kontrollere göre kullanıcıyı bilgilendirme, veri eklenecekse ilgili tablolara ait
            //LINQ sorguları
            var personeller = db.Personel.ToList();
            var departmanlar = db.Departman.ToList();
            var meslekler = db.Meslek.ToList();
            ViewBag.departmanlar = new SelectList(departmanlar, "departman_id", "departman_ad");
            ViewBag.meslekler = new SelectList(meslekler, "meslek_id", "meslek_ad");
            //Tüm kontrol yapıları
            if (personeller.Any(p => p.personel_tcno == personel.personel_tcno))
            {
                ViewBag.ErrorMessage = "Bu TC Numarası başka bir personele ait. Lütfen başka bir TC Numarası deneyin.";
                return View();
            }

            if (personeller.Any(p => p.personel_ad == personel.personel_ad && p.personel_soyad==personel.personel_soyad))
            {
                ViewBag.ErrorMessage = "Bu isim ve soyisim başka bir personele ait. Lütfen başka bir isim ve soyisim deneyin.";
                return View();
            }

            if (personeller.Any(p => p.personel_telefon.Replace(" ", "") == personel.personel_telefon.Trim().Replace(" ", "")))
            {
                ViewBag.ErrorMessage = "Bu telefon numarası başka bir personele ait. Lütfen başka bir telefon numarası deneyin.";
                return View();
            }
            //Personele ait adres kaydı oluşturma ve veritabanını kaydetme
            db.Adres.Add(adres);
            db.SaveChanges();
            //Personel adresi olarak oluşturulan adresi tanımlama, sonrasında personeli veritabanına ekleyerek veritabanını kaydetme ve ardından personelk indexine yönlendirme işlemi
            personel.adres_id = adres.adres_id;

            db.Personel.Add(personel);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult PersonelBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Personel bilgi sayfasına tıklandığı durumda veritabanındaki STORED PROCEDURE çağrılması için oluşturduğum yapı
            var sql = "exec sp_PersonelBilgi @PersonelID = @id";
            var parameters = new[] { new SqlParameter("@id", id) };
            var personel_bilgi = db.Database.SqlQuery<sp_PersonelBilgi_Result>(sql, parameters).ToList();
            //Ardından personelin doktor olması durumunda daha detaylı bilgilendirme için eklediğim kontrol ve viewbag yapıları
            var personel = db.Personel.Find(id);
            var meslek = db.Meslek.SingleOrDefault(m => m.meslek_id == personel.meslek_id);

            if (meslek != null && meslek.meslek_ad.ToLower().Contains("doktor")) 
            {
                ViewBag.doktorDurum = true;
            }
            else { ViewBag.doktorDurum = false; }

            //güncelleme: yaş bilgisi ekledim
            var dogum_tarihi=personel.personel_dogum_tarihi;
            var tarih=DateTime.Now;
            var yas = tarih.Year - dogum_tarihi.Year;
            if (tarih.Date < dogum_tarihi.AddYears(yas))
            {
                yas--;
            }
            ViewBag.yas = yas;

            return View(personel_bilgi);
        }
        //PersonelGetir sayfasında post işlemi yapılması durumunda çağrılacak olan güncelleme methodu
        public ActionResult Güncelle(Personel _personel)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Gerekli kontrollerin yapılması adına veritabanından veri çekmek için oluşturduğum LINQ sorguları
            var personeller = db.Personel.ToList();
            var personel = db.Personel.Find(_personel.personel_id);
            //Tüm yapılması gereken kontroller (tcno-isim soyisim-tel no vs.)
            if (personeller.Any(p => p.personel_tcno == _personel.personel_tcno && p.personel_id != _personel.personel_id))
            {
                ViewBag.ErrorMessage = "Bu TC No başka bir personele ait. Lütfen başka bir TC No deneyin.";
                return View("PersonelGetir", personel);
            }

            if (personeller.Any(p => p.personel_ad == _personel.personel_ad && p.personel_soyad==_personel.personel_soyad &&
                                    p.personel_id != _personel.personel_id))
            {
                ViewBag.ErrorMessage = "Bu isim ve soyisim başka bir personele ait. Lütfen başka bir isim ve soyisim deneyin.";
                return View("PersonelGetir", personel);
            }

            if (personeller.Any(p => p.personel_telefon == _personel.personel_telefon && p.personel_id != _personel.personel_id))
            {
                ViewBag.ErrorMessage = "Bu telefon numarası başka bir personele ait. Lütfen başka bir telefon numarası deneyin.";
                return View("PersonelGetir", personel);
            }
            //Kullanıcının güncelleme işlemine tıkladığında belirli alanları boş bırakması durumunda otomatik olarak eski veriyi kabul eden if yapıları
            if (personel.personel_durum == _personel.personel_durum)
            {
                _personel.personel_durum = personel.personel_durum;
            }

            if (string.IsNullOrEmpty(_personel.personel_tcno))
            {
                _personel.personel_tcno = personel.personel_tcno;
            }

            if (string.IsNullOrEmpty(_personel.personel_ad))
            {
                _personel.personel_ad = personel.personel_ad;
            }

            if (string.IsNullOrEmpty(_personel.personel_soyad))
            {
                _personel.personel_soyad = personel.personel_soyad;
            }

            if (string.IsNullOrEmpty(_personel.personel_telefon))
            {
                _personel.personel_telefon = personel.personel_telefon;
            }

            if (personel.personel_cinsiyet == _personel.personel_cinsiyet)
            {
                _personel.personel_cinsiyet = personel.personel_cinsiyet;
            }

            if (_personel.personel_dogum_tarihi==null)
            {
                _personel.personel_dogum_tarihi = personel.personel_dogum_tarihi;
            }

            if (!_personel.personel_giris_tarihi.HasValue)
            {
                _personel.personel_giris_tarihi = personel.personel_giris_tarihi;
            }
            //Personel bilgilerini güncelleme, kaydetme ve ardından personel indexine yönlendirme işlemi
            personel.personel_durum = _personel.personel_durum;
            personel.personel_tcno = _personel.personel_tcno;
            personel.personel_ad = _personel.personel_ad;
            personel.personel_soyad = _personel.personel_soyad;
            personel.personel_telefon = _personel.personel_telefon;
            personel.personel_cinsiyet = _personel.personel_cinsiyet;
            personel.departman_id = _personel.departman_id;
            personel.meslek_id = _personel.meslek_id;
            personel.personel_dogum_tarihi = _personel.personel_dogum_tarihi;
            personel.personel_giris_tarihi = _personel.personel_giris_tarihi;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yanlış bir personel eklenmesi durumunda veritabanından ilgili veriyi silme, kaydetme ve ardından personel indexine yönlendirme yapması için oluşturduğum yapı
            var personel = db.Personel.Find(id);
            db.Personel.Remove(personel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}