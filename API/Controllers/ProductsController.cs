using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IGenericRepository<Product> repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        var spec = new ProductSpecification(brand, type, sort);

        var product = await repository.ListAsync(spec);

        return Ok(product);
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repository.GetByIdAsync(id);
        if(product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repository.Add(product);

        if(await repository.SaveAllAsync())
            return CreatedAtAction("GetProduct", new{id = product.Id});
        
        return BadRequest("Problem creating product");
    }

    [HttpPut]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || ProductExists(id))
            return BadRequest("Cannot update this product");

        repository.Update(product);

        if(await repository.SaveAllAsync())
            return NoContent();

        return BadRequest("Problem updating the product");
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if(product == null) return NotFound();

        repository.Remove(product);

        if(await repository.SaveAllAsync())
            return NoContent();

        return BadRequest("Problem deleting the product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();
        return Ok(await repository.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetType()
    {
        var spec = new TypeListSpecification();
        return Ok(await repository.ListAsync(spec));
    }

    private bool ProductExists(int id)
    {
        return repository.Exists(id);
    }

}