using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.Azure;

namespace BlobsAndFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            //CloudStorageAccount
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageAccount"));
            //CloudBlobClient
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            //Container
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("azurecontainer3");

            //Blob
            cloudBlobContainer.CreateIfNotExists();

            SetMetadata(cloudBlobContainer);
            //set permission for container
            cloudBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            //get blob reference
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(@"toDownload/image1.jpeg");

            using (var fileStream = System.IO.File.OpenRead(@"C:\ToUpload\Pic1.jpeg"))
            {
                blob.UploadFromStream(fileStream);
            }

            //get blob reference
            CloudBlockBlob blob2 = cloudBlobContainer.GetBlockBlobReference(@"toDownload/image2.jpeg");

            using (var fileStream = System.IO.File.OpenRead(@"C:\ToUpload\Pic2.jpeg"))
            {
                blob2.UploadFromStream(fileStream);
            }
            //download blob
            using (var fileStream = System.IO.File.OpenWrite(@"C:\FromAzure\image1.jpeg"))
            {
                blob.DownloadToStream(fileStream);
            }
            using (var fileStream = System.IO.File.OpenWrite(@"C:\FromAzure\image2.jpeg"))
            {
                blob2.DownloadToStream(fileStream);
            }
            //get blob reference
            CloudBlockBlob blob3copy = cloudBlobContainer.GetBlockBlobReference(@"toDownload/image2copy.jpeg");
            var cb = new AsyncCallback(x=>Console.WriteLine("blob copy completed!!"));
            blob3copy.BeginStartCopy(blob2, cb, null);

            var directory = cloudBlobContainer.GetDirectoryReference("toDownload");
            //list all blobs
            foreach (var item in directory.ListBlobs(false))
            {               
                if (item.GetType()==typeof(CloudBlockBlob))
                {
                    CloudBlockBlob bBlob = (CloudBlockBlob)item;
                    Console.WriteLine(bBlob.Uri);
                }
            }
        }

        static void SetMetadata(CloudBlobContainer container    )
        {
            container.Metadata.Clear();
            container.Metadata.Add("Name", "Amol");
            container.Metadata.Add("createdon", DateTime.Now.ToString());
            container.SetMetadata();

        }
    }
}
