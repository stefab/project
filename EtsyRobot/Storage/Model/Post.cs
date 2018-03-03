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
        [Key]
        public int ID { get; set; }

        [Required, StringLength(2048)]
        public string PostAndUrl { get; set; }
        [Required, System.ComponentModel.DefaultValue(0)]
        public UInt16 Priority { get; set; }
        [Required, System.ComponentModel.DefaultValue(true)]
        public bool Active  { get; set; }
    }
}
