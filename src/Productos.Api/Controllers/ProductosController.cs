using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Productos.Api.Contracts;
using Productos.Api.Services;
namespace Productos.Api.Controllers;
[ApiController]
[Route("api/productos")]
public sealed class ProductosController(ProductosService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<ProductoReadDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProductoReadDto>>> List(
        [FromQuery, Range(1, 1000000)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        CancellationToken ct = default) => Ok(await service.ListAsync(page, pageSize, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductoReadDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoReadDto>> Get(int id, CancellationToken ct)
    {
        var product = await service.GetAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType<ProductoReadDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductoReadDto>> Create(ProductoWriteDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Replace(int id, ProductoWriteDto dto, CancellationToken ct) =>
        await service.ReplaceAsync(id, dto, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
