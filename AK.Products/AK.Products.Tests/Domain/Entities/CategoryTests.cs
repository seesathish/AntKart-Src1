using AK.Products.Domain.Entities;
using AK.Products.Domain.Enums;
using FluentAssertions;

namespace AK.Products.Tests.Domain.Entities;

public sealed class CategoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCategory()
    {
        var cat = Category.Create("Shirts", "shirts", "Men's shirts", Gender.Men);
        cat.Name.Should().Be("Shirts");
        cat.Slug.Should().Be("shirts");
        cat.Description.Should().Be("Men's shirts");
        cat.TargetGender.Should().Be(Gender.Men);
        cat.IsActive.Should().BeTrue();
        cat.ParentCategoryId.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldLowercaseSlug()
    {
        var cat = Category.Create("Shirts", "MEN-SHIRTS");
        cat.Slug.Should().Be("men-shirts");
    }

    [Fact]
    public void Create_WithParentCategoryId_ShouldSetParent()
    {
        var cat = Category.Create("Casual", "casual", null, null, "parent-123");
        cat.ParentCategoryId.Should().Be("parent-123");
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrow()
    {
        var act = () => Category.Create("", "shirts");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptySlug_ShouldThrow()
    {
        var act = () => Category.Create("Shirts", "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ShouldUpdateAllProperties()
    {
        var cat = Category.Create("Shirts", "shirts");
        cat.Update("Trousers", "trousers", "Men's trousers", Gender.Women);
        cat.Name.Should().Be("Trousers");
        cat.Slug.Should().Be("trousers");
        cat.Description.Should().Be("Men's trousers");
        cat.TargetGender.Should().Be(Gender.Women);
    }

    [Fact]
    public void Update_ShouldLowercaseSlug()
    {
        var cat = Category.Create("Shirts", "shirts");
        cat.Update("Shirts", "SHIRTS-UPDATED", null, null);
        cat.Slug.Should().Be("shirts-updated");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var cat = Category.Create("Shirts", "shirts");
        cat.Deactivate();
        cat.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_AfterDeactivate_ShouldSetIsActiveTrue()
    {
        var cat = Category.Create("Shirts", "shirts");
        cat.Deactivate();
        cat.Activate();
        cat.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(Gender.Men)]
    [InlineData(Gender.Women)]
    [InlineData(Gender.Kids)]
    public void Create_ShouldSupportAllGenders(Gender gender)
    {
        var cat = Category.Create("Category", "category", null, gender);
        cat.TargetGender.Should().Be(gender);
    }
}
