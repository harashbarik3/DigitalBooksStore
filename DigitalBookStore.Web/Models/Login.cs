
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DigitalBookStore.Web.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Please enter Username")]
        [Display(Name = "UserName")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
