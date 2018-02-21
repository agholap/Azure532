using Microsoft.Azure.CosmosDB.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageTablesQueuesAndCosmosDB.Entity
{
    public class Customer:TableEntity
    {
        public Customer(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public Customer() { }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
