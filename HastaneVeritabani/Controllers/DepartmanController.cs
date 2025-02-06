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
    public class DepartmanController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Departman
        public ActionResult Index(string arama, string durum, int sayfa=1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanından departman tablosundan verileri alıp, filtrleme/sayfalama işlemi yaparak view'e aktaran method
            var departmanListesi = db.Departman.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                departmanListesi = departmanListesi.Where(d => d.departman_ad.Contains(arama));
            }
            
            if (!string.IsNullOrEmpty(durum))
            {
                if (durum == "aktif")
                {
                    departmanListesi = departmanListesi.Where(d => d.departman_durum == true);
                }
                else if (durum == "pasif")
                {
                    departmanListesi = departmanListesi.Where(d => d.departman_durum == false);
                }
            }

            ViewBag.arama = arama;
            ViewBag.durum = durum;

            var departmanlar = departmanListesi.ToList().ToPagedList(sayfa, 15);

            return View(departmanlar);
        }

        [HttpGet]
        public ActionResult YeniDepartman()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        //Yeni departman ekleme işlemi için view'den alınan departman bilgilerini kontrol eden
        //ve bir sorun görülmemesi halinde departman olarak ekleyen method ve sayfa
        public ActionResult YeniDepartman(Departman departman)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var departmanlar = db.Departman.ToList();
            //Aynı isimde başka bir departman bulunma durumu
            if (departmanlar.Any(d => d.departman_ad == departman.departman_ad))
            {
                ViewBag.ErrorMessage = "Bu isim başka bir departmana ait. Lütfen başka bir isim deneyin.";
                return View();
            }

            db.Departman.Add(departman);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Departman güncelleme işlemleri için view'e ilgili departmanın idsi ile veritabanından çekilmiş bilgileri ileten method
        public ActionResult DepartmanGetir(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var departman = db.Departman.Find(id);
            return View(departman);
        }
        //Departman güncelleme ve kontrolleri işlemleri
        public ActionResult Güncelle(Departman _departman)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var departmanlar = db.Departman.ToList();
            var departman = db.Departman.Find(_departman.departman_id);
            //Acil departmanı için ek kontrol
            if (departman.departman_id == 1)
            {
                ViewBag.ErrorMessage = "Bu departman değiştirilemez ve silinemez.";
                return View("DepartmanGetir", departman);
            }

            if (departmanlar.Any(d => d.departman_ad == _departman.departman_ad && d.departman_id != _departman.departman_id))
            {
                ViewBag.ErrorMessage = "Bu isim başka bir departmana ait. Lütfen başka bir isim deneyin.";
                return View("DepartmanGetir", departman);
            }
            //Kullanıcının boş bilgi girmesi halinde eski bilgileri koruyan if yapıları
            if (string.IsNullOrEmpty(_departman.departman_ad))
            {
                _departman.departman_ad = departman.departman_ad;
            }

            if (departman.departman_durum == _departman.departman_durum) 
            {
                _departman.departman_durum = departman.departman_durum;
            }
            //Departmaknı veritabanına ekleme ve kaydetme ardından indexe yönlendirme işlemi
            departman.departman_ad = _departman.departman_ad;
            departman.departman_durum = _departman.departman_durum;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //View'den alınan id değerine göre STORED PROCEDURE ile departman bilgilerini çekmek için oluşturduğum method
        public ActionResult DepartmanBilgi(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanından aktardığım SP kullanımı için query ve parametre işlemleri
            var sql = "exec sp_DepartmanBilgi @DepartmanID = @id";
            var parameters = new[] { new SqlParameter("@id", id) };
            var departman_bilgi=db.Database.SqlQuery<sp_DepartmanBilgi_Result>(sql, parameters).ToList();
            //Ardındna view'e SP ile gelen verileri iletme 
            return View(departman_bilgi);

        }
        //Hatalı bir departman eklenmesi durumunda silme işlemi
        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Acil departmanı için ek kontrol ve silme engellemesi
            if (id == 1)
            {
                ViewBag.ErrorMessage = "Bu departman değiştirilemez ve silinemez.";
                return View("DepartmanGetir");
            }
            //Veritabanından silme ve veritabanını kaydetme ardından indexe yönlendirme işlemi
            var departman = db.Departman.Find(id);
            db.Departman.Remove(departman);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}