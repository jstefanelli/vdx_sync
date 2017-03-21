using System;
namespace vdxMirrors
{
	public class Mirror
	{
		private string _title;

		public string address { get; }
		public int version { get; } = 1;
		public int id { get; }
		public string title { 
			get {
				if (string.IsNullOrEmpty(_title))
				{
					return address;
				}
				return _title;
			} 
			set{
				_title = value;
			} 
		}

		public Mirror(string address, int id)
		{
			this.id = id;
			this.address = address;
		}
	}
}
