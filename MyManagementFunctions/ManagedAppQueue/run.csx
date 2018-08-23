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

public static void Run(string myQueueItem, ILogger log, IAsyncCollector<string> outputQueueItem,ICollector<MyMessage> outputTable )
{
    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

    outputQueueItem.AddAsync(myQueueItem);

     MyMessage myRow = new MyMessage(myQueueItem);

    outputTable.Add(myRow);
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
