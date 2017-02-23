using System;
using Newtonsoft.Json;

namespace BigDays
{
	public class PixabayRequestModel
	{
		[JsonProperty("key")]
		public string Key {get; set;}
		[JsonProperty("q")]
		public string Q { get; set; }


	}
}
