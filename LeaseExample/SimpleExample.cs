using System;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace LeaseExample
{
    class SimpleExample
    {
        private readonly CloudBlobContainer _leaseContainer;
        private readonly CloudTable _table;

        public SimpleExample()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            _leaseContainer = blobClient.GetContainerReference("leaseobjects");
            _leaseContainer.CreateIfNotExists();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("entities");
            _table.CreateIfNotExists();
        }

        public Guid CreateEntity()
        {
            Guid key = Guid.NewGuid();
            SimpleEntity entity = new SimpleEntity
            {
                PartitionKey = key.ToString(),
                RowKey = "",
                SomeValue = 1
            };

            CloudBlockBlob blob = _leaseContainer.GetBlockBlobReference(String.Format("{0}.lck", key));
            blob.UploadText("");
            _table.Execute(TableOperation.Insert(entity));
            return key;
        }

        public void UpdateEntity(Guid key)
        {
            CloudBlockBlob blob = _leaseContainer.GetBlockBlobReference(String.Format("{0}.lck", key));
            string leaseId = blob.AcquireLease(TimeSpan.FromSeconds(30), Guid.NewGuid().ToString());
            try
            {
                TableResult tableResult = _table.Execute(TableOperation.Retrieve<SimpleEntity>(key.ToString(), ""));
                SimpleEntity entity = (SimpleEntity)tableResult.Result;
                entity.SomeValue++;
                _table.Execute(TableOperation.Replace(entity));
            }
            finally
            {
                blob.ReleaseLease(AccessCondition.GenerateLeaseCondition(leaseId));
            }
        }

        public void UpdateEntityWithDelay(Guid key)
        {
            CloudBlockBlob blob = _leaseContainer.GetBlockBlobReference(String.Format("{0}.lck", key));
            string leaseId = blob.AcquireLease(TimeSpan.FromSeconds(30), Guid.NewGuid().ToString());
            try
            {
                TableResult tableResult = _table.Execute(TableOperation.Retrieve<SimpleEntity>(key.ToString(), ""));
                Thread.Sleep(TimeSpan.FromSeconds(10));
                SimpleEntity entity = (SimpleEntity)tableResult.Result;
                entity.SomeValue++;
                _table.Execute(TableOperation.Replace(entity));
            }
            finally
            {
                blob.ReleaseLease(AccessCondition.GenerateLeaseCondition(leaseId));
            }
        }
    }
}
