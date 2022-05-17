using ToDoList.Business.Entities;

namespace ToDoList.Models
{
	public class EditTaskFormViewModel
	{
		public IEnumerable<Category>? Categories { get; set; }
		public EditTaskViewModel? Task { get; set; }
	}
}
