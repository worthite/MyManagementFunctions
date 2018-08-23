using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;


namespace MyManagementFunctions.ManagedAppQueue
{
    public static class ManagedAppQueue
    {
        internal static string StorageConnectionString = ConfigurationManager.AppSettings["AzureWebJobsStorage"];
        internal static string KeyVault = ConfigurationManager.AppSettings["KeyVault"];

        [FunctionName("ManagedAppQueue")]
        public static void Run([QueueTrigger("mymanagedapps", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function Started: {myQueueItem}");
            
            var appSettings = ConfigurationManager.AppSettings;

            if (appSettings.Count == 0)
            {
                log.LogInformation("AppSettings is empty.");
            }
            else
            {
                foreach (var key in appSettings.AllKeys)
                {
                    log.LogInformation("Key: {0} Value: {1}", key, appSettings[key]);
                }
            }


            log.LogInformation($"StorageConnectionString: {StorageConnectionString}");

            if (!string.IsNullOrEmpty(StorageConnectionString))
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

                // Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                CloudQueue queue = queueClient.GetQueueReference("sourcecontrolsetup");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExistsAsync();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(myQueueItem);
                queue.AddMessageAsync(message);

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

                log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            } else
            {
                log.LogInformation($"C# Queue trigger function Error: No Connection String Found.");
            }
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

    }
}
