using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

public static void Run(string myQueueItem, TraceWriter log)
{
    string StorageConnectionString = ConfigurationManager.AppSettings["AzureWebJobsStorage"];

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

    // Create the queue client.
    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

    // Retrieve a reference to a container.
    CloudQueue queue = queueClient.GetQueueReference("sourcecontrolsetup");

    // Create the queue if it doesn't already exist
    queue.CreateIfNotExists();

    // Create a message and add it to the queue.
    CloudQueueMessage message = new CloudQueueMessage(myQueueItem);
    queue.AddMessage(message);

    // Create the table client.
    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

    // Retrieve a reference to the table.
    CloudTable table = tableClient.GetTableReference("MyManagedApplications");

    // Create the table if it doesn't exist.
    table.CreateIfNotExistsAsync();

    //Source Control Setup           
    MyMessage myRow = new MyMessage(myQueueItem);

    // Create the TableOperation object that inserts the customer entity.
    TableOperation insertOperation = TableOperation.Insert(myRow);

    // Execute the insert operation.
    table.ExecuteAsync(insertOperation);

    log.Info($"C# Queue trigger function processed: {myQueueItem}");
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
