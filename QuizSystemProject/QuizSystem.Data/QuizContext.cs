using Microsoft.AspNet.Identity.EntityFramework;
using QuizSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Data
{
    public class QuizContext : IdentityDbContextWithCustomUser<QuizUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<QuizContext, QuizContextConfigurator>());

            modelBuilder.Entity<Category>().Property(x => x.Name).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Quiz>().Property(x => x.CreatorId).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Quiz>().Property(x => x.Title).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Question>().Property(x => x.Content).IsRequired().HasMaxLength(300);

            modelBuilder.Entity<Answer>().Property(x => x.Content).IsRequired();

            modelBuilder.Entity<Comment>().Property(x => x.Content).IsRequired().HasMaxLength(300);

            base.OnModelCreating(modelBuilder);
        }
    }
}
