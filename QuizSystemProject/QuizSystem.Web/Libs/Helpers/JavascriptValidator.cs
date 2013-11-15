using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Libs.Helpers
{
    public class JavascriptValidator
    {
        private Dictionary<string, string> htmlAttributes;
        private IEnumerable<ValidationAttribute> attributes;

        public JavascriptValidator(IEnumerable<ValidationAttribute> attributes)
        {
            this.attributes = attributes;
            this.htmlAttributes = new Dictionary<string, string>();
        }

        public IDictionary<string,string> GetValidationAttributes(string fieldName)
        {
            var sb = new StringBuilder();

            string attrName;

            foreach(ValidationAttribute attr in this.attributes)
            {
                attrName = attr.GetType().Name.Replace("Attribute", String.Empty).ToLower();
                sb.AppendFormat("method:{0},", attrName);
                ParseAttriuteParameters(attr, sb);
                sb.Append(';');

                this.htmlAttributes.Add("data-v-err-" + attrName, attr.FormatErrorMessage(fieldName));
            }

            this.htmlAttributes.Add("data-v-context", sb.ToString());

            return this.htmlAttributes;
        }

        private void ParseAttriuteParameters(ValidationAttribute attr, StringBuilder sb)
        {
            if(attr is MaxLengthAttribute)
            {
                sb.AppendFormat("max:{0}", (attr as MaxLengthAttribute).Length);
            }
            else if(attr is MinLengthAttribute)
            {
                sb.AppendFormat("min:{0}", (attr as MinLengthAttribute).Length);
            }
            else if(attr is System.ComponentModel.DataAnnotations.CompareAttribute)
            {
                sb.AppendFormat("other:{0}", (attr as System.ComponentModel.DataAnnotations.CompareAttribute).OtherProperty);
            }
            else if(attr is RegularExpressionAttribute)
            {
                sb.AppendFormat("pattern:{0}", (attr as RegularExpressionAttribute).Pattern);
            }
            else if (attr is StringLengthAttribute)
            {
                var stringLengthAttr = attr as StringLengthAttribute;
                attr.ErrorMessage = String.Format("Filed must be between {0} and {1} characters long !", 
                    stringLengthAttr.MinimumLength, stringLengthAttr.MaximumLength);
                sb.AppendFormat("max:{0},min:{1}", stringLengthAttr.MaximumLength, stringLengthAttr.MinimumLength);
            }
            else
            {
                sb.Remove(sb.Length - 1, 1);
            }
        }
    }
}