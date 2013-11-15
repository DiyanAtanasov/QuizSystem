using QuizSystem.Data;
using QuizSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Context
{
    public class QuizUnitOfWork : IQuizUnitOfWork, IDisposable
    {
        private DbContext context;

        private Dictionary<Type, object> storage;

        public QuizUnitOfWork(DbContext context)
        {
            this.context = context;
            this.storage = new Dictionary<Type, object>();
        }

        public IRepository<T> LoadRepository<T>() where T : class
        {
            if (!this.storage.ContainsKey(typeof(T)))
            {
                this.storage.Add(typeof(T), new QuizRepository<T>(this.context));
            }

            return this.storage[typeof(T)] as IRepository<T>;
        }

        public IRepository<QuizUser> Users
        {
            get { return this.LoadRepository<QuizUser>(); }
        }

        public IRepository<Category> Categories
        {
            get { return this.LoadRepository<Category>(); }
        }

        public IRepository<Quiz> Quizzes
        {
            get { return this.LoadRepository<Quiz>(); }
        }

        public IRepository<Question> Questions
        {
            get { return this.LoadRepository<Question>(); }
        }

        public IRepository<Answer> Answers
        {
            get { return this.LoadRepository<Answer>(); }
        }

        public IRepository<Vote> Votes
        {
            get { return this.LoadRepository<Vote>(); }
        }

        public IRepository<Comment> Comments
        {
            get { return this.LoadRepository<Comment>(); }
        }

        public IRepository<QuizResult> Results
        {
            get { return this.LoadRepository<QuizResult>(); }
        }

        public IRepository<UserMessage> Messages
        {
            get { return this.LoadRepository<UserMessage>(); }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}