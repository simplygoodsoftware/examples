﻿using System.Collections.Generic;

namespace Bots.CopyFieldBot
{
	public class BotResponse
	{
			public int StatusCode { get; set; }
			public string Body { get; set; }
			public IDictionary<string, string> Headers { get; set; }
			public bool IsBase64Encoded { get; set; }
	}
}
