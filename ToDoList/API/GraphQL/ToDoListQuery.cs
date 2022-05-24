using GraphQL.Types;
using ToDoList.API.GraphQL.Modules.Statuses;
using ToDoList.API.GraphQL.Modules.Tasks;
using ToDoList.API.GraphQL.Queries;

namespace ToDoList.API.GraphQL
{
	public class ToDoListQuery : ObjectGraphType
	{
		public ToDoListQuery()
		{
			Field<TaskQueries>().Name("Tasks").Resolve(_ => new { });
			Field<CategoryQueries>().Name("Categories").Resolve(_ => new { });
			Field<StatusQueries>().Name("Statuses").Resolve(_ => new { });
		}
	}
}
