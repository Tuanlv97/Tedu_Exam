using Examination.Dtos.Dtos;
using MediatR;

namespace Examination
{
    public class GetHomeExamListQuery : IRequest<IEnumerable<ExamDto>>
    {
    }
}
