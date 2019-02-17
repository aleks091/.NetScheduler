using Dapper;
using MySql.Data.MySqlClient;
using Scheduler.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.DataAccess
{
    public class CategoriesDataAccess
    {
        private readonly string _connectionString;

        public CategoriesDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<CategoryContract>> GetCategories(int take = 10, int skip = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CategoryContract>> Insert(params CategoryContract[] category)
        {
            throw new NotImplementedException();
        }

        public async Task<CategoryContract> Insert(CategoryContract category)
        {
            const string sql = @"INSERT INTO `category`
                                    (`CategoryId`,
                                    `Name`)
                                VALUES
                                    (@CategoryId,
                                     @Name);
                                SELECT LAST_INSERT_ID();";

            using (var db = new MySqlConnection(_connectionString))
            {
                category.Id = await db.ExecuteScalarAsync<int>(sql, new
                {
                    CategoryId = category.Id,
                    category.Name
                });
            }

            return category;
        }

        public async Task<IEnumerable<CategoryContract>> GetCategoriesWithProductCount(int take)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByCategoryId(int categoryId)
        {
            await DeleteByCategoryId(new int[] { categoryId });
        }

        public async Task DeleteByCategoryId(params int[] categoryId)
        {
            const string sql = @"DELETE FROM category WHERE CategoryId IN (@categoryId)";

            using (var db = new MySqlConnection(_connectionString))
            {
                await db.ExecuteAsync(sql, new { categoryId });
            }
        }

        public async Task<CategoryContract> GetByCategoryId(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CategoryContract>> GetByCategoryId(params int[] categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
