using System;
namespace exam.Models.Account
{
    public class MyAccountViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string NewEmail { get; set; }
        public string Name { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
