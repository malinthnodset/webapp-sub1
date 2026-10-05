# ITPE3200 Web Applications — SubApp 1

Study Buddy is a gamified learning application for practising course material through quizzes. Students can browse courses, take quizzes, receive scored results, and review their attempt history. The project is an early-stage implementation of a broader learning platform concept.

## Assignment and project links

- [Requirements specification and contract](https://oslomet.instructure.com/courses/34659/assignments/124642)
- [Minimum viable product (MVP)](https://oslomet.instructure.com/courses/34659/assignments/124641)
- [Exam assignment](https://oslomet.instructure.com/courses/34659/assignments/124639)

## Project goals

The broader assignment concept is to turn course content into interactive challenges that help students practise, check their knowledge, and follow their progress. Longer-term ideas include:

- Creating, editing, and organizing quizzes, question sets, and programming tasks.
- Submitting answers and receiving feedback.
- Reviewing scores and previous submissions.
- Monitoring student progress (for professors).
- Potentially adding leaderboards, achievements, and streaks.

The current application implements part of this concept, focused on text-answer quizzes.

## Current features

- A dashboard groups available quizzes by course.
- Quiz creation collects a title, optional description, course, questions, correct answers, and points.
- Students can take a quiz and receive a score with per-question feedback.
- Quiz attempts and results are stored in SQLite.
- The result page can show attempt history.
- A development database initializer applies migrations and seeds sample courses, students, quizzes, and questions.

## Technology and project layout

- **ASP.NET Core MVC / Razor** for the web application.
- **Entity Framework Core 10** with **SQLite** for persistence and schema migrations.
- **Serilog** for console and file logging.
- **Bootstrap 5.3.3** and **jQuery 3.7.1** for the supplied client-side libraries.
- **xUnit** for tests.

Main folders:

- `Controllers/` — MVC actions for the dashboard, quizzes, and courses.
- `DAL/` — EF Core context, repositories, and development database initialization.
- `Migrations/` — database schema and migration history.
- `Models/` — persisted entities such as courses, quizzes, questions, attempts, and results.
- `Services/` — quiz grading and dashboard construction.
- `ViewModels/` — data passed between controllers, services, and views.
- `Views/` — Razor pages.
- `wwwroot/` — CSS and static assets.
- `../SubApp1.Tests/` — xUnit test project (a sibling of the application folder).

## Requirements

- .NET 10 SDK.
- No external database server is required; SQLite uses a local file.

Restore dependencies from the repository root:

```powershell
dotnet restore .\SubApp1\SubApp1.csproj
dotnet restore .\SubApp1.Tests\SubApp1.Tests.csproj
```

## Configuration

The main application settings are in `appsettings.json`:

- `ConnectionStrings:DefaultConnection` selects the SQLite database. It currently uses `Data Source=quiz.db`.
- The `Logging` section sets the ASP.NET logging levels.

`appsettings.Development.json` contains development-specific logging configuration. `Properties/launchSettings.json` starts the app in the Development environment at `https://localhost:7244`.
Serilog writes timestamped log files to `Logs/` and also writes to the console.

The SQLite filename is relative to the process working directory. Run the application from the `SubApp1` directory if you want the database file to be `SubApp1/quiz.db`. In Development, startup runs EF Core migrations and seeds sample data when the database has no courses. The initializer does not reseed an existing database.

To use a different database file, change `ConnectionStrings:DefaultConnection`, for example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=study-buddy.db"
}
```

Do not commit credentials or other secrets in configuration files. Use .NET user secrets or environment variables if secrets are introduced later.

## Run the application

From the repository root:

```powershell
Set-Location .\SubApp1
dotnet run
```

Open the URL printed by ASP.NET Core (the configured HTTPS launch URL is `https://localhost:7244`). The default route opens the dashboard. The main pages are:

- `/Dashboard/Index` — course and quiz dashboard.
- `/Quiz/Create` — create a quiz.
- `/Quiz/Take/{id}` — take a quiz with the given ID.

The first Development startup applies pending migrations and creates sample data in the configured SQLite database. The app does not automatically initialize or seed the database outside Development.

## Run tests

From the repository root:

```powershell
dotnet test .\SubApp1.Tests\SubApp1.Tests.csproj
```

## Database migrations

EF Core migrations are stored in `Migrations/`. The development initializer applies pending migrations at startup. To update the database manually, run from the `SubApp1` directory so the relative SQLite filename resolves to the application's `quiz.db`:

```powershell
dotnet ef database update
```

If the `dotnet-ef` command is not installed, install the EF Core 10 command-line tool first:

```powershell
dotnet tool install --global dotnet-ef --version 10.*
```

Create a migration after changing the EF model. Run this command from the repository root:

```powershell
dotnet ef migrations add MigrationName --project .\SubApp1\SubApp1.csproj --startup-project .\SubApp1\SubApp1.csproj
```

## Current limitations and future improvements

The following items are based on unfinished implementations and TODOs in the source:

- **Authentication and user identity:** replace the demo dashboard user and hard-coded student ID used when creating quizzes and recording attempts with the signed-in student's ID.
- **Roles and account types:** distinguish students from teachers/instructors and apply the appropriate permissions.
- **Course management:** implement the course repository's unfinished lookup, create, update, and delete operations, and add corresponding user-facing workflows.
- **Personalized courses:** add faculty and course-registration data so course suggestions or access can reflect a student's studies.
- **More question types:** support multiple-choice questions and programming tasks in addition to the current text-answer questions.
- **Creation-form feedback:** warn before leaving a quiz form with unsaved data and show clear status feedback while a quiz is being created and after it succeeds.
- **Improved automated coverage:** add tests for database persistence, migrations/seeding, dashboard behavior, and MVC form validation.

<!--
## Original early design sketches

The original proposal included a start page and login, followed by a dashboard for courses, profiles, quizzes, and results. These diagrams are conceptual sketches and do not describe every feature in the current application.
-->
