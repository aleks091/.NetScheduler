namespace Scheduler.Contracts
{
    public class ProductContract
    {
        public CategoryContract Category { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
    }
}
