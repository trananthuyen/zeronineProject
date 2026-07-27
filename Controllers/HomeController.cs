using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Threading;
using zeronineProject.Core.Entities;
using zeronineProject.Infrastructure.ExternalAPICalls.BinanceAPIs;

namespace zeronineProject.UI.Controllers
{
    
    public class HomeController : Controller
    {
        

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
