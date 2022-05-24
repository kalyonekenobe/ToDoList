using AutoMapper;
using FluentValidation;
using GraphQL;
using GraphQL.Types;
using ToDoList.API.GraphQL.Modules.Categories.InputTypes;
using ToDoList.API.GraphQL.Types;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;
using ToDoList.Enums;
using ToDoList.Extensions;

namespace ToDoList.API.GraphQL.Modules.Categories
{
	public class CategoryMutations : ObjectGraphType
	{
		private readonly ICategoryRepository categoryRepository;
		private readonly Storages storage = StorageController.DefaultStorage;

		public CategoryMutations(IEnumerable<ICategoryRepository> categoryRepositories, IHttpContextAccessor httpContextAccessor, IMapper mapper)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StotageId"] ?? ((int)storage).ToString());
			categoryRepository = categoryRepositories.GetRequired(storage);

			Field<CategoryType, Business.Entities.Category>()
				.Name("CreateAsync")
				.Argument<NonNullGraphType<CategoryInputType>, CategoryInput>("CategoryInput", "Category create input")
				.ResolveAsync(async context =>
				{
					var categoryInput = context.GetArgument<CategoryInput>("CategoryInput");
					var validator = new CategoryInputValidator(categoryRepositories, httpContextAccessor);
					validator.ValidateAndThrow(categoryInput);
					var category = mapper.Map<Business.Entities.Category>(categoryInput);
					await categoryRepository.CreateAsync(category);
					return category;
				});

			Field<CategoryType, Business.Entities.Category>()
				.Name("EditAsync")
				.Argument<NonNullGraphType<IdGraphType>, string>("EditedName", "The edited name of the category")
				.Argument<NonNullGraphType<CategoryInputType>, CategoryInput>("CategoryInput", "Category edit input")
				.ResolveAsync(async context =>
				{
					var categoryInput = context.GetArgument<CategoryInput>("CategoryInput");
					var validator = new CategoryInputValidator(categoryRepositories, httpContextAccessor);
					validator.ValidateAndThrow(categoryInput);
					var category = mapper.Map<Business.Entities.Category>(categoryInput);
					await categoryRepository.EditAsync(context.GetArgument<string>("EditedName"), category);
					return category;
				});

			Field<CategoryType, Business.Entities.Category>()
				.Name("RemoveAsync")
				.Argument<NonNullGraphType<IdGraphType>, string>("Name", "Category name")
				.ResolveAsync(async context =>
				{
					var name = context.GetArgument<string>("Name");
					var category = await categoryRepository.GetByNameAsync(name);
					await categoryRepository.RemoveAsync(name);
					return category;
				});
		}
	}
}
