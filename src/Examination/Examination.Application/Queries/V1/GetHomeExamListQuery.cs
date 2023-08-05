using Examination.Dtos.Dtos;
using MediatR;

namespace Examination.Application.Queries.V1
{
    public class GetHomeExamListQuery : IRequest<IEnumerable<ExamDto>>
    {
    }
}
