﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lektion3Web.Models
{
    public class ImageUploadModel
    {
        public string Name { get; set; }

        public IFormFile ImageFile { get; set; }

    }
}
