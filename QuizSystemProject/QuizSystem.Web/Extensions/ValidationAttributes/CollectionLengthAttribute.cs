using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Extensions.ValidationAttributes
{
    public class CollectionLengthAttribute : ValidationAttribute
    {
        private int min;
        private int max;
        public CollectionLengthAttribute(int min = 0, int max = 0)
        {
            this.max = max;
            this.min = min;
        }

        public override bool IsValid(object value)
        {
            IEnumerable<object> collection = value as IEnumerable<object>;

            if (collection == null)
            {
                return true;
            }

            int length = collection.Count();

            if (this.max > 0)
            {
                return length >= this.min && length <= this.max;
            }

            return length >= this.min;
        }
    }
}