using AK.Products.Domain.ValueObjects;
using FluentAssertions;

namespace AK.Products.Tests.Domain.ValueObjects;

public sealed class ProductCategoryTests
{
    [Fact]
    public void Constructor_WithValidName_ShouldCreateCategory()
    {
        var category = new ProductCategory("Shirts", "Men");
        category.Name.Should().Be("Shirts");
        category.SubCategory.Should().Be("Men");
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        var act = () => new ProductCategory("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithWhitespaceName_ShouldThrowArgumentException()
    {
        var act = () => new ProductCategory("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNullSubCategory_ShouldBeAllowed()
    {
        var category = new ProductCategory("Shirts");
        category.SubCategory.Should().BeNull();
    }

    [Fact]
    public void Equals_WithSameNameAndSubCategory_ShouldBeTrue()
    {
        var c1 = new ProductCategory("Shirts", "Men");
        var c2 = new ProductCategory("Shirts", "Men");
        c1.Equals(c2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentSubCategory_ShouldBeFalse()
    {
        var c1 = new ProductCategory("Shirts", "Men");
        var c2 = new ProductCategory("Shirts", "Women");
        c1.Equals(c2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ShouldBeFalse()
    {
        new ProductCategory("Shirts").Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithObject_ShouldWorkCorrectly()
    {
        var c1 = new ProductCategory("Shirts", "Men");
        var c2 = new ProductCategory("Shirts", "Men");
        c1.Equals((object)c2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNonProductCategoryObject_ShouldBeFalse()
    {
        var c1 = new ProductCategory("Shirts");
        c1.Equals("Shirts").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldBeEqual()
    {
        var c1 = new ProductCategory("Shirts", "Men");
        var c2 = new ProductCategory("Shirts", "Men");
        c1.GetHashCode().Should().Be(c2.GetHashCode());
    }

    [Fact]
    public void ToString_WithSubCategory_ShouldIncludeBoth()
    {
        new ProductCategory("Shirts", "Men").ToString().Should().Be("Men - Shirts");
    }

    [Fact]
    public void ToString_WithoutSubCategory_ShouldReturnNameOnly()
    {
        new ProductCategory("Shirts").ToString().Should().Be("Shirts");
    }

    [Fact]
    public void StaticProperties_ShouldBeInitialized()
    {
        ProductCategory.MenShirts.Name.Should().Be("Shirts");
        ProductCategory.MenShirts.SubCategory.Should().Be("Men");
        ProductCategory.WomenDresses.Name.Should().Be("Dresses");
        ProductCategory.WomenDresses.SubCategory.Should().Be("Women");
        ProductCategory.KidsTShirts.Name.Should().Be("T-Shirts");
        ProductCategory.KidsTShirts.SubCategory.Should().Be("Kids");
    }
}
