using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
	public class CreateTaskViewModel
	{
		[Required(ErrorMessage = "Please, enter the Name!")]
		[MaxLength(255, ErrorMessage = "The number of characters should be less than 256 in the Task field!")]
		public string Name { get; set; } = null!;
		public DateTime? Deadline { get; set; }
		public string? CategoryName { get; set; }
	}
}
