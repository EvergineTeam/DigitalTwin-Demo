using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace DigitalTwin.Web.AspNetCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ResponseCompressionOptions>(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = new[] {
                    "application/octet-stream",
                    // General
                    "text/plain",
                    // Static files
                    "text/css",
                    "application/javascript",
                    // MVC
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",
                    // WebAssembly
                    "application/wasm",
                };
            });
            services.AddResponseCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseResponseCompression();
            app.UseDefaultFiles();
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".dat"] = "application/octet-stream";
            provider.Mappings[".dll"] = "application/octet-stream";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
            var managedPhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "waveengine", "managed");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(managedPhysicalPath),
                RequestPath = "/waveengine/managed",
                ServeUnknownFileTypes = true,
            });
        }
    }
}
