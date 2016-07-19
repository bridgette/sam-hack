using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using System;
using System.Collections.Generic;

namespace StorageClient
{
    public class TableStorageClient
    {
        CloudTableClient tableClient;

        /// <summary>
        /// Initial Table Storage Client
        /// </summary>
        /// <param name="connectionString">Storage Account Connection String</param>
        public TableStorageClient(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Create a table with the given name
        /// </summary>
        /// <param name="tableReferenceName">Table Name</param>
        public void CreateTable(string tableReferenceName)
        {
            CloudTable table = tableClient.GetTableReference(tableReferenceName);
            table.CreateIfNotExists();
        }

        /// <summary>
        /// Insert a record from a base class
        /// </summary>
        /// <param name="tableReferenceName">Table Name</param>
        /// <param name="record">Table Entity to Insert (base class)</param>
        /// <returns>TableResult</returns>
        public TableResult InsertRecord(string tableReferenceName, TableEntity record)
        {   
            // Get the table for the given name
            CloudTable table = tableClient.GetTableReference(tableReferenceName);

            // Create table operation to Upsert 
            TableOperation insertOperation = TableOperation.InsertOrMerge(record);
            return table.Execute(insertOperation);
        }

        /// <summary>
        /// Get all of the partitions in a table
        /// </summary>
        /// <param name="tableReferenceName">Table Name</param>
        /// <returns>List<EventTable /> </returns>
        public List<EventTable> GetAllPartitions(string tableReferenceName)
        {
            // Get the table for the given name
            CloudTable table = tableClient.GetTableReference(tableReferenceName);

            // Create query to get ALL partitions
            TableQuery<EventTable> query = new TableQuery<EventTable>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, null));

            // Save all results to a list and return
            List<EventTable> entities = new List<EventTable>();
            foreach (EventTable entity in table.ExecuteQuery<EventTable>(query))
            {
                entities.Add(entity);
            }
            return entities;
        }

        /// <summary>
        /// Get ActionTable records in the given partition
        /// </summary>
        /// <param name="tableReferenceName">Table Name</param>
        /// <param name="partitionName">Partition Name</param>
        /// <returns>List<ActionTable /></returns>
        public List<ActionTable> GetActionRecordsInPartition(string tableReferenceName, string partitionName)
        {
            // Get the table for the given name
            CloudTable table = tableClient.GetTableReference(tableReferenceName);

            // Create query to get all records in that partition
            TableQuery<ActionTable> query = new TableQuery<ActionTable>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));
            List<ActionTable> entities = new List<ActionTable>();

            // get records if they exist
            try
            {
                foreach (ActionTable entity in table.ExecuteQuery<ActionTable>(query))
                {
                    entities.Add(entity);
                }
            }
            catch (Exception e)
            {
                // No table found or no records Found
                entities = new List<ActionTable>();
            }

            return entities;
        }

        /// <summary>
        /// Delete table by name
        /// </summary>
        /// <param name="tableName">Table name</param>
        public void DeleteTable(string tableName)
        {
            var table = tableClient.GetTableReference(tableName);
            if (table.Exists())
            {
                table.Delete();
            }
        }
    }
}
