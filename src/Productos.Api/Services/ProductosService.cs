using Microsoft.EntityFrameworkCore;
using Productos.Api.Contracts;
using Productos.Api.Data;
using Productos.Api.Models;
namespace Productos.Api.Services;
public sealed class ProductosService(ProductosDbContext db)
{
    public Task<List<ProductoReadDto>> ListAsync(int page, int pageSize, CancellationToken ct) =>
        db.Productos.AsNoTracking().OrderBy(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize)
          .Select(p => new ProductoReadDto(p.Id, p.Nombre, p.Precio, p.Existencias)).ToListAsync(ct);
    public Task<ProductoReadDto?> GetAsync(int id, CancellationToken ct) =>
        db.Productos.AsNoTracking().Where(p => p.Id == id)
          .Select(p => new ProductoReadDto(p.Id, p.Nombre, p.Precio, p.Existencias)).SingleOrDefaultAsync(ct);
    public async Task<ProductoReadDto> CreateAsync(ProductoWriteDto dto, CancellationToken ct)
    {
        var entity = new Producto { Nombre = dto.Nombre.Trim(), Precio = dto.Precio, Existencias = dto.Existencias };
        db.Productos.Add(entity);
        await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Nombre, entity.Precio, entity.Existencias);
    }
    public async Task<bool> ReplaceAsync(int id, ProductoWriteDto dto, CancellationToken ct)
    {
        var entity = await db.Productos.FindAsync([id], ct);
        if (entity is null) return false;
        entity.Nombre = dto.Nombre.Trim(); entity.Precio = dto.Precio; entity.Existencias = dto.Existencias;
        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.Productos.FindAsync([id], ct);
        if (entity is null) return false;
        db.Productos.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
