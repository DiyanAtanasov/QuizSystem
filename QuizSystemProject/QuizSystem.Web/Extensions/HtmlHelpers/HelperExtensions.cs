using QuizSystem.Web.Libs.DataPager;
using QuizSystem.Web.Libs.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Extensions.HtmlHelpers
{
    public static class Extensions
    {
        public static IHtmlString SortLink<T, P>(
            this HtmlHelper<SimpleDataPager<T>> helper, Expression<Func<T, P>> expression, string name = null)
        {
            string propertyName = String.Empty;
            Expression metaExpression = expression.Body;

            List<string> propertyChain = new List<string>();

            while (metaExpression is MemberExpression)
            {
                propertyChain.Add((metaExpression as MemberExpression).Member.Name);
                metaExpression = (metaExpression as MemberExpression).Expression;
            }

            propertyChain.Reverse();
            propertyChain.ForEach(x => propertyName += x + ".");

            propertyName = propertyName.Remove(propertyName.Length - 1, 1);

            bool isCurrentSorting = true;
            var dataPager = helper.ViewData.Model;

            var sb = new StringBuilder();
            sb.Append('?');

            if (!dataPager.UrlParameters.ContainsKey(dataPager.SortByUrlParam) ||
                (dataPager.UrlParameters[dataPager.SortByUrlParam] as String) != propertyName)
            {
                sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(dataPager.SortByUrlParam),
                    helper.Encode(propertyName));
                sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(dataPager.SortDirectionUrlParam),
                    helper.Encode((int)dataPager.DefaultSortDirection));

                isCurrentSorting = false;
            }

            foreach (var pair in dataPager.UrlParameters)
            {
                if (!isCurrentSorting &&
                    (pair.Key == dataPager.SortByUrlParam || pair.Key == dataPager.SortDirectionUrlParam))
                {
                    continue;
                }

                if (pair.Key == dataPager.SortDirectionUrlParam)
                {
                    sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(pair.Key), (int)dataPager.NextSortingDirection);
                    continue;
                }

                sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(pair.Key), HttpUtility.UrlEncode(pair.Value.ToString()));
            }

            sb.Remove(sb.Length - 1, 1);

            HtmlElement link = new HtmlElement("a")
                .AddAttribute("href", sb.ToString())
                .SetInnerText(name ?? propertyName);

            if (isCurrentSorting)
            {
                link.AddAttribute("class", dataPager.CurrentSortingDirection == SortingDirection.Ascending ? " asc" : " desc");
            }

            return new MvcHtmlString(link.Build());
        }

        public static IHtmlString FilterField<T, P>(
           this HtmlHelper<SimpleDataPager<T>> helper, Expression<Func<T, P>> expression, FilterAction action, HtmlElement element)
        {
            string propertyName = String.Empty;
            Expression metaExpression = expression.Body;

            List<string> propertyChain = new List<string>();

            while (metaExpression is MemberExpression)
            {
                propertyChain.Add((metaExpression as MemberExpression).Member.Name);
                metaExpression = (metaExpression as MemberExpression).Expression;
            }

            propertyChain.Reverse();
            propertyChain.ForEach(x => propertyName += x + ".");

            propertyName = propertyName.Remove(propertyName.Length - 1, 1);

            string filterName = String.Format("f${0}${1}",(int)action, propertyName);

            element.AddAttribute("id", propertyName + action + "Filter" );
            element.AddAttribute("name", filterName);

            var dataPager = helper.ViewData.Model;
            if (dataPager != null)
            {
                if (dataPager.UrlParameters.ContainsKey(filterName))
                {
                    if (element.TagName == "select")
                    {
                        if(element.ChildNodes.Any(x => x.Attributes.ContainsKey("selected")))
                        {
                            element.ChildNodes
                                .FirstOrDefault(x => x.Attributes["selected"] != null)
                                .Attributes.Remove("selected");

                            element.ChildNodes
                                .First(x => (x.Attributes["value"] as String) == (dataPager.UrlParameters[filterName] as String))
                                .AddAttribute("selected", "selected");
                        }
                    }
                    else
                    {
                        element.AddAttribute("value", helper.Encode(dataPager.UrlParameters[filterName]));
                    }
                }
            }

            return new MvcHtmlString(element.Build());
        }

        public static IHtmlString GetPageMenu<T>(this HtmlHelper<SimpleDataPager<T>> helper)
        {
            var dataPager = helper.ViewData.Model;

            if (dataPager.Items.Count() == 0)
            {
                return new MvcHtmlString(String.Empty);
            }

            int displayPages = 9;
            int coeff = displayPages / 2;

            string pageParam = dataPager.PageUrlParam;

            var sb = new StringBuilder();

            foreach (var pair in dataPager.UrlParameters)
            {
                if (pair.Key == pageParam)
                {
                    continue;
                }

                sb.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(pair.Key), HttpUtility.UrlEncode(pair.Value.ToString()));
            }

            string queryParams = sb.ToString();

            sb.Clear();

            var container = new HtmlElement("div");

            int lastPage = dataPager.TotalPages;
            int currentPage = dataPager.CurrentPage;

            int startDisplayPage;
            int endDisplayPage;

            if (lastPage <= displayPages)
            {
                startDisplayPage = 0;
                endDisplayPage = lastPage;
            }
            else if (currentPage - coeff > 0 && currentPage + coeff <= lastPage)
            {
                startDisplayPage = currentPage - coeff;
                endDisplayPage = currentPage + coeff;
            }
            else if (currentPage - coeff <= 0)
            {
                startDisplayPage = 0;
                endDisplayPage = displayPages;
            }
            else
            {
                startDisplayPage = lastPage - displayPages;
                endDisplayPage = lastPage;
            }


            var firstLink =
                new HtmlElement("a")
                      .AddAttribute("href", String.Format("?{0}={1}{2}", pageParam, 0, queryParams))
                      .SetInnerText("<<");
            var prevLink =
                new HtmlElement("a")
                    .AddAttribute("href", String.Format("?{0}={1}{2}", pageParam, currentPage - 1, queryParams))
                    .SetInnerText("<");


            if (currentPage == 0)
            {
                firstLink.AddAttribute("style", "visibility:hidden");
                prevLink.AddAttribute("style", "visibility:hidden");
            }

            container.AddChild(firstLink).AddChild(prevLink);

            for (int i = startDisplayPage; i <= endDisplayPage; i++)
            {
                if (i == currentPage)
                {
                    container.AddChild(new HtmlElement("span").SetInnerText(i + 1));
                    continue;
                }

                container.AddChild(new HtmlElement("a")
                      .AddAttribute("href", String.Format("?{0}={1}{2}", pageParam, i, queryParams))
                      .SetInnerText(i + 1));
            }

            var lastLink = new HtmlElement("a")
                       .AddAttribute("href", String.Format("?{0}={1}{2}", pageParam, lastPage, queryParams))
                       .SetInnerText(">>");
            var nextLink = new HtmlElement("a")
                    .AddAttribute("href", String.Format("?{0}={1}{2}", pageParam, currentPage + 1, queryParams))
                    .SetInnerText(">");


            if (currentPage == lastPage)
            {
                lastLink.AddAttribute("style", "visibility:hidden");
                nextLink.AddAttribute("style", "visibility:hidden");
            }

            container.AddChild(nextLink).AddChild(lastLink);

            return helper.Raw(container.Build());
        }

        public static MvcHtmlString ValidationField<T, P>(this HtmlHelper<T> helper,
           Expression<Func<T, P>> expression, HtmlElement element)
        {
            var property = expression.Body as MemberExpression;

            if (!element.Attributes.ContainsKey("name"))
            {
                element.AddAttribute("name", property.Member.Name);
            }

            if (!element.Attributes.ContainsKey("id"))
            {
                element.AddAttribute("id", property.Member.Name);
            }

            string fieldName = element.Attributes["name"].ToString();

            var validationAttributes =
                property.Member.GetCustomAttributes(false)
                .Where(x => x is ValidationAttribute)
                .Select(x => x as ValidationAttribute);

            var validator = new JavascriptValidator(validationAttributes);

            foreach (var item in validator.GetValidationAttributes(fieldName))
            {
                element.AddAttribute(item.Key, helper.Encode(item.Value));
            }

            if (helper.ViewData.Model != null)
            {
                var propertyValue = expression.Compile().Invoke(helper.ViewData.Model);
                if (propertyValue != null)
                {
                    element.SetInnerText(helper.Encode(propertyValue.ToString()));
                }
            }


            return new MvcHtmlString(element.Build());
        }

        public static MvcHtmlString ValidationField<T, P>(this HtmlHelper<T> helper,
          Expression<Func<T, P>> expression, string tagName, object htmlAttributes = null)
        {
            var element = new HtmlElement(tagName, htmlAttributes);

            return helper.ValidationField<T, P>(expression, element);
        }

        public static HtmlElement BuildSelectMenu(this HtmlHelper helper, 
            IEnumerable<SelectListItem> options, object attributes = null)
        {
            var select = new HtmlElement("select", attributes);

            foreach(var item in options)
            {
                select.AddChild(new HtmlElement("option", new { value = item.Value }).SetInnerText(item.Text));

                if (item.Selected)
                {
                    select.ChildNodes.LastOrDefault().AddAttribute("selected", "selected");
                }

            }

            return select;
        }
    }
}