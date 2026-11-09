using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Productos.Api.Contracts;
public sealed class ProductoWriteDto : IValidatableObject
{
    [JsonRequired, Required, StringLength(120)]
    public required string Nombre { get; init; }
    [JsonRequired, Range(typeof(decimal), "0", "99999999.99")]
    public decimal Precio { get; init; }
    [JsonRequired, Range(0, int.MaxValue)]
    public int Existencias { get; init; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (decimal.Round(Precio, 2) != Precio)
            yield return new ValidationResult("El precio admite hasta dos decimales.", [nameof(Precio)]);
    }
}
public sealed record ProductoReadDto(int Id, string Nombre, decimal Precio, int Existencias);
