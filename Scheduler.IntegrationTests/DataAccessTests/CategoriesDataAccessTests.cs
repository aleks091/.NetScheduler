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
    public class CategoriesDataAccessTests
    {
        private CategoryContractComparer _categoryComparer;

        private CategoriesDataAccess _categories;
        private IEnumerable<CategoryContract> _expectedCategories;
        
        [Test]
        [OneTimeSetUp]
        public async Task DADA_consulta_para_insertar_categorias_CUANDO_es_ejecutada_INSERTA_datos()
        {
            _categoryComparer = new CategoryContractComparer();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("Scheduler_MySQL");

            _categories = new CategoriesDataAccess(connectionString);

            var expectedCategoryA = await _categories.Insert(new CategoryContract { Name = RandomString.Generate() });
            var expectedCategoryB = await _categories.Insert(new CategoryContract { Name = RandomString.Generate() });

            _expectedCategories = new List<CategoryContract>
            {
                expectedCategoryA,
                expectedCategoryB
            };

            var actualCategoryA = await _categories.GetByCategoryId(expectedCategoryA.Id);
            Assert.That(actualCategoryA, Is.EqualTo(expectedCategoryA).Using(_categoryComparer));

            var actualCategoryB = await _categories.GetByCategoryId(expectedCategoryB.Id);
            Assert.That(actualCategoryB, Is.EqualTo(expectedCategoryB).Using(_categoryComparer));
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task DADA_consulta_para_regresar_cantidad_de_categorias_CUANDO_es_ejecutada_REGRESA_datos_esperados(int numberOfRecordsToFetch)
        {
            var actualCategories = await _categories.GetCategories(take: numberOfRecordsToFetch);

            Assert.That(actualCategories.Count(), Is.EqualTo(numberOfRecordsToFetch));
        }

        [TestCase(2)]
        public async Task DADA_consulta_para_regresar_categorias_con_cuenta_de_productos_CUANDO_es_ejecutada_REGRESA_datos_almacenados(int numberOfRecordsToFetch)
        {
            var actualCategories = await _categories.GetCategoriesWithProductCount(take: numberOfRecordsToFetch);

            Assert.That(actualCategories.Count(), Is.EqualTo(numberOfRecordsToFetch));
            Assert.That(actualCategories.Any(c => c.ProductCount > 0), Is.True);
        }

        [Test]
        [OneTimeTearDown]
        public async Task DADA_consulta_para_eliminar_categorias_CUANDO_es_ejecutada_ELIMINA_categorias()
        {
            var categoryIds = _expectedCategories.Select(category => category.Id).ToArray();

            await _categories.DeleteByCategoryId(categoryIds);

            var categories = await _categories.GetByCategoryId(categoryIds);

            Assert.That(categories, Is.Empty);
        }
    }
}
