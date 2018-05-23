using System.ComponentModel.DataAnnotations;

namespace Scorpio.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}