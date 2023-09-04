namespace Examination.Dtos.Questions
{
    public class AnswerDto
    {
        public string Id { get; set; }
        public string Content { get; set; }

        public bool IsCorrect { get; set; }

        public Guid ClientId { set; get; } = Guid.NewGuid();
    }
}
