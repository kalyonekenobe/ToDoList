using Microsoft.AspNetCore.Mvc;
using ToDoList.Business.Repositories;
using ToDoList.Business.Entities;
using ToDoList.Models;
using AutoMapper;
using ToDoList.Services;
using ToDoList.Enums;
using ToDoList.Extensions;

namespace ToDoList.Controllers
{
	public class TasksController : Controller
	{
		private readonly Storages storage = StorageController.DefaultStorage;
		private readonly ICategoryRepository categoryRepository;
		private readonly ITaskRepository taskRepository;
		private readonly IMapper mapper;

		public TasksController(IMapper mapper, IEnumerable<ITaskRepository> taskRepositories, IEnumerable<ICategoryRepository> categoryRepositories, IHttpContextAccessor httpContextAccessor)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			this.mapper = mapper;
			categoryRepository = categoryRepositories.GetRequired(storage);
			taskRepository = taskRepositories.GetRequired(storage);
		}

		[HttpGet]
		public async Task<IActionResult> List(string? category)
		{
			var model = await GetHomePageViewModel(category);
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateTaskFormViewModel createTaskFormViewModel)
		{
			try
			{
				var createTaskModel = createTaskFormViewModel.Task!;
				if (!ModelState.IsValid)
				{
					var model = await GetHomePageViewModel();
					model.CreateTaskForm ??= createTaskFormViewModel;
					return View("List", model);
				}
				var task = mapper.Map<CreateTaskViewModel, Business.Entities.Task>(createTaskModel);
				await taskRepository.CreateAsync(task);
				return RedirectToAction("List");
			}
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		[HttpGet]
		[Route("Tasks/Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await taskRepository.RemoveAsync(id);
				return RedirectToAction("List");
			}
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		[HttpGet]
		[Route("Tasks/Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var task = await taskRepository.GetByIdAsync(id);
				var model = new EditTaskFormViewModel()
				{
					Task = mapper.Map<Business.Entities.Task, EditTaskViewModel>(task),
					Categories = await categoryRepository.FetchAllAsync()
				};
				return View(model);
			}
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		[HttpPost]
		[Route("Tasks/Edit/{id}")]
		public async Task<IActionResult> Edit(int id, EditTaskFormViewModel editTaskFormViewModel)
		{
			try
			{
				var editTaskModel = editTaskFormViewModel.Task!;
				if (!ModelState.IsValid)
				{
					return View(editTaskFormViewModel);
				}
				var task = mapper.Map<EditTaskViewModel, Business.Entities.Task>(editTaskModel);
				task.Id = id;
				await taskRepository.EditAsync(task);
				return RedirectToAction("List");
			} 
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		[HttpGet]
		[Route("Tasks/Execute/{id}")]
		public async Task<IActionResult> Execute(int id)
		{
			try
			{
				var task = await taskRepository.GetByIdAsync(id);
				await taskRepository.ExecuteAsync(task);
				return RedirectToAction("List");
			}
			catch (NotImplementedException exception)
			{
				return BadRequest(exception.Message);
			}
		}

		private async Task<HomePageViewModel> GetHomePageViewModel(string? category = null)
		{
			var model = new HomePageViewModel();
			model.Categories = await categoryRepository.FetchAllAsync();
			model.InProcessTasks = await taskRepository.QueryAsync(new { StatusId = (int)Status.StatusCodes.Executing, CategoryName = category });
			model.DoneTasks = await taskRepository.QueryAsync(new { StatusId = (int)Status.StatusCodes.Done, CategoryName = category });
			model.ExpiredTasks = await taskRepository.QueryAsync(new { StatusId = (int)Status.StatusCodes.Expired, CategoryName = category });
			model.CreateTaskForm = new CreateTaskFormViewModel() { Categories = model.Categories };
			await DatabaseService.UpdateTasksStatusesAsync(taskRepository);
			return model;
		}
	}
}