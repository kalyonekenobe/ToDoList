using GraphQL;
using GraphQL.Types;
using ToDoList.API.GraphQL.Types;
using ToDoList.Enums;
using ToDoList.Extensions;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;

namespace ToDoList.API.GraphQL.Modules.Statuses
{
	public class StatusQueries : ObjectGraphType
	{
		private readonly IStatusRepository statusRepository;
		private readonly Storages storage = StorageController.DefaultStorage;

		public StatusQueries(IEnumerable<IStatusRepository> statusRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			statusRepository = statusRepositories.GetRequired(storage);

			Field<NonNullGraphType<ListGraphType<StatusType>>, IEnumerable<Business.Entities.Status>>()
				.Name("FetchAllAsync")
				.ResolveAsync(async context => await statusRepository.FetchAllAsync());

			Field<NonNullGraphType<ListGraphType<StatusType>>, IEnumerable<Business.Entities.Status>>()
				.Name("QueryAsync")
				.Argument<IdGraphType, int?>("Id", "Status id")
				.Argument<StringGraphType, string?>("Name", "Status name")
				.ResolveAsync(async context =>
				{
					object args = new
					{
						Id = context.GetArgument<int?>("Id"),
						Name = context.GetArgument<string?>("Name")
					};
					return await statusRepository.QueryAsync(args);
				});

			Field<StatusType, Business.Entities.Status>()
				.Name("GetByUdAsync")
				.Argument<NonNullGraphType<IdGraphType>, int>("Id", "Status id")
				.ResolveAsync(async context => await statusRepository.GetByIdAsync(context.GetArgument<int>("Id")));
		}
	}
}
