using System.ComponentModel.DataAnnotations;

namespace Adressboken.Models;
public class LoginViewModel
{
    [Required]
    public string? Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    //Vill Bengt ha lösenordet sparat?
    //public bool RememberMe { get; set; }

    
}