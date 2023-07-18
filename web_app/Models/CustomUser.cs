using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace web_app.Models
{
    public class CustomUser : IdentityUser
    {
        [Display(Name = "Name and Surname")] public string FullName { get; set; }
        [Display(Name = "Address")] public string Address { get; set; }
        [Display(Name = "Gender")] public string Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public List<Portfolio> Portfolios { get; set; } = new ();
        public List<Transaction> Transactions { get; set; } = new ();
    }

    public class ApplicationRole : IdentityRole
    {
    }
}
