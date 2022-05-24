using GraphQL.Types;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;
using ToDoList.Enums;
using ToDoList.Extensions;

namespace ToDoList.API.GraphQL.Types
{
	public class CategoryType : ObjectGraphType<Business.Entities.Category>
	{
		private readonly ITaskRepository taskRepository;
		private readonly Storages storage = StorageController.DefaultStorage;
		public CategoryType(IEnumerable<ITaskRepository> taskRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			taskRepository = taskRepositories.GetRequired(storage);

			Field<NonNullGraphType<IdGraphType>, string>().Name("Name").Resolve(context => context.Source.Name);
			Field<ListGraphType<TaskType>, IEnumerable<Business.Entities.Task>>().Name("Tasks").ResolveAsync(async context => await taskRepository.QueryAsync(new { CategoryName = context.Source.Name }));
		}
	}
}
