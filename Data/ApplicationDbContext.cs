using DevoteWebsite.Models.Store;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace DevoteWebsite.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoreItemSaleInfo>()
    .HasOne(saleInfo => saleInfo.StoreItem)
    .WithOne(item => item.StoreItemSaleInfo)
    .HasForeignKey<StoreItemSaleInfo>(saleInfo => saleInfo.ItemId)
    .IsRequired();
    }

    public DbSet<StoreItem> StoreItems { get; set; }

    public DbSet<StoreItemSaleInfo> StoreItemSales { get; set; }
}
