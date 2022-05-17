using ToDoList.Business.Entities;
using ToDoList.Business.Repositories;

namespace ToDoList.Models
{
	public class CategoryListViewModel
	{
		public IEnumerable<Category>? Categories { get; set; }
	}
}
