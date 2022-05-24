using GraphQL.Types;
using ToDoList.API.GraphQL.Modules.Tasks;

namespace ToDoList.API.GraphQL
{
	public class RootMutations : ObjectGraphType
	{
		public RootMutations()
		{
			Field<TaskMutations>().Name("Tasks").Resolve(_ => new { });
		}
	}
}
