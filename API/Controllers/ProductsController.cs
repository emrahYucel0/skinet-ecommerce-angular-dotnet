using System;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StoreContext context;

    public ProductsController(StoreContext context)
    {
        this.context = context;
    }

    // Tüm aktif (silinmemiş) ürünleri listeleme
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        // Soft delete yapılmamış (DeletedDate == null) ürünleri getiriyoruz
        return await context.Products
            .Where(p => p.DeletedDate == null) // Manuel kontrol
            .ToListAsync();
    }

    // Silinmiş ürünleri listeleme
    [HttpGet("deleted")]
    public async Task<ActionResult<IEnumerable<Product>>> GetDeletedProducts()
    {
        // Soft delete yapılmış ürünleri getiriyoruz
        return await context.Products
            .Where(p => p.DeletedDate != null) // Manuel kontrol
            .ToListAsync();
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if(product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || !ProductExist(id))
            return BadRequest("Can not Update This Product");

        product.UpdatedDate = DateTime.Now;

        context.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

        // Silinen ürünü geri getirme (restore)
    [HttpPut("restore/{id:int}")]
    public async Task<ActionResult> RestoreProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null || product.DeletedDate == null) // Eğer ürün zaten aktifse geri getirilemez
            return NotFound();

        product.DeletedDate = null; // Ürünü geri getiriyoruz

        context.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return NoContent();
    }

    // Soft delete işlemi
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> SoftDeleteProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.DeletedDate = DateTime.Now; // Soft delete işlemi

        context.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return NoContent();
    }

    // Hard delete işlemi
    [HttpDelete("{id:int}/hard")]
    public async Task<ActionResult> HardDeleteProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();

        context.Products.Remove(product); // Hard delete işlemi (fiziksel silme)
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExist(int id)
    {
        return context.Products.Any(x => x.Id == id);
    }
}
