using ToDoList.Business.Entities;
using Task = ToDoList.Business.Entities.Task;

namespace ToDoList.Models
{
	public class HomePageViewModel
	{
		public IEnumerable<Category> Categories { get; set; } = null!;
		public IEnumerable<Task> InProcessTasks { get; set; } = null!;
		public IEnumerable<Task> DoneTasks { get; set; } = null!;
		public IEnumerable<Task> ExpiredTasks { get; set; } = null!;
		public CreateTaskFormViewModel CreateTaskForm { get; set; } = null!;
	}
}
