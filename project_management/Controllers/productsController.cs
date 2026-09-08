using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_management.Models;
using project_management.Services;
using project_management.Data;
using project_management.Dtos;

namespace project_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productsController : ControllerBase
    {
        //static List<Product> products = new List<Product>
        //{
        //    new Product { Id = 1, Name = "Laptop", Price = 10.99m, Description = " laptop Description" },
        //    new Product { Id = 2, Name = "smartphone", Price = 19.99m, Description = "smartphone Description" },
        //    new Product { Id = 3, Name = "Tablet", Price = 5.99m, Description = "Tablet Description" }
        //};

        private readonly IProductService service; 

        public productsController(IProductService productService)
        {
            service = productService;
        }
        [HttpGet] 
        public IActionResult GetProducts()
        {
            // Sample data for demonstration purposes
            
            return Ok(service.GetAllProducts());
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetProductById(int id)
        {
            var response = service.GetProductById(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductRequest product)
        {
            var createdProduct = service.AddProduct(product);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateProduct(int id, Product Product)
        {
            try
            {
                service.UpdateProduct(id, Product);
                return NoContent(); // Return 204 No Content to indicate successful update
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating product: {ex.Message}");
            }   
        }
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                service.DeleteProduct(id);
                return NoContent(); // Return 204 No Content to indicate successful deletion
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting product: {ex.Message}");
            }
        }


    }
}
