using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sebagomez.BitLyHelper;
using System;
using System.IO;
using System.Linq;
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

		public static string APILogin { get; set; }
		public static string APIKey { get; set; }

		const string API_LOGIN = "BITLY_API_LOGIN";
		const string API_KEY = "BITLY_API_KEY";

		static BasicTests()
		{
#if DEBUG
			if (File.Exists("Properties\\launchSettings.json"))
			{
				using (var file = File.OpenText("Properties\\launchSettings.json"))
				{
					var reader = new JsonTextReader(file);
					var jObject = JObject.Load(reader);

					var variables = jObject
						.GetValue("profiles")
						.SelectMany(profiles => profiles.Children())
						.SelectMany(profile => profile.Children<JProperty>())
						.Where(prop => prop.Name == "environmentVariables")
						.SelectMany(prop => prop.Value.Children<JProperty>())
						.ToList();

					foreach (var variable in variables)
					{
						Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
					}
				}
			}
#endif
			APILogin = Environment.GetEnvironmentVariable(API_LOGIN);
			APIKey = Environment.GetEnvironmentVariable(API_KEY);

			if (string.IsNullOrEmpty(APIKey) || string.IsNullOrEmpty(APILogin))
				throw new Exception("APIKey and/or APILogin not defined");
		}


		const string GOOGLE = "http://google.com";
		const string SHORT_GOOGLE = "http://bit.ly/2L6Cz94";

		[Fact]
		public async Task JustUrl()
		{
			BitLyShortener shortener = new BitLyShortener(APILogin, APIKey);
			string shortened = await shortener.ShortenUrl(GOOGLE);

			Assert.Equal(SHORT_GOOGLE, shortened);
		}

		[Fact]
		public async Task ShortText()
		{
			string template = "This is a string with Google url {0}";
			BitLyShortener shortener = new BitLyShortener(APILogin, APIKey);
			string shortened = await shortener.GetShortenString(string.Format(template, GOOGLE));

			Assert.Equal(string.Format(template, SHORT_GOOGLE), shortened);
		}

		[Fact]
		public async Task NoUrl()
		{
			string text = "This text has no urls";
			BitLyShortener shortener = new BitLyShortener(APILogin, APIKey);
			string shortened = await shortener.GetShortenString(text);

			Assert.Equal(text, shortened);
		}
	}
}
