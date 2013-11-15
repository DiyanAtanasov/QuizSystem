using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Areas.Administration.Models
{
    public class CategoryAdminModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage="Category name can be maximum 30 characters.")]
        public string Name { get; set; }

        public int QuizzesCount { get; set; }
    }
}