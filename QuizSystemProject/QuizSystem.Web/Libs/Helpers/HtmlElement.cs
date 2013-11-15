using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace QuizSystem.Web.Libs.Helpers
{
    public class HtmlElement
    {
        private readonly string[] SelfClosingTags = new[] { "input"};
        private readonly string[] NonTextTags = new[] { "select", "hr", "table" };

        private IDictionary<string, object> attributes;
        private List<HtmlElement> childElements;
        private string tagName;
        private string innerText;
        private bool isSelfClosing;
        private bool isNonText;

        public IEnumerable<HtmlElement> ChildNodes { get { return this.childElements; } }

        public IDictionary<string,object> Attributes 
        {
            get { return this.attributes; }
        }
        public HtmlElement(string tag) 
            : this(tag, null)
        {
        }

        public string TagName { get { return this.tagName; } }

        public HtmlElement(string tag, object htmlAttributes)
        {
            this.tagName = tag;
            this.attributes = new Dictionary<string, object>();
            this.childElements = new List<HtmlElement>();
            this.innerText = String.Empty;
            this.isSelfClosing = this.SelfClosingTags.Contains(tag);
            this.isNonText = this.NonTextTags.Contains(tag);

            if (htmlAttributes != null)
            {
                this.ParseAttributes(htmlAttributes);
            }
        }

        private void ParseAttributes(object attributes)
        {
            var htmlAttributes = attributes.GetType().GetProperties();

            foreach (var attr in htmlAttributes)
            {
                this.AddAttribute(attr.Name, attr.GetValue(attributes));
            }
        }

        public HtmlElement AddAttribute(string name, object value)
        {
            this.attributes.Add(name, value);

            return this;
        }

        public HtmlElement SetInnerText(object text)
        {
            if (this.isSelfClosing)
            {
                this.AddAttribute("value", text);
            }
            else if(!this.isNonText)
            {
                this.innerText = text as string ?? text.ToString();
            }

            return this;
        }

        public HtmlElement AddChild(HtmlElement el)
        {
            if(this.isSelfClosing)
            {
                throw new InvalidOperationException(
                    String.Format("Self Closing Tag '{0}' Cannot Have Childrens", this.tagName));
            }

            this.childElements.Add(el);

            return this;
        }

        public string Build()
        {
            var sb = new StringBuilder();

            sb.Append('<' + this.tagName);

            foreach(var attr in this.attributes)
            {
                if (attr.Value != null)
                {
                    sb.AppendFormat(" {0}=\"{1}\"", attr.Key, attr.Value);
                }
            }

            if(this.isSelfClosing)
            {
                sb.Append(" />");
            }
            else
            {
                sb.Append('>');
                sb.Append(this.innerText);
                foreach(var child in this.childElements)
                {
                    sb.Append(child.Build());
                }

                sb.AppendFormat("</{0}>", this.tagName);
            }

            return sb.ToString();
        }
    }
}