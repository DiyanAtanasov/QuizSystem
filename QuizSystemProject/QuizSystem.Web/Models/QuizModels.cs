using QuizSystem.Model;
using QuizSystem.Web.Libs.DataPager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Models
{
    public class QuizCreateModel
    {
        [Required(ErrorMessage="Quiz must have category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage="Quiz title is required.")]
        [MaxLength(100, ErrorMessage="Quiz title can be 100 characters max.")]
        [MinLength(5,ErrorMessage="Quiz title can be 5 characters min.")]
        public string Title { get; set; }
    }

    public class QuizUpdateModel : QuizCreateModel
    {
        public int Id { get; set; }

        public QuizState State { get; set; }
    }

    public class QuizAuthorViewModel
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public string Title { get; set; }

        public int Questions { get; set; }

        public int SolvedCount { get; set; }

        public double? AverageScore { get; set; }

        public QuizState State { get; set; }
    }

    public class QuizPreviewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public QuizState State { get; set; }

        public List<QuestionPreviewModel> Questions { get; set; }
    }

    public class QuizViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public int Questions { get; set; }

        public string Creator { get; set; }

        public DateTime PublishDate { get; set; }

        public int Rating { get; set; }
    }

    public class QuizHomeViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public int Questions { get; set; }

        public int Comments { get; set; }

        public int Rating { get; set; }
    }

    public class QuizSolveModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public List<QuestionSolveModel> Questions { get; set; }
    }

    public class QuizSolvedModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Points { get; set; }

        public int Rating { get; set; }

        public bool IsVotableByUser { get; set; }

        public List<QuestionSolvedModel> Questions { get; set; }

        public double CalculateResult()
        {
            double result = ((double)this.Points / this.Questions.Count) * 100;
            result = Math.Round(result, 2);
            return result >= 0 ? result : 0;
        }
    }

    public class QuizArchiveModel
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public string Title { get; set; }

        public double FirstResult { get; set; }

        public double LastResult { get; set; }

        public int Comments { get; set; }

        public string Creator { get; set; }

        public int Rating { get; set; }
    }

    public class QuizDetailsModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Creator { get; set; }

        public int SolvedCount { get; set; }

        public string Category { get; set; }

        public int Questions { get; set; }

        public DateTime PublishDate { get; set; }

        public double? AverageScore { get; set; }

        public bool UserCanVote { get; set; }

        public bool UserCanComment { get; set; }

        public int Rating { get; set; }

        public SimpleDataPager<CommentViewModel> Comments { get; set; }
    }
}