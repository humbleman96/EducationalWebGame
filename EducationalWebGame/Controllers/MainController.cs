using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EducationalWebGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWebGame.Controllers
{
    public class MainController : Controller
    {
        private readonly EducationalWebGameContext context;

        private static string hiddenUser;
        private static bool isLogInfoInitialized = false;

        private static string hiddenUserName;
        private static string hiddenPassword;
        private static string hiddenRepeatedPassword;
        private static string message = string.Empty;
        private static bool isRegInfoInitialized = false;
        private static List<string> educationalTeachers;
        private static List<string> resourceTeachers;

        private static string permName;
        private static string permFamilyName;
        private static string permUserName;
        private static string permPassword;
        private static string permRepeatedPassword;

        public MainController(EducationalWebGameContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            if (!isLogInfoInitialized)
            {
                hiddenUser = "hidden";
                isLogInfoInitialized = true;
            }

            ViewBag.HiddenUser = hiddenUser;
            return View();
        }

        public IActionResult LoginUser(string userName, string password)
        {
            List<Student> students = context.Students.ToList();
            List<Teacher> educationalTeachers = context.Teachers.ToList();
            List<ResourceTeacher> resourceTeachers = context.ResourceTeachers.ToList();

            for (int i = 0; i < students.Count; i++)
            {
                if(students[i].UserName.Equals(userName) && 
                   PasswordHasher.VerifyPassword(password, students[i].Password))
                {
                    Student student = new Student();
                    student = students[i];
                    isLogInfoInitialized = false;
                    return RedirectToAction("Index", "Student", student);
                }

            }

            for (int i = 0; i < educationalTeachers.Count; i++)
            {
                if (educationalTeachers[i].UserName.Equals(userName) &&
                   PasswordHasher.VerifyPassword(password, educationalTeachers[i].Password))
                {
                    Teacher educationalTeacher = new Teacher();
                    educationalTeacher = educationalTeachers[i];
                    isLogInfoInitialized = false;
                    return RedirectToAction("Index", "Teacher", educationalTeacher);
                }

            }

            for (int i = 0; i < resourceTeachers.Count; i++)
            {
                if (resourceTeachers[i].UserName.Equals(userName) &&
                   PasswordHasher.VerifyPassword(password, resourceTeachers[i].Password))
                {
                    ResourceTeacher resourceTeacher = new ResourceTeacher();
                    resourceTeacher = resourceTeachers[i];
                    isLogInfoInitialized = false;
                    return RedirectToAction("Index", "Teacher", resourceTeacher);
                }

            }

            hiddenUser = string.Empty;
            return RedirectToAction("Index");
        }

        public IActionResult RegisterForm()
        {
            if (!isRegInfoInitialized)
            {
                hiddenUserName = "hidden";
                hiddenPassword = "hidden";
                hiddenRepeatedPassword = "hidden";
                isRegInfoInitialized = true;             
                permName = string.Empty;
                permFamilyName = string.Empty;            
                permUserName = string.Empty;
                permPassword = string.Empty;
                permRepeatedPassword = string.Empty;
            }

            ViewBag.HiddenUserName = hiddenUserName;
            ViewBag.HiddenPassword = hiddenPassword;
            ViewBag.HiddenRepeatedPassword = hiddenRepeatedPassword;
            ViewBag.Message = message;
            educationalTeachers = PopulateEducationalTeachers();
            resourceTeachers = PopulateResourceTeachers();
            ViewBag.EducationalTeachers = educationalTeachers;
            ViewBag.ResourceTeachers = resourceTeachers;

            ViewBag.Name = permName;
            ViewBag.FamilyName = permFamilyName;
            ViewBag.UserName = permUserName;
            ViewBag.Password = permPassword;
            ViewBag.RepeatedPassword = permRepeatedPassword;

            return View();
        }
        
        private List<string> PopulateResourceTeachers()
        {
            List<string> resourceTeachers = new List<string>();
            List<ResourceTeacher> dbResourceTeachers = context.ResourceTeachers.ToList();

            for (int i = 0; i < dbResourceTeachers.Count; i++)
            {
                resourceTeachers.Add(dbResourceTeachers[i].Name + " " +
                                     dbResourceTeachers[i].FamilyName + " (" +
                                     dbResourceTeachers[i].UserName + ")");
            }

            return resourceTeachers;
        }

        private List<string> PopulateEducationalTeachers()
        {
            List<string> educationalTeachers = new List<string>();
            List<Teacher> dbTeachers = context.Teachers.ToList();

            for (int i = 0; i < dbTeachers.Count; i++)
            {
                educationalTeachers.Add(dbTeachers[i].Name + " " +
                                     dbTeachers[i].FamilyName + " (" +
                                     dbTeachers[i].UserName + ")");
            }

            return educationalTeachers;
        }

        public IActionResult RegisterUser(string name, string familyName, string role, string educationalTeacher, string resourceTeacher, string userName, string password, string repeatedPassword)
        {
            bool isUserCreated = CheckExistingUser(userName);

            bool isUserNameCorrect = true;
            bool isPasswordCorrect = true;
            bool isRepeatedPasswordCorrect = true;

            if (!isUserCreated)
            {
                Regex userNameRegex = new Regex("^(?=.{6,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$");
                Regex passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");

                permName = name;
                permFamilyName = familyName;             
                permUserName = userName;
                permPassword = password;
                permRepeatedPassword = repeatedPassword;

                if (!userNameRegex.IsMatch(userName))
                {
                    hiddenUserName = string.Empty;
                    message = string.Empty;
                    isUserNameCorrect = false;
                }

                else
                {
                    hiddenUserName = "hidden";
                    isUserNameCorrect = true;
                }

                if (!passwordRegex.IsMatch(password))
                {
                    hiddenPassword = string.Empty;
                    message = string.Empty;
                    isPasswordCorrect = false;
                }

                else
                {
                    hiddenPassword = "hidden";
                    isPasswordCorrect = true;
                }

                if (!repeatedPassword.Equals(password))
                {
                    hiddenRepeatedPassword = string.Empty;
                    message = string.Empty;
                    isRepeatedPasswordCorrect = false;
                }

                else
                {
                    hiddenRepeatedPassword = "hidden";
                    isRepeatedPasswordCorrect = true;
                }


                if (isUserNameCorrect && isPasswordCorrect && isRepeatedPasswordCorrect)
                {
                    switch (role)
                    {
                        case "Ученик":
                            {
                                List<Teacher> eTeachers = context.Teachers.ToList();
                                List<ResourceTeacher> rTeachers = context.ResourceTeachers.ToList();
                                Teacher et = new Teacher();
                                ResourceTeacher rt = new ResourceTeacher();

                                for (int i = 0; i < eTeachers.Count; i++)
                                {
                                    if (educationalTeacher.Contains(eTeachers[i].UserName))
                                    {
                                        et = eTeachers[i];
                                        break;
                                    }
                                }


                                for (int i = 0; i < rTeachers.Count; i++)
                                {
                                    if (resourceTeacher.Contains(rTeachers[i].UserName))
                                    {
                                        rt = rTeachers[i];
                                        break;
                                    }
                                }

                                Student student = new Student();
                                student.Name = name;
                                student.FamilyName = familyName;
                                student.UserName = userName;
                                student.Password = PasswordHasher.CreateSaltedHash(password);
                                student.Coins = 50;
                                student.Points = 0;
                                student.Lives = 3;
                                student.ResourceTeacherId = rt.Id;
                                student.TeacherId = et.Id;
                                context.Students.Add(student);
                                context.SaveChanges();

                                message = "Успешна регистрация на ученик!";
                                isRegInfoInitialized = false;
                                break;
                            }

                        case "Обучаващ учител":
                            {
                                Teacher teacher = new Teacher();
                                teacher.Name = name;
                                teacher.FamilyName = familyName;
                                teacher.UserName = userName;
                                teacher.Password = PasswordHasher.CreateSaltedHash(password);
                                context.Teachers.Add(teacher);
                                context.SaveChanges();
                                message = "Успешна регистрация на обучаващ учител!";
                                isRegInfoInitialized = false;
                                break;
                            }

                        case "Ресурсен учител":
                            {
                                ResourceTeacher resTeacher = new ResourceTeacher();
                                resTeacher.Name = name;
                                resTeacher.FamilyName = familyName;
                                resTeacher.UserName = userName;
                                resTeacher.Password = PasswordHasher.CreateSaltedHash(password);
                                context.ResourceTeachers.Add(resTeacher);
                                context.SaveChanges();
                                message = "Успешна регистрация на ресурсен учител!";
                                isRegInfoInitialized = false;
                                break;
                            }
                    }
                }

            }

            else
            {
                message = "Вече има създаден такъв потребител!";
            }

            return RedirectToAction("RegisterForm");
        }

        private bool CheckExistingUser(string userName)
        {
            bool isUserCreated = false;
            List<Student> students = context.Students.ToList();
            List<Teacher> eTeachers = context.Teachers.ToList();
            List<ResourceTeacher> rTeachers = context.ResourceTeachers.ToList();

            for (int i = 0; i < students.Count; i++)
            {
                if (userName.Equals(students[i].UserName))
                {
                    isUserCreated = true;
                    break;
                }
            }

            for (int i = 0; i < eTeachers.Count; i++)
            {
                if (userName.Equals(eTeachers[i].UserName))
                {
                    isUserCreated = true;
                    break;
                }
            }

            for (int i = 0; i < rTeachers.Count; i++)
            {
                if (userName.Equals(rTeachers[i].UserName))
                {
                    isUserCreated = true;
                    break;
                }
            }

            return isUserCreated;
        }

        public IActionResult BackToLogin()
        {
            isRegInfoInitialized = false;
            isLogInfoInitialized = false;
            message = string.Empty;
            return RedirectToAction("Index");
        }
    }
}