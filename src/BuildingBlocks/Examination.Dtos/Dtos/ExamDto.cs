using Examination.Dtos.Enum;

namespace Examination.Dtos.Dtos
{
    public class ExamDto
    {
        public string Id { set; get; }
        public string Name { get; set; }

        public string ShortDesc { get; set; }

        public int NumberOfQuestions { get; set; }

        public TimeSpan? Duration { get; set; }

        public Level Level { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
