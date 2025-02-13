﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HastaneVeritabani.Models.Entity
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class HastaneVeritabaniEntities2 : DbContext
    {
        public HastaneVeritabaniEntities2()
            : base("name=HastaneVeritabaniEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Acil_Kayit> Acil_Kayit { get; set; }
        public virtual DbSet<Adres> Adres { get; set; }
        public virtual DbSet<Bütce> Bütce { get; set; }
        public virtual DbSet<Departman> Departman { get; set; }
        public virtual DbSet<Hasta> Hasta { get; set; }
        public virtual DbSet<Ilac> Ilac { get; set; }
        public virtual DbSet<Ilac_Recete> Ilac_Recete { get; set; }
        public virtual DbSet<Malzeme_Alim> Malzeme_Alim { get; set; }
        public virtual DbSet<Malzeme_Kullanim> Malzeme_Kullanim { get; set; }
        public virtual DbSet<Malzeme_Stok> Malzeme_Stok { get; set; }
        public virtual DbSet<Meslek> Meslek { get; set; }
        public virtual DbSet<Personel> Personel { get; set; }
        public virtual DbSet<Randevu> Randevu { get; set; }
        public virtual DbSet<Recete> Recete { get; set; }
        public virtual DbSet<Sonuc> Sonuc { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Yönetici> Yönetici { get; set; }
    
        [DbFunction("HastaneVeritabaniEntities2", "fn_HastaToplamKayitRandevuSayi")]
        public virtual IQueryable<fn_HastaToplamKayitRandevuSayi_Result> fn_HastaToplamKayitRandevuSayi(Nullable<int> hastaID)
        {
            var hastaIDParameter = hastaID.HasValue ?
                new ObjectParameter("HastaID", hastaID) :
                new ObjectParameter("HastaID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_HastaToplamKayitRandevuSayi_Result>("[HastaneVeritabaniEntities2].[fn_HastaToplamKayitRandevuSayi](@HastaID)", hastaIDParameter);
        }
    
        [DbFunction("HastaneVeritabaniEntities2", "fn_IlacReceteSayiHesapla")]
        public virtual IQueryable<fn_IlacReceteSayiHesapla_Result> fn_IlacReceteSayiHesapla()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_IlacReceteSayiHesapla_Result>("[HastaneVeritabaniEntities2].[fn_IlacReceteSayiHesapla]()");
        }
    
        [DbFunction("HastaneVeritabaniEntities2", "fn_MeslekPersonelMaasHesapla")]
        public virtual IQueryable<fn_MeslekPersonelMaasHesapla_Result> fn_MeslekPersonelMaasHesapla(Nullable<int> meslekID)
        {
            var meslekIDParameter = meslekID.HasValue ?
                new ObjectParameter("MeslekID", meslekID) :
                new ObjectParameter("MeslekID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_MeslekPersonelMaasHesapla_Result>("[HastaneVeritabaniEntities2].[fn_MeslekPersonelMaasHesapla](@MeslekID)", meslekIDParameter);
        }
    
        [DbFunction("HastaneVeritabaniEntities2", "fn_MeslekPersonelSayiHesapla")]
        public virtual IQueryable<fn_MeslekPersonelSayiHesapla_Result> fn_MeslekPersonelSayiHesapla(Nullable<int> meslekID)
        {
            var meslekIDParameter = meslekID.HasValue ?
                new ObjectParameter("MeslekID", meslekID) :
                new ObjectParameter("MeslekID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_MeslekPersonelSayiHesapla_Result>("[HastaneVeritabaniEntities2].[fn_MeslekPersonelSayiHesapla](@MeslekID)", meslekIDParameter);
        }
    
        [DbFunction("HastaneVeritabaniEntities2", "fn_ToplamKayitRandevuHastaSayiHesapla")]
        public virtual IQueryable<fn_ToplamKayitRandevuHastaSayiHesapla_Result> fn_ToplamKayitRandevuHastaSayiHesapla(Nullable<int> departmanID, Nullable<int> personelID)
        {
            var departmanIDParameter = departmanID.HasValue ?
                new ObjectParameter("DepartmanID", departmanID) :
                new ObjectParameter("DepartmanID", typeof(int));
    
            var personelIDParameter = personelID.HasValue ?
                new ObjectParameter("PersonelID", personelID) :
                new ObjectParameter("PersonelID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_ToplamKayitRandevuHastaSayiHesapla_Result>("[HastaneVeritabaniEntities2].[fn_ToplamKayitRandevuHastaSayiHesapla](@DepartmanID, @PersonelID)", departmanIDParameter, personelIDParameter);
        }
    
        [DbFunction("HastaneVeritabaniEntities2", "fn_ToplamPersonelMaasiHesapla")]
        public virtual IQueryable<fn_ToplamPersonelMaasiHesapla_Result> fn_ToplamPersonelMaasiHesapla(Nullable<int> departmanID)
        {
            var departmanIDParameter = departmanID.HasValue ?
                new ObjectParameter("DepartmanID", departmanID) :
                new ObjectParameter("DepartmanID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_ToplamPersonelMaasiHesapla_Result>("[HastaneVeritabaniEntities2].[fn_ToplamPersonelMaasiHesapla](@DepartmanID)", departmanIDParameter);
        }
    
        [DbFunction("HastaneVeritabaniEntities2", "fn_ToplamPersonelSayiHesapla")]
        public virtual IQueryable<fn_ToplamPersonelSayiHesapla_Result> fn_ToplamPersonelSayiHesapla(Nullable<int> departmanID)
        {
            var departmanIDParameter = departmanID.HasValue ?
                new ObjectParameter("DepartmanID", departmanID) :
                new ObjectParameter("DepartmanID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_ToplamPersonelSayiHesapla_Result>("[HastaneVeritabaniEntities2].[fn_ToplamPersonelSayiHesapla](@DepartmanID)", departmanIDParameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual ObjectResult<sp_AnaSayfa_Result> sp_AnaSayfa()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AnaSayfa_Result>("sp_AnaSayfa");
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual ObjectResult<sp_DepartmanBilgi_Result> sp_DepartmanBilgi(Nullable<int> departmanID)
        {
            var departmanIDParameter = departmanID.HasValue ?
                new ObjectParameter("DepartmanID", departmanID) :
                new ObjectParameter("DepartmanID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DepartmanBilgi_Result>("sp_DepartmanBilgi", departmanIDParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_HastaBilgi_Result> sp_HastaBilgi(Nullable<int> hastaID)
        {
            var hastaIDParameter = hastaID.HasValue ?
                new ObjectParameter("HastaID", hastaID) :
                new ObjectParameter("HastaID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_HastaBilgi_Result>("sp_HastaBilgi", hastaIDParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_MeslekBilgi_Result> sp_MeslekBilgi(Nullable<int> meslekID)
        {
            var meslekIDParameter = meslekID.HasValue ?
                new ObjectParameter("MeslekID", meslekID) :
                new ObjectParameter("MeslekID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_MeslekBilgi_Result>("sp_MeslekBilgi", meslekIDParameter);
        }
    
        public virtual ObjectResult<sp_PersonelBilgi_Result> sp_PersonelBilgi(Nullable<int> personelID)
        {
            var personelIDParameter = personelID.HasValue ?
                new ObjectParameter("PersonelID", personelID) :
                new ObjectParameter("PersonelID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_PersonelBilgi_Result>("sp_PersonelBilgi", personelIDParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    }
}
