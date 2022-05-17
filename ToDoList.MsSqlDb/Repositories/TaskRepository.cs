using Dapper;
using ToDoList.Business.Repositories;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using ToDoList.Business.Entities;

namespace ToDoList.MsSqlDb.Repositories
{
	public class TaskRepository : ITaskRepository
	{
		private readonly string connectionString;
		public TaskRepository(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async System.Threading.Tasks.Task CreateAsync(Business.Entities.Task task)
		{
			task.UpdateStatus();
			string query = @"INSERT INTO Tasks (Name, ExecutionTime, Deadline, CategoryName, StatusId) 
							 VALUES (@Name, @ExecutionTime, @Deadline, @CategoryName, @StatusId)";
			using (var connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, task);
				if (affectedRows == 0) 
					throw new NotImplementedException("Task creation error!");
			}
		}

		public async System.Threading.Tasks.Task RemoveAsync(int id)
		{
			string query = @"DELETE FROM Tasks WHERE Id = @Id";
			using (var connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new { Id = id });
				if (affectedRows == 0)
					throw new NotImplementedException("Task deletion error!");
			}
		}

		public async System.Threading.Tasks.Task EditAsync(Business.Entities.Task task)
		{
			task.UpdateStatus();
			string query = @"UPDATE Tasks SET Name = @Name, 
											 ExecutionTime = @ExecutionTime, 
											 Deadline = @Deadline, 
											 CategoryName = @CategoryName, 
											 StatusId = @StatusId 
											 WHERE Id = @Id";
			using (var connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, task);
				if (affectedRows == 0)
					throw new NotImplementedException("Task editing error!");
			}
		}

		public async System.Threading.Tasks.Task ExecuteAsync(Business.Entities.Task task)
		{
			string query = @"UPDATE Tasks SET StatusId = @StatusId, ExecutionTime = @ExecutionTime WHERE Id = @Id";
			using (var connection = new SqlConnection(connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new { Id = task.Id, ExecutionTime = DateTime.Now, StatusId = Status.StatusCodes.Done });
				if (affectedRows == 0)
					throw new NotImplementedException("Task execution error!");
			}
		}

		public async System.Threading.Tasks.Task<IEnumerable<Business.Entities.Task>> FetchAllAsync()
		{
			string query = @"SELECT * FROM Tasks";
			using (var connection = new SqlConnection(connectionString))
			{
				return await connection.QueryAsync<Business.Entities.Task>(query);
			}
		}

		public async System.Threading.Tasks.Task<Business.Entities.Task> GetByIdAsync(int id)
		{
			string query = @"SELECT * FROM Tasks WHERE Id = @Id";
			using (var connection = new SqlConnection(connectionString))
			{
				var task = await connection.QuerySingleOrDefaultAsync<Business.Entities.Task>(query, new { Id = id });
				if (task == null)
					throw new NotImplementedException("Task wasn't found!");
				return task;
			}
		}

		public async System.Threading.Tasks.Task<IEnumerable<Business.Entities.Task>> QueryAsync(object args)
		{
			string optionalQuery = string.Empty;
			foreach (var arg in args.GetType().GetProperties())
			{
				if (arg.GetValue(args, null) != null)
					optionalQuery += ((string.IsNullOrEmpty(optionalQuery)) ? "WHERE " : "AND ") + $"{arg.Name} = @{arg.Name} ";
			}
			
			string query = @$"SELECT * FROM Tasks {optionalQuery} ORDER BY ExecutionTime DESC, CASE WHEN Deadline IS NULL THEN 1 ELSE 0 END, Deadline ASC";
			using (var connection = new SqlConnection(connectionString))
			{
				return await connection.QueryAsync<Business.Entities.Task>(query, args);
			}
		}
	}
}
