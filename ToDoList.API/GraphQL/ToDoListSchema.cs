using GraphQL.Types;

namespace ToDoList.API.GraphQL
{
	public class ToDoListSchema : Schema
	{
		public ToDoListSchema(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			Query = serviceProvider.GetService<>();
			Mutation = serviceProvider.GetService();
		}
	}
}
