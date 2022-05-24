using FluentValidation;

namespace ToDoList.API.GraphQL.Modules.Tasks.InputTypes
{
	public class TaskInputValidator : AbstractValidator<TaskInput>
	{
		public TaskInputValidator()
		{
			RuleFor(task => task.Deadline);
			RuleFor(task => task.CategoryName);
			RuleFor(task => task.Name).NotEmpty().NotNull();
		}
	}
}
