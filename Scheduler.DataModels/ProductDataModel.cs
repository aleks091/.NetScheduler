using Scheduler.Contracts;

namespace Scheduler.DataModels
{
    public class ProductDataModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public ProductContract ToContract()
        {
            return new ProductContract
            {
                Category = new CategoryContract
                {
                    Id = CategoryId,
                    Name = CategoryName
                },
                Name = ProductName,
                ProductId = ProductId
            };
        }
    }
}
