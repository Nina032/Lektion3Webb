using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lektion3Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Lektion3Web.Controllers
{
    public class MediaController : Controller
    {
        public IActionResult Index()
        {
            List<ImageModel> images = new List<ImageModel>();
            //get a list of images in the container and add to the list


            return View(images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public IActionResult Index(ImageUploadModel model)
        {
            //upload image after authorizing user
            

            return RedirectToAction("Index");
        }

        [HttpGet]
        //[Authorize] // when using auth to make sure they should get the link
        public IActionResult Detail(string imageFileName)
        {
            ImageModel model = new ImageModel();
            //validate user is authenticated before showing the image!!

            //get image from storage and set URL and metadata name on model




            return View(model);
        }
    }
}