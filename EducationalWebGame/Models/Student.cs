using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalWebGame.Models
{
    public class Student : User
    {
        [Required]
        public uint Coins { get; set; }

        [Required]
        public uint Points { get; set; }

        [Required]
        public uint Lives { get; set; }

        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int? ResourceTeacherId { get; set; }
        public ResourceTeacher ResourceTeacher { get; set; }

        public ICollection<StudentGame> StudentGames { get; set; }
    }
}
