using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace vdxNet
{
	public static class Net
	{

        public static char nextFolderChar { get
            {
                switch (System.Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                    case PlatformID.MacOSX:
                    case (PlatformID)128:
                        return '/';
                    default:
                        return '\\';
                }
            }
        }


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
			r.ContentType = "application/x-www-form-urlencoded";
			if (headers != null)
			{
				foreach (string key in headers)
				{
					r.Headers.Add(key, (string) headers[key]);
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


        private static bool vdxUploadPart(string url, string file, string paramName, string contentType, NameValueCollection nvc, NameValueCollection headers, int offset = 0)
        {
            Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            
            foreach(string key in headers)
            {
                wr.Headers.Add(key, headers[key]);
            }
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
                string resp = reader2.ReadToEnd();
                if (!resp.ToLower().Equals("ok")){
                    wr = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                return false;
            }
            finally
            {
                wr = null;
            }
            return true;
        }

        /// <summary>
        /// Uploads a file in parts to a VDX web server.
        /// </summary>
        /// <returns>True if upload succesful, False otherwise</returns>
        /// <param name="uploadFolderAddress">Upload folder address.</param>
        /// <param name="filePath">File path.</param>
        /// <param name="partSize">Part size.</param>
		/// <param name="id">ID of the video</param>
        /// <param name="headers">Headers (ex. X-Requested-With).</param>
        public static bool vdxUpload(string uploadFolderAddress, string filePath, uint partSize, uint id, NameValueCollection headers)
		{

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException();
			}
			FileStream f = File.OpenRead(filePath);
            if (!f.CanRead)
            {
                throw new System.Exception("Unable to read from file.");
            }
            uint parts = (((uint) f.Length) / (partSize)) + (uint) ((f.Length % partSize != 0) ? 1 : 0);
			f.Close();
            string filename = filePath.Substring(filePath.LastIndexOf(nextFolderChar) + 1);

            
            NameValueCollection prms = new NameValueCollection();
            prms.Add("id", id.ToString());
            prms.Add("name", filename);
            prms.Add("n_parts", parts.ToString());
            prms.Add("part_size", partSize.ToString());
            string reqRes = sendPost(uploadFolderAddress + "/upload_mk.php", prms, headers);
            if (!reqRes.ToLower().Equals("ok"))
            {
				Console.WriteLine("Failed: upload_mk(" + reqRes + ").");
				return false;
            }

            for (int i = 0; i < parts; i++) {
                if(!vdxUploadPart(uploadFolderAddress + "/upload_part.php", filePath, "file", "binary/octetStream", prms, headers,(int) ( i * partSize)))
                {
                    return false;
                }
            }

			reqRes = sendPost(uploadFolderAddress + "/upload_merge.php", prms, headers);

            return true;
		}
	}
}
