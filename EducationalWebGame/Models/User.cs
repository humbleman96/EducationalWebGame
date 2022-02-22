using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalWebGame.Models
{
    public abstract class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        [Required, MaxLength(30)]
        public string FamilyName { get; set; }

        [Required, MaxLength(30)]
        public string UserName { get; set; }

        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; }
    }
}
