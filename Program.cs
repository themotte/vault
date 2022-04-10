using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                new QCUtilities.PostDeserializer(new QCUtilities.DiskArchiveValidator(), new QCUtilities.CollectionValidator(), xmlPath, xsd);

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
