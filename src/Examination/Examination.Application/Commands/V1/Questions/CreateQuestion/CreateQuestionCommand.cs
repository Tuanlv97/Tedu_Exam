﻿using Examination.Dtos.Enum;
using Examination.Dtos.Questions;
using Examination.Dtos.SeedWork;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Examination.Application.Commands.V1.Questions.CreateQuestion
{
    public class CreateQuestionCommand : IRequest<ApiResult<QuestionDto>>
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        [Required]
        public Level Level { set; get; }

        [Required]
        public string CategoryId { get; set; }

        [Required]
        public List<AnswerDto> Answers { set; get; } = new List<AnswerDto>();

        public string Explain { get; set; }
    }
}