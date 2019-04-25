
Param(
	[string]$apiLogin,
	[string]$apiKey
)
Write-Output "apiLogin that was passed in from Azure DevOps=>$apiLogin"
Write-Output "apiKey that was passed in from Azure DevOps=>$apiKey"
	
[Environment]::SetEnvironmentVariable("SAUCE_USERNAME", "$apiLogin", "User")
[Environment]::SetEnvironmentVariable("SAUCE_ACCESS_KEY", "$apiLogin", "User")