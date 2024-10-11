using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        return Ok(await repository.GetProductsAsync());
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repository.GetProductByIdAsync(id);
        if(product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repository.AddProduct(product);

        if(await repository.SaveChangesAsync())
            return CreatedAtAction("GetProduct", new{id = product.Id});
        
        return BadRequest("Problem creating product");
    }

    [HttpPut]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || ProductExists(id))
            return BadRequest("Cannot update this product");

        repository.UpdateProduct(product);

        if(await repository.SaveChangesAsync())
            return NoContent();

        return BadRequest("Problem updating the product");
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = repository.GetProductByIdAsync(id);

        if(product == null) return NotFound();

        if(await repository.SaveChangesAsync())
            return NoContent();

        return BadRequest("Problem deleting the product");
    }


    private bool ProductExists(int id)
    {
        return repository.ProductExists(id);
    }

}