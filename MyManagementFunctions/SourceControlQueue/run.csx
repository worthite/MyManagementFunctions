#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"


using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Services.AppAuthentication;

public static void Run(string myQueueItem, ILogger log)
{
    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
    
        string vaultName = "vault4bxudj6xo65by";

        MyMessage myRow = new MyMessage(myQueueItem);

    string token = await GetSecret(vaultName, "Gittoken");

}

public static async Task<string> GetSecret(string vaultName, string nameKey)
{

    string vaultUrl = $"https://{vaultName}.vault.azure.net/secrets/{nameKey}";

    AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

    var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

    var secret = await kv.GetSecretAsync(vaultUrl).ConfigureAwait(false);

    var secretUri = secret.Value;

    return secretUri;
}
public class GitToken
{
    public string Token {get; set;}
}
public class MyMessage : TableEntity
{
    public MyMessage(string msgString)
    {
        MyMessage msg = Newtonsoft.Json.JsonConvert.DeserializeObject<MyMessage>(msgString);

        this.PartitionKey = msg.TenantId;
        this.RowKey = msg.ResourceGroup;

        this.TenantId = msg.TenantId;
        this.SubId = msg.SubId;
        this.ResourceGroup = msg.ResourceGroup;
        this.AppServiceName = msg.AppServiceName;
        this.Repo = msg.Repo;
        this.Branch = msg.Branch;

    }
    public MyMessage() { }

    public string TenantId { get; set; }
    public string SubId { get; set; }
    public string ResourceGroup { get; set; }
    public string AppServiceName { get; set; }
    public string Repo { get; set; }
    public string Branch { get; set; }
}