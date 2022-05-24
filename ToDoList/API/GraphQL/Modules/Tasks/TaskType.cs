using GraphQL.Types;
using ToDoList.Enums;
using ToDoList.Extensions;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;

namespace ToDoList.API.GraphQL.Types
{
	public class TaskType : ObjectGraphType<Business.Entities.Task>
	{
		private readonly ICategoryRepository categoryRepository;
		private readonly IStatusRepository statusRepository;
		private readonly Storages storage = StorageController.DefaultStorage;
		public TaskType(IEnumerable<ICategoryRepository> categoryRepositories, IEnumerable<IStatusRepository> statusRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			categoryRepository = categoryRepositories.GetRequired(storage);
			statusRepository = statusRepositories.GetRequired(storage);

			Field<NonNullGraphType<IdGraphType>, int>().Name("Id").Resolve(context => context.Source.Id);
			Field<NonNullGraphType<StringGraphType>, string>().Name("Name").Resolve(context => context.Source.Name);
			Field<DateTimeGraphType, DateTime?>().Name("Deadline").Resolve(context => context.Source.Deadline);
			Field<DateTimeGraphType, DateTime?>().Name("ExecutionTime").Resolve(context => context.Source.ExecutionTime);
			Field<StringGraphType, string?>().Name("CategoryName").Resolve(context => context.Source.CategoryName);
			Field<IntGraphType, int?>().Name("StatusId").Resolve(context => context.Source.StatusId);

			Field<CategoryType, Business.Entities.Category?>().Name("Category").ResolveAsync(async context => await categoryRepository.GetByNameAsync(context.Source.CategoryName ?? string.Empty));
			Field<StatusType, Business.Entities.Status?>().Name("Status").ResolveAsync(async context => await statusRepository.GetByIdAsync(context.Source.StatusId ?? -1));
		}
	}
}
