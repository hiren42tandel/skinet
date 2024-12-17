using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(string? brands, string? type, string? sort) : base(x => 
        (string.IsNullOrWhiteSpace(brands) || x.Brand == brands) &&
        (string.IsNullOrWhiteSpace(type) || x.Type == type)
        )
    {
        switch (sort)
        {
            case "priceAsc":
                AddOrderBy(x => x.Price);
                break;
            case "priceDesc":
                AddOrderByDescending(x => x.Price);
                break;
            default:
                AddOrderBy(x => x.Name);
                break;
        }
        
    }
    
}