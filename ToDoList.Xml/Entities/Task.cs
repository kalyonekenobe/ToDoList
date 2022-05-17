using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace ToDoList.Xml.Entities
{
	public class Task
	{
		[XmlAttribute("Id")]
		public int Id { get; set; }
		[XmlAttribute("Name")]
		public string Name { get; set; } = null!;
		[XmlIgnore]
		public DateTime? ExecutionTime { get; set; }
		[XmlIgnore]
		public DateTime? Deadline { get; set; }
		[XmlAttribute("CategoryName")]
		public string? CategoryName { get; set; }
		[XmlIgnore]
		public int? StatusId { get; set; }

		[XmlIgnore]
		public Category? Category { get; set; }
		[XmlIgnore]
		public Status? Status { get; set; }

		[XmlAttribute("ExecutionTime")]
		public string? ExecutionTimeXml
		{
			get
			{
				return ExecutionTime?.ToString("O", new CultureInfo("en-US"));
			}
			set
			{
				ExecutionTime = string.IsNullOrEmpty(value) ? null : DateTime.Parse(string.Format(value, "o", new CultureInfo("en-US")));
			}
		}

		[XmlAttribute("Deadline")]
		public string? DeadlineXml
		{
			get
			{
				return Deadline?.ToString("O", new CultureInfo("en-US"));
			}
			set
			{
				Deadline = string.IsNullOrEmpty(value) ? null : DateTime.Parse(string.Format(value, "o", new CultureInfo("en-US")));
			}
		}

		[XmlAttribute("StatusId")]
		public string? StatusIdXml
		{
			get
			{
				return StatusId == null ? string.Empty : StatusId.ToString();
			}
			set
			{
				StatusId = string.IsNullOrEmpty(value) ? null : int.Parse(value);
			}
		}

		public void UpdateStatus()
		{
			StatusId = (int?)((!ExecutionTime.HasValue) ? (!Deadline.HasValue) ? Status.StatusCodes.Executing : (Deadline.Value >= DateTime.Now) ? Status.StatusCodes.Executing : Status.StatusCodes.Expired : Status.StatusCodes.Done);
		}
	}
}
