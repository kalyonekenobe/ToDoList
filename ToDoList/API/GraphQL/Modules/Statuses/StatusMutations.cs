using AutoMapper;
using FluentValidation;
using GraphQL;
using GraphQL.Types;
using ToDoList.API.GraphQL.Modules.Statuses.InputTypes;
using ToDoList.API.GraphQL.Types;
using ToDoList.Business.Repositories;
using ToDoList.Controllers;
using ToDoList.Enums;
using ToDoList.Extensions;

namespace ToDoList.API.GraphQL.Modules.Statuses
{
	public class StatusMutations : ObjectGraphType
	{
		private readonly IStatusRepository statusRepository;
		private readonly Storages storage = StorageController.DefaultStorage;

		public StatusMutations(IEnumerable<IStatusRepository> statusRepositories, IHttpContextAccessor httpContextAccessor, IMapper mapper)
		{
			storage = (Storages)int.Parse(httpContextAccessor.HttpContext?.Request.Cookies["StorageId"] ?? ((int)storage).ToString());
			statusRepository = statusRepositories.GetRequired(storage);

			Field<StatusType, Business.Entities.Status>()
				.Name("CreateAsync")
				.Argument<NonNullGraphType<StatusInputType>, StatusInput>("StatusInput", "Status create input")
				.ResolveAsync(async context =>
				{
					var statusInput = context.GetArgument<StatusInput>("StatusInput");
					var validator = new StatusInputValidator();
					validator.ValidateAndThrow(statusInput);
					var status = mapper.Map<Business.Entities.Status>(statusInput);
					await statusRepository.CreateAsync(status);
					return status;
				});

			Field<StatusType, Business.Entities.Status>()
				.Name("EditAsync")
				.Argument<NonNullGraphType<StatusInputType>, StatusInput>("StatusInput", "Status edit input")
				.ResolveAsync(async context =>
				{
					var statusInput = context.GetArgument<StatusInput>("StatusInput");
					var validator = new StatusInputValidator();
					validator.ValidateAndThrow(statusInput);
					var status = mapper.Map<Business.Entities.Status>(statusInput);
					await statusRepository.EditAsync(status);
					return status;
				});

			Field<StatusType, Business.Entities.Status>()
				.Name("RemoveAsync")
				.Argument<NonNullGraphType<IdGraphType>, int>("Id", "Status id")
				.ResolveAsync(async context =>
				{
					var id = context.GetArgument<int>("Id");
					var status = await statusRepository.GetByIdAsync(id);
					await statusRepository.RemoveAsync(id);
					return status;
				});
		}
	}
}
