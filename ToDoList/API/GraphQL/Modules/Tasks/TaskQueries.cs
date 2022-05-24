using GraphQL;
using GraphQL.Types;
using ToDoList.API.GraphQL.Types;
using ToDoList.Enums;
using ToDoList.Extensions;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;

namespace ToDoList.API.GraphQL.Modules.Tasks
{
	public class TaskQueries : ObjectGraphType
	{
		private readonly ITaskRepository taskRepository;
		private readonly Storages storage = StorageController.DefaultStorage;

		public TaskQueries(IEnumerable<ITaskRepository> taskRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			taskRepository = taskRepositories.GetRequired(storage);

			Field<NonNullGraphType<ListGraphType<TaskType>>, IEnumerable<Business.Entities.Task>>()
				.Name("FetchAllAsync")
				.ResolveAsync(async context => await taskRepository.FetchAllAsync());

			Field<NonNullGraphType<ListGraphType<TaskType>>, IEnumerable<Business.Entities.Task>>()
				.Name("QueryAsync")
				.Argument<IdGraphType, int?>("Id", "Task id")
				.Argument<StringGraphType, string?>("Name", "Task name")
				.Argument<DateTimeGraphType, DateTime?>("Deadline", "Task deadline")
				.Argument<DateTimeGraphType, DateTime?>("ExecutionTime", "Task execution time")
				.Argument<StringGraphType, string?>("CategoryName", "Task category name")
				.Argument<IntGraphType, int?>("StatusId", "Task status id")
				.ResolveAsync(async context =>
				{
					object args = new
					{
						Id = context.GetArgument<int?>("Id"),
						Name = context.GetArgument<string?>("Name"),
						Deadline = context.GetArgument<DateTime?>("Deadline"),
						ExecutionTime = context.GetArgument<DateTime?>("ExecutionTime"),
						CategoryName = context.GetArgument<string?>("CategoryName"),
						StatusId = context.GetArgument<int?>("StatusId")
					};
					return await taskRepository.QueryAsync(args);
				});

			Field<TaskType, Business.Entities.Task>()
				.Name("GetByIdAsync")
				.Argument<NonNullGraphType<IdGraphType>, int>("Id", "Task id")
				.ResolveAsync(async context => await taskRepository.GetByIdAsync(context.GetArgument<int>("Id")));
		}
	}
}
