using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using System.Xml.Serialization;
using ToDoList.Business.Repositories;
using ToDoList.Xml.Entities;
using ToDoList.Xml.Helpers;

namespace ToDoList.Xml.Repositories
{
	public class TaskRepository : ITaskRepository
	{
		private readonly string connectionString;
		private readonly IMapper mapper;

		public TaskRepository(IConfiguration configuration, IMapper mapper)
		{
			connectionString = configuration.GetConnectionString("XmlDatabaseConnection");
			this.mapper = mapper;
		}

		public async System.Threading.Tasks.Task CreateAsync(Business.Entities.Task task)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var taskList = data.Descendants("Tasks").SingleOrDefault();
			task.UpdateStatus();

			if (taskList == null)
			{
				taskList = new XElement("Tasks");
				data.Descendants("Database").SingleOrDefault()!.Add(taskList);
			}

			var nodes = taskList.Descendants("Task").OrderByDescending(t => int.Parse(t.Attribute("Id")!.Value));

			task.Id = nodes.FirstOrDefault() == null ? 1 : int.Parse(nodes.FirstOrDefault()!.Attribute("Id")!.Value) + 1;
			var node = mapper.Map<Business.Entities.Task, Xml.Entities.Task>(task);

			using (var writer = taskList.CreateWriter())
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Task));
				writer.WriteWhitespace(string.Empty);
				try
				{
					serializer.Serialize(writer, node);
				}
				catch (InvalidOperationException)
				{
					throw new NotImplementedException("XML serialization error! (XML)");
				}
				writer.WriteWhitespace("\n");
			}

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task RemoveAsync(int id)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);

			try
			{
				data.Descendants("Task").Where(t => int.Parse(t.Attribute("Id")!.Value) == id).Remove();
			} 
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("Task deletion error! (XML)");
			}

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task EditAsync(Business.Entities.Task task)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var node = data.Descendants("Task").SingleOrDefault(t => int.Parse(t.Attribute("Id")!.Value) == task.Id);
			task.UpdateStatus();

			if (node == null)
				throw new NotImplementedException("Task editing error! (XML)");

			foreach (var property in task.GetType().GetProperties())
			{
				node.SetAttributeValue(property.Name, property.GetValue(task, null) ?? string.Empty);
			}

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task ExecuteAsync(Business.Entities.Task task)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var node = data.Descendants("Task").SingleOrDefault(t => int.Parse(t.Attribute("Id")!.Value) == task.Id);
			
			if (node == null)
				throw new NotImplementedException("Execution task error! (XML)");

			node.SetAttributeValue("ExecutionTime", DateTime.Now);
			node.SetAttributeValue("StatusId", (int)Status.StatusCodes.Done);

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task<IEnumerable<Business.Entities.Task>> FetchAllAsync()
		{

			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var nodes = data.Descendants("Task");

			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Task));
				var tasks = nodes.Select(t => (Xml.Entities.Task)serializer.Deserialize(t.CreateReader())!);
				return mapper.Map<IEnumerable<Xml.Entities.Task>, IEnumerable<Business.Entities.Task>>(tasks);
			} 
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}

		public async System.Threading.Tasks.Task<Business.Entities.Task> GetByIdAsync(int id)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var node = data.Descendants("Task").SingleOrDefault(t => int.Parse(t.Attribute("Id")!.Value) == id);
			
			if (node == null)
				throw new NotImplementedException("Task wasn't found! (XML)");
			
			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Task));
				var task = (Xml.Entities.Task)serializer.Deserialize(node.CreateReader())!;
				task.UpdateStatus();
				return mapper.Map<Xml.Entities.Task, Business.Entities.Task>(task);
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}

		public async System.Threading.Tasks.Task<IEnumerable<Business.Entities.Task>> QueryAsync(object args)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var nodes = data.Descendants("Task");

			foreach (var property in args.GetType().GetProperties())
			{
				if (!string.IsNullOrEmpty(property.GetValue(args, null)?.ToString()))
					nodes = nodes.Where(t => t.Attribute(property.Name)?.Value == (property.GetValue(args, null)?.ToString() ?? string.Empty));
			}

			nodes = nodes.OrderByDescending(t => DateTime.Parse(string.IsNullOrEmpty(t.Attribute("ExecutionTime")?.Value) ? DateTime.MaxValue.ToString() : t.Attribute("ExecutionTime")!.Value))
						 .ThenBy(t => DateTime.Parse(string.IsNullOrEmpty(t.Attribute("Deadline")?.Value) ? DateTime.MaxValue.ToString() : t.Attribute("Deadline")!.Value));

			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Task));
				var tasks = nodes.Select(t => (Xml.Entities.Task)serializer.Deserialize(t.CreateReader())!);
				return mapper.Map<IEnumerable<Xml.Entities.Task>, IEnumerable<Business.Entities.Task>>(tasks);
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}
	}
}
