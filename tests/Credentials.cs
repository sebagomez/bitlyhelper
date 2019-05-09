using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BitLyTests
{
	public sealed class Credentials
	{
		//KeyVault stuff taken from https://stackoverflow.com/questions/47875589/cant-access-azure-key-vault-from-desktop-console-app
		public static Credentials Current = new Credentials();

		const string API_LOGIN = "BITLY-API-LOGIN";
		const string API_KEY = "BITLY-API-KEY";
		const string VAULT_URL = "VAULT_URL";
		const string APPLICATION_ID = "APPLICATION_ID";
		const string APPLICATION_SECRET = "APPLICATION_SECRET";

#if DEBUG

		private Credentials()
		{
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
		}
#endif

		#region Properties

		private string m_apiLogin;
		public string APILOGIN
		{
			get
			{
				if (string.IsNullOrEmpty(m_apiLogin) || string.IsNullOrEmpty(m_apiKey))
					LoadCredentials();

				return m_apiLogin;
			}
		}

		private string m_apiKey;
		public string APIKEY
		{
			get
			{
				if (string.IsNullOrEmpty(m_apiLogin) || string.IsNullOrEmpty(m_apiKey))
					LoadCredentials();

				return m_apiKey;
			}
		}

		private string m_vaultUrl;
		public string VaultUrl
		{
			get
			{
				if (string.IsNullOrEmpty(m_vaultUrl))
					m_vaultUrl = Environment.GetEnvironmentVariable(VAULT_URL);

				return m_vaultUrl;
			}
		}

		private string m_appId;
		public string ApplicationId
		{
			get
			{
				if (string.IsNullOrEmpty(m_appId))
					m_appId = Environment.GetEnvironmentVariable(APPLICATION_ID);

				return m_appId;
			}
		}

		private string m_appSecret;
		public string ApplicationSecret
		{
			get
			{
				if (string.IsNullOrEmpty(m_appSecret))
					m_appSecret = Environment.GetEnvironmentVariable(APPLICATION_SECRET);

				return m_appSecret;
			}
		}

		#endregion

		void LoadCredentials()
		{
			m_apiLogin = GetSecretAsync(VaultUrl, API_LOGIN).Result;
			m_apiKey = GetSecretAsync(VaultUrl, API_KEY).Result;
		}

		private async Task<string> GetSecretAsync(string vaultUrl, string vaultKey)
		{
			var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync), new HttpClient());
			var secret = await client.GetSecretAsync(vaultUrl, vaultKey);

			return secret.Value;
		}

		private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
		{
			var appCredentials = new ClientCredential(ApplicationId, ApplicationSecret);
			var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

			var result = await context.AcquireTokenAsync(resource, appCredentials);

			return result.AccessToken;
		}

		
	}
}
