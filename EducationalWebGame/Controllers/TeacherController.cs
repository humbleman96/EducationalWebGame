using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EducationalWebGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWebGame.Controllers
{
    public class TeacherController : Controller
    {
        private readonly EducationalWebGameContext context;

        private static int currentLoggedInTeacherRole;
        private static Teacher currentTeacher = null;
        private static ResourceTeacher currentResourceTeacher = null;
        private static bool isTeacherRoleAssigned = false;

        private static List<Game> selectedQuestions;
        private static Game currentQuestion = null;
        private static bool areSelQuestionsInitialized = false;
        private static bool isAddEditQueFormInitialized = false;
        private static string addEditContent;
        private static string addEditTitle;
        private static string message = string.Empty;
        private static string hiddenSelects;
        private static Dictionary<string, int> gameTypes;
        private static Dictionary<string, int> subjects;

        private static List<Student> students;
        private static List<int> rankings;

        public TeacherController(EducationalWebGameContext context)
        {
            this.context = context;
        }

        public IActionResult Index(Teacher teacher, ResourceTeacher resourceTeacher)
        {
            if (!isTeacherRoleAssigned)
            {
                currentLoggedInTeacherRole = AssignTeacherRole(teacher, resourceTeacher);
                isTeacherRoleAssigned = true;
            }

            if (currentLoggedInTeacherRole == 1)
            {
                return View(currentTeacher);
            }

            else
            {
                return View(currentResourceTeacher);
            }
        }

        private int AssignTeacherRole(Teacher teacher, ResourceTeacher resourceTeacher)
        {
            List<Teacher> teachers = context.Teachers.ToList();

            for (int i = 0; i < teachers.Count; i++)
            {
                if (teachers[i].UserName.Equals(teacher.UserName))
                {
                    currentLoggedInTeacherRole = 1;
                    break;
                }

                else
                {
                    currentLoggedInTeacherRole = 2;
                }
            }

            if (currentLoggedInTeacherRole == 1)
            {
                currentTeacher = teacher;
            }

            else
            {
                currentResourceTeacher = resourceTeacher;
            }

            return currentLoggedInTeacherRole;
        }

        public IActionResult Questions()
        {
            if (!areSelQuestionsInitialized)
            {
                selectedQuestions = (from g in context.Games
                                     join s in context.Subjects on g.SubjectId equals s.Id
                                     join gt in context.GameTypes on g.GameTypeId equals gt.Id
                                     where s.SubjectName.Equals("Човекът и природата")
                                     where gt.GameTypeName.Equals("Бесеница")
                                     select g).ToList();
                areSelQuestionsInitialized = true;
            }
            return View(selectedQuestions);
        }

        public IActionResult LoadQuestions(string gameType, string subject)
        {
            selectedQuestions = (from g in context.Games
                                 join s in context.Subjects on g.SubjectId equals s.Id
                                 join gt in context.GameTypes on g.GameTypeId equals gt.Id
                                 where s.SubjectName.Equals(subject)
                                 where gt.GameTypeName.Equals(gameType)
                                 select g).ToList();

            return RedirectToAction("Questions", selectedQuestions);
        }

        public IActionResult AddOrEditQuestionForm(int Id)
        {
            if (!isAddEditQueFormInitialized)
            {
                if (Id == 0)
                {
                    addEditContent = "Добави";
                    addEditTitle = "Добавяне на въпрос";
                    hiddenSelects = string.Empty;
                    currentQuestion = new Game();
                    message = string.Empty;
                }

                else
                {
                    addEditContent = "Редактирай";
                    addEditTitle = "Редактиране на въпрос";
                    hiddenSelects = "hidden";
                    currentQuestion = context.Games.Find(Id);
                    message = string.Empty;
                }

                InitializeGameTypesSubjects();
                isAddEditQueFormInitialized = true;
            }

            ViewBag.AddEditContent = addEditContent;
            ViewBag.AddEditTitle = addEditTitle;
            ViewBag.Message = message;
            ViewBag.HiddenSelects = hiddenSelects;

            return View(currentQuestion);

        }

        private void InitializeGameTypesSubjects()
        {
            gameTypes = new Dictionary<string, int>();
            subjects = new Dictionary<string, int>();

            gameTypes.Add("Бесеница", 1);
            gameTypes.Add("Подреди думата", 2);
            gameTypes.Add("Балони", 3);
            gameTypes.Add("Викторина", 4);
            gameTypes.Add("Шифър", 5);

            subjects.Add("Човекът и природата", 1);
            subjects.Add("Човекът и обществото", 2);
        }

        public IActionResult AddOrEditQuestion(string question, string correctAnswer, string extraInfo, string imagePath, uint coinsGiven, uint pointsGiven, string gameType, string subject)
        {
            Regex coinsPointsRegex = new Regex("^[0-9]{2}$");

            Regex hangmanCorrectAnswerRegex = new Regex(@"^([А-Я]\s)+[А-Я]$|^(([А-Я])\s)+(\s([А-Я]\s)+)+[А-Я]$");

            Regex CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex = new Regex("^[А-Я]+$");

            Regex balloonsCorrectAnswerRegex = new Regex(@"^[А-Я]([А-Яа-я])*(\s{1}([А-Яа-я-])+)+[!?.]$");

            Regex quizCorrectAnswerRegex = new Regex(@"^[А-Яа-я]([а-я]+)[а-я]$|^[А-Яа-я]([А-Яа-я])+(\s{1}([А-Яа-я])+)+[а-я]$");
            Regex quizExtraInfoRegex = new Regex(@"^[А-Яа-я]([а-я\s{1}]+)(\s{2}[А-Яа-я\s{1}]+){2}[а-я]$|^[А-Яа-я]([А-Яа-я])+(\s{1}([А-Яа-я])+)+(\s{2}[А-Яа-я]([А-Яа-я])+(\s{1}([А-Яа-я])+)+){2}[а-я]$");

            Regex cipherExtraInfoRegex = new Regex(@"^([0-9].)(\s[0-9]+\-([а-я]|\s)+.)(\s{1}([0-9].\s[0-9]+\-([а-я]|\s)+.))+.$");

            currentQuestion.Question = question;

            if (addEditContent.Equals("Добави"))
            {
                currentQuestion.GameTypeId = gameTypes[gameType];
                currentQuestion.SubjectId = subjects[subject];

                switch (gameType)
                {
                    case "Бесеница":
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (!hangmanCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [С Л О Н]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }


                            if (hangmanCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Add(currentQuestion);
                                context.SaveChanges();
                                currentQuestion = new Game();
                                message = "Успешно добавяне на въпрос!";
                            }

                            break;
                        }

                    case "Подреди думата":
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (extraInfo is null)
                            {
                                message = "Моля попълнете полето 'Допълнителна информация'";
                            }

                            else
                            {
                                if (!CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(extraInfo))
                                {
                                    message = "Примерен формат на допълнителната информация: [ОНСЛВКТПЧ]";
                                }

                                else
                                {
                                    currentQuestion.ExtraInfo = extraInfo;
                                }
                            }

                            if (!CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [СЛОН]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }


                            if (CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(extraInfo) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Add(currentQuestion);
                                context.SaveChanges();
                                currentQuestion = new Game();
                                message = "Успешно добавяне на въпрос!";
                            }

                            break;
                        }

                    case "Балони":
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (!balloonsCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен Формат на отговора: [Магнитът привлича метални предмети.]";
                                break;
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }

                            if (balloonsCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Add(currentQuestion);
                                context.SaveChanges();
                                currentQuestion = new Game();
                                message = "Успешно добавяне на въпрос!";
                            }

                            break;
                        }

                    case "Викторина":
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (extraInfo is null)
                            {
                                message = "Моля попълнете полето 'Допълнителна информация'";
                            }

                            else
                            {
                                if (!quizExtraInfoRegex.IsMatch(extraInfo))
                                {
                                    message = "Примерен формат на допълнителната информация: [Сатурн  Юпитер  Земя]";
                                }

                                else
                                {
                                    currentQuestion.ExtraInfo = extraInfo;
                                }
                            }

                            if (!quizCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [Юпитер]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }

                            if (quizCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               quizExtraInfoRegex.IsMatch(extraInfo) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                currentQuestion.ImagePath = imagePath;
                                context.Games.Add(currentQuestion);
                                context.SaveChanges();
                                currentQuestion = new Game();
                                message = "Успешно добавяне на въпрос!";
                            }

                            break;
                        }

                    case "Шифър":
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (extraInfo is null)
                            {
                                message = "Моля попълнете полето 'Допълнителна информация'";
                            }

                            else
                            {
                                if (!cipherExtraInfoRegex.IsMatch(extraInfo))
                                {
                                    message = "Примерен формат на допълнителната информация: [1. 18-та буква от азбуката. 2. 15-та буква от азбуката. 3. 12-та буква от азбуката.]";
                                }

                                else
                                {
                                    currentQuestion.ExtraInfo = extraInfo;
                                }
                            }

                            if (!CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [СОЛ]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }

                            if (CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               cipherExtraInfoRegex.IsMatch(extraInfo) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Add(currentQuestion);
                                context.SaveChanges();
                                currentQuestion = new Game();
                                message = "Успешно добавяне на въпрос!";
                            }
                            break;
                        }
                }
            }

            else
            {
                switch (currentQuestion.GameTypeId)
                {
                    case 1:
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (!hangmanCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [С Л О Н]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }


                            if (hangmanCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Update(currentQuestion);
                                context.SaveChanges();
                                message = "Успешно редактиране на въпрос!";
                            }

                            break;
                        }

                    case 2:
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (extraInfo is null)
                            {
                                message = "Моля попълнете полето 'Допълнителна информация'";
                            }

                            else
                            {
                                if (!CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(extraInfo))
                                {
                                    message = "Примерен формат на допълнителната информация: [ОНСЛВКТПЧ]";
                                }

                                else
                                {
                                    currentQuestion.ExtraInfo = extraInfo;
                                }
                            }

                            if (!CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [СЛОН]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }


                            if (CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(extraInfo) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Update(currentQuestion);
                                context.SaveChanges();
                                message = "Успешно редактиране на въпрос!";
                            }

                            break;
                        }

                    case 3:
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (!balloonsCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен Формат на отговора: [Магнитът привлича метални предмети.]";
                                break;
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }

                            if (balloonsCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Update(currentQuestion);
                                context.SaveChanges();
                                message = "Успешно редактиране на въпрос!";
                            }

                            break;
                        }

                    case 4:
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (extraInfo is null)
                            {
                                message = "Моля попълнете полето 'Допълнителна информация'";
                            }

                            else
                            {
                                if (!quizExtraInfoRegex.IsMatch(extraInfo))
                                {
                                    message = "Примерен формат на допълнителната информация: [Сатурн  Юпитер  Земя]";
                                }

                                else
                                {
                                    currentQuestion.ExtraInfo = extraInfo;
                                }
                            }

                            if (!quizCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [Юпитер]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }

                            if (quizCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               quizExtraInfoRegex.IsMatch(extraInfo) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                currentQuestion.ImagePath = imagePath;
                                context.Games.Update(currentQuestion);
                                context.SaveChanges();
                                message = "Успешно редактиране на въпрос!";
                            }

                            break;
                        }

                    case 5:
                        {
                            if (!coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                message = "Примерен формат на брой точки: [10]";
                            }

                            else
                            {
                                currentQuestion.PointsGiven = pointsGiven;
                            }

                            if (!coinsPointsRegex.IsMatch(coinsGiven.ToString()))
                            {
                                message = "Примерен формат на брой монети: [10]";
                            }

                            else
                            {
                                currentQuestion.CoinsGiven = coinsGiven;
                            }

                            if (extraInfo is null)
                            {
                                message = "Моля попълнете полето 'Допълнителна информация'";
                            }

                            else
                            {
                                if (!cipherExtraInfoRegex.IsMatch(extraInfo))
                                {
                                    message = "Примерен формат на допълнителната информация: [1. 18-та буква от азбуката. 2. 15-та буква от азбуката. 3. 12-та буква от азбуката.]";
                                }

                                else
                                {
                                    currentQuestion.ExtraInfo = extraInfo;
                                }
                            }

                            if (!CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer))
                            {
                                message = "Примерен формат на отговора: [СОЛ]";
                            }

                            else
                            {
                                currentQuestion.CorrectAnswer = correctAnswer;
                            }

                            if (CWCorrectAnswerExtraInfoCipherCorrectAnswerRegex.IsMatch(correctAnswer) &&
                               cipherExtraInfoRegex.IsMatch(extraInfo) &&
                               coinsPointsRegex.IsMatch(coinsGiven.ToString()) &&
                               coinsPointsRegex.IsMatch(pointsGiven.ToString()))
                            {
                                context.Games.Update(currentQuestion);
                                context.SaveChanges();
                                message = "Успешно редактиране на въпрос!";
                            }
                            break;
                        }
                }
            }

            return RedirectToAction("AddOrEditQuestionForm", currentQuestion);
        }

        public IActionResult BackToQuestions()
        {
            areSelQuestionsInitialized = false;
            isAddEditQueFormInitialized = false;
            return RedirectToAction("Questions");
        }

        public IActionResult BackToMainMenu()
        {
            areSelQuestionsInitialized = false;
            return RedirectToAction("Index");
        }

        public IActionResult DeleteQuestion(int Id)
        {
            currentQuestion = context.Games.Find(Id);
            context.Games.Remove(currentQuestion);
            context.SaveChanges();
            selectedQuestions = (from g in context.Games
                                 where g.SubjectId.Equals(currentQuestion.SubjectId)
                                 where g.GameTypeId.Equals(currentQuestion.GameTypeId)
                                 select g).ToList();
            return RedirectToAction("Questions", selectedQuestions);
        }

        public IActionResult Rankings()
        {
            if (currentLoggedInTeacherRole == 2)
            {
                students = (from st in context.Students
                            where st.ResourceTeacherId.Equals(currentResourceTeacher.Id)
                            select st).ToList();

                students = students.OrderByDescending(p => p.Points).ThenByDescending(c => c.Coins).ThenByDescending(l => l.Lives).ToList();
                CreateRankings(students);
               
            }

            else
            {
                students = (from st in context.Students
                            where st.TeacherId.Equals(currentTeacher.Id)
                            select st).ToList();

                students = students.OrderByDescending(p => p.Points).ThenByDescending(c => c.Coins).ThenByDescending(l => l.Lives).ToList();
                CreateRankings(students);
            }

            ViewBag.Students = students;
            ViewBag.Rankings = rankings;

            return View(students);
        }

        private void CreateRankings(List<Student> students)
        {
            rankings = new List<int>();
            int rank = 1;

            for (int i = 1; i < students.Count; i++)
            {
                if (i == 1)
                {
                    if (students[i].Points == students[i - 1].Points && students[i].Coins ==
                       students[i - 1].Coins && students[i].Lives == students[i - 1].Lives)
                    {
                        rankings.Add(rank);
                        rankings.Add(rank);
                    }

                    else
                    {
                        rankings.Add(rank);
                        rank = i + 1;
                        rankings.Add(rank);
                    }
                }

                else
                {
                    if (students[i].Points == students[i - 1].Points && students[i].Coins ==
                       students[i - 1].Coins && students[i].Lives == students[i - 1].Lives)
                    {
                        rankings.Add(rank);
                    }

                    else
                    {
                        rank = i + 1;
                        rankings.Add(rank);
                    }
                }
            }
        }

        public IActionResult LogOut()
        {
            isTeacherRoleAssigned = false;
            return RedirectToAction("Index", "Main");
        }
    }
}