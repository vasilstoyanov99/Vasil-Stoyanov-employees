using System.ComponentModel.DataAnnotations;

namespace Vasil_Stoyanov_employees.Web.Models
{
    using Microsoft.AspNetCore.Http;

    public class FileFormModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
