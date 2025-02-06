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
    public class BütceController : Controller
    {
        HastaneVeritabaniEntities2 db = new HastaneVeritabaniEntities2();
        // GET: Bütce
        //Veritabanındaki bütçe tablosunu listeleyen method
        public ActionResult Index(string bütce_kategori, string bütce_tipi, int sayfa = 1)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanından bütçe tablosunu alma işlemi
            var bütceListesi = db.Bütce.AsQueryable();
            //if else yapıları ile filtreleme ve arama işlemleri
            if (!string.IsNullOrEmpty(bütce_tipi))
            {
                if (bütce_tipi == "gelir")
                {
                    bütceListesi = bütceListesi.Where(b => b.bütce_tipi == true);
                }
                else if (bütce_tipi == "gider")
                {
                    bütceListesi = bütceListesi.Where(b => b.bütce_tipi == false);
                }
            }

            if (!string.IsNullOrEmpty(bütce_kategori))
            {
                bütceListesi = bütceListesi.Where(b => b.bütce_kategori == bütce_kategori);
            }
            //Detaylı bir view için LINQ sorguları ve viewbag ile view'e aktarma işlemleri
            var kategoriler = db.Bütce
                .Select(b => b.bütce_kategori)
                .Distinct()
                .ToList();

            var toplamGelir = bütceListesi.Where(b => b.bütce_tipi == true)
                .Select(b => (decimal?)b.bütce_tutar)
                .Sum() ?? 0;
            var toplamGider = bütceListesi.Where(b => b.bütce_tipi == false)
                .Select(b => (decimal?)b.bütce_tutar)
                .Sum() ?? 0;
            var toplamBütce = toplamGelir - toplamGider;


            ViewBag.kategoriler = kategoriler;
            ViewBag.secilenKategori = bütce_kategori;
            ViewBag.secilenTip = bütce_tipi;

            ViewBag.toplamGelir = toplamGelir;
            ViewBag.toplamGider = toplamGider;
            ViewBag.toplamBütce = toplamBütce;
            //Son olarak sayfalama işlemi ve view'e filtrelenmiş veriyi iletme
            var bütceler = bütceListesi.ToList().ToPagedList(sayfa, 15);
            return View(bütceler);
        }

        [HttpGet]
        public ActionResult YeniBütce()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        [HttpPost]
        public ActionResult YeniBütce(Bütce bütce)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Veritabanına yeni bütçe kaydı ekleme ve Bütçe indexine yönlendirme işlemi
            db.Bütce.Add(bütce);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Olası bir hatalı bütçe kaydı ekleme durumunda silme işlemi
        public ActionResult Sil(int id)
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Tıklanan bütçe kaydının id değerine göre bütçeyi veritabanından alma

            //Malzeme Alım işlemleri otomatik olarak yapıldığından if else yapısı ile kontrol sağladım
            var bütce=db.Bütce.Find(id);

            if (bütce.bütce_kategori=="Malzeme Alım") 
            {

                return RedirectToAction("Index");
            }
            else
            {
                db.Bütce.Remove(bütce);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        //Personel maaşlarının otomatik bütçe kaydının tutulması için oluşturulmuş method
        public ActionResult PersonelMaas()
        {//Giriş Kontrolü
            if (Session["UserLoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Aktif personellere maaş ödemesi yapılacağından aktif personelleri veritabanından alan LINQ sorgusu
            var aktifPersoneller=db.Personel.Where(p=>p.personel_durum==true).ToList();
            //Toplam aktif personel sayısına göre toplam maaş miktarını hesaplayan LINQ sorgusu ve sayısal işlem
            decimal toplamMaas = 0;

            foreach(var personel in aktifPersoneller)
            {
                var meslekMaas=db.Meslek.FirstOrDefault(m=>m.meslek_id==personel.meslek_id);
                toplamMaas += meslekMaas.meslek_maas;
            }
            //Yeni bütçe kaydı oluşturma işlemi, controllerda işlenen verilere göre
            var yeniBütce = new Bütce
            {
                bütce_tipi = false,
                bütce_kategori="Personel Maaş",
                bütce_tutar=toplamMaas,
                bütce_tarihi=DateTime.Now,
            };
            //Bütçe kaydını ekleme, veritabanını kaydetme ve bütçe indexine yönlendirme işlemi
            db.Bütce.Add(yeniBütce);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}