using GraphQL.Types;
using ToDoList.API.GraphQL.Types;

namespace ToDoList.API.GraphQL.Modules.Statuses.InputTypes
{
	public class StatusInputType : InputObjectGraphType<StatusType>
	{
		public StatusInputType()
		{
			Field<NonNullGraphType<StringGraphType>, string>().Name("Name").Resolve(context => context.Source.Name);
		}
	}
}
