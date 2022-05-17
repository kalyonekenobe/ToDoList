using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ToDoList.Business.Entities;
using ToDoList.Business.Repositories;
using ToDoList.Xml.Helpers;

namespace ToDoList.Xml.Repositories
{
	public class StatusRepository : IStatusRepository
	{
		private readonly string connectionString;
		private readonly IMapper mapper;

		public StatusRepository(IConfiguration configuration, IMapper mapper)
		{
			connectionString = configuration.GetConnectionString("XmlDatabaseConnection");
			this.mapper = mapper;
		}

		public async System.Threading.Tasks.Task CreateAsync(Status status)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var statusList = data.Descendants("Statuses").SingleOrDefault();

			if (statusList == null)
			{
				statusList = new XElement("Statuses");
				data.Descendants("Database").SingleOrDefault()!.Add(statusList);
			}

			var nodes = statusList.Descendants("Status").OrderByDescending(s => int.Parse(s.Attribute("Id")!.Value));

			status.Id = nodes.FirstOrDefault() == null ? 1 : int.Parse(nodes.FirstOrDefault()!.Attribute("Id")!.Value) + 1;
			var node = mapper.Map<Business.Entities.Status, Xml.Entities.Status>(status);

			using (var writer = statusList.CreateWriter())
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Status));
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
				data.Descendants("Status").Where(s => int.Parse(s.Attribute("Id")!.Value) == id).Remove();
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("Status deletion error! (XML)");
			}

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task EditAsync(Status status)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var node = data.Descendants("Status").SingleOrDefault(s => int.Parse(s.Attribute("Id")!.Value) == status.Id);

			if (node == null)
				throw new NotImplementedException("Status editing error! (XML)");

			foreach (var property in status.GetType().GetProperties())
			{
				node.SetAttributeValue(property.Name, property.GetValue(status, null) ?? string.Empty);
			}

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task<IEnumerable<Status>> FetchAllAsync()
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var nodes = data.Descendants("Status");

			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Status));
				var statuses = nodes.Select(s => (Xml.Entities.Status)serializer.Deserialize(s.CreateReader())!);
				return mapper.Map<IEnumerable<Xml.Entities.Status>, IEnumerable<Business.Entities.Status>>(statuses);
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}

		public async System.Threading.Tasks.Task<Status> GetByIdAsync(int id)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var node = data.Descendants("Status").SingleOrDefault(s => int.Parse(s.Attribute("Id")!.Value) == id);

			if (node == null)
				throw new NotImplementedException("Status wasn't found! (XML)");

			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Status));
				var status = (Xml.Entities.Status)serializer.Deserialize(node.CreateReader())!;
				return mapper.Map<Xml.Entities.Status, Business.Entities.Status>(status);
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}

		public async System.Threading.Tasks.Task<IEnumerable<Status>> QueryAsync(object args)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var nodes = data.Descendants("Status");

			foreach (var property in args.GetType().GetProperties())
			{
				if (!string.IsNullOrEmpty(property.GetValue(args, null)?.ToString()))
					nodes = nodes.Where(s => s.Attribute(property.Name)?.Value == (property.GetValue(args, null)?.ToString() ?? string.Empty));
			}

			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Status));
				var statuses = nodes.Select(t => (Xml.Entities.Status)serializer.Deserialize(t.CreateReader())!);
				return mapper.Map<IEnumerable<Xml.Entities.Status>, IEnumerable<Business.Entities.Status>>(statuses);
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}
	}
}
