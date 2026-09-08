using System.Collections.Generic;
using project_management.Models;
using project_management.Dtos;

namespace project_management.Services
{
    public interface IProductService
    {
        IEnumerable<ProductResponse> GetAllProducts();
        ProductResponse? GetProductById(int id);
        ProductResponse AddProduct(ProductRequest product);
        void UpdateProduct(int id, Product product);
        void DeleteProduct(int id);
    }
}
