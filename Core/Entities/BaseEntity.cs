using System;

namespace Core.Entities;

public class BaseEntity<TId>
{
    public TId Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public BaseEntity()
    {
        Id = default;
        CreatedDate = DateTime.Now;
    }

    public BaseEntity(TId id)
    {
        Id = id;
    }
}
