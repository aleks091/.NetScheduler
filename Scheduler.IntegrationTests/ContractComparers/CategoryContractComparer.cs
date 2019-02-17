using Scheduler.Contracts;
using System.Collections;

namespace Scheduler.IntegrationTests.ContractComparers
{
    public class CategoryContractComparer : IComparer
    {
        public int Compare(object actual, object expected)
        {
            var actualCategory = (CategoryContract)actual;
            var expectedCategory = (CategoryContract)expected;

            return actualCategory.Id == expectedCategory.Id
                && actualCategory.Name == expectedCategory.Name
                && actualCategory.ProductCount == expectedCategory.ProductCount ? 0 : -1;
        }
    }
}
