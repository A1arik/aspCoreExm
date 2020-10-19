using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace exam.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime BirthDate { get; set; }
        public string Sex { get; set; }
        public List<ClientTraining> Trainings { get; set; }

        public AppUser()
        {
            Trainings = new List<ClientTraining>();
        }

    }

}
