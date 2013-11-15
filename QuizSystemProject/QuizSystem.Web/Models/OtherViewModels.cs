using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Models
{
    public class HomeQuizzesViewModel
    {
        public IEnumerable<QuizHomeViewModel> NewestQuizzes { get; set; }
        public IEnumerable<QuizHomeViewModel> MostRatedQuizzes { get; set; }
        public IEnumerable<QuizHomeViewModel> MostCommentedQuizzes { get; set; }
    }

    public class MessageViewModel
    {
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public string User { get; set; }
    }

    public class MessageCreateModel
    {
        [Required]
        [MaxLength(300, ErrorMessage="Maximum length is 300 characters.")]
        public string Content { get; set; }
    }

    public class CommentViewModel
    {
        public string Content { get; set; }

        public string User { get; set; }

        public DateTime PublishDate { get; set; }
    }

    public class CommentCreateModel
    {
        public int QuizId { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage= "Comments can be maximum 300 characters.")]
        public string Content { get; set; }
    }
}