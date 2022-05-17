using ToDoList.Business.Entities;
using Task = System.Threading.Tasks.Task;

namespace ToDoList.Business.Repositories
{
	public interface IStatusRepository
	{
		Task<IEnumerable<Status>> FetchAllAsync();
		Task<IEnumerable<Status>> QueryAsync(object args);
		Task<Status> GetByIdAsync(int id);
		Task CreateAsync(Status status);
		Task EditAsync(Status status);
		Task RemoveAsync(int id);
	}
}
