using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Lektion3Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lektion3Web.Controllers
{
    public class MediaController : Controller
    {
        private IConfiguration config;
        public MediaController(IConfiguration appConfig)
        {
            config = appConfig;
        }
        public async Task<IActionResult> Index()
        {
            List<ImageModel> images = new List<ImageModel>();
            //get a list of images in the container and add to the list
            var containerClient = new BlobContainerClient(
                config["BlobCNN"], "lektion4images");
            var blobs = containerClient.GetBlobsAsync(BlobTraits.Metadata);
            await foreach (var item in blobs)
            {
                images.Add(new ImageModel
                {
                    ImageFileName = item.Name,
                    Name = item.Metadata["customName"],
                });
            }

            return View(images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public async Task<IActionResult> Index(ImageUploadModel model)
        {
            //upload image after authorizing user
            var containerClient = new BlobContainerClient(
                config["BlobCNN"], "lektion4images");

            var blobClient = containerClient.GetBlobClient(
                model.ImageFile.FileName); //använd temp file name istället

            var result = await blobClient.UploadAsync(model.ImageFile.OpenReadStream(),
                new BlobHttpHeaders
                {
                    ContentType = model.ImageFile.ContentType,
                    CacheControl = "public"
                },
                new Dictionary<string, string>
                {
            { "customName", model.Name }
                }
            );
            return RedirectToAction("Index");
        }


        [HttpGet]
        //[Authorize] // when using auth to make sure they should get the link
        public IActionResult Detail(string imageFileName)
        {
            ImageModel model = new ImageModel();
            //validate user is authenticated before showing the image!!

            //get image from storage and set URL and metadata name on model
            var containerClient = new BlobContainerClient(
                config["BlobCNN"], "lektion4images");

            var blob = containerClient.GetBlobClient(imageFileName);

            BlobSasBuilder builder = new BlobSasBuilder
            {
                BlobContainerName = "lektion4images",
                BlobName = blob.Name,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),
                Protocol = SasProtocol.Https
            };
            builder.SetPermissions(BlobSasPermissions.Read);

            UriBuilder uBuilder = new UriBuilder(blob.Uri);
            uBuilder.Query = builder.ToSasQueryParameters(
                new StorageSharedKeyCredential(
                    containerClient.AccountName,
                    config["BlobKey"])).ToString();

            model.Url = uBuilder.Uri.ToString();
            model.ImageFileName = imageFileName;


            return View(model);
        }
    }
}