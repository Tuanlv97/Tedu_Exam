using Examination.Domain.AggregateModels.ExamResultAggregate;
using Examination.Infrastructure.MongoDb.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Examination.Infrastructure.MongoDb.Repositories
{
    public class ExamResultRepository : BaseRepository<ExamResult>, IExamResultRepository
    {
        public ExamResultRepository(IMongoClient mongoClient,
        IOptions<ExamSettings> settings,
        IMediator mediator)
        : base(mongoClient, settings, mediator, Constants.Collections.ExamResult)
        {
        }

        public async Task<ExamResult> GetDetails(string userId, string examId)
        {
            var filter = Builders<ExamResult>.Filter.Where(s => s.ExamId == examId && s.UserId == userId);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
