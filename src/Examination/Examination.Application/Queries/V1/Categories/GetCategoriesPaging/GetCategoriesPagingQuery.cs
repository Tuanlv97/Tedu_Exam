using Examination.Dtos.Category;
using Examination.Dtos.SeedWork;
using MediatR;

namespace Examination
{
    public class GetCategoriesPagingQuery : IRequest<PagedList<CategoryDto>>
    {
        public string SearchKeyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}