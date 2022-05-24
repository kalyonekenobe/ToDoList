using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using ToDoList.API.GraphQL.Types;
using ToDoList.Business.Enums;
using ToDoList.Business.Extensions;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;

namespace ToDoList.API.GraphQL.Queries
{
	public class CategoryQueries : ObjectGraphType
	{
		private readonly ICategoryRepository categoryRepository;
		private readonly Storages storage = StorageController.DefaultStorage;
		
		public CategoryQueries(IEnumerable<ICategoryRepository> categoryRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			categoryRepository = categoryRepositories.GetRequired(storage);

			Field<NonNullGraphType<ListGraphType<CategoryType>>, IEnumerable<Business.Entities.Category>>()
				.Name("FetchAllAsync")
				.ResolveAsync(async context => await categoryRepository.FetchAllAsync());

			Field<NonNullGraphType<CategoryType>, Business.Entities.Category>()
				.Name("GetByNameAsync")
				.Argument<IdGraphType, string>("Name", "Category name")
				.ResolveAsync(async context => await categoryRepository.GetByNameAsync(context.GetArgument<string>("Name")));
		}
	}
}
