using AutoMapper;
using Examination.Domain.AggregateModels.ExamAggregate;
using Examination.Dtos.Category;
using Examination.Dtos.Dtos;

namespace Examination.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Exam, ExamDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
