using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using ToDoList.Business.Entities;
using ToDoList.Business.Repositories;

namespace ToDoList.MsSqlDb.Repositories
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly string connectionString;

		public CategoryRepository(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async System.Threading.Tasks.Task CreateAsync(Category category)
		{
			string query = @"INSERT INTO Categories(Name) VALUES (@Name)";
			using (var connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, category);
				if (affectedRows == 0)
					throw new NotImplementedException("Category creation error!");
			}
		}

		public async System.Threading.Tasks.Task RemoveAsync(string name)
		{
			string query = @"DELETE FROM Categories WHERE Name = @Name";
			using (var connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new { Name = name });
				if (affectedRows == 0)
					throw new NotImplementedException("Category deletion error!");
			}
		}

		public async System.Threading.Tasks.Task EditAsync(string editedName, Category category)
		{
			string query = @"UPDATE Categories SET Name = @Name WHERE Name = @EditedName";
			using (var connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new { Name = category.Name, EditedName = editedName });
				if (affectedRows == 0)
					throw new NotImplementedException("Category editing error!");
			}
		}

		public async System.Threading.Tasks.Task<IEnumerable<Category>> FetchAllAsync()
		{
			string query = @"SELECT * FROM Categories";
			using (var connection = new SqlConnection(connectionString))
			{
				return await connection.QueryAsync<Category>(query);
			}
		}

		public async System.Threading.Tasks.Task<Category> GetByNameAsync(string name)
		{
			string query = @"SELECT * FROM Categories WHERE Name = @Name";
			using (var connection = new SqlConnection(connectionString))
			{
				var category = await connection.QuerySingleOrDefaultAsync<Category>(query, new { Name = name });
				if (category == null)
					throw new NotImplementedException("Category wasn't found!");
				return category;
			}
		}
	}
}
