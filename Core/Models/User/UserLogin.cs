using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.User
{
    [NotMapped]
    public class UserLogin
    {
        [EmailAddress]
        [Required(ErrorMessage = "O e-mail é obrigatorio!")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "A senha é obrigatorio!")]
        public string Password { get; set; }

    }
}
