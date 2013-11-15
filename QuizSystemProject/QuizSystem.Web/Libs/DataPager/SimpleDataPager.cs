using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace QuizSystem.Web.Libs.DataPager
{
    public class SimpleDataPager<T>
    {
        public readonly string SortByUrlParam = "sortBy";
        public readonly string SortDirectionUrlParam = "sortDir";
        public readonly string PageUrlParam = "page";

        public readonly SortingDirection DefaultSortDirection = SortingDirection.Ascending;

        private int currentPage;
        private int totalPages;
        private int pageSize;

        private int totalResults;

        protected string sortExpression;
        private SortingDirection currentSortingDirection;
        private SortingDirection nextSortingDirection;

        protected IQueryable<T> data;
        private IEnumerable<T> items;

        protected Dictionary<string, object> urlParameters;

        public int CurrentPage { get { return this.currentPage; } }
        public int TotalPages { get { return this.totalPages; } }
        public int PageSize { get { return this.pageSize; } }

        public int TotalResults { get { return this.totalResults; } }

        public string SortExpression { get { return this.sortExpression; } }
        public SortingDirection CurrentSortingDirection { get { return this.currentSortingDirection; } }
        public SortingDirection NextSortingDirection { get { return this.nextSortingDirection; } }

        public IDictionary<string, object> UrlParameters { get { return this.urlParameters; } }

        public IQueryable<T> Data { get { return this.data; } }
        public IEnumerable<T> Items {  get {  return this.items; } }

        public SimpleDataPager(IQueryable<T> data, int pageSize)
            : this(data, 0, pageSize)
        {

        }

        public SimpleDataPager(IQueryable<T> data, int page, int pageSize)
        {
            this.pageSize = pageSize;
            this.currentPage = page;

            this.data = data;

            this.urlParameters = new Dictionary<string, object>();
            this.UrlParameters.Add(PageUrlParam, page);
        }

        public virtual SimpleDataPager<T> Filter(string filterProperty, object filterValue, FilterAction action)
        {
            if (filterProperty == null)
            {
                throw new ArgumentNullException("Filter Property Is NULL. Filter Method Need To Have SortExpression .");
            }

            var parameter = Expression.Parameter(typeof(T), "x");

            var filterProperties = filterProperty.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Expression filteringExpression = Expression.Property(parameter, filterProperties[0]);

            for (int i = 1; i < filterProperties.Length; i++)
            {
                filteringExpression = Expression.Property(filteringExpression, filterProperties[i]);
            }

            if (filteringExpression.Type.IsGenericType && 
                filteringExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                filteringExpression = Expression.Convert(filteringExpression, filteringExpression.Type.GenericTypeArguments[0]);
            }

            Expression value;
            MethodInfo parser = this.ExtractMethod(filteringExpression.Type, "Parse", 1);
            if (parser != null)
            {
                try
                {
                    value = Expression.Constant(parser.Invoke(null, new object[] { filterValue.ToString() }));
                }
                catch (Exception)
                {
                    throw new FormatException(String.Format("Filter value '{0}' for '{1}' {2} filter is invalid.",
                        filterValue, filterProperty, action));
                }
            }
            else if (filteringExpression.Type.IsEnum)
            {
                try
                {
                    value = Expression.Constant(int.Parse(filterValue.ToString()));
                    value = Expression.Convert(value, filteringExpression.Type);
                }
                catch (Exception)
                {
                    throw new FormatException(String.Format("Filter value '{0}' for '{1}' {2} filter is invalid.",
                        filterValue, filterProperty, action));
                }
            }
            else
            {
                value = Expression.Constant(filterValue);
            }

            switch(action)
            {
                case FilterAction.Equal:
                    filteringExpression = Expression.Equal(filteringExpression, value); break;
                case FilterAction.Greater:
                    filteringExpression = Expression.GreaterThanOrEqual(filteringExpression, value); break;
                case FilterAction.Less: 
                    filteringExpression = Expression.LessThanOrEqual(filteringExpression, value); break;
                case FilterAction.Contains: 
                    filteringExpression = Expression.Call(filteringExpression, 
                        ExtractMethod(typeof(String), "Contains", 1), value); break;
            }


            filteringExpression = Expression.Lambda(filteringExpression, parameter);

            MethodInfo filterMethod =
              ExtractMethod(typeof(Queryable), "Where" , 2)
              .MakeGenericMethod(new[] { typeof(T)});

            this.data = filterMethod.Invoke(null, new object[] {this.data, filteringExpression}) as IQueryable<T>;

            this.urlParameters.Add(String.Format("f${0}${1}", (int)action, filterProperty), filterValue);

            return this;
        }

        public virtual SimpleDataPager<T> Sort(string sortExpression, SortingDirection direction)
        {
            if (sortExpression == null)
            {
               throw new ArgumentNullException("Sort Property Is NULL. Sort Method Need To Have SortExpression .");
            }

            this.sortExpression = sortExpression;
            this.currentSortingDirection = direction;

            if (direction == SortingDirection.Ascending)
            {
                this.nextSortingDirection = SortingDirection.Descending;
            }
            else
            {
                this.nextSortingDirection = SortingDirection.Ascending;
            }

           
            this.UrlParameters[SortByUrlParam] = this.sortExpression;
            this.UrlParameters[SortDirectionUrlParam] = (int)this.currentSortingDirection;

            return this;
        }

        protected virtual void SetPages()
        {
            this.totalResults = this.data.Count();
            this.totalPages = (this.totalResults - 1) / this.pageSize;
            this.currentPage = this.currentPage >= 0 ? this.currentPage : 0;
            this.currentPage = this.currentPage <= this.totalPages ? this.currentPage : this.totalPages;
        }

        public virtual SimpleDataPager<T> Load()
        {
            this.SetPages();
            this.DoSorting();
            this.DoPaging();

            this.items = this.data.ToList();

            return this;
        }

        protected virtual void DoPaging()
        {
            this.data = this.data.Skip(this.currentPage * this.pageSize).Take(this.pageSize);
        }

        protected virtual void DoSorting()
        {
            var sortExpression = this.sortExpression ?? typeof(T).GetProperties().FirstOrDefault().Name;

            var parameter = Expression.Parameter(typeof(T), "x");

            var sortProperties = sortExpression.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            var sortProperty = Expression.Property(parameter, sortProperties[0]);

            for (int i = 1; i < sortProperties.Length; i++)
            {
                sortProperty = Expression.Property(sortProperty, sortProperties[i]);
            }

            var expression = Expression.Lambda(sortProperty, parameter);

            string sortingMethodName = this.currentSortingDirection == SortingDirection.Ascending ? 
                "OrderBy" : "OrderByDescending";

            MethodInfo orderByMethod =
                ExtractMethod(typeof(Queryable), sortingMethodName, 2)
                .MakeGenericMethod(new[] { typeof(T), sortProperty.Type });

            this.data = orderByMethod.Invoke(null, new object[] { this.data, expression }) as IQueryable<T>;
        }

        public SimpleDataPager<T> ProcessUrlParameters(NameValueCollection query)
        {
            foreach(var key in query.AllKeys)
            {
                if (key == this.PageUrlParam)
                {
                    this.currentPage = int.Parse(query[key]);
                }
                else if(key == this.SortByUrlParam)
                {
                    string sortDirection = query[this.SortDirectionUrlParam];

                    if(sortDirection != null)
                    {
                        this.Sort(query[key], (SortingDirection)int.Parse(sortDirection));
                    }
                    else
                    {
                        this.Sort(query[key], this.DefaultSortDirection);
                    }
                }
                else if(Regex.IsMatch(key, @"f\$\d+\$\w+"))
                {
                    if (query[key] == String.Empty || query[key] == null) 
                    {
                        if (this.urlParameters.ContainsKey(key)) { this.urlParameters.Remove(key); }
                        continue; 
                    }

                    string[] filter = key.Split('$');

                    this.Filter(filter[2], query[key], (FilterAction)int.Parse(filter[1]));
                }
               
            }

            return this;
        }

        protected MethodInfo ExtractMethod(Type objType, string methodName, int parametersCount)
        {
            return objType.GetMethods()
                .FirstOrDefault(x => x.Name == methodName &&
                x.GetParameters().Length == parametersCount);
        }
    }
}