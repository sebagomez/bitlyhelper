using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sebagomez.BitLyHelper
{
	public class BitLyShortener
	{
		const string API_URL = "http://api.bit.ly/shorten?version={0}&longUrl={1}&login={2}&apiKey={3}&format=xml";
		const string API_VERSION = "2.0.1";

		public string ApiLogin { get; private set; }
		public string ApiKey { get; private set; }

		public BitLyShortener(string apiLogin, string apiKey)
		{
			ApiLogin = apiLogin;
			ApiKey = apiKey;
		}

		public async Task<string> GetShortenString(string[] args)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string word in args)
			{
				string newWord = word;
				try
				{
					Uri url = new Uri(word);
					if (url.Host.ToLower() != "bit.ly")
						newWord = await ShortenUrl(WebUtility.UrlEncode(url.ToString()));
				}
				catch (UriFormatException) {	} //not a url

				builder.AppendFormat("{0} ",newWord);
			}

			return builder.ToString().Remove(builder.Length -1);
		}

		public  async Task<string> GetShortenString(string status)
		{
			return await GetShortenString(status.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
		}

		public  async Task<string> ShortenUrl(string url)
		{
			string data = await new HttpClient().GetStringAsync(string.Format(API_URL, API_VERSION, url, ApiLogin, ApiKey));

			XDocument xDoc = XDocument.Parse(data);

			return GetShortUrl(xDoc);
		}

		private string GetShortUrl(XDocument xDoc)
		{
			string shortUrl = (from n in xDoc.Descendants()
							   where n.Name == "shortUrl"
							   select n.Value).First();
			return shortUrl;
		}
	}
}
