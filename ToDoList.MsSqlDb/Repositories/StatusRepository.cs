using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using ToDoList.Business.Entities;
using ToDoList.Business.Repositories;

namespace ToDoList.MsSqlDb.Repositories
{
	public class StatusRepository : IStatusRepository
	{
		private readonly string connectionString;

		public StatusRepository(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async System.Threading.Tasks.Task CreateAsync(Status status)
		{
			string query = @"INSERT INTO Statuses (Name) VALUES (@Name) WHERE Id = @Id";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, status);
				if (affectedRows == 0)
					throw new NotImplementedException("Status creation error!");
			}
		}

		public async System.Threading.Tasks.Task RemoveAsync(int id)
		{
			string query = @"DELETE FROM Statuses WHERE Id = @Id";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Execute(query, new { Id = id });
				int affectedRows = await connection.ExecuteAsync(query, new { Id = id });
				if (affectedRows == 0)
					throw new NotImplementedException("Status deletion error!");
			}
		}

		public async System.Threading.Tasks.Task EditAsync(Status status)
		{
			string query = @"UPDATE Statuses SET Name = @Name WHERE Id = @Id";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, status);
				if (affectedRows == 0)
					throw new NotImplementedException("Status editing error!");
			}
		}

		public async System.Threading.Tasks.Task<IEnumerable<Status>> FetchAllAsync()
		{
			string query = @"SELECT * FROM Statuses";
			using (var connection = new SqlConnection(connectionString))
			{
				return await connection.QueryAsync<Status>(query);
			}
		}

		public async System.Threading.Tasks.Task<Status> GetByIdAsync(int id)
		{
			string query = @"SELECT * FROM Statuses WHERE Id = @Id";
			using (var connection = new SqlConnection(connectionString))
			{
				var status = await connection.QuerySingleOrDefaultAsync<Status>(query, new { Id = id });
				if (status == null)
					throw new NotImplementedException("Status wasn't found!");
				return status;
			}
		}

		public async System.Threading.Tasks.Task<IEnumerable<Status>> QueryAsync(object args)
		{
			string optionalQuery = string.Empty;
			foreach (var arg in args.GetType().GetProperties())
			{
				if (arg.GetValue(args, null) != null)
					optionalQuery += ((string.IsNullOrEmpty(optionalQuery)) ? "WHERE " : "AND ") + $"{arg.Name} = @{arg.Name} ";
			}

			string query = $@"SELECT * FROM Statuses {optionalQuery} ORDER BY StatusId ASC";
			using (var connection = new SqlConnection(connectionString))
			{
				return await connection.QueryAsync<Status>(query, args);
			}
		}
	}
}
