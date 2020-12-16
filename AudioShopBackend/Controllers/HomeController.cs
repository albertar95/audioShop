﻿using System;
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

        //category section
        public ActionResult Categories()
        {
            dbTransfer = new DbTransfer();
            var categoryList = dbTransfer.GetAllCategories();
            return View("Categories",categoryList);
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
                List<Tuple<string, string,bool>> labels = new List<Tuple<string, string,bool>>();
                if (brandAdded)
                {
                    foreach (var lbl in dbTransfer.GetCategoryBrandsByNidCategory(tmpNidcategory))
                    {
                        labels.Add(new Tuple<string, string,bool>(lbl.NidBrand.ToString(), lbl.BrandName,true));
                    }
                }
                if(typeAdded)
                {
                    foreach (var lbl in dbTransfer.GetCategoryTypesByNidCategory(tmpNidcategory))
                    {
                        labels.Add(new Tuple<string, string,bool>(lbl.NidType.ToString(), lbl.TypeName,false));
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
            TempData["NidEditcategory"] = -1;
            if (categoryAdded)
                return Json(new JsonResults() { HasValue = true });
            else
                return Json(new JsonResults() { HasValue = false, Message = "unexcpected error" });

        }
        public ActionResult SyncCategoryTable()
        {
            dbTransfer = new DbTransfer();
            var categoryList = dbTransfer.GetAllCategories();
            return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_CategoryTable", categoryList) });
        }
        public ActionResult GetEditCategory(int NidCategory)
        {
            dbTransfer = new DbTransfer();
            Category category = dbTransfer.GetCategoryByNidCategory(NidCategory);
            List<Tuple<string, string,bool>> typelabels = new List<Tuple<string, string,bool>>();
            List<Tuple<string, string,bool>> brandlabels = new List<Tuple<string, string,bool>>();
            foreach (var lbl in category.Category_Types)
            {
                typelabels.Add(new Tuple<string, string,bool>(lbl.NidType.ToString(), lbl.TypeName,false));
            }
            foreach (var lbl in category.Category_Brands)
            {
                brandlabels.Add(new Tuple<string, string,bool>(lbl.NidBrand.ToString(), lbl.BrandName,true));
            }
            string types = RenderViewToString(this.ControllerContext, "_CategoryBrandAndTypeLabel",typelabels);
            string brands = RenderViewToString(this.ControllerContext, "_CategoryBrandAndTypeLabel", brandlabels);
            TempData["NidEditcategory"] = category.NidCategory;
            return Json(new JsonCategoryEdit() {  CategoryName = category.CategoryName, Description = category.Description, Keywords = category.keywords, NidCategory = category.NidCategory, BrandWrap = brands, TypeWrap = types});
        }
        public ActionResult DeleteCategory(int NidCategory)
        {
            dbTransfer = new DbTransfer();
            //check for brands and type and product
            if (dbTransfer.CheckForBrandByNidcategory(NidCategory) || dbTransfer.CheckForTypeByNidcategory(NidCategory) || dbTransfer.CheckForProductByNidcategory(NidCategory))
                return Json(new JsonResults() {  HasValue = false, Message = "type or brand or product included"});
            else
            {
                dbTransfer.Delete(dbTransfer.GetCategoryByNidCategory(NidCategory));
                if(dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "deleted successfully" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error in delete!" });
            }
        }
        public ActionResult DeleteType(Guid NidType)
        {
            dbTransfer = new DbTransfer();
            //check for product
            if (dbTransfer.CheckForProductByNidType(NidType))
                return Json(new JsonResults() { HasValue = false, Message = "product included" });
            else
            {
                dbTransfer.Delete(dbTransfer.GetCategoryTypeByNidType(NidType));
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "deleted successfully" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error in delete!" });
            }
        }
        public ActionResult DeleteBrand(Guid NidBrand)
        {
            dbTransfer = new DbTransfer();
            //check for product
            if (dbTransfer.CheckForProductByNidBrand(NidBrand))
                return Json(new JsonResults() { HasValue = false, Message = "product included" });
            else
            {
                dbTransfer.Delete(dbTransfer.GetCategoryBrandByNidBrand(NidBrand));
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "deleted successfully" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error in delete!" });
            }
        }
        public ActionResult CategoryDetail(int NidCategory)
        {
            dbTransfer = new DbTransfer();
            Category category = dbTransfer.GetCategoryByNidCategory(NidCategory);
            return Json(new JsonResults() {  HasValue = true, Html = RenderViewToString(this.ControllerContext, "_CategoryDetail", category)});
        }
        public ActionResult ManageTypeAndBrand(bool IsBrand)
        {
            int NidCategory = int.Parse(TempData["NidEditcategory"].ToString());
            dbTransfer = new DbTransfer();
            ViewModels.CategoryAndBrandViewModel cbvm = new ViewModels.CategoryAndBrandViewModel();
            if (NidCategory != -1)
            {
            Category category = dbTransfer.GetCategoryByNidCategory(NidCategory);
                if(IsBrand)
                {
                    cbvm.category_Brand = category.Category_Brands.ToList();
                    cbvm.IsBrand = true;
                }
                else
                {
                    cbvm.category_Type = category.Category_Types.ToList();
                    cbvm.IsBrand = false;
                }
                cbvm.NidCategory = category.NidCategory;
                cbvm.CategoryName = category.CategoryName;
            }

            return View(cbvm);
        }
        public ActionResult EditBrand(Guid NidBrand)
        {
            dbTransfer = new DbTransfer();
            Category_Brands brand = dbTransfer.GetCategoryBrandByNidBrand(NidBrand);
            return Json(new JsonBnTEdit() {  Description = brand.Description, Keywords = brand.Keywords, Name = brand.BrandName, Nid = brand.NidBrand.ToString()});
        }
        public ActionResult EditType(Guid NidType)
        {
            dbTransfer = new DbTransfer();
            Category_Types type = dbTransfer.GetCategoryTypeByNidType(NidType);
            return Json(new JsonBnTEdit() { Description = type.Description, Keywords = type.Keywords, Name = type.TypeName, Nid = type.NidType.ToString() });
        }
        public ActionResult ManageBrand(bool IsNewBrand,string Name,int NidCategory, string Description = "",string Keywords = "", string NidBrand = "")
        {
            dbTransfer = new DbTransfer();
            if(IsNewBrand)
            {
                Category_Brands cb = new Category_Brands() {  BrandName = Name, NidBrand = Guid.NewGuid(), Description = Description, Keywords = Keywords, NidCategory = NidCategory};
                dbTransfer.Add(cb);
                if (dbTransfer.Save())
                    return Json(new JsonResults() {  HasValue = true});
                else
                    return Json(new JsonResults() { HasValue = false,Message = "error in db" });
            }
            else
            {
                Category_Brands cb = dbTransfer.GetCategoryBrandByNidBrand(Guid.Parse(NidBrand));
                cb.BrandName = Name;cb.Description = Description;cb.Keywords = Keywords;
                dbTransfer.Update(cb);
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error in db" });
            }
        }
        public ActionResult ManageType(bool IsNewType, string Name, int NidCategory, string Description = "", string Keywords = "", string NidType = "")
        {
            dbTransfer = new DbTransfer();
            if (IsNewType)
            {
                Category_Types cb = new Category_Types() { TypeName = Name, NidType = Guid.NewGuid(), Description = Description, Keywords = Keywords, NidCategory = NidCategory };
                dbTransfer.Add(cb);
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error in db" });
            }
            else
            {
                Category_Types cb = dbTransfer.GetCategoryTypeByNidType(Guid.Parse(NidType));
                cb.TypeName = Name; cb.Description = Description; cb.Keywords = Keywords;
                dbTransfer.Update(cb);
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "error in db" });
            }
        }
        public ActionResult SyncBnTTable(bool IsBrand,int NidCategory)
        {
            ViewModels.CategoryAndBrandViewModel cbvm = new ViewModels.CategoryAndBrandViewModel();
            if (NidCategory != -1)
            {
                dbTransfer = new DbTransfer();
                if (IsBrand)
                {
                    cbvm.category_Brand = dbTransfer.GetCategoryByNidCategory(NidCategory).Category_Brands.ToList();
                    cbvm.IsBrand = true;
                }
                else
                {
                    cbvm.category_Type = dbTransfer.GetCategoryByNidCategory(NidCategory).Category_Types.ToList();
                    cbvm.IsBrand = false;
                }
            }
            else
                cbvm.IsBrand = true;
            return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_BnTTable", cbvm) });
        }

        //products
        public ActionResult Products()
        {
            dbTransfer = new DbTransfer();
            var prods = dbTransfer.GetAllProducts();
            return View(prods);
        }
        public ActionResult AddProduct()
        {
            return View();
        }
        public ActionResult EditProduct()
        {
            return View();
        }
        public ActionResult DeleteProduct()
        {
            return View();
        }
        //generals
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
    public class JsonCategoryEdit
    {
        public int NidCategory { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string BrandWrap { get; set; }
        public string TypeWrap { get; set; }
    }
    public class JsonBnTEdit
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Nid { get; set; }
    }
}