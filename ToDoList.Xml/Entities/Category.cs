using System.Xml.Serialization;

namespace ToDoList.Xml.Entities
{
	public class Category
	{
		[XmlAttribute("Name")]
		public string Name { get; set; } = null!;
		[XmlIgnore]
		public IEnumerable<Task>? Tasks { get; set; }
	}
}
