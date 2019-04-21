using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;

namespace AttendanceManagement.Controllers
{
	public class APIController : ApiController
	{
		public string ReadData(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			Stream receiveStream = response.GetResponseStream();
			StreamReader readStream = null;
			readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
			string data = readStream.ReadToEnd().ToString();
			response.Close();
			readStream.Close();
			return data;
		}
	}
}
