using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.CosmosDB.Table;
using StorageTablesQueuesAndCosmosDB.Entity;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
//reference - https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-dotnet
//create free cosmosdb here - https://azure.microsoft.com/en-us/try/cosmosdb/
namespace StorageTablesQueuesAndCosmosDB
{
    class Program
    {
        static void Main(string[] args)
        {
            #region "Tables"
            //var table =  GetCosmostDBTable();
            ////var table =  CallAzureTable();
            ////InsertEntity(table);
            //InsertBatch(table);
            //RetrieveAll(table);
            #endregion

            #region Queue
            var queue = GetCloudQueue();
            AddRemoveMessageToQueue(queue);

            #endregion
        }

        #region "Helper for Azure Tables"
        static void RetrieveAll(CloudTable table)
        {
            TableQuery<Customer> query = new TableQuery<Customer>().Where(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Warne"),
               TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "Shane")));

            foreach (var customer in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", customer.PartitionKey, customer.RowKey,
     customer.Email, customer.PhoneNumber);
            }
            Console.ReadKey();
        }
        static void InsertEntity(CloudTable table)
        {
            Customer customer1 = new Customer("Warne", "Shane");
            customer1.Email = "shane.w@adv.com";
            customer1.PhoneNumber = "145-578-0000";
            TableOperation insertOperation = TableOperation.Insert(customer1);
            table.Execute(insertOperation);
        }

        static void InsertBatch(CloudTable table)
        {
            TableBatchOperation tableBatchOperation = new TableBatchOperation();
            Customer customer1 = new Customer("Warne", "Shane");
            customer1.Email = "shane.w@adv.com";
            customer1.PhoneNumber = "145-578-0000";

            Customer customer2 = new Customer("Warne", "Don");
            customer2.Email = "Don.w@adv.com";
            customer2.PhoneNumber = "145-578-0001";

            tableBatchOperation.Insert(customer1);
            tableBatchOperation.Insert(customer2);
            table.ExecuteBatch(tableBatchOperation);
        }
        static CloudTable GetCosmostDBTable()
        {
            Microsoft.Azure.Storage.CloudStorageAccount storageAccount = Microsoft.Azure.Storage.CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("CosmosDB"));
            Microsoft.Azure.CosmosDB.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("customers");
            //cleanup
            table.DeleteIfExists();
            table.CreateIfNotExists();
            return table;
        }
        static Microsoft.WindowsAzure.Storage.Table.CloudTable GetAzureTable()
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageAccount"));
            //create cloud table client
            Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            //get cloud table
            Microsoft.WindowsAzure.Storage.Table.CloudTable table = tableClient.GetTableReference("customers");
            //cleanup
            table.DeleteIfExists();
            table.CreateIfNotExists();
            return table;
        }
        #endregion

        #region "Helpers for Azure Queue"
        static CloudQueue GetCloudQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageAccount"));
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
            //It take sometime for azure to cleanup the queue, if you are running code again and again, it may fail here. Try using different names of queue to avoid error
            CloudQueue queue =  cloudQueueClient.GetQueueReference("customerqueue10");
            //cleanup
            queue.DeleteIfExists();
            //create new
            queue.CreateIfNotExists();
            return queue;
            
        }
        static void AddRemoveMessageToQueue(CloudQueue queue)
        {
            Customer customer1 = new Customer("Warne", "Shane");
            customer1.Email = "shane.w@adv.com";
            //passing customer object to queue to processes
            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(customer1));
            queue.AddMessage(message);

            //Peek messsage from Queue
            var fromPeekQueue = queue.PeekMessage();
            Console.WriteLine("from queue " + fromPeekQueue.AsString);
            Console.ReadKey();
            //get message and update the content

            var fromQueue = queue.GetMessage();
            var customer = JsonConvert.DeserializeObject<Customer>(fromQueue.AsString);
            customer.Email = "update@email.com";
            fromQueue.SetMessageContent(JsonConvert.SerializeObject(customer));
            queue.UpdateMessage(fromQueue, TimeSpan.FromMilliseconds(1),MessageUpdateFields.Content|MessageUpdateFields.Visibility);
            queue.FetchAttributes();
            int? cachedMsg = queue.ApproximateMessageCount;
            Console.WriteLine("Cached Message " + cachedMsg);
            //Peek messsage from Queue
           var updatedMessage = queue.GetMessage();
            Console.WriteLine("after update " + updatedMessage != null? updatedMessage.AsString: "null");
            Console.ReadKey();

            //Process the message in less than 30 seconds, and then delete the message
            queue.DeleteMessage(updatedMessage);
            queue.FetchAttributes();
           cachedMsg = queue.ApproximateMessageCount;
            Console.WriteLine("Cached Message " + cachedMsg);
        }
        #endregion
    }
}
