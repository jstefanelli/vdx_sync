using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

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

		public static void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc, int offset = 0)
		{
			Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
			wr.ContentType = "multipart/form-data; boundary=" + boundary;
			wr.Method = "POST";
			wr.KeepAlive = true;
			wr.Headers.Add("X-Requested-With", "xmlhttprequest");
			wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

			Stream rs = wr.GetRequestStream();

			string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
			foreach (string key in nvc.Keys)
			{
				rs.Write(boundarybytes, 0, boundarybytes.Length);
				string formitem = string.Format(formdataTemplate, key, nvc[key]);
				byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
				rs.Write(formitembytes, 0, formitembytes.Length);
			}
			rs.Write(boundarybytes, 0, boundarybytes.Length);

			string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string header = string.Format(headerTemplate, paramName, file, contentType);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
			rs.Write(headerbytes, 0, headerbytes.Length);

			FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
			byte[] buffer = new byte[1024 * 1024];
			int bytesRead = 0;
			fileStream.Seek(offset, SeekOrigin.Begin);
			bytesRead = fileStream.Read(buffer, 0, buffer.Length);
			rs.Write(buffer, 0, bytesRead);


			byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
			rs.Write(trailer, 0, trailer.Length);
			rs.Close();

			WebResponse wresp = null;
			try
			{
				wresp = wr.GetResponse();
				Stream stream2 = wresp.GetResponseStream();
				StreamReader reader2 = new StreamReader(stream2);
				Console.WriteLine(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error uploading file", ex);
				if (wresp != null)
				{
					wresp.Close();
					wresp = null;
				}
			}
			finally
			{
				wr = null;
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
			FileStream f = File.OpenRead(m.file);
			Console.WriteLine("File Found. Length: " + f.Length);
			ulong parts = (ulong) ((f.Length / (1024 * 1024)) + ((f.Length % (1024 * 1024) > 0) ? 1 : 0));
			HttpWebRequest r = WebRequest.CreateHttp("http://" + m.ip + "/vdx_archive/ajax/upload_mk.php");
			r.Method = "POST";
			r.Headers.Add("X-Requested-With", "xmlhttprequest");
			r.ContentType = "application/x-www-form-urlencoded";
			using (var writer = new StreamWriter(r.GetRequestStream()))
			{
				Console.WriteLine("Starting with:\nName:\t\t" + m.file.Substring(m.file.LastIndexOf(nextFolderChar) + 1) + "\nParts:\t\t" + parts + "\nID:\t\t" + m.id);
				writer.Write("id=" + m.id + "&name=" + m.file.Substring(m.file.LastIndexOf(nextFolderChar) + 1) + "&n_parts=" + parts + "&part_size=" + 1024*1024);
				writer.Flush();
				writer.Close();
			}
			try
			{
				WebResponse resp = r.GetResponse();
				StreamReader rr = new StreamReader(resp.GetResponseStream());
				String s = rr.ReadToEnd();
				if (s.ToLower().Equals("ok"))
				{
					Console.WriteLine("success.");
				}
				else
				{
					Console.WriteLine("Error: " + s);
				}
				rr.Close();
				for (int i = 0; i < (int) parts; i++)
				{
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("name", m.file.Substring(m.file.LastIndexOf(nextFolderChar) + 1));
					nvc.Add("id", m.id.ToString());
					HttpUploadFile("http://" + m.ip + "/vdx_archive/ajax/upload_part.php", m.file, "file", "binary/octetstream", nvc, i * 1024 * 1024);
				}
				HttpWebRequest r2 = WebRequest.CreateHttp("http://" + m.ip + "/vdx_archive/ajax/upload_merge.php");
				r2.Method = "POST";
				r2.Headers.Add("X-Requested-With", "xmlhttprequest");
				r2.ContentType = "application/x-www-form-urlencoded";
				using (var writer = new StreamWriter(r2.GetRequestStream()))
				{
					Console.WriteLine("Merging with:\nName:\t\t" + m.file.Substring(m.file.LastIndexOf(nextFolderChar) + 1) + "\nID:\t\t" + m.id);
					writer.Write("id=" + m.id + "&name=" + m.file.Substring(m.file.LastIndexOf(nextFolderChar) + 1));
					writer.Flush();
					writer.Close();
				}
				WebResponse resp2 = r2.GetResponse();
				StreamReader rr2 = new StreamReader(resp2.GetResponseStream());
				String ss = rr2.ReadToEnd();
				if (ss.ToLower().Equals("ok"))
				{
					Console.WriteLine("Merge success.");
				}
				else
				{
					Console.WriteLine("Error: " + s);
				}
				rr2.Close();
			}
			catch (WebException ex)
			{
				Console.WriteLine("Exception: " + ex.ToString());
			}
		}
	}
}
