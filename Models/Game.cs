using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalWebGame.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Question { get; set; }

        [Required, MaxLength(255)]
        public string CorrectAnswer { get; set; }

        [MaxLength(255)]
        public string ExtraInfo { get; set; }

        [MaxLength(255)]
        public string ImagePath { get; set; }

        [Required]
        public uint CoinsGiven { get; set; }

        [Required]
        public uint PointsGiven { get; set; }

        public int? GameTypeId { get; set; }
        public GameType GameType { get; set; }

        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

        public ICollection<StudentGame> StudentGames { get; set; }
    }
}
