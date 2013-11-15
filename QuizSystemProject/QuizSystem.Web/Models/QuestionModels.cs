using Newtonsoft.Json;
using QuizSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using QuizSystem.Web.Extensions.ValidationAttributes;

namespace QuizSystem.Web.Models
{
    public class QuizEditQuestionCrudModel
    {
        [JsonPropertyAttribute("id")]
        public int Id { get; set; }

        [Required(ErrorMessage="Question must have content.")]
        [MaxLength(300, ErrorMessage="Question content must be 300 characters max.")]
        [JsonPropertyAttribute("content")]
        public string Content { get; set; }

        [CollectionLength(2, ErrorMessage="Question must have atleast 2 answers.")]
        [JsonPropertyAttribute("answers")]
        public IEnumerable<QuizEditAnswerCrudModel> Answers { get; set; }
    }

    public class QuizEditQuestionViewModel
    {
        [JsonPropertyAttribute("id")]
        public int Id { get; set; }

        [JsonPropertyAttribute("rightId")]
        public int RightAnswerId { get; set; }

        [JsonPropertyAttribute("content")]
        public string Content { get; set; }

        [JsonPropertyAttribute("answers")]
        public IEnumerable<QuizEditAnswerViewModel> Answers { get; set; }
    }

    public class QuestionPreviewModel
    {
        public int Id { get; set; }

        public int RightAnswerId { get; set; }

        public string Content { get; set; }

        public IEnumerable<AnswerViewModel> Answers { get; set; }
    }

    public class QuestionSolveModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public IEnumerable<AnswerViewModel> Answers { get; set; }
    }

    public class QuestionSolvedModel
    {
        public int Id { get; set; }

        public int RightAnswerId { get; set; }

        public int SelectedAnswerId { get; set; }

        public string Content { get; set; }

        public IEnumerable<AnswerViewModel> Answers { get; set; }
    }
}