using GraphQL.Types;
using ToDoList.API.GraphQL.Modules.Categories;
using ToDoList.API.GraphQL.Modules.Statuses;
using ToDoList.API.GraphQL.Modules.Tasks;

namespace ToDoList.API.GraphQL
{
	public class ToDoListMutation : ObjectGraphType
	{
		public ToDoListMutation()
		{
			Field<TaskMutations>().Name("Tasks").Resolve(_ => new { });
			Field<CategoryMutations>().Name("Categories").Resolve(_ => new { });
			Field<StatusMutations>().Name("Statuses").Resolve(_ => new { });
		}
	}
}
