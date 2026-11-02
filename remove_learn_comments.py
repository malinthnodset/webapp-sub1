import re
from pathlib import Path

# [LEARN] List of files that contain learning comments
files = [
    "SubApp1/Program.cs",
    "SubApp1/Controllers/CourseController.cs",
    "SubApp1/Controllers/DashboardController.cs",
    "SubApp1/Controllers/QuizController.cs",
    "SubApp1/DAL/CourseRepository.cs",
    "SubApp1/DAL/DBInit.cs",
    "SubApp1/DAL/ICourseRepository.cs",
    "SubApp1/DAL/IQuizRepository.cs",
    "SubApp1/DAL/QuizRepository.cs",
    "SubApp1/DAL/QuizDbContext.cs",
    "SubApp1/DAL/QuizDbContextFactory.cs",
    "SubApp1/Models/Course.cs",
    "SubApp1/Models/Quiz.cs",
    "SubApp1/Models/QuizAnswer.cs",
    "SubApp1/Models/QuizAttempt.cs",
    "SubApp1/Models/QuizQuestion.cs",
    "SubApp1/Models/QuizResult.cs",
    "SubApp1/Models/Student.cs",
    "SubApp1/Services/DashboardServices.cs",
    "SubApp1/Services/IQuizService.cs",
    "SubApp1/Services/QuizService.cs",
    "SubApp1/ViewModels/DashboardViewModels.cs",
    "SubApp1/ViewModels/QuizCreateViewModel.cs",
    "SubApp1/ViewModels/QuizResultViewModel.cs",
    "SubApp1/ViewModels/TakeQuizViewModel.cs",
    "SubApp1/Views/_ViewImports.cshtml",
    "SubApp1/Views/_ViewStart.cshtml",
    "SubApp1/Views/Shared/_Layout.cshtml",
    "SubApp1/Views/Shared/_CourseModule.cshtml",
    "SubApp1/Views/Dashboard/Index.cshtml",
    "SubApp1/Views/Quiz/Create.cshtml",
    "SubApp1/Views/Quiz/Result.cshtml",
    "SubApp1/Views/Quiz/Take.cshtml",
    "SubApp1.Tests/UnitTest1.cs",
]

root = Path(__file__).resolve().parent

for rel in files:
    path = root / rel
    if not path.exists():
        print("SKIP (missing):", rel)
        continue
    original = path.read_text(encoding="utf-8")
    out_lines = []
    for line in original.splitlines(keepends=True):
        stripped = line.strip()
        # Whole-line comment removal
        if re.fullmatch(r'\s*// \[LEARN\].*', stripped) or \
           re.fullmatch(r'\s*/\* \[LEARN\].*\*/', stripped) or \
           re.fullmatch(r'\s*<!-- \[LEARN\].*-->', stripped) or \
           re.fullmatch(r'\s*@\* \[LEARN\].*\*@', stripped):
            continue
        # Inline removal
        line = re.sub(r'\s*// \[LEARN\].*$', '', line)
        line = re.sub(r'/\* \[LEARN\].*?\*/', '', line)
        line = re.sub(r'<!-- \[LEARN\].*?-->', '', line)
        line = re.sub(r'@\* \[LEARN\].*?\*@', '', line)
        out_lines.append(line)
    path.write_text(''.join(out_lines), encoding="utf-8")
    print("Cleaned:", rel)

print("Done. You can delete this script afterward.")
