using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Scheduler.Contracts;
using Scheduler.DataAccess;
using Scheduler.IntegrationTests.ContractComparers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.IntegrationTests.DataAccessTests
{
    [TestFixture]
    public class ProductsDataAccessTests
    {
        private CategoryContractComparer _categoryComparer;
        private ProductContractComparer _productComparer;

        private ProductsDataAccess _products;
        private CategoriesDataAccess _categories;
        private CategoryContract _testCategory;

        private ProductContract _expectedProductA;
        private ProductContract _expectedProductB;
        private IEnumerable<ProductContract> _expectedProducts;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _categoryComparer = new CategoryContractComparer();
            _productComparer = new ProductContractComparer(_categoryComparer);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("Scheduler_MySQL");

            _products = new ProductsDataAccess(connectionString);
            _categories = new CategoriesDataAccess(connectionString);
        }

        [Test, Order(1)]
        public async Task DADA_consulta_para_insertar_productos_CUANDO_es_ejecutada_INSERTA_datos()
        {
            _testCategory = await _categories.Insert(new CategoryContract
            {
                Name = RandomString.Generate()
            });

            _expectedProductA = await _products.Insert(new ProductContract
            {
                Category = new CategoryContract
                {
                    Id = _testCategory.Id,
                    Name = _testCategory.Name
                },
                Name = RandomString.Generate()
            });

            _expectedProductB = await _products.Insert(new ProductContract
            {
                Category = new CategoryContract
                {
                    Id = _testCategory.Id,
                    Name = _testCategory.Name
                },
                Name = RandomString.Generate()
            });

            _expectedProducts = new List<ProductContract> { _expectedProductA, _expectedProductB };
        }

        [Test, Order(2)]
        public async Task DADA_consulta_por_producto_id_CUANDO_es_ejecutada_REGRESA_productos_esperados()
        {
            var actualProductA = await _products.GetByProductId(_expectedProductA.ProductId);
            Assert.That(actualProductA, Is.EqualTo(_expectedProductA).Using(_productComparer));

            var actualProductB = await _products.GetByProductId(_expectedProductB.ProductId);
            Assert.That(actualProductB, Is.EqualTo(_expectedProductB).Using(_productComparer));
        }

        [TestCase(1)]
        [TestCase(2), Order(3)]
        public async Task DADA_consulta_para_regresar_cantidad_de_productos_CUANDO_es_ejecutada_REGRESA_productos_esperados(int numberOfRecordsToFetch)
        {
            var actualProducts = await _products.GetProducts(take: numberOfRecordsToFetch);

            Assert.That(actualProducts.Count(), Is.EqualTo(numberOfRecordsToFetch));
        }

        [Test, Order(4)]
        public async Task DADA_consulta_para_eliminar_productos_por_categoria_id_CUANDO_es_ejecutada_ELIMINA_productos()
        {            
            await _products.DeleteByCategoryId(_testCategory.Id);
            await _categories.DeleteByCategoryId(_testCategory.Id);

            var products = await _products.GetByCategoryId(_testCategory.Id);

            Assert.That(products, Is.Empty);
        }
    }
}
