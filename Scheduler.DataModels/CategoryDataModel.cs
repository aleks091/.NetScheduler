using Scheduler.Contracts;

namespace Scheduler.DataModels
{
    public class CategoryDataModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public CategoryContract ToContract()
        {
            return new CategoryContract
            {
                Id = CategoryId,
                Name = Name
            };
        }
    }
}
