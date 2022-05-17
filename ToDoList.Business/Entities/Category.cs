namespace ToDoList.Business.Entities
{
	public class Category
	{
		public string Name { get; set; } = null!;

		public IEnumerable<Task>? Tasks { get; set; }
	}
}
