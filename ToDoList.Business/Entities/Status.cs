namespace ToDoList.Business.Entities
{
	public class Status
	{
		public enum StatusCodes
		{
			Executing = 1,
			Done = 2,
			Expired = 3
		};

		public int Id { get; set; }
		public string Name { get; set; } = null!;

		public IEnumerable<Task>? Tasks { get; set; }
	}
}
