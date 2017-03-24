using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;
using vdxNet;

namespace vdxMirrors
{
	/// <summary>
	/// Represents a Cache adn it's tacked videos.
	/// </summary>
	public class Cache
	{
		private string _title;

		public string address { get; }
		public int version { get; } = 1;
		public int id { get; } = 1;
		public string title
		{
			get
			{
				if (string.IsNullOrEmpty(_title))
				{
					return address;
				}
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		protected List<int> videoIds { get; set; }
		protected List<Video> videos { get; set; } 


		public Cache(string address) : this(address, true)
		{

		}

		public Cache(string address, bool autoCheck)
		{
			this.videoIds = new List<int>();
			this.address = address;
			if (autoCheck)
			{
				NameValueCollection headers = new NameValueCollection();
				headers.Add("X-Requested-With", "xmlhttprequest");
				string response = Net.sendPost(address + "/ajax/videoList.php", null, headers);
				Console.WriteLine("Cache::Response: " + response);
				try
				{
					JsonTextReader reader = new JsonTextReader(new StringReader(response));
					while (reader.Read())
					{
						if (reader.ValueType == typeof(string))
						{
							string r =(string) reader.Value;
							int valx = Int32.Parse(r);
							videoIds.Add(valx);
							Console.WriteLine("Video: " + valx);
						}
						else if (reader.ValueType == typeof(int))
						{
							videoIds.Add((int) reader.Value);
							Console.WriteLine("Video: " + (int)reader.Value);
						}

					}

				}
				catch (JsonReaderException ex)
				{
					Console.WriteLine(ex);
				}
				this.videos = new List<Video>(videoIds.Count);
			}
		}

		/// <summary>
		/// Gets the  <see cref="T:vdxMirrors.Video"/> with the specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public Video getVideo(int id)
		{
			if (videos[id] == null)
			{
				videos[id] = new Video(id, address);
			}
			return videos[id];
		}

		/// <summary>
		/// Gets the <see cref="T:vdxMirrors.Video"/> with the specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public Video this[int id]
		{
			get
			{
				return getVideo(id);
			}
		}

		/// <summary>
		/// Number of videos collected by this cache
		/// </summary>
		public int Length { get { return videoIds.Count; } }

		/// <summary>
		/// Number of videos collected by this cache
		/// </summary>
		public int Count { get { return Length; } }
	}
}
