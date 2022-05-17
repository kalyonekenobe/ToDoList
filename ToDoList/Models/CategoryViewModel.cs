using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
	public class CategoryViewModel
	{
		[Required(ErrorMessage = "Please, enter the Name!")]
		public string Name { get; set; } = null!;
				
	}
}
