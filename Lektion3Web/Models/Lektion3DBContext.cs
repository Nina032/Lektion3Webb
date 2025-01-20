using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lektion3Web.Models
{
    public class Lektion3DBContext : DbContext
    {
        public Lektion3DBContext(DbContextOptions options):base(options)
        {

        }
        public virtual DbSet<BrandModel> Brands { get; set; }

    }
}
