using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Model
{
    public class Answer
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public Answer()
        {
            this.Questions = new HashSet<Question>();
        }
    }
}
