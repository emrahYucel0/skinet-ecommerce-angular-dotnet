using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort); // Aktif ürünleri getirir
    Task<IReadOnlyList<Product>> GetDeletedProductsAsync(); // Silinmiş ürünleri getirir
    Task<Product?> GetProductByIdAsync(int id); // ID'ye göre ürün getirir
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();
    void AddProduct(Product product); // Ürün ekler
    void UpdateProduct(Product product); // Ürün günceller
    void SoftDeleteProduct(int id); // Ürünü soft delete yapar
    void RestoreProduct(int id); // Soft delete yapılmış ürünü geri getirir
    void HardDeleteProduct(int id); // Ürünü hard delete ile tamamen siler
    Task<bool> ProductExistsAsync(int id); // Ürünün var olup olmadığını kontrol eder
    Task<bool> SaveChangesAsync(); // Değişiklikleri kaydeder
}
