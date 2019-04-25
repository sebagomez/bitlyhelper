using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sebagomez.BitLyHelper;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BitLyTests
{
	public class BasiTests
	{
		public static string APILogin { get; set; }
		public static string APIKey { get; set; }

		const string API_LOGIN = "BITLY_API_LOGIN";
		const string API_KEY = "BITLY_API_KEY";

		static BasiTests()
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
		}

		[Fact]
		public async Task Test1()
		{
			BitLyShortener shortener = new BitLyShortener(APILogin, APIKey);
			string shortened = await shortener.GetShortenString("This is a string with Google url http://google.com");

			Assert.Equal("This is a string with Google url http://bit.ly/2L6Cz94", shortened);
		}
	}
}
