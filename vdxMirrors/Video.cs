using System;
using System.Collections.Specialized;
using vdxNet;
using Newtonsoft.Json;
using System.IO;

namespace vdxMirrors
{
	public class Video
	{
		public int Id { get; protected set; }
		public string Title { get; protected set; }
		public string Description { get; protected set; }
		public int UserId { get; protected set; }


		public Video(int id, string cacheAddr)
		{
			this.Id = id;

			NameValueCollection prms = new NameValueCollection();
			prms.Add("id", id.ToString());
			NameValueCollection headers = new NameValueCollection();
			headers.Add("X-Requested-With", "xmlhttprequest");

			string response = Net.sendPost(cacheAddr + "/ajax/videoInfo.php", prms, headers);
			JsonTextReader reader = new JsonTextReader(new StringReader(response));
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonToken.PropertyName:
						string propertyName = reader.ReadAsString();
						reader.Read();
						switch (propertyName.ToLower())
						{
							case "id":
								this.Id = reader.ReadAsInt32().Value;
								break;
							case "title":
								this.Title = reader.ReadAsString();
								break;
							case "description":
								this.Description = reader.ReadAsString();
								break;
							case "user_id":
								this.UserId = reader.ReadAsInt32().Value;
								break;
						}
						break;
				}
			}

		}

	}
}
