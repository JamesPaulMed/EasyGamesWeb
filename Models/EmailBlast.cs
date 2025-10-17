using System.ComponentModel.DataAnnotations;

namespace EasyGamesWeb.Models
{
    public class EmailBlast
    {
        public EmailSegment Segment { get; set; }

        [Required]
        public string Subject { get; set; } = "";

        [Required]
        public string BodyHtml { get; set; } = "";

    }
}
