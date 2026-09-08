using project_management.Models;
using project_management.Data;
using project_management.Dtos;

namespace project_management.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext context;
        public ProductService(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public ProductResponse AddProduct(ProductRequest productRequest) 
        {
            var product = new Product
            {
                Id = 0,
                Name = productRequest.Name,
                Description = productRequest.Description,
                Price = productRequest.Price
            };

            var newProduct = context.Products.Add(product);
            context.SaveChanges();

            var response = new ProductResponse
            {
                Id = newProduct.Entity.Id,
                Name = newProduct.Entity.Name,
                Description = newProduct.Entity.Description,
                Price = newProduct.Entity.Price
            };

            return response;
        }

        public void DeleteProduct(int id)
        {
            var product = context.Products.Find(id);
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
        }

        public IEnumerable<ProductResponse> GetAllProducts()
        {
            var products = context.Products.ToList();

            var response = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            });

            return response;
        }

        public ProductResponse? GetProductById(int id)
        {
            var product = context.Products.Find(id);

            var response = product  == null ? null :  new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            });
            return response;
        } 

        public void UpdateProduct(int id, Product product)
        {
            var existingProduct = context.Products.Find(id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                context.SaveChanges();
            }
        }
    }
}
