using System;
using System.Security.Cryptography.X509Certificates;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository : IProductRepository
{
    private readonly StoreContext _context;

    public ProductRepository(StoreContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
    {
        var query = _context.Products.AsQueryable();

        if(!string.IsNullOrWhiteSpace(brand))
            query = query.Where(x => x.Brand == brand);
        
        if(!string.IsNullOrWhiteSpace(type))
            query = query.Where(x => x.Type == type);

        query = sort switch
        {
            "priceAsc" => query.OrderBy(x => x.Price),
            "priceDesc" => query.OrderByDescending(x => x.Price),
            _ => query.OrderBy(x => x.Name)
        }; 

        return await query
            .Where(p => p.DeletedDate == null) // Silinmemiş ürünler
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetDeletedProductsAsync()
    {
        return await _context.Products
            .Where(p => p.DeletedDate != null) // Silinmiş ürünler
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await _context.Products
            .Where(p => p.DeletedDate == null)
            .Select(x => x.Brand)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await _context.Products
            .Where(p => p.DeletedDate == null)
            .Select(x => x.Type)
            .Distinct()
            .ToListAsync();
    }

    public void AddProduct(Product product)
    {
        _context.Products.Add(product); // Ürün ekler
    }

    public void UpdateProduct(Product product)
    {
        _context.Products.Update(product); // Ürün günceller
    }

    public void SoftDeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            product.DeletedDate = DateTime.Now; // Soft delete işlemi
            _context.Products.Update(product);
        }
    }

    public void RestoreProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null && product.DeletedDate != null) // Eğer silindiyse
        {
            product.DeletedDate = null; // Soft delete'i geri al
            _context.Products.Update(product);
        }
    }

    public void HardDeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product); // Hard delete işlemi
        }
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

}
