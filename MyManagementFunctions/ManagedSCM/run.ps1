 
Import-Module AzureRM.Profile

    $apiversion = "2017-09-01"
    $TenantID    = "f833efa0-4c9a-4791-b782-bdedd8197150"
    #$subid = "4ba71b45-e24d-4f8c-804c-ec0391af1b13"
  
    if ($req_query_TenantID) 
    {
        $TenantID = $req_query_TenantID
    }

    if ($req_query_Sub) 
    {
        $SubID = $req_query_Sub
    }

    if ($req_query_resourcegroup) 
    {
        $AppServiceResourceGroupName = $req_query_resourcegroup
    }
    
    if ($req_query_AppServiceName) 
    {
        $AppServiceName = $req_query_AppServiceName
    }

    if ($req_query_Repo) 
    {
        $Repo = $req_query_Repo
    }
    
    if ($req_query_Branch) 
    {
        $Branch = $req_query_Branch
    }

    $Props = @{
        RepoUrl = "$repo"
        Branch = "$Branch"
        isManualIntegration = "false" 
    }
    $vaultName = "vault4bxudj6xo65by"
    $apiversion = "2017-09-01"
    $resourceURi = "https://vault.azure.net/"
    $tokenAuthUri = "$env:MSI_EndPoint"+ "?resource=$resourceUri&api-version=$apiversion"
    $tokenResponse = invoke-RestMethod -method Get -Headers @{"Secret"="$env:MSI_Secret"} -uri $tokenAuthURI
    $vaultAccessToken = $tokenResponse.Access_Token

    $headers = @{ 'Authorization' = "Bearer $vaultAccessToken" }
    $queryUrl = "https://vault4bxudj6xo65by.vault.azure.net/secrets/Gittoken/ec176d5614794b25961f4578fb950304"

    $queryUrl

        $keyResponse = Invoke-RestMethod -Uri $queryUrl -Headers $headers

    # Call the Vault and Retrieve Token   
    $creds = Invoke-RestMethod -Method GET -Uri $vaultSecretURI -ContentType 'application/json' -Headers $Headers

    $resourceURi = "https://management.azure.com/"
    $tokenAuthUri = "$env:MSI_EndPoint"+ "?resource=$resourceUri&api-version=$apiversion"
    $tokenResponse = invoke-RestMethod -method Get -Headers @{"Secret"="$env:MSI_Secret"} -uri $tokenAuthURI
    $accessToken = $tokenResponse.Access_Token
   
    $token = $($creds.value)

if($token) {

# Use the access token to sign in under the MSI service principal
    Login-AzureRmAccount -AccessToken $accesstoken  -Tenant $tenantId -AccountId "$env:MSI_Secret" -SubscriptionID $subid

    $PropertiesObject = @{
        token = "$token";
    }
    Set-AzureRmResource -PropertyObject $PropertiesObject -ResourceId /providers/Microsoft.Web/sourcecontrols/GitHub -ApiVersion 2015-08-01 -Force

    Set-AzureRmResource -PropertyObject $Props -ResourceGroupName $AppServiceResourceGroupName -ResourceType Microsoft.Web/sites/sourcecontrols -ResourceName $AppServiceName/web -ApiVersion 2015-08-01 -Force

    Out-File -Encoding Ascii -FilePath $res -inputObject "Hello $AppServiceName"
} else {
    Out-File -Encoding Ascii -FilePath $res -inputObject "No Git Token found"
}


