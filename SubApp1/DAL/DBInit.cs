using Microsoft.EntityFrameworkCore; // [LEARN] EF Core
using SubApp1.Models; // [LEARN] Entities

namespace SubApp1.DAL; // [LEARN] Data access namespace

// [LEARN] Seed test data for development environment 
public static class DBInit // [LEARN] Static initializer
{
    public static void Seed(IApplicationBuilder app) // [LEARN] Seed DB on startup
    {
        using var serviceScope = app.ApplicationServices.CreateScope(); // [LEARN] Create DI scope
        var context = serviceScope.ServiceProvider.GetRequiredService<QuizDbContext>(); // [LEARN] Get DbContext

        // [LEARN] Migration - creates/updates the schema
        context.Database.Migrate(); // [LEARN] Apply pending migrations

        // [LEARN] Everything is seeded together, so if Courses exist -> the seed has already run
        if (context.Courses.Any()) return; // [LEARN] Skip if already seeded

        // [LEARN] create courses
        var testCourse = new Course( "TEST1001", "Test course" ); // [LEARN] Test course
        var webCourse = new Course( "ITPE3200", "Web applications" ); // [LEARN] Web course

        // [LEARN] create students
        var alice = new Student { Name = "Alice Hansen", Email = "alice@hansen.no" }; // [LEARN] Student Alice
        var bob = new Student { Name = "Bob Johansen", Email = "bob@johansen.no" }; // [LEARN] Student Bob

        // [LEARN] create quiz with questions
        context.Quizzes.AddRange( // [LEARN] Add quizzes
            new Quiz // [LEARN] Test quiz
            {
                Title = "Test", // [LEARN] Title
                Description = "A test quiz", // [LEARN] Description
                Course = testCourse, // [LEARN] Link course
                CreatedByStudent = alice, // [LEARN] Creator
                Questions = new List<QuizQuestion> // [LEARN] Questions
                {
                    // [LEARN] questions
                    new() { Prompt = "A", CorrectAnswer = "A", Points = 1, Order = 1 }, // [LEARN] Q1
                    new() { Prompt = "B", CorrectAnswer = "B", Points = 1, Order = 2 }, // [LEARN] Q2
                    new() { Prompt = "123", CorrectAnswer = "123", Points = 1, Order = 3 } // [LEARN] Q3
                }
            },
            new Quiz // [LEARN] Web basics quiz
            {
                Title = "Web basics Quiz", // [LEARN] Title
                Description = "A short introduction to web development.", // [LEARN] Description
                Course = webCourse, // [LEARN] Link course
                CreatedByStudent = bob, // [LEARN] Creator
                Questions = new List<QuizQuestion> // [LEARN] Questions
                {
                    new() { Prompt = "What does HTTP stand for?", CorrectAnswer = "Hypertext Transfer Protocol", Points = 1, Order = 1 }, // [LEARN] Q1
                    new() { Prompt = "What does HTML stand for?", CorrectAnswer = "Hypertext Markup Language", Points = 1, Order = 2 }, // [LEARN] Q2
                    new() { Prompt = "Which language is commonly used to add interactivity to webpages?", CorrectAnswer = "JavaScript", Points = 1, Order = 3 }, // [LEARN] Q3
                    new() { Prompt = "What does CSS stand for?", CorrectAnswer = "Cascading Style Sheets", Points = 1, Order = 4 }, // [LEARN] Q4
                    new() { Prompt = "Which HTTP method is commonly used to retrieve data?", CorrectAnswer = "GET", Points = 1, Order = 5 }, // [LEARN] Q5
                    new() { Prompt = "Which HTTP method is commonly used to submit data?", CorrectAnswer = "POST", Points = 1, Order = 6 } // [LEARN] Q6
                }
            });

        // [LEARN] EF inserts the courses and students (found through the quizzes),
        // [LEARN] then the quizzes, then the questions, in the right order, in a single transaction -> avoids half seeded db
        context.SaveChanges(); // [LEARN] Persist seed data
    }
}
