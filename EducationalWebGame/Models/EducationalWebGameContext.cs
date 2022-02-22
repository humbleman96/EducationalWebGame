using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalWebGame.Models
{
    public class EducationalWebGameContext : DbContext
    {
        public EducationalWebGameContext(DbContextOptions<EducationalWebGameContext> options) : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<ResourceTeacher> ResourceTeachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentGame> StudentGames { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           /* if (System.Diagnostics.Debugger.IsAttached == false)
            {
                System.Diagnostics.Debugger.Launch();
            }
            */
           
            int counter = 1;
            modelBuilder.Entity<StudentGame>().HasKey(sg => new { sg.StudentId, sg.GameId });

            modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = 1, SubjectName = "Човекът и природата" },
            new Subject { Id = 2, SubjectName = "Човекът и обществото" });


            modelBuilder.Entity<GameType>().HasData(
            new GameType { Id = 1, GameTypeName = "Бесеница"},
            new GameType { Id = 2, GameTypeName = "Подреди думата"},
            new GameType { Id = 3, GameTypeName = "Балони"},
            new GameType { Id = 4, GameTypeName = "Викторина"},
            new GameType { Id = 5, GameTypeName = "Шифър"}
            );

            string[] bes = File.ReadAllLines(@"wwwroot\txt files\Бесеница.txt");
            string[] constrWord = File.ReadAllLines(@"wwwroot\txt files\Подреди думата.txt");
            string[] balloons = File.ReadAllLines(@"wwwroot\txt files\Балони.txt");
            string[] quiz = File.ReadAllLines(@"wwwroot\txt files\Викторина.txt");
            string[] cipher = File.ReadAllLines(@"wwwroot\txt files\Шифър.txt");

            for (int i = 0; i < bes.Length; i = i+6)
            {               
                modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = counter,
                    Question = bes[i],
                    CorrectAnswer = bes[i + 1],
                    CoinsGiven = UInt32.Parse(bes[i + 2]),
                    PointsGiven = UInt32.Parse(bes[i + 3]),
                    GameTypeId = 1,
                    SubjectId = int.Parse(bes[i + 4])
                });
                counter++;
            }

            for (int i = 0; i < constrWord.Length; i = i + 7)
            {
                modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = counter,
                    Question = constrWord[i],
                    CorrectAnswer = constrWord[i + 1],
                    ExtraInfo = constrWord[i + 2],
                    CoinsGiven = UInt32.Parse(constrWord[i + 3]),
                    PointsGiven = UInt32.Parse(constrWord[i + 4]),
                    GameTypeId = 2,
                    SubjectId = int.Parse(constrWord[i + 5])
                });
                counter++;
            }

            for (int i = 0; i < balloons.Length; i = i + 6)
            {             
                modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = counter,
                    Question = balloons[i],
                    CorrectAnswer = balloons[i + 1],
                    CoinsGiven = UInt32.Parse(balloons[i + 2]),
                    PointsGiven = UInt32.Parse(balloons[i + 3]),
                    GameTypeId = 3,
                    SubjectId = int.Parse(balloons[i+4])
                });
                counter++;
            }

            for (int i = 0; i < quiz.Length; i = i + 8)
            {
                modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = counter,
                    Question = quiz[i],
                    CorrectAnswer = quiz[i + 1],
                    ExtraInfo = quiz[i + 2],
                    ImagePath = quiz[i + 3],
                    CoinsGiven = UInt32.Parse(quiz[i + 4]),
                    PointsGiven = UInt32.Parse(quiz[i + 5]),
                    GameTypeId = 4,
                    SubjectId = int.Parse(quiz[i + 6])
                });
                counter++;
            }

            for (int i = 0; i < cipher.Length; i = i + 7)
            {            
                modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = counter,
                    Question = cipher[i],
                    CorrectAnswer = cipher[i + 1],
                    ExtraInfo = cipher[i + 2],
                    CoinsGiven = UInt32.Parse(cipher[i + 3]),
                    PointsGiven = UInt32.Parse(cipher[i + 4]),
                    GameTypeId = 5,
                    SubjectId = Int32.Parse(cipher[i + 5])
                });
                counter++;
            }

        }
    }
}
