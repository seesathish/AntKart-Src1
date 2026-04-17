using AK.Products.Domain.Entities;
using AK.Products.Domain.Enums;
using AK.Products.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AK.Products.Infrastructure.Seeders;

public sealed class ProductSeeder
{
    private readonly IMongoCollection<Product> _collection;
    private static readonly Random _rng = new(42);

    public ProductSeeder(MongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _collection = context.GetCollection<Product>(settings.Value.ProductsCollection);
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var count = await _collection.CountDocumentsAsync(_ => true, null, ct);
        if (count >= 300) return;

        await _collection.DeleteManyAsync(_ => true, ct);
        var products = GenerateProducts();
        await _collection.InsertManyAsync(products, null, ct);
    }

    private static List<Product> GenerateProducts()
    {
        var products = new List<Product>();

        // Men's products (100 records)
        string[] menCategories = ["Shirts", "Pants", "Jackets", "Suits", "Casual Wear", "T-Shirts", "Jeans", "Shorts", "Blazers", "Ethnic Wear"];
        string[] menBrands = ["ArrowMen", "PeterEngland", "Louis Philippe", "Raymond", "HRX", "UCB", "Wrangler", "Levis", "Zara Man", "H&M Men"];
        string[] menSizes = ["S", "M", "L", "XL", "XXL"];
        string[] menColors = ["Navy Blue", "White", "Black", "Grey", "Khaki", "Olive", "Brown", "Maroon", "Charcoal", "Sky Blue"];
        string[] menMaterials = ["Cotton", "Polyester", "Linen", "Wool", "Denim", "Silk Blend", "Rayon"];

        var menProductNames = new Dictionary<string, string[]>
        {
            ["Shirts"] = ["Classic Formal Shirt", "Casual Oxford Shirt", "Slim Fit Dress Shirt", "Mandarin Collar Shirt", "Check Print Shirt"],
            ["Pants"] = ["Slim Fit Chinos", "Regular Fit Trousers", "Cargo Pants", "Linen Trousers", "Formal Dress Pants"],
            ["Jackets"] = ["Leather Biker Jacket", "Denim Jacket", "Bomber Jacket", "Sports Jacket", "Windbreaker"],
            ["Suits"] = ["Classic Business Suit", "Wedding Tuxedo", "Single Breasted Suit", "Slim Fit Suit", "Double Breasted Suit"],
            ["Casual Wear"] = ["Weekend Casual Set", "Relaxed Fit Polo", "Graphic Print Tee", "Henley Top", "Button Down Casual"],
            ["T-Shirts"] = ["Plain Round Neck Tee", "V-Neck T-Shirt", "Polo T-Shirt", "Striped T-Shirt", "Printed T-Shirt"],
            ["Jeans"] = ["Slim Fit Jeans", "Regular Fit Jeans", "Skinny Jeans", "Straight Cut Jeans", "Ripped Jeans"],
            ["Shorts"] = ["Cargo Shorts", "Chino Shorts", "Beach Shorts", "Sports Shorts", "Casual Bermuda"],
            ["Blazers"] = ["Linen Blazer", "Formal Blazer", "Casual Sports Blazer", "Check Blazer", "Velvet Blazer"],
            ["Ethnic Wear"] = ["Kurta Pajama Set", "Dhoti Kurta", "Sherwani", "Nehru Jacket", "Pathani Suit"]
        };

        int menIdx = 0;
        foreach (var category in menCategories)
        {
            var names = menProductNames.ContainsKey(category) ? menProductNames[category]
                : [$"{category} Item A", $"{category} Item B", $"{category} Item C", $"{category} Item D", $"{category} Item E"];

            for (int i = 0; i < 10; i++)
            {
                var brand = menBrands[_rng.Next(menBrands.Length)];
                var name = $"{brand} {names[i % names.Length]} {(i / names.Length > 0 ? $"v{i / names.Length + 1}" : "")}".Trim();
                var price = Math.Round((decimal)(_rng.NextDouble() * 4500 + 499), 2);
                var stock = _rng.Next(0, 201);
                var colors = menColors.OrderBy(_ => _rng.Next()).Take(_rng.Next(1, 4)).ToList();
                var sizes = menSizes.OrderBy(_ => _rng.Next()).Take(_rng.Next(2, 5)).ToList();

                var product = Product.Create(
                    name,
                    $"Premium quality {name.ToLower()} for men. Made with {menMaterials[_rng.Next(menMaterials.Length)]} fabric. Perfect for all occasions.",
                    $"MEN-{category.Replace(" ", "").ToUpper().Substring(0, Math.Min(4, category.Replace(" ", "").Length))}-{(menIdx + 1):D3}",
                    brand, Gender.Men, category, null, price, "USD", stock, sizes, colors,
                    menMaterials[_rng.Next(menMaterials.Length)]);

                if (_rng.Next(5) == 0) product.SetFeatured(true);
                if (stock == 0) { /* already OutOfStock */ }
                else if (_rng.NextDouble() > 0.7) product.SetDiscount(Math.Round(price * (decimal)(0.6 + _rng.NextDouble() * 0.3), 2));

                products.Add(product);
                menIdx++;
            }
        }

        // Women's products (100 records)
        string[] womenCategories = ["Dresses", "Tops", "Skirts", "Blouses", "Jackets", "Kurtis", "Sarees", "Lehenga", "Jumpsuits", "Ethnic Fusion"];
        string[] womenBrands = ["W for Woman", "Biba", "Fabindia", "Mango", "Zara Women", "H&M Women", "AND", "Global Desi", "Aurelia", "Vero Moda"];
        string[] womenSizes = ["XS", "S", "M", "L", "XL", "XXL"];
        string[] womenColors = ["Rose Pink", "Mint Green", "Coral", "Ivory", "Teal", "Mauve", "Burgundy", "Royal Blue", "Peach", "Mustard Yellow"];
        string[] womenMaterials = ["Chiffon", "Georgette", "Crepe", "Silk", "Cotton", "Linen", "Rayon", "Jersey"];

        var womenProductNames = new Dictionary<string, string[]>
        {
            ["Dresses"] = ["Floral Maxi Dress", "Bodycon Dress", "A-Line Dress", "Wrap Dress", "Off-Shoulder Dress"],
            ["Tops"] = ["Crop Top", "Peplum Top", "Fitted Tank Top", "Flowy Blouse Top", "Cold Shoulder Top"],
            ["Skirts"] = ["Pleated Midi Skirt", "Mini Skirt", "Flared Skirt", "Pencil Skirt", "Wrap Skirt"],
            ["Blouses"] = ["Silk Blouse", "Embroidered Blouse", "Ruffled Blouse", "Formal Blouse", "Casual Blouse"],
            ["Jackets"] = ["Cropped Jacket", "Blazer Jacket", "Denim Jacket", "Puffer Vest", "Trench Coat"],
            ["Kurtis"] = ["Straight Kurti", "Anarkali Kurti", "A-Line Kurti", "Flared Kurti", "Short Kurti"],
            ["Sarees"] = ["Silk Saree", "Cotton Saree", "Georgette Saree", "Chiffon Saree", "Linen Saree"],
            ["Lehenga"] = ["Bridal Lehenga", "Party Lehenga", "Casual Lehenga", "Embroidered Lehenga", "Floral Lehenga"],
            ["Jumpsuits"] = ["Casual Jumpsuit", "Formal Jumpsuit", "Printed Jumpsuit", "Belted Jumpsuit", "Palazzo Jumpsuit"],
            ["Ethnic Fusion"] = ["Indo-Western Set", "Dhoti Pants Set", "Fusion Salwar Suit", "Modern Ethnic Top", "Palazzo Set"]
        };

        int womenIdx = 0;
        foreach (var category in womenCategories)
        {
            var names = womenProductNames.ContainsKey(category) ? womenProductNames[category]
                : [$"{category} Style A", $"{category} Style B", $"{category} Style C", $"{category} Style D", $"{category} Style E"];

            for (int i = 0; i < 10; i++)
            {
                var brand = womenBrands[_rng.Next(womenBrands.Length)];
                var name = $"{brand} {names[i % names.Length]} {(i / names.Length > 0 ? $"v{i / names.Length + 1}" : "")}".Trim();
                var price = Math.Round((decimal)(_rng.NextDouble() * 5000 + 599), 2);
                var stock = _rng.Next(0, 201);
                var colors = womenColors.OrderBy(_ => _rng.Next()).Take(_rng.Next(1, 4)).ToList();
                var sizes = womenSizes.OrderBy(_ => _rng.Next()).Take(_rng.Next(2, 5)).ToList();
                var material = womenMaterials[_rng.Next(womenMaterials.Length)];

                var product = Product.Create(
                    name,
                    $"Elegant and stylish {name.ToLower()} for women. Crafted with premium {material}. Perfect for every occasion.",
                    $"WOM-{category.Replace(" ", "").ToUpper().Substring(0, Math.Min(4, category.Replace(" ", "").Length))}-{(womenIdx + 1):D3}",
                    brand, Gender.Women, category, null, price, "USD", stock, sizes, colors, material);

                if (_rng.Next(5) == 0) product.SetFeatured(true);
                if (_rng.NextDouble() > 0.65) product.SetDiscount(Math.Round(price * (decimal)(0.55 + _rng.NextDouble() * 0.35), 2));

                products.Add(product);
                womenIdx++;
            }
        }

        // Kids' products (100 records)
        string[] kidsCategories = ["T-Shirts", "Pants", "Dresses", "Jumpsuits", "School Wear", "Party Wear", "Ethnic Wear", "Nightwear", "Jackets", "Shorts"];
        string[] kidsBrands = ["H&M Kids", "Zara Kids", "Gap Kids", "FirstCry", "Mothercare", "Gini & Jony", "Allen Solly Junior", "US Polo Kids", "Lilliput", "Toffyhouse"];
        string[] kidsSizes = ["3-4Y", "4-5Y", "5-6Y", "6-7Y", "7-8Y", "8-9Y", "9-10Y", "10-11Y", "11-12Y"];
        string[] kidsColors = ["Bright Red", "Sky Blue", "Sunny Yellow", "Lime Green", "Hot Pink", "Orange", "Purple", "Aqua", "Lavender", "Pastel Blue"];
        string[] kidsMaterials = ["Soft Cotton", "Fleece", "Denim", "Jersey", "Knit", "Velvet", "Organic Cotton"];

        var kidsProductNames = new Dictionary<string, string[]>
        {
            ["T-Shirts"] = ["Cartoon Print Tee", "Striped T-Shirt", "Superhero Tee", "Plain Round Neck", "Graphic Tee"],
            ["Pants"] = ["Elastic Waist Pants", "Jogger Pants", "Cargo Pants", "Slim Fit Jeans", "Corduroys"],
            ["Dresses"] = ["Frock with Bow", "Floral Sundress", "Party Frock", "Casual Dress", "Princess Dress"],
            ["Jumpsuits"] = ["Dungaree Jumpsuit", "Playsuit", "Romper", "Bibshort", "Overall Set"],
            ["School Wear"] = ["School Uniform Shirt", "School Trousers", "School Dress", "School Blazer", "PT Uniform"],
            ["Party Wear"] = ["Birthday Party Dress", "Ethnic Party Set", "Formal Party Suit", "Princess Gown", "Tuxedo Set"],
            ["Ethnic Wear"] = ["Kids Kurta Set", "Sherwani for Boys", "Lehenga Choli", "Dhoti Kurta", "Anarkali Suit"],
            ["Nightwear"] = ["Cartoon Pajama Set", "Night Suit", "Sleep Romper", "Onesie", "Star Print Nightwear"],
            ["Jackets"] = ["Puffer Jacket", "Hooded Sweatshirt", "Windcheater", "Denim Jacket", "Fleece Hoodie"],
            ["Shorts"] = ["Casual Shorts", "Cargo Shorts", "Swimming Shorts", "Cycle Shorts", "Sports Shorts"]
        };

        int kidsIdx = 0;
        foreach (var category in kidsCategories)
        {
            var names = kidsProductNames.ContainsKey(category) ? kidsProductNames[category]
                : [$"{category} Style A", $"{category} Style B", $"{category} Style C", $"{category} Style D", $"{category} Style E"];

            for (int i = 0; i < 10; i++)
            {
                var brand = kidsBrands[_rng.Next(kidsBrands.Length)];
                var name = $"{brand} {names[i % names.Length]} {(i / names.Length > 0 ? $"v{i / names.Length + 1}" : "")}".Trim();
                var price = Math.Round((decimal)(_rng.NextDouble() * 1500 + 199), 2);
                var stock = _rng.Next(0, 151);
                var colors = kidsColors.OrderBy(_ => _rng.Next()).Take(_rng.Next(1, 4)).ToList();
                var sizes = kidsSizes.OrderBy(_ => _rng.Next()).Take(_rng.Next(2, 5)).ToList();
                var material = kidsMaterials[_rng.Next(kidsMaterials.Length)];

                var product = Product.Create(
                    name,
                    $"Adorable and comfortable {name.ToLower()} for kids. Made with soft {material}. Easy to wear and care for.",
                    $"KID-{category.Replace(" ", "").ToUpper().Substring(0, Math.Min(4, category.Replace(" ", "").Length))}-{(kidsIdx + 1):D3}",
                    brand, Gender.Kids, category, null, price, "USD", stock, sizes, colors, material);

                if (_rng.Next(4) == 0) product.SetFeatured(true);
                if (_rng.NextDouble() > 0.6) product.SetDiscount(Math.Round(price * (decimal)(0.5 + _rng.NextDouble() * 0.4), 2));

                products.Add(product);
                kidsIdx++;
            }
        }

        return products;
    }
}
