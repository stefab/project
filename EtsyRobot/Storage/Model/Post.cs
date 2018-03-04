using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtsyRobot.Storage.Model
{
    public class Post
    {
        public Post(string post)
        {
            PostAndUrl = post;
        }
        public Post()
        {
            PostAndUrl = "";
            Active = true;
        }
        [Key]
        public int ID { get; set; }

        [Required, StringLength(2048)]
        public string PostAndUrl { get; set; }
        [Required]
        public int Priority { get; set; } = 1;
        [Required]
        public uint Priority2 { get; set; } = 1;
        [Required]
        public bool Active { get; set; } = true;
    }
}
