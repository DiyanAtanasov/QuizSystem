using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Data
{
    public class QuizRepository<T> : IRepository<T> where T : class
    {
        protected DbContext context;

        public QuizRepository()
            : this(new QuizContext())
        {
        }

        public QuizRepository(DbContext context)
        {
            this.context = context;
        }

        public virtual IQueryable<T> All()
        {
            return this.context.Set<T>();
        }

        public virtual T GetById(object id)
        {
            return this.context.Set<T>().Find(id);
        }

        public virtual T Add(T item)
        {
            return this.context.Set<T>().Add(item);
        }

        public virtual T Update(T item)
        {
            var entry = this.context.Entry<T>(item);
            entry.State = EntityState.Modified;
            return entry.Entity;
        }

        public virtual void Remove(object id)
        {
            this.context.Set<T>().Remove(this.GetById(id));
        }

        public virtual void RemoveItem(T item)
        {
            this.context.Set<T>().Remove(item);
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }
    }
}
