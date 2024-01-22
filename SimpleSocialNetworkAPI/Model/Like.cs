using System.ComponentModel.DataAnnotations;

namespace SimpleSocialNetworkAPI.Model
{
    public class Like
    {
        public int Id { get; set; }
        [Required]
        public ApplicationUser User { get; set; }
        public Post? Post { get; set; }
    }
}
