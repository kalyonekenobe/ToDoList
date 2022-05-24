using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using ToDoList.Business.Enums;
using ToDoList.Business.Extensions;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;

namespace ToDoList.API.GraphQL.Types
{
	public class StatusType : ObjectGraphType<Business.Entities.Status>
	{
		private readonly ITaskRepository taskRepository;
		private readonly Storages storage = StorageController.DefaultStorage;
		public StatusType(IEnumerable<ITaskRepository> taskRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			taskRepository = taskRepositories.GetRequired(storage);

			Field<NonNullGraphType<IdGraphType>, int>().Name("Id").Resolve(context => context.Source.Id);
			Field<NonNullGraphType<StringGraphType>, string>().Name("Name").Resolve(context => context.Source.Name);

			Field<ListGraphType<TaskType>, IEnumerable<Business.Entities.Task>>().Name("Tasks").ResolveAsync(async context => await taskRepository.QueryAsync(new { StatusId = context.Source.Id }));
		}
	}
}
