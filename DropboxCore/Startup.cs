using DropboxCore.Data;
using DropboxCore.Service;
using DropboxCore.Service.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            #region App Database Settings
            services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(Configuration.GetConnectionString("AppDbConnection")));
            #endregion
            #region Areas Config
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Areas/{2}/{0}" + RazorViewEngine.ViewExtension);
                options.AreaViewLocationFormats.Add("/areas/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/areas/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

            });
            #endregion
            services.AddSingleton<IConfiguration>(Configuration);
            #region DropboxManager
            services.AddScoped<IDropboxManager, DropboxManager>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IDeleteDropBoxService,DeleteDropBoxService>();
            services.AddScoped<ICreateDropBoxService,CreateDropBoxService>();
            services.AddScoped<IDownloadDropBoxService, DownloadDropBoxService>();
            services.AddScoped<IEditDropBoxService, EditDropBoxService>();
            #endregion

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 85899345920;
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDefaultFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMvc(routes => {
                routes.MapRoute(
                name: "DropBox",
                template: "{area=exists}/{controller=CreateDropBox}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
