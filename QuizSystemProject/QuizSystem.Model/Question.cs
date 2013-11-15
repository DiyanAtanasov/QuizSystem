using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Model
{
    public class Question
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int RightAnswerId { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }

        public Question()
        {
            this.Answers = new HashSet<Answer>();
        }
    }
}
