using ToDoList.Business.Repositories;
using static ToDoList.Controllers.StorageController;

namespace ToDoList.Extensions
{
	public static class RepositoryExtensions
	{
		public static T GetRequired<T>(this IEnumerable<T> repositories, Storages storage)
		{
			switch (storage)
			{
				case Storages.MsSql:
					switch (repositories)
					{
						case IEnumerable<ITaskRepository>:
							return GetRepository<T, MsSqlDb.Repositories.TaskRepository>(repositories);
						case IEnumerable<ICategoryRepository>:
							return GetRepository<T, MsSqlDb.Repositories.CategoryRepository>(repositories);
						case IEnumerable<IStatusRepository>:
							return GetRepository<T, MsSqlDb.Repositories.StatusRepository>(repositories);
						default:
							return GetRepository<T, MsSqlDb.Repositories.TaskRepository>(repositories);
					}
				case Storages.Xml:
					switch (repositories)
					{
						case IEnumerable<ITaskRepository>:
							return GetRepository<T, Xml.Repositories.TaskRepository>(repositories);
						case IEnumerable<ICategoryRepository>:
							return GetRepository<T, Xml.Repositories.CategoryRepository>(repositories);
						case IEnumerable<IStatusRepository>:
							return GetRepository<T, Xml.Repositories.StatusRepository>(repositories);
						default:
							return GetRepository<T, Xml.Repositories.TaskRepository>(repositories);
					}
				default:
					switch (repositories)
					{
						case IEnumerable<ITaskRepository>:
							return GetRepository<T, MsSqlDb.Repositories.TaskRepository>(repositories);
						case IEnumerable<ICategoryRepository>:
							return GetRepository<T, MsSqlDb.Repositories.CategoryRepository>(repositories);
						case IEnumerable<IStatusRepository>:
							return GetRepository<T, MsSqlDb.Repositories.StatusRepository>(repositories);
						default:
							return GetRepository<T, MsSqlDb.Repositories.TaskRepository>(repositories);
					}
			}
		}
			
		public static T GetRepository<T, E>(this IEnumerable<T> repositories) => repositories.SingleOrDefault(r => r?.GetType() == typeof(E))!;
	}
}
