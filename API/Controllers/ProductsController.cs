using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // Tüm aktif (silinmemiş) ürünleri listeleme
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        var products = await _productRepository.GetProductsAsync(brand, type, sort);
        return Ok(products);
    }

    // Silinmiş ürünleri listeleme
    [HttpGet("deleted")]
    public async Task<ActionResult<IEnumerable<Product>>> GetDeletedProducts()
    {
        var deletedProducts = await _productRepository.GetDeletedProductsAsync();
        return Ok(deletedProducts);
    }

    // Belirli bir ürünü ID ile alma
    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<string>> GetBrands()
    {
        return Ok(await _productRepository.GetBrandsAsync()); 
    }

    [HttpGet("types")]
    public async Task<ActionResult<string>> GetTypes()
    {
        return Ok(await _productRepository.GetTypesAsync());
    }

    // Yeni ürün ekleme
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _productRepository.AddProduct(product);
        var success = await _productRepository.SaveChangesAsync();

        if (!success)
            return BadRequest("Problem creating product");

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // Ürün güncelleme
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id) return BadRequest("ID mismatch");

        // Ürün var mı kontrol et
        if (!await _productRepository.ProductExistsAsync(id))
            return NotFound("The product could not be found");

        _productRepository.UpdateProduct(product);
        var success = await _productRepository.SaveChangesAsync();

        if (!success)
            return BadRequest("Problem updating the product");

        return NoContent();
    }

    // Soft delete işlemi
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> SoftDeleteProduct(int id)
    {
        // Ürün var mı kontrol et
        if (!await _productRepository.ProductExistsAsync(id))
            return NotFound("The product could not be found");

        _productRepository.SoftDeleteProduct(id);
        var success = await _productRepository.SaveChangesAsync();

        if (!success)
            return BadRequest("Problem deleting the product");

        return NoContent();
    }

    // Silinmiş ürünü geri getirme (restore)
    [HttpPut("restore/{id:int}")]
    public async Task<ActionResult> RestoreProduct(int id)
    {
        // Silinmiş ürün var mı kontrol et
        if (!await _productRepository.ProductExistsAsync(id))
            return NotFound("The product could not be found");

        _productRepository.RestoreProduct(id);
        var success = await _productRepository.SaveChangesAsync();

        if (!success)
            return BadRequest("Problem restoring the product");

        return NoContent();
    }

    // Hard delete işlemi
    [HttpDelete("{id:int}/hard")]
    public async Task<ActionResult> HardDeleteProduct(int id)
    {
        // Ürün var mı kontrol et
        if (!await _productRepository.ProductExistsAsync(id))
            return NotFound("The product could not be found");

        _productRepository.HardDeleteProduct(id);
        var success = await _productRepository.SaveChangesAsync();

        if (!success)
            return BadRequest("Problem completely deleting the product");

        return NoContent();
    }
}

