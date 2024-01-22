using SimpleSocialNetworkAPI.Model.Enum;
using System.ComponentModel.DataAnnotations;

namespace SimpleSocialNetworkAPI.Model
{
    public class Friendship
    {
        public int Id { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public ApplicationUser? Sender { get; set; }
        public string? SenderId { get; set; }
        public ApplicationUser? Reciver { get; set; }
        public string? ReciverId { get; set; }
    }
}
