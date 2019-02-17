using NUnit.Framework;
using Scheduler.Contracts;
using System.Collections;

namespace Scheduler.IntegrationTests.ContractComparers
{
    public class ProductContractComparer : IComparer
    {
        private readonly CategoryContractComparer _categoryComparer;

        public ProductContractComparer(CategoryContractComparer categoryComparer)
        {
            _categoryComparer = categoryComparer;
        }

        public int Compare(object actual, object expected)
        {
            var actualProduct = (ProductContract)actual;
            var expectedProduct = (ProductContract)expected;

            Assert.That(actualProduct.Category, Is.EqualTo(expectedProduct.Category).Using(_categoryComparer));

            return actualProduct.ProductId == expectedProduct.ProductId
                && actualProduct.Name == expectedProduct.Name ? 0 : -1;
        }
    }
}
