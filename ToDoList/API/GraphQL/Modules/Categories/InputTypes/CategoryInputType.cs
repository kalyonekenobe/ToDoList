using GraphQL.Types;

namespace ToDoList.API.GraphQL.Modules.Categories.InputTypes
{
	public class CategoryInputType : InputObjectGraphType<CategoryInput>
	{
		public CategoryInputType()
		{
			Field<NonNullGraphType<IdGraphType>, string>().Name("Name").Resolve(context => context.Source.Name);
		}
	}
}
