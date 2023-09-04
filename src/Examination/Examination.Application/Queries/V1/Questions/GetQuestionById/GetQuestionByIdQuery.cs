using Examination.Dtos.Questions;
using Examination.Dtos.SeedWork;
using MediatR;

namespace Examination.Application.Queries.V1.Questions.GetQuestionById
{
    public class GetQuestionByIdQuery : IRequest<ApiResult<QuestionDto>>
    {
        public GetQuestionByIdQuery(string id)
        {
            Id = id;
        }
        public string Id { set; get; }
    }
}
