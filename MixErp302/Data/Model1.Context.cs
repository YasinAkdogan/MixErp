﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MixErp302.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MixErpDbEntities : DbContext
    {
        public MixErpDbEntities()
            : base("name=MixErpDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<bBirim> bBirims { get; set; }
        public virtual DbSet<bKategori> bKategoris { get; set; }
        public virtual DbSet<blgCariTipi> blgCariTipis { get; set; }
        public virtual DbSet<blgDepartman> blgDepartmen { get; set; }
        public virtual DbSet<bMensei> bMenseis { get; set; }
        public virtual DbSet<bOdemeTurleri> bOdemeTurleris { get; set; }
        public virtual DbSet<ilceler> ilcelers { get; set; }
        public virtual DbSet<iller> illers { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tblCari> tblCaris { get; set; }
        public virtual DbSet<tblPersonel> tblPersonels { get; set; }
        public virtual DbSet<tblSatisAlisAlt> tblSatisAlisAlts { get; set; }
        public virtual DbSet<tblStokDurum> tblStokDurums { get; set; }
        public virtual DbSet<tblUrunAli> tblUrunAlis { get; set; }
        public virtual DbSet<tblUrunAlisAlt> tblUrunAlisAlts { get; set; }
        public virtual DbSet<tblUrunAlisUst> tblUrunAlisUsts { get; set; }
        public virtual DbSet<tblUrunler> tblUrunlers { get; set; }
        public virtual DbSet<tblUrunSatisAlt> tblUrunSatisAlts { get; set; }
        public virtual DbSet<tblUrunSatisUst> tblUrunSatisUsts { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<vwUrunAlisAltUst> vwUrunAlisAltUsts { get; set; }
    }
}
