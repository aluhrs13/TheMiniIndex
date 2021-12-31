using Azure.Storage;
using Azure.Storage.Blobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MiniIndex.Core.Utilities
{
    public static class StorageHelper
    {
        public static async Task<bool> UploadFileToStorage(string url, string MiniID, AzureStorageConfig _storageConfig, HttpClient httpClient)
        {
            MemoryStream uploadStream = new MemoryStream();

            //Configure blob and connect
            Uri blobUri = new Uri("https://" +
                                  _storageConfig.AccountName +
                                  ".blob.core.windows.net/" +
                                  _storageConfig.ImageContainer +
                                  "/" + MiniID + ".jpg");

            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(_storageConfig.AccountName, _storageConfig.AccountKey);

            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            Stream fileStream = await httpClient.GetStreamAsync(url);

            //Load image and resize it before uploading
            Image image = await Image.LoadAsync(fileStream);

            //Resizing to 0 automatically maintains aspect ratio
            if (image.Width > image.Height)
            {
                image.Mutate(x => x.Resize(480, 0));
            }
            else
            {
                image.Mutate(x => x.Resize(0, 480));
            }

            await image.SaveAsJpegAsync(uploadStream);

            uploadStream.Position = 0;
            await blobClient.UploadAsync(uploadStream, true);

            return await Task.FromResult(true);
        }
    }
}