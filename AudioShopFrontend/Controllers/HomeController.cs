using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AudioShopFrontend.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Category()
        {
            ViewModels.CategoryViewModel cvm = new ViewModels.CategoryViewModel();
            cvm.CategoryBrands = new List<Models.Category_Brands>();
            cvm.CategoryTypes = new List<Models.Category_Types>();
            cvm.Products = new List<Models.Product>();
            return View(cvm);
        }
        public ActionResult Generals()
        {
            return View();
        }
    }
}