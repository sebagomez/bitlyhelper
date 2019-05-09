using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sebagomez.BitLyHelper;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BitLyTests
{
	public class BasicTests
	{
		private readonly ITestOutputHelper output;

		public BasicTests(ITestOutputHelper output)
		{
			this.output = output;
		}


		const string GOOGLE = "http://google.com";
		const string SHORT_GOOGLE = "http://bit.ly/2L6Cz94";


		

		[Fact]
		public async Task JustUrl()
		{
			BitLyShortener shortener = new BitLyShortener(Credentials.Current.APILOGIN, Credentials.Current.APIKEY);
			string shortened = await shortener.ShortenUrl(GOOGLE);

			Assert.Equal(SHORT_GOOGLE, shortened);
		}

		[Fact]
		public async Task ShortText()
		{
			string template = "This is a string with Google url {0}";
			BitLyShortener shortener = new BitLyShortener(Credentials.Current.APILOGIN, Credentials.Current.APIKEY);
			string shortened = await shortener.GetShortenString(string.Format(template, GOOGLE));

			Assert.Equal(string.Format(template, SHORT_GOOGLE), shortened);
		}

		[Fact]
		public async Task NoUrl()
		{
			string text = "This text has no urls";
			BitLyShortener shortener = new BitLyShortener(Credentials.Current.APILOGIN, Credentials.Current.APIKEY);
			string shortened = await shortener.GetShortenString(text);

			Assert.Equal(text, shortened);
		}
	}
}
