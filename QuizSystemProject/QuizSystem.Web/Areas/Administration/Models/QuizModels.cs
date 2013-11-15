using QuizSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Areas.Administration.Models
{
    public class QuizAdminViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Creator { get; set; }

        public int Questions { get; set; }

        public string Category { get; set; }

        public QuizState State { get; set; }

        public int? LastModifiedBefore { get; set; }

    }
}