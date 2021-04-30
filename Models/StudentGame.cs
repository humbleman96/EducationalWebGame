using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalWebGame.Models
{
    public class StudentGame
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        public bool IsCorrect { get; set; }
    }
}
