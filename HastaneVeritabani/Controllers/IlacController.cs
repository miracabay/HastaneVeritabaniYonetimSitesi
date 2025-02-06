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
    public class IlacController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Ilac
        public ActionResult Index(string arama, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanındaki ilaçları listelemek için LINQ sorgusu
            var ilacListesi = db.Ilac.AsQueryable();
            //Filtreleme işlemleri
            if (!string.IsNullOrEmpty(arama))
            {
                ilacListesi = ilacListesi.Where(i => i.ilac_ad.Contains(arama) || i.ilac_barkod.Contains(arama));
            }
            //View'e veri aktarma ve filtrelenmiş veri için sayfalama işlemi
            ViewBag.Arama = arama;

            var ilacReceteSayi = db.fn_IlacReceteSayiHesapla().ToList();
            //İlaçlara ait kesilen reçete sayısını bulmak adına yazdığım LINQ sorgusu
            //IlacReceteViewModel manuel olarak oluşturdum aktarma kısmında bir sorun olduğundan dolayı
            //Ve ardından basit bir LINQ sorgusu ile ilaca ait kesilen reçete sayısını bulup
            //filtrelenmiş ilaclar listesine ekleyerek view'e ilettik.
            //Viewbag kullanmama sebebim, view tarafında daha karmaşık işlemler gerektirmesi.
            var ilaclar = ilacListesi.ToList().Select(ilac => new IlacReceteViewModel
            {
                Ilac = ilac,
                ReceteSayi = ilacReceteSayi.FirstOrDefault(r => r.IlacID == ilac.ilac_id)?.IlacReceteSayi ?? 0
            }).ToPagedList(sayfa, 15);

            return View(ilaclar);
        }

        [HttpGet]
        public ActionResult YeniIlac()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        [HttpPost]
        public ActionResult YeniIlac(Ilac ilac)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Yeni eklenecek ilacın isim kontrolü için veritabanındaki ilaçları alan LINQ sorgusu
            var ilaclar = db.Ilac.ToList();

            if (ilaclar.Any(i => i.ilac_ad.ToLower() == ilac.ilac_ad.ToLower()))
            {
                ViewBag.ErrorMessage = "Bu isimde başka bir ilaç mevcut. Lütfen başka bir isim deneyin.";
                return View();
            }
            //İlacı ekleme, kaydetme ve indexe yönlendirme işlemi
            db.Ilac.Add(ilac);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Olası bir hatalı ekleme durumunda ilgili ilacı veritabanından silen, kaydeden ve indexe yönlendiren method
            var ilac = db.Ilac.Find(id);
            db.Ilac.Remove(ilac);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}