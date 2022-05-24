using ToDoList.Business.Repositories;
using ToDoList.Enums;

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
							return repositories.GetRepository<T, MsSqlDb.Repositories.TaskRepository>();
						case IEnumerable<ICategoryRepository>:
							return repositories.GetRepository<T, MsSqlDb.Repositories.CategoryRepository>();
						case IEnumerable<IStatusRepository>:
							return repositories.GetRepository<T, MsSqlDb.Repositories.StatusRepository>();
						default:
							return repositories.GetRepository<T, MsSqlDb.Repositories.TaskRepository>();
					}
				case Storages.Xml:
					switch (repositories)
					{
						case IEnumerable<ITaskRepository>:
							return repositories.GetRepository<T, Xml.Repositories.TaskRepository>();
						case IEnumerable<ICategoryRepository>:
							return repositories.GetRepository<T, Xml.Repositories.CategoryRepository>();
						case IEnumerable<IStatusRepository>:
							return repositories.GetRepository<T, Xml.Repositories.StatusRepository>();
						default:
							return repositories.GetRepository<T, Xml.Repositories.TaskRepository>();
					}
				default:
					switch (repositories)
					{
						case IEnumerable<ITaskRepository>:
							return repositories.GetRepository<T, MsSqlDb.Repositories.TaskRepository>();
						case IEnumerable<ICategoryRepository>:
							return repositories.GetRepository<T, MsSqlDb.Repositories.CategoryRepository>();
						case IEnumerable<IStatusRepository>:
							return repositories.GetRepository<T, MsSqlDb.Repositories.StatusRepository>();
						default:
							return repositories.GetRepository<T, MsSqlDb.Repositories.TaskRepository>();
					}
			}
		}

		public static T GetRepository<T, E>(this IEnumerable<T> repositories) => repositories.SingleOrDefault(r => r?.GetType() == typeof(E))!;
	}
}
