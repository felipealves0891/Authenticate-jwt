using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.User
{
    [NotMapped]
    public class UserRegistration
    {
        [Required(ErrorMessage = "O nome é obrigatorio!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "O sobrenome é obrigatorio!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatorio!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatorio!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "A senhas não são iguais, valide a senha e tente novamente!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
