using System;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class ProductsController : BaseApiController
{
    private readonly IGenericRepository<Product> _repo;

    public ProductsController(IGenericRepository<Product> repo)
    {
        _repo = repo;
    }

    // Tüm aktif (silinmemiş) ürünleri listeleme
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);

        return await CreatePagedResult(_repo, spec, specParams.PageIndex, specParams.PageSize);
    }

    // Silinmiş ürünleri listeleme
    [HttpGet("deleted")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetDeletedProducts()
    {
        var deletedProducts = await _repo.ListAllDeletedAsync();
        return Ok(deletedProducts);
    }

    // Belirli bir ürünü ID ile alma
    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<string>> GetBrands()
    {
        var spec = new BrandListSpecification();

        return Ok( await _repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<string>> GetTypes()
    {
        var spec = new TypeListSpecification();
        
        return Ok( await _repo.ListAsync(spec));
    }

    // Yeni ürün ekleme
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _repo.Add(product);
        var success = await _repo.SaveAll();

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
        var exists = await _repo.ExistsAsync(id);
        if (!exists)
            return NotFound("The product could not be found");

        _repo.Update(product);
        var success = await _repo.SaveAll();

        if (!success)
            return BadRequest("Problem updating the product");

        return NoContent();
    }

    // Soft delete işlemi
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> SoftDeleteProduct(int id)
    {
        var exists = await _repo.ExistsAsync(id);
        if (!exists)
            return NotFound("The product could not be found");

        _repo.SoftDelete(id);
        var success = await _repo.SaveAll();

        if (!success)
            return BadRequest("Problem deleting the product");

        return NoContent();
    }

    // Silinmiş ürünü geri getirme (restore)
    [HttpPut("restore/{id:int}")]
    public async Task<ActionResult> RestoreProduct(int id)
    {
        var exists = await _repo.ExistsAsync(id);
        if (!exists)
            return NotFound("The product could not be found");

        _repo.Restore(id);
        var success = await _repo.SaveAll();

        if (!success)
            return BadRequest("Problem restoring the product");

        return NoContent();
    }

    // Hard delete işlemi
    [HttpDelete("{id:int}/hard")]
    public async Task<ActionResult> HardDeleteProduct(int id)
    {
        var exists = await _repo.ExistsAsync(id);
        if (!exists)
            return NotFound("The product could not be found");

        _repo.HardDelete(id);
        var success = await _repo.SaveAll();

        if (!success)
            return BadRequest("Problem completely deleting the product");

        return NoContent();
    }
}
