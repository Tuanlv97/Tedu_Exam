using Examination.Dtos.Category;
using MediatR;

namespace Examination
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { set; get; }
        public string UrlPath { get; set; }
    }

}
