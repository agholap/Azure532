using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.CosmosDB;
using Microsoft.Azure.CosmosDB.Table;
using StorageTablesQueuesAndCosmosDB.Entity;
//reference - https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-dotnet
//create free cosmosdb here - https://azure.microsoft.com/en-us/try/cosmosdb/
namespace StorageTablesQueuesAndCosmosDB
{
    class Program
    {
        static void Main(string[] args)
        {
             var table =  GetCosmostDBTable();

            //var table =  CallAzureTable();
            //InsertEntity(table);
            InsertBatch(table);
            RetrieveAll(table);
        }

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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("CosmosDB"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("customers");
            //cleanup
            table.DeleteIfExists();
            table.CreateIfNotExists();
            return table;
        }
        static CloudTable GetAzureTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageAccount"));
            //create cloud table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            //get cloud table
            CloudTable table = tableClient.GetTableReference("customers");
            //cleanup
            table.DeleteIfExists();
            table.CreateIfNotExists();
            return table;
        }
    }
}
