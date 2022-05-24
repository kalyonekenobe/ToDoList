using GraphQL.Types;

namespace ToDoList.API.GraphQL.Modules.Tasks.InputTypes
{
	public class TaskInputType : InputObjectGraphType<TaskInput>
	{
		public TaskInputType()
		{
			Field<DateTimeGraphType, DateTime?>().Name("Deadline").Resolve(context => context.Source.Deadline);
			Field<StringGraphType, string?>().Name("CategoryName").Resolve(context => context.Source.CategoryName);
			Field<NonNullGraphType<IdGraphType>, string>().Name("Name").Resolve(context => context.Source.Name);
		}
	}
}
