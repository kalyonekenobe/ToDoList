using Task = System.Threading.Tasks.Task;

namespace ToDoList.Business.Repositories
{
	public interface ITaskRepository
	{
		Task<IEnumerable<Entities.Task>> FetchAllAsync();
		Task<IEnumerable<Entities.Task>> QueryAsync(object args);
		Task<Entities.Task> GetByIdAsync(int id);
		Task CreateAsync(Entities.Task task);
		Task EditAsync(Entities.Task task);
		Task ExecuteAsync(Entities.Task task);
		Task RemoveAsync(int id);
	}
}
