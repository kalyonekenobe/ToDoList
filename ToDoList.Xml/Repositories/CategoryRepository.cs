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
	public class CategoryRepository : ICategoryRepository
	{
		private readonly string connectionString;
		private readonly IMapper mapper;

		public CategoryRepository(IConfiguration configuration, IMapper mapper)
		{
			connectionString = configuration.GetConnectionString("XmlDatabaseConnection");
			this.mapper = mapper;
		}

		public async System.Threading.Tasks.Task CreateAsync(Category category)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var categoryList = data.Descendants("Categories").SingleOrDefault();

			if (categoryList == null)
			{
				categoryList = new XElement("Categories");
				data.Descendants("Database").SingleOrDefault()?.Add(categoryList);
			}

			var nodes = categoryList.Descendants("Category");

			if (nodes.Any(c => c.Attribute("Name")!.Value == category.Name))
				throw new NotImplementedException("Category creation error! (XML)");

			var node = mapper.Map<Business.Entities.Category, Xml.Entities.Category>(category);
			
			using (var writer = categoryList.CreateWriter())
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Category));
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

		public async System.Threading.Tasks.Task RemoveAsync(string name)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			try
			{
				foreach (var task in data.Descendants("Task").Where(t => t.Attribute("CategoryName")?.Value.ToString() == name))
				{
					task.SetAttributeValue("CategoryName", string.Empty);
				}
				data.Descendants("Category").Where(c => c.Attribute("Name")!.Value == name).Remove();
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("Category deletion error! (XML)");
			}

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task EditAsync(string editedName, Category category)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var node = data.Descendants("Category").SingleOrDefault(c => c.Attribute("Name")!.Value == editedName);

			if (node == null)
				throw new NotImplementedException("Category editing error! (XML)");

			foreach (var property in category.GetType().GetProperties())
			{
				node.SetAttributeValue(property.Name, property.GetValue(category, null) ?? string.Empty);
			}

			foreach (var task in data.Descendants("Task"))
			{
				task.SetAttributeValue("CategoryName", category.Name);
			}

			await data.SaveXmlDocument(connectionString);
		}

		public async System.Threading.Tasks.Task<IEnumerable<Category>> FetchAllAsync()
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var nodes = data.Descendants("Category");
			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Category));
				var categories = nodes.Select(c => (Xml.Entities.Category)serializer.Deserialize(c.CreateReader())!);
				return mapper.Map<IEnumerable<Xml.Entities.Category>, IEnumerable<Business.Entities.Category>>(categories);
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}

		public async System.Threading.Tasks.Task<Category> GetByNameAsync(string name)
		{
			var data = await XmlDataHandler.ReadXmlDocument(connectionString);
			var node = data.Descendants("Category").SingleOrDefault(c => c.Attribute("Name")!.Value == name);

			if (node == null)
				throw new NotImplementedException("Category wasn't found! (XML)");

			try
			{
				var serializer = new XmlSerializer(typeof(Xml.Entities.Category));
				var category = (Xml.Entities.Category)serializer.Deserialize(node.CreateReader())!;
				return mapper.Map<Xml.Entities.Category, Business.Entities.Category>(category);
			}
			catch (InvalidOperationException)
			{
				throw new NotImplementedException("XML deserialization error! (XML)");
			}
		}
	}
}
