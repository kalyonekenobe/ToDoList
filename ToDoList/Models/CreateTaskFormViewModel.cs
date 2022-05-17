using ToDoList.Business.Entities;

namespace ToDoList.Models
{
	public class CreateTaskFormViewModel
	{
		public IEnumerable<Category>? Categories { get; set; }
		public CreateTaskViewModel? Task { get; set; }
	}
}
