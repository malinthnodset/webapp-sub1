// using Microsoft.EntityFrameworkCore.Migrations;

// #nullable disable

// #pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

// namespace SubApp1.Migrations
// {
//     /// <inheritdoc />
//     public partial class SeedWebQuiz : Migration
//     {
//         /// <inheritdoc />
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.InsertData(
//                 table: "Quizzes",
//                 columns: new[] { "Id", "CourseId", "CreatedByStudentId", "Description", "Title" },
//                 values: new object[] { 1000, 1, 1, "A ten-question introduction to web development.", "Web basics" });

//             migrationBuilder.InsertData(
//                 table: "QuizQuestions",
//                 columns: new[] { "Id", "CorrectAnswer", "Order", "Points", "Prompt", "QuizId" },
//                 values: new object[,]
//                 {
//                     { 10001, "Hypertext Transfer Protocol", 1, 1m, "What does HTTP stand for?", 1000 },
//                     { 10002, "4", 2, 1m, "What is 2 + 2?", 1000 },
//                     { 10003, "HyperText Markup Language", 3, 1m, "What does HTML stand for?", 1000 },
//                     { 10004, "Cascading Style Sheets", 4, 1m, "What does CSS stand for?", 1000 },
//                     { 10005, "JavaScript", 5, 1m, "Which language is commonly used to add interactivity to webpages?", 1000 },
//                     { 10006, "GET", 6, 1m, "Which HTTP method is commonly used to retrieve data?", 1000 },
//                     { 10007, "POST", 7, 1m, "Which HTTP method is commonly used to submit data?", 1000 },
//                     { 10008, "Uniform Resource Locator", 8, 1m, "What does URL stand for?", 1000 },
//                     { 10009, "200", 9, 1m, "Which HTTP status code indicates a successful request?", 1000 },
//                     { 10010, "404", 10, 1m, "Which HTTP status code means a requested resource was not found?", 1000 }
//                 });
//         }

//         /// <inheritdoc />
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10001);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10002);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10003);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10004);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10005);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10006);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10007);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10008);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10009);

//             migrationBuilder.DeleteData(
//                 table: "QuizQuestions",
//                 keyColumn: "Id",
//                 keyValue: 10010);

//             migrationBuilder.DeleteData(
//                 table: "Quizzes",
//                 keyColumn: "Id",
//                 keyValue: 1000);
//         }
//     }
// }
