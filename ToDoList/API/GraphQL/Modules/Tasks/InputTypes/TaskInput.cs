namespace ToDoList.API.GraphQL.Modules.Tasks.InputTypes
{
	public class TaskInput
	{
		public DateTime? Deadline { get; set; }
		public string? CategoryName { get; set; }
		public string Name { get; set; } = null!;
	}
}
