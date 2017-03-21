using System;
namespace vdxMirrors
{
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

		public Cache(string address)
		{
			this.address = address;
		}


	}
}
