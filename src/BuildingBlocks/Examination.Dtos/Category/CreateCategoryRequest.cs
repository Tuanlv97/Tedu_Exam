using System.ComponentModel.DataAnnotations;

namespace Examination.Dtos.Category
{
    public class CreateCategoryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string UrlPath { get; set; }
    }
}
