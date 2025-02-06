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

namespace HastaneVeritabani.Controllers
{
    public class MeslekController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Meslek
        public ActionResult Index(string arama, string durum, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Meslek index sayfasında arama işlemleri yapan LINQ sorgusu
            var meslekListesi = db.Meslek.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                meslekListesi = meslekListesi.Where(m => m.meslek_ad.Contains(arama));
            }
            //View'e gerekli filtrelenmiş veriyi ve sayfalama işlemini ileten yapı
            ViewBag.arama = arama;

            var meslekler = meslekListesi.ToList().ToPagedList(sayfa, 15);

            return View(meslekler);
        }

        [HttpGet]
        public ActionResult YeniMeslek()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        [HttpPost]
        public ActionResult YeniMeslek(Meslek meslek)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni meslek sayfasında kontrolleri yapabilmemiz için oluşturduğum LINQ sorgusu
            var meslekler = db.Meslek.ToList();
            //Yeni meslek adına ait kontrol yapısı
            if (meslekler.Any(m => m.meslek_ad == meslek.meslek_ad))
            {
                ViewBag.ErrorMessage = "Bu isim başka bir mesleğe ait. Lütfen başka bir isim deneyin.";
                return View();
            }
            //Yeni meslek bilgisini veritabanına ekleme, kaydetme ve meslek indexine yönlendirme yapısı
            db.Meslek.Add(meslek);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult MeslekBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //İlgili mesleğin idsi ile veritabanı üzerinden STORED PROCEDURE çağırmakj için LINQ yapısı
            var sql = "exec sp_MeslekBilgi @MeslekID = @id";
            var parameters = new[] { new SqlParameter("@id", id) };
            var meslek_bilgi = db.Database.SqlQuery<sp_MeslekBilgi_Result>(sql, parameters).ToList();

            return View(meslek_bilgi);

        }

        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Hatalı bir meslek eklenmesi durumunda silme, kaydetme ve sonrasında meslek indexine yönlendirilmesi için oluşturduğum yapı
            var meslek = db.Meslek.Find(id);
            db.Meslek.Remove(meslek);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult MeslekGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Meslek güncelleme sayfası için oluşturulan, ilgili mesleğin idsi ile meslek bilgilerini getirerek view'e aktaran yapı
            var meslek = db.Meslek.Find(id);
            return View(meslek);
        }

        public ActionResult Güncelle(Meslek _meslek)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //MeslekGetir sayfasında post olma durumunda çalıştırılacak güncelleme işlemi için LINQ yapıları ve kontrol mekanizması
            var meslekler = db.Meslek.ToList();
            var meslek = db.Meslek.Find(_meslek.meslek_id);

            if (meslekler.Any(m => m.meslek_ad == _meslek.meslek_ad && m.meslek_id != _meslek.meslek_id))
            {
                ViewBag.ErrorMessage = "Bu isim başka bir mesleğe ait. Lütfen başka bir isim deneyin.";
                return View("MeslekGetir", meslek);
            }
            //Kullanıcının boş girme durumunda ilgili mesleğin eski değerlerini korumak için controller yapısı
            if (_meslek.meslek_ad == null)
            {
                _meslek.meslek_ad = meslek.meslek_ad;
            }

            if (_meslek.meslek_maas == 0)  
            {
                _meslek.meslek_maas = meslek.meslek_maas;
            }
            //Meslek güncelleme, kaydetme ve meslek indexine yönlendirme işlemi
            meslek.meslek_ad = _meslek.meslek_ad;
            meslek.meslek_maas = _meslek.meslek_maas;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}