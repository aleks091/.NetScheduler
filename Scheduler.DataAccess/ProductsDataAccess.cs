using Dapper;
using MySql.Data.MySqlClient;
using Scheduler.Contracts;
using Scheduler.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.DataAccess
{
    public class ProductsDataAccess
    {
        private readonly string _connectionString;

        public ProductsDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ProductContract>> GetByCategoryId(int categoryId)
        {
            const string sql = @"SELECT 
	                                 p.ProductId, 
                                     p.`Name` AS ProductName,
                                     c.CategoryId, 
                                     c.`Name` AS CategoryName
                                 FROM products p
                                 INNER JOIN category c
	                                 ON c.CategoryId = p.CategoryId
                                 WHERE c.CategoryId = @categoryId";

            using (var db = new MySqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<ProductDataModel>(sql, new
                {
                    categoryId
                });

                return result.Select(r => r.ToContract());
            }
        }

        public async Task<ProductContract> GetByProductId(int productId)
        {
            const string sql = @"SELECT 
	                                 p.ProductId, 
                                     p.`Name` AS ProductName,
                                     c.CategoryId, 
                                     c.`Name` AS CategoryName
                                 FROM products p
                                 INNER JOIN category c
	                                 ON c.CategoryId = p.CategoryId
                                 WHERE p.ProductId = @productId";

            using (var db = new MySqlConnection(_connectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<ProductDataModel>(sql, new
                {
                    productId
                });

                return result?.ToContract();
            }
        }

        public async Task<IEnumerable<ProductContract>> GetProducts(int take = 10, int skip = 0)
        {
            const string sql = @"SELECT 
	                                 p.ProductId, 
                                     p.`Name` AS ProductName,
                                     c.CategoryId, 
                                     c.`Name` AS CategoryName
                                 FROM products p
                                 INNER JOIN category c
	                                 ON c.CategoryId = p.CategoryId
                                 ORDER BY ProductName
                                 LIMIT @skip, @take;";

            using (var db = new MySqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<ProductDataModel>(sql, new
                {
                    take,
                    skip
                });

                return result.Select(r => r.ToContract());
            }
        }

        public async Task<IEnumerable<ProductContract>> Insert(params ProductContract[] products)
        {
            using (var copy = new SqlBulkCopy(_connectionString))
            {
                copy.DestinationTableName = "products";
                var table = new DataTable("products");
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("CategoryId", typeof(int));

                for (int i = 0; i < products.Length; ++i)
                {
                    table.Rows.Add(products[i].Name, products[i].Category.Id);
                }

                await copy.WriteToServerAsync(table);
            }

            return products;
        }
        
        public async Task<ProductContract> Insert(ProductContract product)
        {
            const string sql = @"INSERT INTO `products`
                                        (`ProductId`,
                                        `Name`,
                                        `CategoryId`)
                                    VALUES
                                        (@ProductId,
                                         @Name,
                                         @CategoryId);
                                    SELECT LAST_INSERT_ID();";

            using (var db = new MySqlConnection(_connectionString))
            {
                product.ProductId = await db.ExecuteScalarAsync<int>(sql, new
                {
                    product.ProductId,
                    product.Name,
                    CategoryId = product.Category.Id
                });
            }

            return product;
        }

        public async Task DeleteByCategoryId(int categoryId)
        {
            const string sql = @"DELETE FROM products WHERE CategoryId = @CategoryId";

            using (var db = new MySqlConnection(_connectionString))
            {
                await db.ExecuteAsync(sql, new
                {
                    categoryId
                });
            }
        }
    }
}
