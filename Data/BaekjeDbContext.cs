using BaekjeCulturalComplexApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaekjeCulturalComplexApi.Data
{
    public class BaekjeDbContext : DbContext
    {
        public BaekjeDbContext(DbContextOptions<BaekjeDbContext> options)
        : base(options)
        {
        }

        public DbSet<Manager> Managers { get; set; }
        public DbSet<CodeItem> CodeItems { get; set; }
        public DbSet<IncidentalSalesItem> IncidentalSalesItems { get; set; }
        public DbSet<RentalSalesItem> RentalSalesItems { get; set; }
        public DbSet<RentalSalesAccount> RentalSalesAccounts { get; set; }
        public DbSet<IncidentalSales> IncidentalSales { get; set; }
        public DbSet<RentalSales> RentalSales { get; set; }
        public DbSet<FlashreportTargetYear> FlashreportTargetYears { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manager>().ToTable("tb_manager");
            modelBuilder.Entity<CodeItem>().ToTable("tb_code");
            modelBuilder.Entity<IncidentalSalesItem>().ToTable("tb_incidental_sales_item");
            modelBuilder.Entity<RentalSalesItem>().ToTable("tb_rental_sales_item");
            modelBuilder.Entity<RentalSalesAccount>().ToTable("tb_rental_sales_account");
            modelBuilder.Entity<IncidentalSales>().ToTable("tb_incidental_sales_status");
            modelBuilder.Entity<RentalSales>().ToTable("tb_rental_sales_status");
            modelBuilder.Entity<FlashreportTargetYear>().ToTable("tb_flashreport_target_year");

            modelBuilder.Entity<Manager>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");

            modelBuilder.Entity<CodeItem>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");

            modelBuilder.Entity<IncidentalSalesItem>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");

            modelBuilder.Entity<RentalSalesItem>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");

            modelBuilder.Entity<RentalSalesAccount>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");

            modelBuilder.Entity<IncidentalSales>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");

            modelBuilder.Entity<RentalSales>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");

            modelBuilder.Entity<FlashreportTargetYear>()
            .HasKey(b => b.Seq)
            .HasName("PrimaryKey_Seq");
        }
    }
}