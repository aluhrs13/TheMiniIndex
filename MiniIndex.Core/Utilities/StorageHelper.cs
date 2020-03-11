using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Utilities
{
    public static class StorageHelper
    {
        public static async Task<bool> UploadFileToStorage(string url, string MiniID, AzureStorageConfig _storageConfig)
        {
            Stream fileStream;
            MemoryStream uploadStream = new MemoryStream();


            Uri blobUri = new Uri("https://" +
                                  _storageConfig.AccountName +
                                  ".blob.core.windows.net/" +
                                  _storageConfig.ImageContainer +
                                  "/" + MiniID + ".jpg");

            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(_storageConfig.AccountName, _storageConfig.AccountKey);

            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                fileStream = webClient.OpenRead(url);

            }


            //TODO - Resize smarter, not everything is a square
            Image image = Image.Load(fileStream);
            image.Mutate(x => x.Resize(480, 480));

            image.SaveAsJpeg(uploadStream);

            uploadStream.Position = 0;

            await blobClient.UploadAsync(uploadStream);

            return await Task.FromResult(true);
        }
    }
}