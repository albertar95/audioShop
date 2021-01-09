using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AudioShopFrontend.Services;
using AudioShopFrontend.DTO;
using AudioShopFrontend.ViewModels;

namespace AudioShopFrontend.Controllers
{
    public class HomeController : Controller
    {
        DataTransfer dataTransfer = null;
        // GET: Home
        public ActionResult Index()
        {
            IndexViewModel ivm = new IndexViewModel();
            dataTransfer = new DataTransfer();
            ivm.Categories = dataTransfer.GetcategoryList();
            return View();
        }
        public ActionResult Category(int Nidcategory)
        {
            dataTransfer = new DataTransfer();
            ViewModels.CategoryViewModel cvm = new ViewModels.CategoryViewModel();
            var tmpCategory = dataTransfer.GetCategoryByNidCategory(Nidcategory);
            cvm.CategoryBrands = tmpCategory.Category_Brands.ToList();
            cvm.CategoryTypes = tmpCategory.Category_Types.ToList();
            cvm.Products = tmpCategory.Products.ToList();
            return View(cvm);
        }
        public ActionResult Generals()
        {
            return View();
        }
    }
}