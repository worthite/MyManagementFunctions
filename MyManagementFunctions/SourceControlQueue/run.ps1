 
Import-Module AzureRM.Profile

$in = Get-Content $triggerInput | ConvertFrom-Json

Write-Output "PowerShell script processed queue message '$in'"

    $Props = @{
        RepoUrl = $($in.Repo)
        Branch = "$($in.Branch)"
        isManualIntegration = "false" 
    }

    $tenantId = $($in.TenantId)
    $subid = $($in.subid)
    $AppServiceResourceGroupName = $($in.ResourceGroup)
    $AppServiceName = $($in.AppServiceName)

    $vaultName = "vault4bxudj6xo65by"
    $secretKey ="Gittoken"
    $apiversion = "2017-09-01"

    $resourceURi = "https://vault.azure.net"
    $tokenAuthUri = "$env:MSI_EndPoint"+ "?resource=$resourceUri&api-version=$apiversion"
    $tokenResponse = invoke-RestMethod -method Get -Headers @{"Secret"="$env:MSI_Secret"} -uri $tokenAuthURI
    $vaultAccessToken = $tokenResponse.Access_Token
	
     $headers= @{'Authorization'="Bearer $vaultAccessToken"}
     $queryUrl="https://$vaultName.vault.azure.net/secrets/$secretKey"+'?api-version=2016-10-01'
     $keyResponse= Invoke-RestMethod -Method GET -Uri $queryUrl -Headers $headers
     $token = $($keyResponse.value)

    $resourceURi = "https://management.azure.com/"
    $tokenAuthUri = "$env:MSI_EndPoint"+ "?resource=$resourceUri&api-version=$apiversion"
    $tokenResponse = invoke-RestMethod -method Get -Headers @{"Secret"="$env:MSI_Secret"} -uri $tokenAuthURI
    $accessToken = $tokenResponse.Access_Token

    # Use the access token to sign in under the MSI service principal
    Login-AzureRmAccount -AccessToken $accesstoken  -Tenant $tenantId -AccountId "$env:MSI_Secret" -SubscriptionID $subid
    
    if($token) {
    $PropertiesObject = @{
        token = "$token";
    }

    Set-AzureRmResource -PropertyObject $PropertiesObject -ResourceId /providers/Microsoft.Web/sourcecontrols/GitHub -ApiVersion 2015-08-01 -Force

    Set-AzureRmResource -PropertyObject $Props -ResourceGroupName $AppServiceResourceGroupName -ResourceType Microsoft.Web/sites/sourcecontrols -ResourceName $AppServiceName/web -ApiVersion 2015-08-01 -Force
    
    }