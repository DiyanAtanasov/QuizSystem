using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using QuizSystem.Model;


namespace QuizSystem.Web.Models
{
    public class QuizEditAnswerViewModel
    {
        [JsonPropertyAttribute("id")]
        public int Id { get; set; }

        [Required(ErrorMessage="Answers should have content.")]
        [MaxLength(100, ErrorMessage = "Answer content must be 100 characters max.")]
        [JsonPropertyAttribute("content")]
        public string Content { get; set; }
    }

    public class QuizEditAnswerCrudModel : QuizEditAnswerViewModel
    {
        [JsonPropertyAttribute("isCorrect")]
        public bool IsCorrect { get; set; }

        public override bool Equals(object obj)
        {
            QuizEditAnswerCrudModel other = obj as QuizEditAnswerCrudModel;

            if (other == null)
            {
                return false;
            }

            return this.Content == other.Content;
        }

        public override int GetHashCode()
        {
            return this.Content.GetHashCode();
        }
    }

    public class AnswerEditModel
    {
        public Answer Answer { get; set; }
        public int QuestionsCount { get; set; }
    }

    public class AnswerViewModel
    {
        public int Id { get; set; }

        public string Content { get; set; }
    }
}