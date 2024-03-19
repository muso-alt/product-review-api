using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TestProject.Dto;
using TestProject.Interfaces;
using TestProject.Models;

namespace TestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public MyProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        public IActionResult GetProducts()
        {
            var products = _mapper.Map<List<ProductDto>>(_productRepository.GetProducts());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(products);
        }

        [HttpGet("{name}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(400)]
        public IActionResult GetProducts(string name)
        {
            if (!_productRepository.ProductExist(name))
            {
                return NotFound();
            }

            var products = _mapper.Map<List<ProductDto>>(_productRepository.GetProducts());

            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }

            //var found = products.Where(p => p.Name.Contains(name)).ToList();
            var found = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(found);
        }

        [HttpGet("guid")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public IActionResult GetProduct(Guid guid)
        {
            if (!_productRepository.ProductExist(guid))
            {
                return NotFound();
            }

            var product = _mapper.Map<ProductDto>(_productRepository.GetProduct(guid));

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult AddProduct(ProductDto product)
        {
            var products = _mapper.Map<List<ProductDto>>(_productRepository.GetProducts());
            if(products.Any(p => p.Name.Equals(product.Name)))
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var productMap = _mapper.Map<Product>(product);

            if (!_productRepository.AddProduct(productMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully added");
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateProduct(Guid guid, [FromBody]Product updateProduct)
        {
            if(updateProduct == null)
            {
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExist(guid))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_productRepository.UpdateProduct(updateProduct))
            {
                ModelState.AddModelError("", "Something went wrong updating product");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{guid}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteProduct(Guid guid)
        {
            if (!_productRepository.ProductExist(guid))
            {
                return NotFound();
            }

            var product = _productRepository.GetProduct(guid);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("", "Something went wrong deleting product");
            }

            return NoContent();
        }
    }
}
