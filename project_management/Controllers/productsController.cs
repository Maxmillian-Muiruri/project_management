using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_management.Models;

namespace project_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productsController : ControllerBase
    {
        static List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 10.99m, Description = " laptop Description" },
            new Product { Id = 2, Name = "smartphone", Price = 19.99m, Description = "smartphone Description" },
            new Product { Id = 3, Name = "Tablet", Price = 5.99m, Description = "Tablet Description" }
        };
        [HttpGet]
        public IActionResult GetProducts()
        {
            // Sample data for demonstration purposes
            
            return Ok(products);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetProductById(int id)
        {
            var response = products.FirstOrDefault(p => p.Id == id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateProduct(int id, Product updatedProduct)
        {
            var existingProduct = products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            // Update the properties of the existing product
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Description = updatedProduct.Description;
            return NoContent(); // Return 204 No Content to indicate successful update
        }
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            products.Remove(product);
            return NoContent(); // Return 204 No Content to indicate successful deletion
        }


    }
}
