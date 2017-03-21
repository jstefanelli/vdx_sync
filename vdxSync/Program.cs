using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using vdxNet;

namespace vdxSync
{
	class MainClass
	{
		protected string file;
		protected string ip;
		protected short port;
		protected bool hasError;
		protected int id;
		protected static char nextFolderChar;

		public MainClass()
		{
			port = 80;
			file = "";
			ip = "";
			id = -1;
			hasError = false;
			switch (System.Environment.OSVersion.Platform)
			{
				case PlatformID.Unix:
				case PlatformID.MacOSX:
				case (PlatformID) 128:
					nextFolderChar = '/';
					break;
				default:
					nextFolderChar = '\\';
					break;
			}
		}


		public static void Main(string[] args)
		{
			MainClass m = new MainClass();
			for (int i = 0; i < args.Length; i++)
			{
				string a = args[i];
				if (a.ToLower().Equals("--file"))
				{
					i++;
					m.file = args[i];
				}
				else if (a.ToLower().Equals("--ip"))
				{
					i++;
					m.ip = args[i];
				}
				else if (a.ToLower().Equals("--port"))
				{
					i++;
					m.port = short.Parse(args[i]);
				}
				else if (a.ToLower().Equals("--id"))
				{
					i++;
					m.id = int.Parse(args[i]);
				}
			}
			if (m.ip.Equals(""))
			{
				Console.WriteLine("Error: No ip address provided");
				m.hasError = true;
			}
			if (m.file.Equals(""))
			{
				Console.WriteLine("Error: No File Specified");
				m.hasError = true;
			}
			if (m.id == -1)
			{
				Console.WriteLine("Error: No ID Specified");
				m.hasError = true;
			}
			if (m.hasError)
			{
				Console.WriteLine("Usage: vdxSync --id \"id\" --file \"file\" --ip \"ip\" [--port \"port\"]");
				return;
			}
			if (!File.Exists(m.file))
			{
				Console.WriteLine("Error: File not found");
				return;
			}


			NameValueCollection headers = new NameValueCollection();
			headers.Add("X-Requested-With", "xmlhttprequest");

			bool u = Net.vdxUpload("http://" + m.ip + "/vdx_archive/ajax", m.file, 1024 * 1024,(uint) m.id, headers);

			if (u)
			{
				Console.WriteLine("Upload successful.");
			}
			else
			{
				Console.WriteLine("Upload Failed.");
			}
		}
	}
}
