using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleSocialNetworkAPI.Model
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        public ApplicationUser User { get; set; }
        public string? Content { get; set; }
        public string? MediaPath { get; set; }
        public List<Comment>? Comments { get; set; }
        [NotMapped]
        public IFormFile? MediaFile { get; set; }
    }
}
