using Examination.Dtos.Dtos;
using MediatR;

namespace Examination.Application.Queries
{
    public class GetHomeExamListQuery : IRequest<IEnumerable<ExamDto>>
    {
    }
}
