using FluentValidation;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;
using ToDoList.Enums;
using ToDoList.Extensions;

namespace ToDoList.API.GraphQL.Modules.Categories.InputTypes
{
	public class CategoryInputValidator : AbstractValidator<CategoryInput>
	{
		private readonly ICategoryRepository categoryRepository;
		private readonly Storages storage = StorageController.DefaultStorage;
		public CategoryInputValidator(IEnumerable<ICategoryRepository> categoryRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			categoryRepository = categoryRepositories.GetRequired(storage);
			RuleFor(category => category.Name).NotEmpty().NotNull().WhenAsync(async (category, cancellationToken) => await categoryRepository.GetByNameAsync(category.Name) is not null);
		}
	}
}
