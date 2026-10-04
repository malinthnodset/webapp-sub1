using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

// Seed test data for development environment 
public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<QuizDbContext>();

        // migration - creates/updates the schema
        context.Database.Migrate();

        // everything is seeded together, so if courses exist -> the seed has already run
        if (context.Courses.Any()) return;

        // create courses
        var testCourse = new Course( "TEST1001", "Test course" );
        var webCourse = new Course( "ITPE3200", "Web applications" );

        // create students
        var alice = new Student { Name = "Alice Hansen", Email = "alice@hansen.no" };
        var bob = new Student { Name = "Bob Johansen", Email = "bob@johansen.no" };

        // create quiz + it's questions
        context.Quizzes.AddRange(
            new Quiz
            {
                Title = "Test",
                Description = "A test quiz",
                Course = testCourse,
                CreatedByStudent = alice,
                Questions = new List<QuizQuestion>
                {
                    // questions
                    new() { Prompt = "A", CorrectAnswer = "A", Points = 1, Order = 1 },
                    new() { Prompt = "B", CorrectAnswer = "B", Points = 1, Order = 2 },
                    new() { Prompt = "123", CorrectAnswer = "123", Points = 1, Order = 3 }
                }
            },
            new Quiz
            {
                Title = "Web basics Quiz",
                Description = "A short introduction to web development.",
                Course = webCourse,
                CreatedByStudent = bob,
                Questions = new List<QuizQuestion>
                {
                    new() { Prompt = "What does HTTP stand for?", CorrectAnswer = "Hypertext Transfer Protocol", Points = 1, Order = 1 },
                    new() { Prompt = "What does HTML stand for?", CorrectAnswer = "Hypertext Markup Language", Points = 1, Order = 2 },
                    new() { Prompt = "Which language is commonly used to add interactivity to webpages?", CorrectAnswer = "JavaScript", Points = 1, Order = 3 },
                    new() { Prompt = "What does CSS stand for?", CorrectAnswer = "Cascading Style Sheets", Points = 1, Order = 4 },
                    new() { Prompt = "Which HTTP method is commonly used to retrieve data?", CorrectAnswer = "GET", Points = 1, Order = 5 },
                    new() { Prompt = "Which HTTP method is commonly used to submit data?", CorrectAnswer = "POST", Points = 1, Order = 6 }
                }
            });

        // EF inserts the courses and students (found through the quizzes),
        // then the quizzes, then the questions, in the right order, in a single transaction -> avoids half seeded db
        context.SaveChanges();
    }
}