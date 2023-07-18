using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using web_app.Models;

namespace web_app.Core.ViewModel
{
    public class EditUserViewModel
    {
        public CustomUser User { get; set; }

        public IList<SelectListItem> Roles { get; set; }
    }
}
