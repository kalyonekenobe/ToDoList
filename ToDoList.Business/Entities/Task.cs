namespace ToDoList.Business.Entities
{
	public class Task
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public DateTime? ExecutionTime { get; set; }
		public DateTime? Deadline { get; set; }
		public string? CategoryName { get; set; }
		public int? StatusId { get; set; }

		public Category? Category { get; set; }
		public Status? Status { get; set; }

		public void UpdateStatus()
		{
			StatusId = (int?)((!ExecutionTime.HasValue) ? (!Deadline.HasValue) ? Status.StatusCodes.Executing : (Deadline.Value >= DateTime.Now) ? Status.StatusCodes.Executing : Status.StatusCodes.Expired : Status.StatusCodes.Done);	
		}
	}
}
