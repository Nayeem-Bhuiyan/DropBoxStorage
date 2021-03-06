using DropboxCore.Data.Entity.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Data
{
    public class AppDbContext : DbContext
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;

        //public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor _httpContextAccessor) : base(options)
        //{
        //    this._httpContextAccessor = _httpContextAccessor;
        //}
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }
        public DbSet<FolderUploadInfo> folderUploadInfos { get; set; }

    }
}