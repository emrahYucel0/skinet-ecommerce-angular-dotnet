using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity<int>
{
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<IReadOnlyList<T>> ListAllDeletedAsync(); 
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> spec);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec); 
    void Add(T entity);
    void Update(T entity);
    void SoftDelete(int id);
    void Restore(int id);
    void HardDelete(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> SaveAll();
    Task<int> CountAsync(ISpecification<T> spec);
}
