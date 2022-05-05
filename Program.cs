using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using QCVault.Utilities.Services;


namespace QCVault
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--test"))
            {
                Console.WriteLine("Testing begin . . .");

                // All we really want to do here is load all the posts, then exit
                string xmlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts");
                string xsd = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts.xsd");
                new PostDeserializer(new DiskArchiveValidator(), new CollectionValidator(), xmlPath, xsd);

                Console.WriteLine("Testing complete!");

                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
