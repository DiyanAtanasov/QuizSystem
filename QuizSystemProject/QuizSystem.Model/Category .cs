using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Model
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }

        public Category()
        {
            this.SubCategories = new HashSet<Category>();
            this.Quizzes = new HashSet<Quiz>();
        }
    }
}
