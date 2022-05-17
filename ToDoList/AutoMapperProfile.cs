using AutoMapper;

namespace ToDoList
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Models.CreateTaskViewModel, Business.Entities.Task>();
			CreateMap<Models.EditTaskViewModel, Business.Entities.Task>().ReverseMap();
			CreateMap<Models.CategoryViewModel, Business.Entities.Category>().ReverseMap();
			CreateMap<Xml.Entities.Task, Business.Entities.Task>().ReverseMap();
			CreateMap<Xml.Entities.Category, Business.Entities.Category>().ReverseMap();
			CreateMap<Xml.Entities.Status, Business.Entities.Status>().ReverseMap();
		}
	}
}
