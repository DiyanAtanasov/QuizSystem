﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Model
{
    public class QuizResult
    {
        public int Id { get; set; }

        public double FirstResult { get; set; }

        public double LastResult { get; set; }

        public string UserId { get; set; }
        public virtual QuizUser User { get; set; }

        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
