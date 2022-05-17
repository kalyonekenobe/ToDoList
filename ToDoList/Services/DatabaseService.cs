using ToDoList.Business.Entities;
using ToDoList.Business.Repositories;

namespace ToDoList.Services
{
	public static class DatabaseService
	{
		public static async System.Threading.Tasks.Task UpdateTasksStatusesAsync(ITaskRepository taskRepository)
		{
			var tasks = await taskRepository.FetchAllAsync();
			foreach (var task in tasks)
			{
				await UpdateTaskStatusAsync(task, taskRepository);
			}
		}

		public static async System.Threading.Tasks.Task UpdateTaskStatusAsync(Business.Entities.Task task, ITaskRepository taskRepository)
		{
			int statusId;
			if (!task.ExecutionTime.HasValue)
			{
				if (!task.Deadline.HasValue)
				{
					statusId = (int)Status.StatusCodes.Executing;
				}
				else
				{
					if (task.Deadline.Value >= DateTime.Now)
					{
						statusId = (int)Status.StatusCodes.Executing;
					}
					else
					{
						statusId = (int)Status.StatusCodes.Expired;
					}
				}
			}
			else
			{
				statusId = (int)Status.StatusCodes.Done;
			}

			if (statusId != task.StatusId)
			{
				task.StatusId = statusId;
				await taskRepository.EditAsync(task);
			}
		}
	}
}
