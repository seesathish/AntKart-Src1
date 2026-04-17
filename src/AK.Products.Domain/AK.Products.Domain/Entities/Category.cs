using AK.Products.Domain.Common;
using AK.Products.Domain.Enums;

namespace AK.Products.Domain.Entities;

public class Category : Entity
{
    private Category() { }

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Gender? TargetGender { get; private set; }
    public string? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static Category Create(string name, string slug, string? description = null, Gender? targetGender = null, string? parentCategoryId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        return new Category
        {
            Name = name,
            Slug = slug.ToLower(),
            Description = description,
            TargetGender = targetGender,
            ParentCategoryId = parentCategoryId
        };
    }

    public void Update(string name, string slug, string? description, Gender? targetGender)
    {
        Name = name;
        Slug = slug.ToLower();
        Description = description;
        TargetGender = targetGender;
        SetUpdatedAt();
    }

    public void Deactivate() { IsActive = false; SetUpdatedAt(); }
    public void Activate() { IsActive = true; SetUpdatedAt(); }
}
