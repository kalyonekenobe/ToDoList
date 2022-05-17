using ToDoList.Business.Entities;
using Task = System.Threading.Tasks.Task;

namespace ToDoList.Business.Repositories
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> FetchAllAsync();
		Task<Category> GetByNameAsync(string name);
		Task CreateAsync(Category category);
		Task EditAsync(string editedName, Category category);
		Task RemoveAsync(string name);
	}
}
