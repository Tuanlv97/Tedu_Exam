using MediatR;

namespace Examination
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public string Id { get; set; }
        public string Name { set; get; }
        public string UrlPath { get; set; }
    }
}
