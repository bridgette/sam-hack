using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using System;
using System.Collections.Generic;
using System.Net;

namespace TableStorageClient
{
    public class TableStorageClient
    {
        string connectionString;
        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;

        public TableStorageClient(string connectionString)
        {
            this.connectionString = connectionString;
            storageAccount = CloudStorageAccount.Parse(this.connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public void CreateTable(string tableReferenceName)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.connectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(tableReferenceName);
            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
        }

        public TableResult InsertRecord(string tableReferenceName, TableEntity record)
        {
            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(tableReferenceName);

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(record);

            TableResult result = table.Execute(insertOperation);

            return result;
        }

        public List<TableEntity> GetRecordsInPartition(string tableReferenceName, string partitionName)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.connectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(tableReferenceName);

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<TableEntity> query = new TableQuery<TableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));

            List<TableEntity> entities = new List<TableEntity>();

            // Print the fields for each customer.
            foreach (TableEntity entity in table.ExecuteQuery<TableEntity>(query))
            {
                entities.Add(entity);
            }

            return entities;
        }

        public List<TableEntity> GetRecordsByPartitionAndSubkey(string tableReferenceName, string partitionName, string subKeyName)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.connectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(tableReferenceName);

            // Create the table query.
            TableQuery<TableEntity> rangeQuery = new TableQuery<TableEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, subKeyName)));

            List<TableEntity> entities = new List<TableEntity>();

            // Print the fields for each customer.
            foreach (TableEntity entity in table.ExecuteQuery(rangeQuery))
            {
                entities.Add(entity);
            }

            return entities;
        }
    }
}
