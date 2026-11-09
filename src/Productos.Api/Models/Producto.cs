namespace Productos.Api.Models;
public sealed class Producto
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Existencias { get; set; }
}
