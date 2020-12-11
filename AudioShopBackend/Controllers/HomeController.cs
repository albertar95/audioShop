using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AudioShopBackend.Services;
using AudioShopBackend.Models;

namespace AudioShopBackend.Controllers
{
    public class HomeController : Controller
    {
        DbTransfer dbTransfer = null;
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Categories()
        {
            return View();
        }
        public ActionResult AddBrandOrType(bool IsBrand,string Name, bool IsNewCategory, string CategoryName = "",int NidCategory = 0,string categoryKeywords="",string CategoryDescription="")
        {
            dbTransfer = new DbTransfer();
            bool categoryAdded = false;
            bool brandAdded = false;
            bool typeAdded = false;
            int tmpNidcategory = 0;
            if (IsNewCategory)
            {
                tmpNidcategory = dbTransfer.GenerateNewNidCategory();
                Category category = new Category() { CategoryName = CategoryName,IsSubmmited = false,NidCategory = tmpNidcategory, Description = CategoryDescription, keywords = categoryKeywords };
                dbTransfer.Add(category);
                if (dbTransfer.Save())
                    categoryAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error adding category" });
            }
            else
            {
                tmpNidcategory = NidCategory;
                categoryAdded = true;
            }    
            if(IsBrand)
            {
                Category_Brands categoryBrand = new Category_Brands() { NidBrand = Guid.NewGuid(), BrandName = Name, NidCategory = tmpNidcategory };
                dbTransfer.Add(categoryBrand);
                if (dbTransfer.Save())
                    brandAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error adding brand" });
            }
            else
            {
                Category_Types categoryType = new Category_Types() { NidType = Guid.NewGuid(), NidCategory = tmpNidcategory, TypeName = Name };
                dbTransfer.Add(categoryType);
                if (dbTransfer.Save())
                    typeAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error adding type" });
            }
            if (categoryAdded)
            {
                List<Tuple<string, string>> labels = new List<Tuple<string, string>>();
                if (brandAdded)
                {
                    foreach (var lbl in dbTransfer.GetCategoryBrandsByNidCategory(tmpNidcategory))
                    {
                        labels.Add(new Tuple<string, string>(lbl.NidBrand.ToString(), lbl.BrandName));
                    }
                }
                if(typeAdded)
                {
                    foreach (var lbl in dbTransfer.GetCategoryTypesByNidCategory(tmpNidcategory))
                    {
                        labels.Add(new Tuple<string, string>(lbl.NidType.ToString(), lbl.TypeName));
                    }
                }
                return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_CategoryBrandAndTypeLabel", labels), tmpNidCategory = tmpNidcategory });
            }
            else
                return Json(new JsonResults() { HasValue = false, Message = "unexpected error" });
        }
        public ActionResult AddCategory(bool IsNewCategory,string Name,string CategoryDescription,string categoryKeywords, int NidCategory = 0)
        {
            bool categoryAdded = false;
            dbTransfer = new DbTransfer();
            if(IsNewCategory)
            {
                Category category = new Category() { NidCategory = dbTransfer.GenerateNewNidCategory(), CategoryName = Name, IsSubmmited = true, Description = CategoryDescription, keywords = categoryKeywords };
                dbTransfer.Add(category);
                if (dbTransfer.Save())
                    categoryAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error adding category" });
            }
            else
            {
                Category category = dbTransfer.GetCategoryByNidCategory(NidCategory);
                category.IsSubmmited = true;
                category.CategoryName = Name;
                category.Description = CategoryDescription;
                category.keywords = categoryKeywords;
                dbTransfer.Update(category);
                if (dbTransfer.Save())
                    categoryAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error adding category" });
            }
            if(categoryAdded)
                return Json(new JsonResults() { HasValue = true });
            else
                return Json(new JsonResults() { HasValue = false, Message = "unexcpected error" });

        }
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            ViewDataDictionary viewData = new ViewDataDictionary(model);

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
    public class JsonResults
    {
        public bool HasValue { get; set; }
        public string Message { get; set; }
        public string Html { get; set; }
        public int tmpNidCategory { get; set; }
    }
}