using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using QCUtilities;
using QCUtilities.Interfaces;
using System;
using System.IO;

namespace QCVault
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/PostList", "");
            });

            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts");
            string xsd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts.xsd");

            services.AddSingleton<IPostLoader, PostDeserializer>(provider => new PostDeserializer(new DiskArchiveValidator(), new CollectionValidator(), xmlPath, xsd));

            if (Environment.IsDevelopment())
            {
                services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
                {
                    options.CacheProfiles.Add("Static", new Microsoft.AspNetCore.Mvc.CacheProfile { NoStore = true });
                });
            }
            else
            {
                services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
                {
                    options.CacheProfiles.Add("Static", new Microsoft.AspNetCore.Mvc.CacheProfile { Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any, Duration = 60 * 60 });
                });
            }
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            if (env.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "no-store";
                    }
                });
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=3600";
                    }
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
