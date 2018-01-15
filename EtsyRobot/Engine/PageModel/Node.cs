using System;
using System.Drawing;

using Newtonsoft.Json;

namespace EtsyRobot.Engine.PageModel
{
	[JsonObject]
	public sealed class Node
	{
		[JsonProperty("p")]
		public string Selector { get; set; }

		[JsonProperty("l")]
		public Point Location { get; set; }

		[JsonProperty("s")]
		public Size Size { get; set; }

		[JsonProperty("u", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string ContentUrl { get; set; }
        
        [JsonProperty("dt")]
        public int data_id { get; set; }
	}

	[JsonObject]
	public sealed class AdZoneNode
	{
        [JsonProperty("d")]
        public bool Displayed { get; set; }
        [JsonProperty("w")]
        public int Width { get; set; }
        [JsonProperty("h")]
        public int Height { get; set; }
        [JsonProperty("sw")]
        public int SafeWidth { get; set; }
        [JsonProperty("sh")]
        public int SafeHeight { get; set; }
        [JsonProperty("a")]
        public string Advert { get; set; }
        [JsonProperty("div")]
        public string ta_divid { get; set; }
        [JsonProperty("dt")]
        public int data_id { get; set; }

        public string GetShortDescription()
        {
            return string.Format(@"advert: {0:s}, size: {1:d}x{2:d}, safe size: {3:d}x{4:d}",
                            Advert, Width, Height, SafeWidth, SafeHeight);
        }    
    }

}