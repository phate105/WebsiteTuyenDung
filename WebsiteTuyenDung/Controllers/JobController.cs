using System.Web.Mvc;

namespace WebsiteTuyenDung.Controllers
{
    public class JobController : Controller
    {
        // GET: Job/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Job/Search
        public ActionResult Search(string keyword = "", string location = "")
        {
            ViewBag.Keyword = keyword;
            ViewBag.Location = location;
            return View();
        }
    }
}