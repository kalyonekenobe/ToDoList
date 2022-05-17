using System.Xml;
using System.Xml.Linq;

namespace ToDoList.Xml.Helpers
{
	public static class XmlDataHandler
	{
		public static async System.Threading.Tasks.Task<XElement> ReadXmlDocument(string connectionString)
		{
			using (var reader = XmlReader.Create(connectionString, new XmlReaderSettings() { Async = true }))
			{
				return await XElement.LoadAsync(reader, LoadOptions.None, CancellationToken.None);
			}
		}

		public static async System.Threading.Tasks.Task SaveXmlDocument(this XElement data, string connectionString)
		{
			using (var writer = XmlWriter.Create(connectionString, new XmlWriterSettings() { Async = true }))
			{
				await data.SaveAsync(writer, CancellationToken.None);
			}
		}
	}
}
