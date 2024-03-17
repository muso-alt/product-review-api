using TestProject.Data;
using TestProject.Models;

namespace TestProject
{
    public class Seed
    {
        private readonly DataContext dataContext;

        public Seed(DataContext context)
        {
            dataContext = context;
        }

        public void SeedDataContext()
        {
            if (!dataContext.Products.Any())
            {
                var products = new List<Product>()
                {
                    new Product{ Name = "Some Item ", Description = "Good items"}
                };

                dataContext.Products.AddRange(products);
                dataContext.SaveChanges(); 
            }
        }
    }
}
