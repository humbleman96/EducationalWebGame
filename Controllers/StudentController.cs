using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EducationalWebGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWebGame.Controllers
{
    public class StudentController : Controller
    {
        private readonly EducationalWebGameContext context;

        private static List<string> bosses;
        private static List<string> bossNames;
        private static List<string> bossesImages;
        private static string currentBoss;

        private static Dictionary<string, List<string>> allPaths;
        private static List<string> availablePaths;
        private static List<string> pathChoicesLog;
        private static int pathCounterIndex = 0;
        private static bool canAddNewPath = true;
        private static string chosenPath;
        private static int pathClickCounter = 0;
        private static bool arePathsInitialized = false;
        private static bool isGameCreated = false;
        private static bool areBossesCreated = false;

        private static int correctAnswerCounter = 0;
        private static int strikes;
        private static string answerMessage;
        private static string contentButton = string.Empty;
        private static string hiddenContentButton;

        private static List<string> letterColours;
        private static List<string> disabledLetters;
        private static string disabledClue = string.Empty;
        private static string currentAnswer;
        private static int currentGameType;
        private static Student currentStudent;
        private static Game currentGame;

        private static uint totalPoints;
        private static uint totalCoins;
        private static uint livesLeft;
        private static uint extraLifePoints; 
        private static int bossHealthLeft = 28;
        private static int playerHealthLeft = 28;
        private static string currentPlayerHealth;
        private static string currentBossHealth;

        private static char currentPressedLetter;
        private static List<string> hiddenExtraLetters;
        private static List<char> lettersInAnswer;
        private static string disabledCheckButton = string.Empty;
        private static List<string> disabledAnswerLetters;
        private static List<string> disabledExtraLetters;

        private static List<string> hiddenWords;
        private static List<string> wordsInGame;
        private static List<string> wordsInAnswer;
        private static List<string> disabledAnswerWords;

        private static string image;
        private static List<string> shuffledAnswers;
        private static List<string> buttonColours;
        private static List<string> disabledQuizAnswers;
        private static List<string> hiddenQuizAnswers;

        private static List<int> counters;
        private static Dictionary<int, char> alphabeticalLetters;
        private static List<string> clues;
        private static List<string> disabledCipherButtons;
        private static string alphabetImage;


        public StudentController(EducationalWebGameContext context)
        {
            this.context = context;
        }

        public IActionResult Index(Student student)
        {
            currentStudent = student;
            return View(currentStudent);
        }

        public IActionResult StartGame()
        {
            return RedirectToAction("BossSelect", currentStudent);
        }

        public IActionResult BossSelect()
        {
            if (!areBossesCreated)
            {
                bosses = LoadBosses();
                bossNames = LoadBossNames();
                bossesImages = LoadBossImages();
                areBossesCreated = true;
            }

            ViewBag.Bosses = bosses;
            ViewBag.BossesImages = bossesImages;
            ViewBag.BossNames = bossNames;
            return View(currentStudent);
        }

        private List<string> LoadBossNames()
        {
            List<string> bossNames = new List<string>();

            bossNames.Add("Зевс");
            bossNames.Add("д-р Всезнайко");
            bossNames.Add("Посейдон");

            return bossNames;

        }

        private List<string> LoadBossImages()
        {
            List<string> bossImages = new List<string>();

            bossImages.Add(@"\images\elecman\elecman not defeated.png");
            bossImages.Add(@"\images\wily\wily not defeated.png");
            bossImages.Add(@"\images\iceman\iceman not defeated.png");

            return bossImages;
        }

        private List<string> LoadBosses()
        {
            List<string> bosses = new List<string>();

            bosses.Add("elecman");
            bosses.Add("wily");
            bosses.Add("iceman");

            return bosses;
        }


        public IActionResult LoadStage(string bossName)
        {
            if (!arePathsInitialized)
            {
                allPaths = PopulatePaths();
                availablePaths = AvailablePaths();
                chosenPath = @"\images\pyramid\entrance.jpg";
                pathCounterIndex = 0;
                pathChoicesLog = new List<string>();
                pathChoicesLog.Add("entrance");
                currentBoss = bossName;
                arePathsInitialized = true;
                canAddNewPath = true;
            }

            ViewBag.Paths = availablePaths;
            ViewBag.Path = chosenPath;

            return View(currentStudent);
        }

        public IActionResult ChoosePath(string position)
        {
            Dictionary<string, int> pathPositions = new Dictionary<string, int>();
            pathPositions.Add("top", 0);
            pathPositions.Add("middle", 1);
            pathPositions.Add("bottom", 2);

            pathClickCounter++;

            if (pathClickCounter == 1)
            {
                if (canAddNewPath)
                {
                    pathChoicesLog.Add(position);
                }

                else
                {
                    pathChoicesLog[pathCounterIndex + 1] = position;
                }

                int index = pathPositions[position];
                chosenPath = allPaths[pathChoicesLog[pathCounterIndex]][index];

                for (int i = 0; i < availablePaths.Count; i++)
                {
                    if (i != index)
                    {
                        availablePaths[i] = "hidden";
                    }
                }
                return RedirectToAction("LoadStage", currentStudent);
            }

            else
            {
                pathClickCounter = 0;
                for (int i = 0; i < availablePaths.Count; i++)
                {
                    availablePaths[i] = string.Empty;
                }

                string gameTypeName = string.Empty;

                switch (correctAnswerCounter)
                {
                    case 0: gameTypeName = "Hangman"; break;
                    case 1: gameTypeName = "ConstructTheWord"; break;
                    case 2: gameTypeName = "Balloons"; break;
                    case 3: gameTypeName = "Quiz"; break;
                    case 4: gameTypeName = "Hangman"; break;
                    case 5: gameTypeName = "ConstructTheWord"; break;
                    case 6: gameTypeName = "Balloons"; break;
                    case 7: gameTypeName = "Quiz"; break;
                    case 8: gameTypeName = "Cipher"; break;
                }

                return RedirectToAction(gameTypeName, currentStudent);
            }

        }

        private List<string> AvailablePaths()
        {
            List<string> hiddenPaths = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                hiddenPaths.Add(string.Empty);
            }
            return hiddenPaths;
        }

        private Dictionary<string, List<string>> PopulatePaths()
        {
            Dictionary<string, List<string>> paths = new Dictionary<string, List<string>>();
            List<string> entrances = new List<string>();
            List<string> tops = new List<string>();
            List<string> middles = new List<string>();
            List<string> bottoms = new List<string>();

            entrances.Add(@"\images\pyramid\entrancetop.gif");
            entrances.Add(@"\images\pyramid\entrancemiddle.gif");
            entrances.Add(@"\images\pyramid\entrancebottom.gif");

            tops.Add(@"\images\pyramid\toptop.gif");
            tops.Add(@"\images\pyramid\topmiddle.gif");
            tops.Add(@"\images\pyramid\topbottom.gif");

            middles.Add(@"\images\pyramid\middletop.gif");
            middles.Add(@"\images\pyramid\middlemiddle.gif");
            middles.Add(@"\images\pyramid\middlebottom.gif");

            bottoms.Add(@"\images\pyramid\bottomtop.gif");
            bottoms.Add(@"\images\pyramid\bottommiddle.gif");
            bottoms.Add(@"\images\pyramid\bottombottom.gif");

            paths.Add("entrance", entrances);
            paths.Add("top", tops);
            paths.Add("middle", middles);
            paths.Add("bottom", bottoms);

            return paths;
        }

        #region Hangman
        public IActionResult Hangman()
        {
            if (!isGameCreated)
            {
                answerMessage = string.Empty;
                currentGameType = 1;
                currentGame = ChooseQuestion(currentStudent);
                currentAnswer = CreateGame(currentGame.CorrectAnswer);
                letterColours = PopulateColours();
                disabledLetters = PopulateDisabledLetters();
                isGameCreated = true;
                strikes = 0;
                hiddenContentButton = "hidden";
                livesLeft = currentStudent.Lives;
                totalCoins = currentStudent.Coins;
                totalPoints = currentStudent.Points;
                disabledClue = string.Empty;

                if (correctAnswerCounter == 0)
                {
                    currentPlayerHealth = @"\images\rockman\rockmanenergyrefill0.gif";
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + "energyrefill.gif";
                    bossHealthLeft = 28;
                    playerHealthLeft = 28;
                }
            }

            ViewBag.Colours = letterColours;
            ViewBag.CurrentAnswer = currentAnswer;
            ViewBag.DisabledLetters = disabledLetters;
            ViewBag.Message = answerMessage;
            ViewBag.ContentButton = contentButton;
            ViewBag.HiddenContentButton = hiddenContentButton;
            ViewBag.Coins = totalCoins;
            ViewBag.Points = totalPoints;
            ViewBag.Lives = livesLeft;
            ViewBag.CurrentBossImage = @"\images\" + currentBoss + "\\" + currentBoss + ".png";
            ViewBag.CurrentBossHealth = currentBossHealth;
            ViewBag.CurrentPlayerHealth = currentPlayerHealth;
            ViewBag.DisabledClue = disabledClue;

            return View(currentGame);
        }

        private List<string> PopulateDisabledLetters()
        {
            List<string> disLetters = new List<string>();

            for (int i = 0; i < 30; i++)
            {
                disLetters.Add(string.Empty);
            }

            return disLetters;
        }


        [NonAction]
        private Game ChooseQuestion(Student student)
        {
            Dictionary<string, int> bossSubjects = new Dictionary<string, int>();
            bossSubjects.Add("elecman", 1);
            bossSubjects.Add("iceman", 2);
            bossSubjects.Add("wily", 3);

            Game game = new Game();
            List<Game> availableGames = new List<Game>();

            int subjectId = bossSubjects[currentBoss];
            if (subjectId != 3)
            {
                availableGames = (from hg in context.Games
                                  where hg.SubjectId.Equals(subjectId)
                                  where hg.GameTypeId.Equals(currentGameType)
                                  select hg).ToList();
            }

            else
            {
                availableGames = (from hg in context.Games
                                  where hg.GameTypeId.Equals(currentGameType)
                                  select hg).ToList();
            }

            List<StudentGame> studentGames = context.StudentGames.ToList();
            StudentGame studentGame = new StudentGame();

            while (true)
            {
                Random random = new Random();
                int index = random.Next(availableGames.Count);
                game = availableGames[index];

                if (studentGames.Count == 0)
                {
                    studentGame.StudentId = student.Id;
                    studentGame.GameId = game.Id;
                    studentGame.IsCorrect = false;
                    context.StudentGames.Add(studentGame);
                    context.SaveChanges();
                    break;
                }

                else
                {
                    studentGame = (from sg in context.StudentGames
                                   where sg.StudentId == student.Id
                                   where sg.GameId == game.Id
                                   select sg).FirstOrDefault();

                    if (studentGame is null)
                    {
                        studentGame = new StudentGame();
                        studentGame.StudentId = student.Id;
                        studentGame.GameId = game.Id;
                        studentGame.IsCorrect = false;
                        context.StudentGames.Add(studentGame);
                        context.SaveChanges();
                        break;
                    }

                    if (studentGame.IsCorrect == false)
                    {
                        break;
                    }
                }
            }

            return game;
        }


        [NonAction]
        private List<string> PopulateColours()
        {
            List<string> colours = new List<string>();
            for (int i = 0; i < 30; i++)
            {
                colours.Add("white");
            }
            return colours;
        }

        [NonAction]
        private string CreateGame(string answer)
        {
            string dashedAnswer = null;

            for (int i = 0; i < answer.Length; i++)
            {
                if (!char.IsWhiteSpace(answer[i]))
                {
                    dashedAnswer += "_";
                }
                else
                {
                    dashedAnswer += " ";
                }
            }
            return dashedAnswer;
        }


        public IActionResult CheckLetter(char letter)
        {
            Dictionary<char, int> letters = PopulateLetters();
            StringBuilder answerSoFar = new StringBuilder();
            answerSoFar.Append(currentAnswer);
            bool isCorrect = false;

            for (int i = 0; i < currentAnswer.Length; i++)
            {
                if (letter.Equals(currentGame.CorrectAnswer[i]))
                {
                    answerSoFar[i] = letter;
                    letterColours[letters[letter]] = "green";
                    disabledLetters[letters[letter]] = "disabled";
                    isCorrect = true;
                }
            }

            if (!isCorrect)
            {
                letterColours[letters[letter]] = "red";
                disabledLetters[letters[letter]] = "disabled";
                strikes++;
                if (strikes <= 2)
                {
                    playerHealthLeft = playerHealthLeft - 4;
                    currentPlayerHealth = ViewBag.CurrentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }

                else
                {
                    playerHealthLeft = playerHealthLeft - 5;
                    currentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }
            }

            currentAnswer = answerSoFar.ToString();

            if (!currentAnswer.Contains("_"))
            {
                correctAnswerCounter++;
                answerMessage = @"\images\answers\correctanswer.gif";
                contentButton = "Продължи напред!";
                totalCoins += currentGame.CoinsGiven;
                totalPoints += currentGame.PointsGiven;
                currentStudent.Coins = totalCoins;
                currentStudent.Points = totalPoints;
                context.Students.Update(currentStudent);
                context.SaveChanges();
                hiddenContentButton = string.Empty;
                bossHealthLeft = bossHealthLeft - 3;
                currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                DisableLetters();
                StudentGame stGame = (from sg in context.StudentGames
                                      where sg.GameId.Equals(currentGame.Id)
                                      where sg.StudentId.Equals(currentStudent.Id)
                                      select sg).FirstOrDefault();
                stGame.IsCorrect = true;
                context.StudentGames.Update(stGame);
                context.SaveChanges();
                disabledClue = "disabled";
                ExtraLifeChecker();
            }

            if (playerHealthLeft == 0)
            {
                answerMessage = @"\images\answers\wronganswer.gif";
                livesLeft = livesLeft - 1;
                currentStudent.Lives = livesLeft;
                context.Students.Update(currentStudent);
                context.SaveChanges();

                if (livesLeft != 0)
                {
                    contentButton = "Върни се обратно!";
                }

                else
                {
                    contentButton = "Избери нов противник!";
                }

                hiddenContentButton = string.Empty;
                DisableLetters();
                disabledClue = "disabled";
                currentAnswer = string.Empty;

                for (int i = 0; i < currentGame.CorrectAnswer.Length; i++)
                {
                    currentAnswer += currentGame.CorrectAnswer[i];
                    if (!char.IsWhiteSpace(currentGame.CorrectAnswer[i]))
                    {
                        int index = letters[currentGame.CorrectAnswer[i]];
                        letterColours[index] = "green";
                    }
                }

            }

            return RedirectToAction("Hangman", currentGame);
        }

        private void DisableLetters()
        {
            for (int i = 0; i < disabledLetters.Count; i++)
            {
                disabledLetters[i] = "disabled";
            }
        }

        private Dictionary<char, int> PopulateLetters()
        {
            Dictionary<char, int> letters = new Dictionary<char, int>();
            string alphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";

            for (int i = 0; i < alphabet.Length; i++)
            {
                letters.Add(alphabet[i], i);
            }

            return letters;
        }


        public IActionResult RevealLetterH()
        {
            if (currentStudent.Coins >= 10)
            {
                totalCoins = totalCoins - 10;
                currentStudent.Coins = totalCoins;
                context.Students.Update(currentStudent);
                context.SaveChanges();

                Dictionary<char, int> letters = PopulateLetters();
                StringBuilder answerSoFar = new StringBuilder();
                answerSoFar.Append(currentAnswer);

                char letter;

                while (true)
                {
                    Random random = new Random();
                    int index = random.Next(currentGame.CorrectAnswer.Length);
                    letter = currentGame.CorrectAnswer[index];

                    if (!currentAnswer.Contains(letter))
                    {
                        break;
                    }
                }

                for (int i = 0; i < currentAnswer.Length; i++)
                {
                    if (letter.Equals(currentGame.CorrectAnswer[i]))
                    {
                        answerSoFar[i] = letter;
                        letterColours[letters[letter]] = "green";
                        disabledLetters[letters[letter]] = "disabled";
                    }
                }

                currentAnswer = answerSoFar.ToString();

                if (!currentAnswer.Contains("_"))
                {
                    correctAnswerCounter++;
                    answerMessage = @"\images\answers\correctanswer.gif";
                    contentButton = "Продължи напред!";
                    totalCoins += currentGame.CoinsGiven;
                    totalPoints += currentGame.PointsGiven;
                    currentStudent.Coins = totalCoins;
                    currentStudent.Points = totalPoints;
                    context.Students.Update(currentStudent);
                    context.SaveChanges();
                    hiddenContentButton = string.Empty;
                    bossHealthLeft = bossHealthLeft - 3;
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                    DisableLetters();
                    StudentGame stGame = (from sg in context.StudentGames
                                          where sg.GameId == currentGame.Id
                                          where sg.StudentId == currentStudent.Id
                                          select sg).FirstOrDefault();
                    stGame.IsCorrect = true;
                    context.StudentGames.Update(stGame);
                    context.SaveChanges();
                    ExtraLifeChecker();
                }

                disabledClue = "disabled";
            }

            else
            {
                disabledClue = "disabled";
            }
            return RedirectToAction("Hangman");
        }
        #endregion     

        #region ConstructTheWord
        public IActionResult ConstructTheWord()
        {
            if (!isGameCreated)
            {
                answerMessage = string.Empty;
                currentGameType = 2;
                currentGame = ChooseQuestion(currentStudent);
                currentGame.ExtraInfo = ShuffleLetters();
                currentAnswer = string.Empty;
                isGameCreated = true;
                strikes = 0;
                hiddenContentButton = "hidden";
                livesLeft = currentStudent.Lives;
                totalCoins = currentStudent.Coins;
                totalPoints = currentStudent.Points;

                if (correctAnswerCounter == 0)
                {
                    currentPlayerHealth = @"\images\rockman\rockmanenergyrefill0.gif";
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + "energyrefill.gif";
                }

                currentPressedLetter = ' ';
                hiddenExtraLetters = PopulateHiddenValues();
                lettersInAnswer = new List<char>();
                disabledAnswerLetters = DisabledCWLetters(string.Empty);
                disabledExtraLetters = DisabledCWLetters(string.Empty);
                disabledClue = string.Empty;
                disabledCheckButton = string.Empty;
            }

            ViewBag.Letters = lettersInAnswer;
            ViewBag.HiddenExtraLetters = hiddenExtraLetters;
            ViewBag.Message = answerMessage;
            ViewBag.ContentButton = contentButton;
            ViewBag.HiddenContentButton = hiddenContentButton;
            ViewBag.CurrentAnswer = currentAnswer;
            ViewBag.Coins = totalCoins;
            ViewBag.Points = totalPoints;
            ViewBag.Lives = livesLeft;
            ViewBag.CurrentBossImage = @"\images\" + currentBoss + "\\" + currentBoss + ".png";
            ViewBag.CurrentBossHealth = currentBossHealth;
            ViewBag.CurrentPlayerHealth = currentPlayerHealth;
            ViewBag.DisabledClue = disabledClue;
            ViewBag.DisabledExtraLetters = disabledExtraLetters;
            ViewBag.DisabledAnswerLetters = disabledAnswerLetters;
            ViewBag.DisabledCheckButton = disabledCheckButton;

            return View(currentGame);
        }

        private string ShuffleLetters()
        {
            string shuffledLetters = string.Empty;
            List<int> letterIndexes = new List<int>();
            Dictionary<int, char> letters = new Dictionary<int, char>();

            for (int i = 0; i < currentGame.ExtraInfo.Length; i++)
            {
                letters.Add(i, currentGame.ExtraInfo[i]);
            }

            for (int i = 0; i < currentGame.ExtraInfo.Length; i++)
            {
                while (true)
                {
                    Random random = new Random();
                    int index = random.Next(currentGame.ExtraInfo.Length);
                    if (!letterIndexes.Contains(index))
                    {
                        letterIndexes.Add(index);
                        break;
                    }
                }
            }

            for (int i = 0; i < letterIndexes.Count; i++)
            {
                shuffledLetters += letters[letterIndexes[i]];
            }

            return shuffledLetters;
        }

        private List<string> PopulateHiddenValues()
        {
            List<string> hiddenVals = new List<string>();

            for (int i = 0; i < currentGame.ExtraInfo.Length; i++)
            {
                hiddenVals.Add(string.Empty);
            }

            return hiddenVals;
        }

        public IActionResult CheckWord()
        {
            if (currentAnswer.Equals(currentGame.CorrectAnswer))
            {
                if (currentBoss.Equals("wily"))
                {
                    bossHealthLeft = bossHealthLeft - 4;
                }
                else
                {
                    bossHealthLeft = bossHealthLeft - 5;
                }

                disabledAnswerLetters = DisabledCWLetters("disabled");
                disabledExtraLetters = DisabledCWLetters("disabled");
                answerMessage = @"\images\answers\correctanswer.gif";
                correctAnswerCounter++;
                contentButton = "Продължи напред!";
                totalCoins += currentGame.CoinsGiven;
                totalPoints += currentGame.PointsGiven;
                currentStudent.Coins = totalCoins;
                currentStudent.Points = totalPoints;
                context.Students.Update(currentStudent);
                context.SaveChanges();
                hiddenContentButton = string.Empty;
                currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                StudentGame stGame = (from sg in context.StudentGames
                                      where sg.GameId == currentGame.Id
                                      where sg.StudentId == currentStudent.Id
                                      select sg).FirstOrDefault();
                stGame.IsCorrect = true;
                context.StudentGames.Update(stGame);
                context.SaveChanges();
                disabledClue = "disabled";
                disabledCheckButton = "disabled";
                ExtraLifeChecker();
            }

            else
            {
                strikes++;
                if (strikes == 2)
                {
                    playerHealthLeft = playerHealthLeft - 10;
                    currentPlayerHealth = ViewBag.CurrentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }

                else
                {
                    playerHealthLeft = playerHealthLeft - 9;
                    currentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }

                if (playerHealthLeft == 0)
                {
                    answerMessage = @"\images\answers\wronganswer.gif";
                    disabledAnswerLetters = DisabledCWLetters("disabled");
                    disabledExtraLetters = DisabledCWLetters("disabled");
                    disabledCheckButton = "disabled";
                    livesLeft = livesLeft - 1;
                    currentStudent.Lives = livesLeft;
                    context.Students.Update(currentStudent);
                    context.SaveChanges();

                    if (livesLeft != 0)
                    {
                        contentButton = "Върни се обратно!";
                    }

                    else
                    {
                        contentButton = "Избери нов противник!";
                    }

                    hiddenContentButton = string.Empty;
                    disabledClue = "disabled";

                    lettersInAnswer.Clear();
                    List<char> lettersInGame = new List<char>();
                    for (int i = 0; i < currentGame.ExtraInfo.Length; i++)
                    {
                        lettersInGame.Add(currentGame.ExtraInfo[i]);
                    }

                    for (int i = 0; i < currentGame.CorrectAnswer.Length; i++)
                    {
                        lettersInAnswer.Add(currentGame.CorrectAnswer[i]);
                    }

                    hiddenExtraLetters = PopulateHiddenValues();

                    for (int i = 0; i < lettersInGame.Count; i++)
                    {
                        for (int j = 0; j < lettersInAnswer.Count; j++)
                        {
                            if (lettersInAnswer[j].Equals(lettersInGame[i]))
                            {
                                if (!hiddenExtraLetters[i].Equals("hidden"))
                                {
                                    hiddenExtraLetters[i] = "hidden";
                                    break;
                                }
                            }

                            else
                            {
                                hiddenExtraLetters[i] = string.Empty;
                            }
                        }
                    }
                }
            }
            return RedirectToAction("ConstructTheWord", currentGame);
        }

        private List<string> DisabledCWLetters(string command)
        {
            List<string> extraLetters = new List<string>();

            for (int i = 0; i < currentGame.ExtraInfo.Length; i++)
            {
                extraLetters.Add(command);
            }

            return extraLetters;
        }

        public IActionResult GenLetterInAnswer(int letterPosition)
        {
            currentAnswer = string.Empty;
            Dictionary<int, char> letters = new Dictionary<int, char>();
            for (int i = 0; i < currentGame.ExtraInfo.Length; i++)
            {
                letters.Add(i, currentGame.ExtraInfo[i]);
            }

            currentPressedLetter = letters[letterPosition];
            hiddenExtraLetters[letterPosition] = "hidden";

            lettersInAnswer.Add(currentPressedLetter);
            for (int i = 0; i < lettersInAnswer.Count; i++)
            {
                currentAnswer += lettersInAnswer[i];
            }

            return RedirectToAction("ConstructTheWord", currentGame);
        }
        // 0 1 2 3 4

        public IActionResult ReturnLetter(int letterPosition)
        {
            currentAnswer = string.Empty;
            Dictionary<int, char> letters = new Dictionary<int, char>();
            for (int i = 0; i < currentGame.ExtraInfo.Length; i++)
            {
                letters.Add(i, currentGame.ExtraInfo[i]);
            }

            currentPressedLetter = lettersInAnswer[letterPosition];

            foreach (KeyValuePair<int, char> dict in letters)
            {
                if (dict.Value.Equals(currentPressedLetter))
                {
                    int index = dict.Key;

                    if (hiddenExtraLetters[index].Equals("hidden"))
                    {
                        hiddenExtraLetters[index] = string.Empty;
                        break;
                    }
                }
            }

            lettersInAnswer.RemoveAt(letterPosition);

            for (int i = 0; i < lettersInAnswer.Count; i++)
            {
                currentAnswer += lettersInAnswer[i];
            }

            return RedirectToAction("ConstructTheWord", currentGame);
        }

        // 0 1 2 3 4
        public IActionResult RevealLetterCW()
        {
            int correctLetterCounter = 1;
            if (currentStudent.Coins >= 20)
            {
                disabledClue = "disabled";
                totalCoins = totalCoins - 20;
                currentStudent.Coins = totalCoins;
                context.Students.Update(currentStudent);
                context.SaveChanges();

                hiddenExtraLetters = PopulateHiddenValues();

                for (int i = 0; i < currentGame.CorrectAnswer.Length; i++)
                {
                    if (currentAnswer.Length == 0 || i == currentAnswer.Length)
                    {
                        break;
                    }

                    else
                    {
                        if (currentAnswer[i].Equals(currentGame.CorrectAnswer[i]))
                        {
                            correctLetterCounter++;
                        }

                        else
                        {
                            break;
                        }
                    }
                }

                if(correctLetterCounter > currentGame.CorrectAnswer.Length)
                {
                    correctLetterCounter = currentGame.CorrectAnswer.Length;
                }

                currentAnswer = string.Empty;
                for (int i = 0; i < correctLetterCounter; i++)
                {
                    currentAnswer += currentGame.CorrectAnswer[i];
                    disabledAnswerLetters[i] = "disabled";

                    for (int j = 0; j < currentGame.ExtraInfo.Length; j++)
                    {
                        if (currentAnswer[i].Equals(currentGame.ExtraInfo[j]))
                        {
                            if (!hiddenExtraLetters[j].Equals("hidden"))
                            {
                                hiddenExtraLetters[j] = "hidden";
                                break;
                            }
                        }
                    }
                }

                lettersInAnswer.Clear();
                for (int i = 0; i < currentAnswer.Length; i++)
                {
                    lettersInAnswer.Add(currentAnswer[i]);
                }

                if (currentAnswer.Equals(currentGame.CorrectAnswer))
                {
                    if (currentBoss.Equals("wily"))
                    {
                        bossHealthLeft = bossHealthLeft - 4;
                    }
                    else
                    {
                        bossHealthLeft = bossHealthLeft - 5;
                    }

                    disabledAnswerLetters = DisabledCWLetters("disabled");
                    disabledExtraLetters = DisabledCWLetters("disabled");
                    answerMessage = @"\images\answers\correctanswer.gif";
                    correctAnswerCounter++;
                    contentButton = "Продължи напред!";
                    totalCoins += currentGame.CoinsGiven;
                    totalPoints += currentGame.PointsGiven;
                    currentStudent.Coins = totalCoins;
                    currentStudent.Points = totalPoints;
                    context.Students.Update(currentStudent);
                    context.SaveChanges();
                    hiddenContentButton = string.Empty;
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                    StudentGame stGame = (from sg in context.StudentGames
                                          where sg.GameId == currentGame.Id
                                          where sg.StudentId == currentStudent.Id
                                          select sg).FirstOrDefault();
                    stGame.IsCorrect = true;
                    context.StudentGames.Update(stGame);
                    context.SaveChanges();
                    disabledClue = "disabled";
                    disabledCheckButton = "disabled";
                    ExtraLifeChecker();
                }
            }

            else
            {
                disabledClue = "disabled";
            }

            return RedirectToAction("ConstructTheWord");
        }
        #endregion

        #region Balloons
        public IActionResult Balloons()
        {
            if (!isGameCreated)
            {
                answerMessage = string.Empty;
                currentGameType = 3;
                currentGame = ChooseQuestion(currentStudent);
                currentAnswer = string.Empty;
                isGameCreated = true;
                strikes = 0;
                hiddenContentButton = "hidden";
                livesLeft = currentStudent.Lives;
                totalCoins = currentStudent.Coins;
                totalPoints = currentStudent.Points;

                if (correctAnswerCounter == 0)
                {
                    currentPlayerHealth = @"\images\rockman\rockmanenergyrefill0.gif";
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + "energyrefill.gif";
                }

                disabledClue = string.Empty;
                disabledCheckButton = string.Empty;
                disabledAnswerWords = DisabledWords(string.Empty);

                hiddenWords = PopulateHiddenGameWords();
                wordsInAnswer = new List<string>();
                wordsInGame = CreateBalloons();
            }

            ViewBag.Message = answerMessage;
            ViewBag.ContentButton = contentButton;
            ViewBag.HiddenContentButton = hiddenContentButton;
            ViewBag.CurrentAnswer = currentAnswer;
            ViewBag.Coins = totalCoins;
            ViewBag.Points = totalPoints;
            ViewBag.Lives = livesLeft;
            ViewBag.CurrentBossImage = @"\images\" + currentBoss + "\\" + currentBoss + ".png";
            ViewBag.CurrentBossHealth = currentBossHealth;
            ViewBag.CurrentPlayerHealth = currentPlayerHealth;
            ViewBag.DisabledClue = disabledClue;
            ViewBag.DisabledCheckButton = disabledCheckButton;
            ViewBag.HiddenWords = hiddenWords;
            ViewBag.WordsInAnswer = wordsInAnswer;
            ViewBag.WordsInGame = wordsInGame;
            ViewBag.DisabledAnswerWords = disabledAnswerWords;

            return View(currentGame);
        }

        private List<string> PopulateHiddenGameWords()
        {
            List<string> hiddenWords = new List<string>();
            int wordsCount = currentGame.CorrectAnswer.Split(' ').Length;

            for (int i = 0; i < wordsCount; i++)
            {
                hiddenWords.Add(string.Empty);
            }

            return hiddenWords;
        }

        [NonAction]
        private List<string> CreateBalloons()
        {
            List<string> balloons = new List<string>();
            Dictionary<int, string> wordsInSentence = new Dictionary<int, string>();
            List<int> wordsIndexes = new List<int>();

            string[] words = currentGame.CorrectAnswer.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                wordsInSentence.Add(i, words[i]);
            }

            for (int i = 0; i < words.Length; i++)
            {
                while (true)
                {
                    Random random = new Random();
                    int index = random.Next(words.Length);

                    if (!wordsIndexes.Contains(index))
                    {
                        wordsIndexes.Add(index);
                        break;
                    }
                }
            }

            for (int i = 0; i < wordsIndexes.Count; i++)
            {
                balloons.Add(wordsInSentence[wordsIndexes[i]]);
            }

            return balloons;
        }

        public IActionResult GenWordInAnswer(int wordPosition)
        {
            currentAnswer = string.Empty;
            wordsInAnswer.Add(wordsInGame[wordPosition]);
            hiddenWords[wordPosition] = "hidden";

            for (int i = 0; i < wordsInAnswer.Count; i++)
            {
                if (i == wordsInAnswer.Count - 1)
                {
                    currentAnswer += wordsInAnswer[i];
                }

                else
                {
                    currentAnswer += wordsInAnswer[i] + " ";
                }
            }

            return RedirectToAction("Balloons", currentGame);
        }

        public IActionResult ReturnWord(int wordPosition)
        {
            currentAnswer = string.Empty;
            for (int i = 0; i < wordsInGame.Count; i++)
            {
                if (wordsInGame[i].Equals(wordsInAnswer[wordPosition]))
                {
                    if (!hiddenWords[i].Equals(string.Empty))
                    {
                        hiddenWords[i] = string.Empty;
                        break;
                    }

                }
            }

            wordsInAnswer.RemoveAt(wordPosition);

            for (int i = 0; i < wordsInAnswer.Count; i++)
            {
                if (i == wordsInAnswer.Count - 1)
                {
                    currentAnswer += wordsInAnswer[i];
                }

                else
                {
                    currentAnswer += wordsInAnswer[i] + " ";
                }
            }

            return RedirectToAction("Balloons", currentGame);
        }

        public IActionResult CheckSentence()
        {
            string[] correctAnswerParts = currentGame.CorrectAnswer.Split(' ');
            string correctAnswer = string.Empty;

            for (int i = 0; i < correctAnswerParts.Length; i++)
            {
                if (i == correctAnswerParts.Length - 1)
                {
                    correctAnswer += correctAnswerParts[i];
                }

                else
                {
                    correctAnswer += correctAnswerParts[i] + " ";
                }

            }

            if (currentAnswer.Equals(correctAnswer))
            {
                disabledAnswerWords = DisabledWords("disabled");
                answerMessage = @"\images\answers\correctanswer.gif";
                correctAnswerCounter++;
                contentButton = "Продължи напред!";
                totalCoins += currentGame.CoinsGiven;
                totalPoints += currentGame.PointsGiven;
                currentStudent.Coins = totalCoins;
                currentStudent.Points = totalPoints;
                context.Students.Update(currentStudent);
                context.SaveChanges();
                hiddenContentButton = string.Empty;
                bossHealthLeft = bossHealthLeft - 4;
                currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                StudentGame stGame = (from sg in context.StudentGames
                                      where sg.GameId == currentGame.Id
                                      where sg.StudentId == currentStudent.Id
                                      select sg).FirstOrDefault();
                stGame.IsCorrect = true;
                context.StudentGames.Update(stGame);
                context.SaveChanges();
                disabledClue = "disabled";
                disabledCheckButton = "disabled";
                ExtraLifeChecker();
            }

            else
            {
                strikes++;
                if (strikes == 2)
                {
                    playerHealthLeft = playerHealthLeft - 10;
                    currentPlayerHealth = ViewBag.CurrentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }

                else
                {
                    playerHealthLeft = playerHealthLeft - 9;
                    currentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }

                if (playerHealthLeft == 0)
                {
                    answerMessage = @"\images\answers\wronganswer.gif";
                    disabledAnswerWords = DisabledWords("disabled");
                    disabledCheckButton = "disabled";
                    livesLeft = livesLeft - 1;
                    currentStudent.Lives = livesLeft;
                    context.Students.Update(currentStudent);
                    context.SaveChanges();

                    if (livesLeft != 0)
                    {
                        contentButton = "Върни се обратно!";
                    }

                    else
                    {
                        contentButton = "Избери нов противник!";
                    }

                    hiddenContentButton = string.Empty;
                    disabledClue = "disabled";

                    wordsInAnswer.Clear();

                    wordsInAnswer = currentGame.CorrectAnswer.Split(' ').ToList();
                    currentAnswer = string.Empty;
                    for (int i = 0; i < wordsInAnswer.Count; i++)
                    {
                        if (i == wordsInAnswer.Count - 1)
                        {
                            currentAnswer += wordsInAnswer[i];
                        }

                        else
                        {
                            currentAnswer += wordsInAnswer[i] + " ";
                        }

                    }

                    for (int i = 0; i < hiddenWords.Count; i++)
                    {
                        hiddenWords[i] = "hidden";
                    }
                }
            }
            return RedirectToAction("Balloons", currentGame);
        }

        private List<string> DisabledWords(string command)
        {
            List<string> disabledWords = new List<string>();
            int sentenceLength = currentGame.CorrectAnswer.Split(' ').Length;

            for (int i = 0; i < sentenceLength; i++)
            {
                disabledWords.Add(command);
            }

            return disabledWords;
        }

        public IActionResult RevealWord()
        {
            int correctWordCounter = 1;

            if (currentStudent.Coins >= 20)
            {
                disabledClue = "disabled";
                totalCoins = totalCoins - 20;
                currentStudent.Coins = totalCoins;
                context.Students.Update(currentStudent);
                context.SaveChanges();

                hiddenWords = PopulateHiddenGameWords();
                string[] correctAnswerParts = currentGame.CorrectAnswer.Split(' ');
                string[] currentAnswerParts = currentAnswer.Split(' ');

                for (int i = 0; i < correctAnswerParts.Length; i++)
                {
                    if (currentAnswerParts.Length == 0 || i == currentAnswerParts.Length)
                    {
                        break;
                    }

                    else
                    {
                        if (currentAnswerParts[i].Equals(correctAnswerParts[i]))
                        {
                            correctWordCounter++;
                        }

                        else
                        {
                            break;
                        }
                    }
                }

                if(correctWordCounter > correctAnswerParts.Length)
                {
                    correctWordCounter = correctAnswerParts.Length;
                }

                currentAnswer = string.Empty;
                for (int i = 0; i < correctWordCounter; i++)
                {
                    if (i == correctWordCounter - 1)
                    {
                        currentAnswer += correctAnswerParts[i];
                    }

                    else
                    {
                        currentAnswer += correctAnswerParts[i] + " ";
                    }

                    disabledAnswerWords[i] = "disabled";

                    for (int j = 0; j < wordsInGame.Count; j++)
                    {
                        if (correctAnswerParts[i].Equals(wordsInGame[j]))
                        {
                            if (!hiddenWords[j].Equals("hidden"))
                            {
                                hiddenWords[j] = "hidden";
                                break;
                            }
                        }
                    }
                }

                wordsInAnswer.Clear();
                currentAnswerParts = currentAnswer.Split(' ');
                for (int i = 0; i < currentAnswerParts.Length; i++)
                {
                    wordsInAnswer.Add(currentAnswerParts[i]);
                }

                string correctAnswer = string.Empty;

                for (int i = 0; i < correctAnswerParts.Length; i++)
                {
                    if (i == correctAnswerParts.Length - 1)
                    {
                        correctAnswer += correctAnswerParts[i];
                    }

                    else
                    {
                        correctAnswer += correctAnswerParts[i] + " ";
                    }
                }

                if (currentAnswer.Equals(correctAnswer))
                {
                    disabledAnswerWords = DisabledWords("disabled");
                    answerMessage = @"\images\answers\correctanswer.gif";
                    correctAnswerCounter++;
                    contentButton = "Продължи напред!";
                    totalCoins += currentGame.CoinsGiven;
                    totalPoints += currentGame.PointsGiven;
                    currentStudent.Coins = totalCoins;
                    currentStudent.Points = totalPoints;
                    context.Students.Update(currentStudent);
                    context.SaveChanges();
                    hiddenContentButton = string.Empty;
                    bossHealthLeft = bossHealthLeft - 4;
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                    StudentGame stGame = (from sg in context.StudentGames
                                          where sg.GameId == currentGame.Id
                                          where sg.StudentId == currentStudent.Id
                                          select sg).FirstOrDefault();
                    stGame.IsCorrect = true;
                    context.StudentGames.Update(stGame);
                    context.SaveChanges();
                    disabledClue = "disabled";
                    disabledCheckButton = "disabled";
                    ExtraLifeChecker();
                }
            }

            else
            {
                disabledClue = "disabled";
            }

            return RedirectToAction("Balloons");
        }
        #endregion

        #region Quiz
        public IActionResult Quiz()
        {
            if (!isGameCreated)
            {
                answerMessage = string.Empty;
                currentGameType = 4;
                currentGame = ChooseQuestion(currentStudent);
                currentAnswer = string.Empty;
                isGameCreated = true;
                strikes = 0;
                hiddenContentButton = "hidden";
                livesLeft = currentStudent.Lives;
                totalCoins = currentStudent.Coins;
                totalPoints = currentStudent.Points;

                image = @"\images\questions\" + currentGame.ImagePath;

                if (correctAnswerCounter == 0)
                {
                    currentPlayerHealth = @"\images\rockman\rockmanenergyrefill0.gif";
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + "energyrefill.gif";
                }

                disabledClue = string.Empty;

                shuffledAnswers = GenerateQuizAnswers(currentGame);
                buttonColours = GenerateButtonColours();
                disabledQuizAnswers = ManageQuizAnswers(string.Empty);
                hiddenQuizAnswers = ManageQuizAnswers(string.Empty);
            }

            ViewBag.Message = answerMessage;
            ViewBag.ContentButton = contentButton;
            ViewBag.HiddenContentButton = hiddenContentButton;
            ViewBag.CurrentAnswer = currentAnswer;
            ViewBag.Coins = totalCoins;
            ViewBag.Points = totalPoints;
            ViewBag.Lives = livesLeft;
            ViewBag.CurrentBossImage = @"\images\" + currentBoss + "\\" + currentBoss + ".png";
            ViewBag.CurrentBossHealth = currentBossHealth;
            ViewBag.CurrentPlayerHealth = currentPlayerHealth;
            ViewBag.DisabledClue = disabledClue;

            ViewBag.Image = image;
            ViewBag.ShuffledAnswers = shuffledAnswers;
            ViewBag.ButtonColours = buttonColours;
            ViewBag.DisabledQuizAnswers = disabledQuizAnswers;
            ViewBag.HiddenQuizAnswers = hiddenQuizAnswers;

            return View(currentGame);
        }

        private List<string> ManageQuizAnswers(string command)
        {
            List<string> quizAnswers = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                quizAnswers.Add(command);
            }

            return quizAnswers;
        }

        private List<string> GenerateButtonColours()
        {
            List<string> colours = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                colours.Add("btn-default");
            }

            return colours;
        }

        private List<string> GenerateQuizAnswers(Game game)
        {
            List<string> shuffledAnswers = new List<string>();
            Dictionary<int, string> answerPositions = new Dictionary<int, string>();
            string[] answers = game.ExtraInfo.Split("  ");
            List<int> answerIndexes = new List<int>();

            for (int i = 0; i < answers.Length; i++)
            {
                answerPositions.Add(i, answers[i]);
            }

            for (int i = 0; i < answers.Length; i++)
            {
                while (true)
                {
                    Random random = new Random();
                    int index = random.Next(answers.Length);
                    if (!answerIndexes.Contains(index))
                    {
                        answerIndexes.Add(index);
                        break;
                    }
                }
            }

            for (int i = 0; i < answerIndexes.Count; i++)
            {
                shuffledAnswers.Add(answerPositions[answerIndexes[i]]);
            }

            return shuffledAnswers;
        }

        public IActionResult CheckAnswer(string givenAnswer)
        {
            if (givenAnswer.Equals(currentGame.CorrectAnswer))
            {
                buttonColours[shuffledAnswers.IndexOf(currentGame.CorrectAnswer)] = "btn-success";
                correctAnswerCounter++;

                answerMessage = @"\images\answers\correctanswer.gif";
                if (!currentBoss.Equals("wily") && correctAnswerCounter == 8)
                {
                    contentButton = "Завърши нивото!";
                }

                else
                {
                    contentButton = "Продължи напред!";
                }

                totalCoins += currentGame.CoinsGiven;
                totalPoints += currentGame.PointsGiven;
                currentStudent.Coins = totalCoins;
                currentStudent.Points = totalPoints;
                context.Students.Update(currentStudent);
                context.SaveChanges();
                hiddenContentButton = string.Empty;
                bossHealthLeft = bossHealthLeft - 2;
                currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                StudentGame stGame = (from sg in context.StudentGames
                                      where sg.GameId == currentGame.Id
                                      where sg.StudentId == currentStudent.Id
                                      select sg).FirstOrDefault();
                stGame.IsCorrect = true;
                context.StudentGames.Update(stGame);
                context.SaveChanges();
                disabledClue = "disabled";
                disabledQuizAnswers = ManageQuizAnswers("disabled");
                ExtraLifeChecker();
            }

            else
            {
                strikes++;

                playerHealthLeft = playerHealthLeft - 28;
                currentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";

                answerMessage = @"\images\answers\wronganswer.gif";
                livesLeft = livesLeft - 1;
                currentStudent.Lives = livesLeft;
                context.Students.Update(currentStudent);
                context.SaveChanges();

                if (livesLeft != 0)
                {
                    contentButton = "Върни се обратно!";
                }

                else
                {
                    contentButton = "Избери нов противник!";
                }

                hiddenContentButton = string.Empty;
                disabledClue = "disabled";

                buttonColours[shuffledAnswers.IndexOf(givenAnswer)] = "btn-danger";
                buttonColours[shuffledAnswers.IndexOf(currentGame.CorrectAnswer)] = "btn-success";
                disabledQuizAnswers = ManageQuizAnswers("disabled");
            }

            return RedirectToAction("Quiz", currentGame);
        }

        public IActionResult EliminateAnswer()
        {
            if (currentStudent.Coins >= 30)
            {
                disabledClue = "disabled";
                totalCoins = totalCoins - 30;
                currentStudent.Coins = totalCoins;
                context.Students.Update(currentStudent);
                context.SaveChanges();

                while (true)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(shuffledAnswers.Count);
                    int correctIndex = shuffledAnswers.IndexOf(currentGame.CorrectAnswer);
                    if (randomIndex != correctIndex)
                    {
                        hiddenQuizAnswers[randomIndex] = "hidden";
                        break;
                    }
                }
            }

            else
            {
                disabledClue = "disabled";
            }

            return RedirectToAction("Quiz");
        }
        #endregion

        #region Cipher
        public IActionResult Cipher()
        {
            if (!isGameCreated)
            {
                answerMessage = string.Empty;
                currentGameType = 5;
                currentGame = ChooseQuestion(currentStudent);
                currentAnswer = string.Empty;
                isGameCreated = true;
                strikes = 0;
                hiddenContentButton = "hidden";
                livesLeft = currentStudent.Lives;
                totalCoins = currentStudent.Coins;
                totalPoints = currentStudent.Points;

                if (correctAnswerCounter == 0)
                {
                    currentPlayerHealth = @"\images\rockman\rockmanenergyrefill0.gif";
                    currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + "energyrefill.gif";
                }

                disabledClue = string.Empty;
                disabledCheckButton = string.Empty;

                lettersInAnswer = GenerateAlphaLetters(currentGame);
                counters = DefineCounters(currentGame);
                alphabeticalLetters = PopulateAlphabeticalLetters();
                clues = new List<string>();
                disabledCipherButtons = DisableCipherButtons(string.Empty);
                alphabetImage = string.Empty;
            }


            ViewBag.Message = answerMessage;
            ViewBag.ContentButton = contentButton;
            ViewBag.HiddenContentButton = hiddenContentButton;
            ViewBag.CurrentAnswer = currentAnswer;
            ViewBag.Coins = totalCoins;
            ViewBag.Points = totalPoints;
            ViewBag.Lives = livesLeft;
            ViewBag.CurrentBossImage = @"\images\" + currentBoss + "\\" + currentBoss + ".png";
            ViewBag.CurrentBossHealth = currentBossHealth;
            ViewBag.CurrentPlayerHealth = currentPlayerHealth;
            ViewBag.DisabledClue = disabledClue;
            ViewBag.DisabledCheckButton = disabledCheckButton;
            ViewBag.Letters = lettersInAnswer;
            ViewBag.Clues = clues;
            ViewBag.DisabledCipherButtons = disabledCipherButtons;
            ViewBag.AlphabetImage = alphabetImage;

            return View(currentGame);
        }

        private List<string> DisableCipherButtons(string command)
        {
            List<string> disabledCipherBtns = new List<string>();

            for (int i = 0; i < currentGame.CorrectAnswer.Length * 2; i++)
            {
                disabledCipherBtns.Add(command);
            }

            return disabledCipherBtns;
        }

        public IActionResult GiveClue()
        {
            if (currentStudent.Coins >= 30)
            {
                disabledClue = "disabled";
                totalCoins = totalCoins - 30;
                currentStudent.Coins = totalCoins;
                context.Students.Update(currentStudent);
                context.SaveChanges();

                Regex pattern = new Regex(@"[0-9]\.\s[0-9]+\-([а-я]|\s)+\.");
                MatchCollection matches = pattern.Matches(currentGame.ExtraInfo);

                foreach (var m in matches)
                {
                    clues.Add(m.ToString());
                }

                alphabetImage = @"\images\cipher\alphabet.gif";
            }

            else
            {
                disabledClue = "disabled";
            }

            return RedirectToAction("Cipher");
        }

        private Dictionary<int, char> PopulateAlphabeticalLetters()
        {
            Dictionary<int, char> letters = new Dictionary<int, char>();
            string alphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";

            for (int i = 0; i < alphabet.Length; i++)
            {
                letters.Add(i, alphabet[i]);
            }

            return letters;
        }

        private List<int> DefineCounters(Game game)
        {
            List<int> counters = new List<int>();
            int counter = 0;
            for (int i = 0; i < game.CorrectAnswer.Length; i++)
            {
                counters.Add(counter);
            }

            return counters;
        }

        private List<char> GenerateAlphaLetters(Game game)
        {
            List<char> letters = new List<char>();

            for (int i = 0; i < game.CorrectAnswer.Length; i++)
            {
                letters.Add('А');
            }

            return letters;
        }

        public IActionResult ChangeLetterUp(int letterPosition)
        {
            if (counters[letterPosition] == 29)
            {
                counters[letterPosition] = 0;
                lettersInAnswer[letterPosition] = alphabeticalLetters[counters[letterPosition]];
            }

            else
            {
                counters[letterPosition]++;
                lettersInAnswer[letterPosition] = alphabeticalLetters[counters[letterPosition]];
            }

            return RedirectToAction("Cipher", currentGame);
        }

        public IActionResult ChangeLetterDown(int letterPosition)
        {
            if (counters[letterPosition] == 0)
            {
                counters[letterPosition] = 29;
                lettersInAnswer[letterPosition] = alphabeticalLetters[counters[letterPosition]];
            }

            else
            {
                counters[letterPosition]--;
                lettersInAnswer[letterPosition] = alphabeticalLetters[counters[letterPosition]];
            }

            return RedirectToAction("Cipher", currentGame);
        }

        public IActionResult Unlock()
        {
            currentAnswer = string.Empty;
            for (int i = 0; i < lettersInAnswer.Count; i++)
            {
                currentAnswer += lettersInAnswer[i];
            }

            if (currentAnswer.Equals(currentGame.CorrectAnswer))
            {
                answerMessage = @"\images\answers\correctanswer.gif";
                correctAnswerCounter++;
                contentButton = "Завърши играта!";
                totalCoins += currentGame.CoinsGiven;
                totalPoints += currentGame.PointsGiven;
                currentStudent.Coins = totalCoins;
                currentStudent.Points = totalPoints;
                context.Students.Update(currentStudent);
                context.SaveChanges();
                hiddenContentButton = string.Empty;
                bossHealthLeft = bossHealthLeft - 2;
                currentBossHealth = @"\images\" + currentBoss + "\\" + currentBoss + " " + bossHealthLeft + ".png";
                StudentGame stGame = (from sg in context.StudentGames
                                      where sg.GameId == currentGame.Id
                                      where sg.StudentId == currentStudent.Id
                                      select sg).FirstOrDefault();
                stGame.IsCorrect = true;
                context.StudentGames.Update(stGame);
                context.SaveChanges();
                disabledClue = "disabled";
                disabledCheckButton = "disabled";
                disabledCipherButtons = DisableCipherButtons("disabled");
                ExtraLifeChecker();
            }

            else
            {
                strikes++;
                if (strikes == 2)
                {
                    playerHealthLeft = playerHealthLeft - 10;
                    currentPlayerHealth = ViewBag.CurrentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }

                else
                {
                    playerHealthLeft = playerHealthLeft - 9;
                    currentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
                }

                if (playerHealthLeft == 0)
                {
                    answerMessage = @"\images\answers\wronganswer.gif";
                    disabledCheckButton = "disabled";
                    livesLeft = livesLeft - 1;
                    currentStudent.Lives = livesLeft;
                    context.Students.Update(currentStudent);
                    context.SaveChanges();

                    if (livesLeft != 0)
                    {
                        contentButton = "Върни се обратно!";
                    }

                    else
                    {
                        contentButton = "Избери нов противник!";
                    }

                    hiddenContentButton = string.Empty;
                    disabledClue = "disabled";
                    disabledCipherButtons = DisableCipherButtons("disabled");
                }
            }

            return RedirectToAction("Cipher", currentGame);
        }

        public IActionResult ResetForm()
        {
            isGameCreated = false;

            if (playerHealthLeft == 28)
            {
                currentPlayerHealth = @"\images\rockman\rockman " + playerHealthLeft + ".png";
            }

            else
            {
                currentPlayerHealth = @"\images\rockman\rockmanenergyrefill" + playerHealthLeft + ".gif";
                playerHealthLeft = 28;
            }

            string destiny = string.Empty;

            switch (contentButton)
            {
                case "Върни се обратно!":
                    {
                        chosenPath = @"\images\pyramid\" + pathChoicesLog[pathCounterIndex] + ".jpg";
                        canAddNewPath = false;
                        destiny = "LoadStage";
                        break;
                    }

                case "Избери нов противник!":
                    {
                        arePathsInitialized = false;
                        correctAnswerCounter = 0;

                        currentStudent.Lives = 3;
                        currentStudent.Coins = 50;
                        currentStudent.Points = 0;
                        context.Students.Update(currentStudent);
                        context.SaveChanges();

                        ResetQuestions();
                        extraLifePoints = 0;

                        destiny = "BossSelect";
                        break;
                    }

                case "Продължи напред!":
                    {
                        pathCounterIndex++;
                        chosenPath = @"\images\pyramid\" + pathChoicesLog[pathCounterIndex] + ".jpg";
                        canAddNewPath = true;
                        destiny = "LoadStage";
                        break;
                    }

                case "Завърши нивото!":
                    {
                        arePathsInitialized = false;

                        int bossIndex = bosses.IndexOf(currentBoss);
                        bossesImages[bossIndex] = @"\images\" + currentBoss + "\\" + currentBoss + " defeated.png";                      
                        /*int defeatedBossesCounter = 0;
                        for (int i = 0; i < bossesImages.Count; i++)
                        {
                            if (!bossesImages[i].Contains("not"))
                            {
                                defeatedBossesCounter++;
                            }
                        }

                        if (defeatedBossesCounter == 2)
                        {
                            bosses.Insert(1, "wily");
                            bossesImages.Insert(1, @"\images\wily\wily not defeated.png");
                        }
                        */
                        correctAnswerCounter = 0;
                        context.Students.Update(currentStudent);
                        context.SaveChanges();

                        ResetQuestions();
                        extraLifePoints = 0;
                        destiny = "BossSelect";
                        break;
                    }

                case "Завърши играта!":
                    {
                        arePathsInitialized = false;
                        int bossIndex = bosses.IndexOf(currentBoss);
                        bossesImages[bossIndex] = @"\images\" + currentBoss + "\\" + currentBoss + " defeated.png";
                        destiny = "BossSelect";
                        correctAnswerCounter = 0;
                        context.Students.Update(currentStudent);
                        context.SaveChanges();
                        ResetQuestions();
                        extraLifePoints = 0;
                      
                        break;
                    }
            }

            return RedirectToAction(destiny, currentStudent);
        }

        private void ResetQuestions()
        {
            Dictionary<string, int> bossSubjects = new Dictionary<string, int>();
            bossSubjects.Add("elecman", 1);
            bossSubjects.Add("iceman", 2);
            bossSubjects.Add("wily", 3);

            int subjectId = bossSubjects[currentBoss];
            List<StudentGame> stGames = new List<StudentGame>();
            if (subjectId != 3)
            {
                stGames = (from sg in context.StudentGames
                           join game in context.Games on sg.GameId equals game.Id
                           where sg.StudentId.Equals(currentStudent.Id)
                           where game.SubjectId.Equals(subjectId)
                           select sg).ToList();
            }

            else
            {
                stGames = (from sg in context.StudentGames
                           join game in context.Games on sg.GameId equals game.Id
                           where sg.StudentId.Equals(currentStudent.Id)
                           select sg).ToList();
            }

            for (int i = 0; i < stGames.Count; i++)
            {
                context.StudentGames.Remove(stGames[i]);
                context.SaveChanges();
            }

        }
        #endregion

        private void ExtraLifeChecker()
        {
            extraLifePoints += currentGame.PointsGiven;

            if(extraLifePoints >= 100)
            {
                extraLifePoints -= 100;
                livesLeft++;
                currentStudent.Lives = livesLeft;
                context.Students.Update(currentStudent);
                context.SaveChanges();
            }
        }

        public IActionResult BackToMainMenu()
        {
            areBossesCreated = false;
            return RedirectToAction("Index", currentStudent);
        }

        public IActionResult LogOut()
        {
            return RedirectToAction("Index", "Main");
        }
    }
}
