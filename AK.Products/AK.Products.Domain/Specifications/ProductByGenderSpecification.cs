using AK.Products.Domain.Entities;
using AK.Products.Domain.Enums;

namespace AK.Products.Domain.Specifications;

public class ProductByGenderSpecification : BaseSpecification<Product>
{
    public ProductByGenderSpecification(Gender gender) => AddCriteria(p => p.Gender == gender);
}
