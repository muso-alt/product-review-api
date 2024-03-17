using TestProject.Dto;
using TestProject.Models;

namespace TestProject.Interfaces
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProduct(Guid id);
        Product GetProduct(string name);
        bool ProductExist(Guid id);
        bool ProductExist(string name);
        bool AddProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Product product);
    }
}
