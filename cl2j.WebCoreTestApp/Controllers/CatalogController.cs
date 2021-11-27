using Microsoft.AspNetCore.Mvc;

namespace cl2j.WebCore.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}