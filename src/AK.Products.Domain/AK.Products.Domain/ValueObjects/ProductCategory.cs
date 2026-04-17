namespace AK.Products.Domain.ValueObjects;

public sealed class ProductCategory : IEquatable<ProductCategory>
{
    public string Name { get; }
    public string? SubCategory { get; }

    public static readonly ProductCategory MenShirts = new("Shirts", "Men");
    public static readonly ProductCategory MenPants = new("Pants", "Men");
    public static readonly ProductCategory MenJackets = new("Jackets", "Men");
    public static readonly ProductCategory MenSuits = new("Suits", "Men");
    public static readonly ProductCategory MenCasual = new("Casual Wear", "Men");
    public static readonly ProductCategory WomenDresses = new("Dresses", "Women");
    public static readonly ProductCategory WomenTops = new("Tops", "Women");
    public static readonly ProductCategory WomenSkirts = new("Skirts", "Women");
    public static readonly ProductCategory WomenBlouses = new("Blouses", "Women");
    public static readonly ProductCategory WomenJackets = new("Jackets", "Women");
    public static readonly ProductCategory KidsTShirts = new("T-Shirts", "Kids");
    public static readonly ProductCategory KidsPants = new("Pants", "Kids");
    public static readonly ProductCategory KidsDresses = new("Dresses", "Kids");
    public static readonly ProductCategory KidsJumpsuits = new("Jumpsuits", "Kids");
    public static readonly ProductCategory KidsSchoolWear = new("School Wear", "Kids");

    public ProductCategory(string name, string? subCategory = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Category name is required", nameof(name));
        Name = name;
        SubCategory = subCategory;
    }

    public bool Equals(ProductCategory? other) => other is not null && Name == other.Name && SubCategory == other.SubCategory;
    public override bool Equals(object? obj) => obj is ProductCategory c && Equals(c);
    public override int GetHashCode() => HashCode.Combine(Name, SubCategory);
    public override string ToString() => SubCategory is not null ? $"{SubCategory} - {Name}" : Name;
}
