using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EtsyRobot.Storage.Model
{

    public enum GameType
    {
        [Display(Name = "Favorite")]
        Fave = 0,

        [Display(Name = "Other")]
        Other = 1,
    }

    public class GameTypeLookup
    {
        public string Name { get; set; }
        public GameType Value { get; set; }        
    }

    public class Game
    {
        public Game()
        {
            Created = DateTime.UtcNow;
        }
        [Key]
        public int ID { get; set; }

        [Required, StringLength(2048)]
        public string Url { get; set; }
        public UInt16 Priority { get; set; }
        [Required, System.ComponentModel.DefaultValue(GameType.Fave)]
        public GameType GameType { get; set; }
        [Required, StringLength(1024), System.ComponentModel.DefaultValue("")]
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public static string Name { get; private set; }

        public static IEnumerable<Object> getGameTypeLookUp()
        {
            return Enum.GetNames(typeof(GameType)).Select(o => new { Name = o, Value = (GameType)(Enum.Parse(typeof(GameType), o)) });
        }

        public static IList<GameTypeLookup> getGameTypeLookUp2()
        {
            return Enum.GetNames(typeof(GameType))
                .Select(o => new GameTypeLookup { Name = o, Value = (GameType)(Enum.Parse(typeof(GameType), o)) }).ToList<GameTypeLookup>();
        }

    }
}
