using FluentValidation;

namespace ToDoList.API.GraphQL.Modules.Statuses.InputTypes
{
	public class StatusInputValidator : AbstractValidator<StatusInput>
	{
		public StatusInputValidator()
		{
			RuleFor(status => status.Name).NotEmpty().NotNull();
		}
	}
}
