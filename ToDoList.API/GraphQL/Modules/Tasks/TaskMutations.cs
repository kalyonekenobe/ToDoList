using AutoMapper;
using FluentValidation;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using ToDoList.API.GraphQL.Modules.Tasks.InputTypes;
using ToDoList.API.GraphQL.Types;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;
using ToDoList.Enums;
using ToDoList.Extensions;

namespace ToDoList.API.GraphQL.Modules.Tasks
{
	public class TaskMutations : ObjectGraphType
	{
		private readonly ITaskRepository taskRepository;
		private readonly Storages storage = StorageController.DefaultStorage;

		public TaskMutations(IEnumerable<ITaskRepository> taskRepositories, IHttpContextAccessor httpContextAccessor, IMapper mapper)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			taskRepository = taskRepositories.GetRequired(storage);

			Field<TaskType, Business.Entities.Task>()
				.Name("CreateAsync")
				.Argument<NonNullGraphType<TaskInputType>, TaskInput>("TaskInput", "Task create input")
				.ResolveAsync(async context =>
				{
					var taskInput = context.GetArgument<TaskInput>("TaskInput");
					var validator = new TaskInputValidator();
					validator.ValidateAndThrow(taskInput);
					var task = mapper.Map<Business.Entities.Task>(taskInput);
					await taskRepository.CreateAsync(task);
					return task;
				});

			Field<TaskType, Business.Entities.Task>()
				.Name("EditAsync")
				.Argument<NonNullGraphType<TaskInputType>, TaskInput>("TaskInput", "Task edit input")
				.ResolveAsync(async context =>
				{
					var taskInput = context.GetArgument<TaskInput>("TaskInput");
					var validator = new TaskInputValidator();
					validator.ValidateAndThrow(taskInput);
					var task = mapper.Map<Business.Entities.Task>(taskInput);
					await taskRepository.EditAsync(task);
					return task;
				});

			Field<TaskType, Business.Entities.Task>()
				.Name("RemoveAsync")
				.Argument<NonNullGraphType<IdGraphType>, int>("Id", "Task id")
				.ResolveAsync(async context =>
				{
					var id = context.GetArgument<int>("Id");
					var task = await taskRepository.GetByIdAsync(id);
					await taskRepository.RemoveAsync(id);
					return task;
				});

			Field<TaskType, Business.Entities.Task>()
				.Name("ExecuteAsync")
				.Argument<NonNullGraphType<TaskType>, Business.Entities.Task>("Task", "Task to execute")
				.ResolveAsync(async context =>
				{
					var task = context.GetArgument<Business.Entities.Task>("Task");
					await taskRepository.ExecuteAsync(task);
					return await taskRepository.GetByIdAsync(task.Id);
				});
		}

	}
}
