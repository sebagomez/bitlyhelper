[![Build & Publish](https://github.com/sebagomez/bitlyhelper/actions/workflows/dotnet.yaml/badge.svg)](https://github.com/sebagomez/bitlyhelper/actions/workflows/dotnet.yaml)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sebagomez.BitLyHelper.svg?label=NuGet%20downloads)](https://www.nuget.org/packages/Sebagomez.BitLyHelper/)

# BitLy Helper
The simplest BitLy SDK for .NET Core to shorten a url

Go to https://bitly.com/a/your_api_key to get your API login and key.

That's it, install this package and start shortening your urls!

```c#
BitLyShortener shortener = new BitLyShortener(APILogin, APIKey);
string shortened = await shortener.ShortenUrl("http://www.google.com");
```