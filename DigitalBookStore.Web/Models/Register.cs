using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DigitalBookStore.Web.Models
{
    public class Register
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter FirstName")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter LastName")]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Please enter password")]
        //[DataType(DataType.Password)]
        //[StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        //[RegularExpression(@"^([a-zA-Z0-9@*#]{8,15})$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase      Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }

        public Guid RoleId { get; set; }

        public string? UserType { get; set; }

    }
}
