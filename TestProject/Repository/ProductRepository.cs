using TestProject. Data;
using TestProject.Interfaces;
using TestProject.Models;

namespace TestProject.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public bool AddProduct(Product product)
        {
            _context.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _context.Remove(product);
            return Save();
        }

        public Product GetProduct(Guid id)
        {
            return _context.Products.Where(p => p.ID == id).FirstOrDefault();
        }

        public Product GetProduct(string name)
        {
            return _context.Products.Where(p => p.Name == name).FirstOrDefault();
        }

        public ICollection<Product> GetProducts()
        {
            return _context.Products.OrderBy(p => p.ID).ToList();
        }

        public bool ProductExist(Guid id)
        {
            return _context.Products.Any(p => p.ID == id);
        }

        public bool ProductExist(string name)
        {
            foreach (var product in _context.Products)
            {
                if(product.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public bool UpdateProduct(Product product)
        {
            _context.Update(product);
            return Save();
        }

        private bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
