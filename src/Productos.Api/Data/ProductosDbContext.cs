using Microsoft.EntityFrameworkCore;
using Productos.Api.Models;
namespace Productos.Api.Data;
public sealed class ProductosDbContext(DbContextOptions<ProductosDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(p =>
        {
            p.ToTable("productos", table =>
            {
                table.HasCheckConstraint("ck_precio_no_negativo", "\"Precio\" >= 0");
                table.HasCheckConstraint("ck_existencias_no_negativas", "\"Existencias\" >= 0");
            });
            p.HasKey(x => x.Id);
            p.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            p.Property(x => x.Precio).HasPrecision(10, 2);
        });
    }
}
