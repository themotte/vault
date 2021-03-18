using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QCUtilities.Entities;

namespace QCVault.Pages
{
    public class TestModel : PageModel
    {
        private readonly ILogger<TestModel> _logger;

        public List<Post> Posts { get; set; }

        public TestModel(ILogger<TestModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
          
        }
    }
}
