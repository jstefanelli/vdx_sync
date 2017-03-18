using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace vdxNet
{
	public static class Net
	{
		/// <summary>
		/// Sends a generic Post Request to a HTTP server.
		/// </summary>
		/// <returns>Response from the server (EmptyString if it fails)</returns>
		/// <param name="address">Page Address.</param>
		/// <param name="post">Post Values.</param>
		/// <param name="headers">Headers (ex. X-Requested-With).</param>
		public static string sendPost(string address, NameValueCollection post, NameValueCollection headers = null)
		{
			string retVal = "";
			string postString = "";
			HttpWebRequest r = WebRequest.CreateHttp(address);
			r.Method = "POST";
			r.KeepAlive = true;
			if (headers != null)
			{
				foreach (DictionaryEntry entry in headers)
				{
					r.Headers.Add((string) entry.Key, (string) entry.Value);
				}
			}
			if (post != null)
			{
				for (int i = 0; i < post.Count; i++)
				{
					string key = post.GetKey(i);
					string val = post[i];
					postString += key + "=" + val;
					if (i != post.Count - 1)
					{
						postString += "&";
					}
				}
			}
			byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(postString);
			Stream requestStream = r.GetRequestStream();
			requestStream.Write(utf8Bytes, 0, utf8Bytes.Length);
			requestStream.Close();
			StreamReader responseStreamReader = new StreamReader(r.GetResponse().GetResponseStream());
			retVal = responseStreamReader.ReadToEnd();
			return retVal;
		}

		public struct UploadResult
		{
			public int id;
			public bool result;
			public string name;
		}

		/// <summary>
		/// Uploads a file in parts to a VDX-Like web server.
		/// </summary>
		/// <returns>The upload result (ID, success and name).</returns>
		/// <param name="uploadFolderAddress">Upload folder address.</param>
		/// <param name="filePath">File path.</param>
		/// <param name="partSize">Part size.</param>
		/// <param name="prms">Parameters (ex. Id, name for official VDX).</param>
		/// <param name="headers">Headers (ex. X-Requested-With).</param>
		public static UploadResult vdxUpload(string uploadFolderAddress, string filePath, uint partSize, NameValueCollection prms, NameValueCollection headers)
		{
			UploadResult res = new UploadResult();

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException();
			}
			FileStream f = File.OpenRead(filePath);
			if(f.

			return res;
		}
	}
}
