using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalWebGame.Models
{
    public class ResourceTeacher : User
    {        
        public ICollection<Student> Students { get; set; }
    }
}
