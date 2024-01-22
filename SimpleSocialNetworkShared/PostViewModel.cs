using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace SimpleSocialNetworkShared
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? MediaPath { get; set; }
        public int LikeCount { get; set; }
        public ApplicationUserViewModel? User { get; set; }
        public List<CommentViewModel>? Comments { get; set; }
        [JsonIgnore]
        public IFormFile? MediaFile { get; set; }
    }
}
