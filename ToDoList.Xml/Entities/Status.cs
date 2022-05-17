using System.Xml.Serialization;

namespace ToDoList.Xml.Entities
{
	public class Status
	{
		public enum StatusCodes
		{
			Executing = 1,
			Done = 2,
			Expired = 3
		};

		[XmlAttribute("Id")]
		public int Id { get; set; }
		[XmlAttribute("Name")]
		public string Name { get; set; } = null!;
		
		[XmlIgnore]
		public IEnumerable<Task>? Tasks { get; set; }
	}
}
