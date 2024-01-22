using System.ComponentModel.DataAnnotations;

namespace SimpleSocialNetworkAPI.Model
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public ApplicationUser User { get; set; }
        public Post? Post { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
