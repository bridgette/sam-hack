using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using System;
using System.Collections.Generic;

namespace StorageClient
{
    public class BlobStorageClient
    {
        CloudStorageAccount storageAccount;
        string ContainerName = "reports";

        public BlobStorageClient(string connectionString)
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public string UploadBlob(string report)
        {
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(this.ContainerName);
            container.CreateIfNotExists();

            string blobPath = "AttendanceReport-" + DateTime.Now.ToShortDateString() + DateTime.Now.Millisecond + ".csv";

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobPath);

            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadText(report);

            //In case blob container's ACL is private, the blob can't be accessed via simple URL. For that we need to
            //create a Shared Access Signature (SAS) token which gives time/permission bound access to private resources.
            var sasToken = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),//Asssuming user stays on the page for an hour.
            });
            var blobUrl = blockBlob.Uri.AbsoluteUri + sasToken;//This will ensure that user will be able to access the blob for one hour.
    
            return blobUrl;
        }
    }
}