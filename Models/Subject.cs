using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalWebGame.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string SubjectName { get; set; }

        public ICollection<Game> Games { get; set; }

    }
}
