using BusinessObject;
using BusinessObject.DTO.ProductDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> getAllProduct()
        {
            try
            {
                List<Product> products = _context.Products.Include(x=>x.Category).Select(x => new Product
                {
                    ProductId = x.ProductId,
                    Weight = x.Weight,
                    UnitsInStock = x.UnitsInStock,
                    UnitPrice = x.UnitPrice,
                    CategoryId = x.Category.CategoryId,
                    ProductName = x.ProductName
                }).ToList();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> addNewProduct(ProductDTO productDTO)
        {
            try
            {
                _context.Products.Add(new Product
                {
                    CategoryId = productDTO.CategoryId,
                    ProductName = productDTO.ProductName,
                    UnitPrice = productDTO.UnitPrice,
                    UnitsInStock = productDTO.UnitsInStock,
                    Weight = productDTO.Weight,
                });
                _context.SaveChanges();
                return Ok("Add thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> getProductById(int id)
        {
            try
            {
                var product = _context.Products.Where(p => p.ProductId == id).Select(x => new Product
                {
                    ProductId = x.ProductId,
                    Weight = x.Weight,
                    UnitsInStock = x.UnitsInStock,
                    UnitPrice = x.UnitPrice,
                    CategoryId = x.Category.CategoryId,
                    ProductName = x.ProductName
                }).FirstOrDefault();

                if (product == null)
                {
                    return NotFound("Không tìm thấy sản phẩm");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> updateProduct(int id, ProductDTO productDTO)
        {
            try
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.ProductId == id);
                if (existingProduct == null)
                {
                    return NotFound("Sản phẩm không tồn tại");
                }

                existingProduct.CategoryId = productDTO.CategoryId;
                existingProduct.ProductName = productDTO.ProductName;
                existingProduct.UnitPrice = productDTO.UnitPrice;
                existingProduct.UnitsInStock = productDTO.UnitsInStock;
                existingProduct.Weight = productDTO.Weight;

                _context.SaveChanges();
                return Ok("Update thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteProduct(int id)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    return NotFound("Sản phẩm không tồn tại");
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
                return Ok("Delete thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }



        [HttpGet("search")]
        public async Task<ActionResult> searchProduct(string keyword, decimal? unitPrice)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(p => p.ProductName.Contains(keyword));
                }

                if (unitPrice.HasValue)
                {
                    query = query.Where(p => p.UnitPrice == unitPrice.Value);
                }

                var products = query.Select(x => new Product
                {
                    ProductId = x.ProductId,
                    Weight = x.Weight,
                    UnitsInStock = x.UnitsInStock,
                    UnitPrice = x.UnitPrice,
                    CategoryId = x.Category.CategoryId,
                    ProductName = x.ProductName
                }).ToList();

                if (products.Count == 0)
                {
                    return NotFound("Không tìm thấy sản phẩm nào phù hợp");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }

    }
}
