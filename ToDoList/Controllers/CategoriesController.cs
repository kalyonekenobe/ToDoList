using Microsoft.AspNetCore.Mvc;
using ToDoList.Business.Entities;
using ToDoList.Business.Repositories;
using ToDoList.Models;
using AutoMapper;
using ToDoList.Enums;
using ToDoList.Extensions;

namespace ToDoList.Controllers
{
	public class CategoriesController : Controller
	{
		private readonly Storages storage = StorageController.DefaultStorage;
		private readonly ICategoryRepository categoryRepository;
		private readonly ITaskRepository taskRepository;
		private readonly IMapper mapper;

		public CategoriesController(IMapper mapper, IEnumerable<ICategoryRepository> categoryRepositories, IEnumerable<ITaskRepository> taskRepositories, IHttpContextAccessor httpContextAccessor)
		{

			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			this.mapper = mapper;
			categoryRepository = categoryRepositories.GetRequired(storage);
			taskRepository = taskRepositories.GetRequired(storage);
		}

		[HttpGet]
		public async Task<IActionResult> List()
		{
			CategoryListViewModel model = new CategoryListViewModel()
			{
				Categories = await categoryRepository.FetchAllAsync(),
			};
			foreach (var category in model.Categories)
			{
				category.Tasks = await taskRepository.QueryAsync(new { CategoryName = category.Name });
			}
			return View(model);
		}

		[HttpGet]
		public IActionResult Create() => View();
		

		[HttpPost]
		public async Task<IActionResult> Create(CategoryViewModel model)
		{
			try
			{
				if (!ModelState.IsValid)
					return View(model);

				var category = mapper.Map<Category>(model);

				try
				{
					await categoryRepository.GetByNameAsync(category.Name);
				} 
				catch (NotImplementedException)
				{
					await categoryRepository.CreateAsync(category);
					return RedirectToAction("List");
				}

				ModelState.AddModelError("Name", "The category with this name already exists!");
				return View(model);
			}
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		[HttpGet]
		[Route("Categories/Delete/{name}")]
		public async Task<IActionResult> Delete(string name)
		{
			try
			{
				if (name != null)
					await categoryRepository.RemoveAsync(name);
				return RedirectToAction("List");
			} 
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		[HttpGet]
		[Route("Categories/Edit/{name}")]
		public async Task<IActionResult> Edit(string name)
		{
			try
			{
				var category = await categoryRepository.GetByNameAsync(name);
				var categoryModel = mapper.Map<CategoryViewModel>(category);
				var model = new EditCategoryViewModel() { Category = categoryModel, EditedName = name };
				return View(model);
			}
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditCategoryViewModel model)
		{
			try
			{
				if (!ModelState.IsValid)
					return View(model);

				var category = mapper.Map<Category>(model.Category);

				try
				{
					await categoryRepository.GetByNameAsync(category.Name);
				}
				catch(NotImplementedException)
				{
					await categoryRepository.EditAsync(model.EditedName ?? string.Empty, category);
					return RedirectToAction("List");
				}

				ModelState.AddModelError("Name", "The category with this name already exists!");
				return View(model);
			}
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}
	}
}
