using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Data
{
    public interface IRepository <T>
    {
        IQueryable<T> All();
        T GetById(object id);
        T Add(T item);
        T Update(T item);
        void Remove(object id);
        void RemoveItem(T item);
        int SaveChanges();
    }
}
