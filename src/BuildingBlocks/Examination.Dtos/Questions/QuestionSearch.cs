using Examination.Dtos.SeedWork;

namespace Examination.Dtos.Questions
{
    public class QuestionSearch : PagingParameters
    {
        public string CategoryId { get; set; }
        public string Name { get; set; }
    }
}
