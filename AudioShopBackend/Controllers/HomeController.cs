using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AudioShopBackend.Services;
using AudioShopBackend.Models;
using AudioShopBackend.ViewModels;
using System.Web.Security;

namespace AudioShopBackend.Controllers
{
    public class HomeController : Controller
    {
        DbTransfer dbTransfer = null;
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult SignIn(string Username,string Password,bool IsChecked)
        {
            dbTransfer = new DbTransfer();
            Tuple<byte, User> result = dbTransfer.Authenticate(Username,Password);
            if(result.Item1 == 1)
                return Json(new JsonResults() {  HasValue = false, Message = "رمز عبور اشتباه است"});
            else if (result.Item1 == 2)
                return Json(new JsonResults() { HasValue = false, Message = "نام کاربری یافت نشد" });
            else if (result.Item1 == 0)
            {
                FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(1, result.Item2.Username, DateTime.Now, DateTime.Now.AddMinutes(30), IsChecked, result.Item2.NidUser.ToString(),FormsAuthentication.FormsCookiePath);
                string encTicket = FormsAuthentication.Encrypt(Ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                return Json(new JsonResults() { HasValue = true});
            }
            else
                return Json(new JsonResults() { HasValue = false, Message = "خطای ناشناخته" });
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        //category section
        public ActionResult Categories()
        {
            dbTransfer = new DbTransfer();
            var categoryList = dbTransfer.GetAllCategories(100);
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
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در اضافه کردن دسته بندی" });
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
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در اضافه کردن برند" });
            }
            else
            {
                Category_Types categoryType = new Category_Types() { NidType = Guid.NewGuid(), NidCategory = tmpNidcategory, TypeName = Name };
                dbTransfer.Add(categoryType);
                if (dbTransfer.Save())
                    typeAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در اضافه کردن نوع" });
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
                return Json(new JsonResults() { HasValue = false, Message = "خطای ناشناخته" });
        }
        public ActionResult AddCategory(bool IsNewCategory,string Name,string CategoryDescription,string categoryKeywords, int NidCategory = 0,string pictures = "")
        {
            bool categoryAdded = false;
            dbTransfer = new DbTransfer();
            if(IsNewCategory)
            {
                Category category = new Category() { NidCategory = dbTransfer.GenerateNewNidCategory(), CategoryName = Name, IsSubmmited = true, Description = CategoryDescription, keywords = categoryKeywords, Pictures = pictures };
                dbTransfer.Add(category);
                if (dbTransfer.Save())
                    categoryAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در اضافه کردن دسته بندی" });
            }
            else
            {
                Category category = dbTransfer.GetCategoryByNidCategory(NidCategory);
                category.IsSubmmited = true;
                category.CategoryName = Name;
                category.Description = CategoryDescription;
                category.keywords = categoryKeywords;
                category.Pictures = pictures;
                dbTransfer.Update(category);
                if (dbTransfer.Save())
                    categoryAdded = true;
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در اضافه کردن دسته بندی" });
            }
            TempData["NidEditcategory"] = -1;
            if (categoryAdded)
                return Json(new JsonResults() { HasValue = true });
            else
                return Json(new JsonResults() { HasValue = false, Message = "خطای ناشناخته" });

        }
        public ActionResult SyncCategoryTable()
        {
            dbTransfer = new DbTransfer();
            var categoryList = dbTransfer.GetAllCategories(100);
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
            string pics = RenderViewToString(this.ControllerContext, "_UploadedImages", category.Pictures.Split(',').ToList());
            TempData["NidEditcategory"] = category.NidCategory;
            return Json(new JsonCategoryEdit() {  CategoryName = category.CategoryName, Description = category.Description, Keywords = category.keywords, NidCategory = category.NidCategory, BrandWrap = brands, TypeWrap = types, PicturesWrap = pics, Pictures = category.Pictures});
        }
        public ActionResult DeleteCategory(int NidCategory)
        {
            dbTransfer = new DbTransfer();
            //check for brands and type and product
            if (dbTransfer.CheckForBrandByNidcategory(NidCategory) || dbTransfer.CheckForTypeByNidcategory(NidCategory) || dbTransfer.CheckForProductByNidcategory(NidCategory))
                return Json(new JsonResults() {  HasValue = false, Message = "حذف امکان پذیر نمی باشد.دسته بندی دارای محصول یا برند یا نوع می باشد"});
            else
            {
                dbTransfer.Delete(dbTransfer.GetCategoryByNidCategory(NidCategory));
                if(dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "دسته بندی با موفقیت حذف گردید" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در حذف" });
            }
        }
        public ActionResult DeleteType(Guid NidType)
        {
            dbTransfer = new DbTransfer();
            //check for product
            if (dbTransfer.CheckForProductByNidType(NidType))
                return Json(new JsonResults() { HasValue = false, Message = "این نوع دارای محصول می باشد" });
            else
            {
                dbTransfer.Delete(dbTransfer.GetCategoryTypeByNidType(NidType));
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "نوع با موفقیت حذف گردید" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در حذف" });
            }
        }
        public ActionResult DeleteBrand(Guid NidBrand)
        {
            dbTransfer = new DbTransfer();
            //check for product
            if (dbTransfer.CheckForProductByNidBrand(NidBrand))
                return Json(new JsonResults() { HasValue = false, Message = "این برند دارای محصول می باشد" });
            else
            {
                dbTransfer.Delete(dbTransfer.GetCategoryBrandByNidBrand(NidBrand));
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "برند با موفقیت حذف گردید" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در حذف" });
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
            CategoryAndBrandViewModel cbvm = new CategoryAndBrandViewModel();
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
                    return Json(new JsonResults() { HasValue = false,Message = "خطا در دیتابیس" });
            }
            else
            {
                Category_Brands cb = dbTransfer.GetCategoryBrandByNidBrand(Guid.Parse(NidBrand));
                cb.BrandName = Name;cb.Description = Description;cb.Keywords = Keywords;
                dbTransfer.Update(cb);
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در دیتابیس" });
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
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در دیتابیس" });
            }
            else
            {
                Category_Types cb = dbTransfer.GetCategoryTypeByNidType(Guid.Parse(NidType));
                cb.TypeName = Name; cb.Description = Description; cb.Keywords = Keywords;
                dbTransfer.Update(cb);
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در دیتابیس" });
            }
        }
        public ActionResult SyncBnTTable(bool IsBrand,int NidCategory)
        {
            CategoryAndBrandViewModel cbvm = new CategoryAndBrandViewModel();
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
        public ActionResult GetBnTOptions(int NidCategory,bool IsBrand)
        {
            dbTransfer = new DbTransfer();
            CategoryAndBrandViewModel cbvm = new CategoryAndBrandViewModel();
            if(IsBrand)
                cbvm.category_Brand = dbTransfer.GetCategoryBrandsByNidCategory(NidCategory);
            else
                cbvm.category_Type = dbTransfer.GetCategoryTypesByNidCategory(NidCategory);
            cbvm.IsBrand = IsBrand;
            return Json(new JsonResults() {  HasValue = true, Html = RenderViewToString(this.ControllerContext, "_BnTOptions", cbvm)});
        }
        //products
        public ActionResult Products()
        {
            dbTransfer = new DbTransfer();
            var prods = dbTransfer.GetAllProducts(100);
            return View(prods);
        }
        public ActionResult AddProduct()
        {
            ProductViewModel pvm = new ProductViewModel();
            dbTransfer = new DbTransfer();
            pvm.Categories = dbTransfer.GetAllCategories(100);
            return View(pvm);
        }
        public ActionResult EditProduct(Guid NidProduct)
        {
            ProductViewModel pvm = new ProductViewModel();
            dbTransfer = new DbTransfer();
            Product product = dbTransfer.GetProductByProductId(NidProduct);
            pvm.Categories = dbTransfer.GetAllCategories();
            pvm.Category_Brands = dbTransfer.GetCategoryBrandsByNidCategory(product.NidCategory);
            pvm.category_Types = dbTransfer.GetCategoryTypesByNidCategory(product.NidCategory);
            pvm.Product = product;
            return View(pvm);
        }
        [ValidateInput(false)]
        public ActionResult SubmitEditProduct(ProductViewModel pvm)
        {
            dbTransfer = new DbTransfer();
            pvm.Product.LastModified = DateTime.Now;
            dbTransfer.Update(pvm.Product);
            if(dbTransfer.Save())
            {
                TempData["SucessfullAddProduct"] = "محصول با موفقیت ویرایش گردید";
                return RedirectToAction("Products");
            }
            else
            {
                TempData["ErrorEditProduct"] = "خطا در دیتابیس";
                return RedirectToAction("EditProduct",new { NidProduct = pvm.Product.NidProduct});
            }
        }
        public ActionResult DeleteProduct(Guid NidProduct)
        {
            dbTransfer = new DbTransfer();
            //check for orders
            if (dbTransfer.CheckForOrderByNidProduct(NidProduct))
                return Json(new JsonResults() { HasValue = false, Message = "این محصول دارای سفارش می باشد" });
            else
            {
                dbTransfer.Delete(dbTransfer.GetProductByProductId(NidProduct));
                if (dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "محصول با موفقیت حذف گردید" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در حذف" });
            }
        }
        [ValidateInput(false)]
        public ActionResult SubmitAddProduct(ProductViewModel pvm)
        {
            pvm.Product.CreateDate = DateTime.Now;
            pvm.Product.NidProduct = Guid.NewGuid();
            dbTransfer = new DbTransfer();
            dbTransfer.Add(pvm.Product);
            if(dbTransfer.Save())
            {
                TempData["SucessfullAddProduct"] = "محصول با موفقیت اضافه شد";
                return RedirectToAction("Products");
            }
            else
            {
                TempData["ErrorAddProduct"] = "خطا در دیتابیس";
                return RedirectToAction("AddProduct");
            }
        }
        //orders
        public ActionResult Orders()
        {
            dbTransfer = new DbTransfer();
            var orders = dbTransfer.GetAllOrders(1000);
            return View(orders);
        }
        public ActionResult OrderDetail(Guid NidOrder)
        {
            dbTransfer = new DbTransfer();
            Order order = dbTransfer.GetOrderByNidOrder(NidOrder);
            return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_OrderDetail", order) });
        }
        //ships
        public ActionResult Ships()
        {
            dbTransfer = new DbTransfer();
            ShipsViewModel svm = new ShipsViewModel();
            svm.Doing = dbTransfer.GetAllDoingShips(100);
            svm.Done = dbTransfer.GetAllDoneShips(100);
            return View(svm);
        }
        public ActionResult ShipDetail(Guid NidShip)
        {
            dbTransfer = new DbTransfer();
            Ship ship = dbTransfer.GetShipByNidShip(NidShip);
            return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_ShipDetail", ship) });
        }
        public ActionResult AcceptShip(Guid NidShip)
        {
            dbTransfer = new DbTransfer();
            Ship current = dbTransfer.GetShipByNidShip(NidShip);
            current.State = 1;
            dbTransfer.Update(current);
            if (dbTransfer.Save())
                return Json(new JsonResults() { HasValue = true, Message = "با موفقیت تایید شد" });
            else
                return Json(new JsonResults() { HasValue = false, Message = "خطا در تایید" });
        }
        public ActionResult SyncShipsTable(bool IsShipDone)
        {
            dbTransfer = new DbTransfer();
            ShipsViewModel svm = new ShipsViewModel();
            if (IsShipDone)
                svm.Done = dbTransfer.GetAllDoneShips(100);
            else
                svm.Doing = dbTransfer.GetAllDoingShips(100);
            svm.IsDone = IsShipDone;
            return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_ShipTable", svm) });
        }
        //users
        public ActionResult Users()
        {
            dbTransfer = new DbTransfer();
            var userList = dbTransfer.GetAllUsers(100);
            return View("Users", userList);
        }
        public ActionResult AddUser(string Username, string Password, string Name = "", string LastName = "")
        {
            dbTransfer = new DbTransfer();
            if (!dbTransfer.CheckForUserExistance(Username.Trim()))
            {
                dbTransfer.Add(new User() {  NidUser = Guid.NewGuid(), CreateDate = DateTime.Now, Enabled = true, FirstName = Name, LastName = LastName, IsAdmin = true, Password = DbTransfer.Encrypt(Password), Username = Username});
                if(dbTransfer.Save())
                    return Json(new JsonResults() { HasValue = true, Message = "کاربر با موفقیت ایجاد شد" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در دیتابیس" });
            }
            else
            {
                return Json(new JsonResults() { HasValue = false, Message = "این نام کاربری موجود می باشد" });
            }
        }
        public ActionResult SyncUserTable()
        {
            dbTransfer = new DbTransfer();
            var userlist = dbTransfer.GetAllUsers(100);
            return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_UserTable", userlist) });
        }
        public ActionResult DeleteUser(Guid NidUser)
        {
            dbTransfer = new DbTransfer();
            User u = dbTransfer.GetUserByNidUser(NidUser);
            u.Enabled = false;
            dbTransfer.Update(u);
            if (dbTransfer.Save())
                return Json(new JsonResults() { HasValue = true, Message = "کاربر با موفقیت حذف گردید" });
            else
                return Json(new JsonResults() { HasValue = false, Message = "خطا در حذف" });
        }
        //comments
        public ActionResult Comments()
        {
            dbTransfer = new DbTransfer();
            var comments = dbTransfer.GetAllComments(100);
            TempData["Comments"] = "0";
            return View(comments);
        }
        public ActionResult AcceptedComments()
        {
            dbTransfer = new DbTransfer();
            var comments = dbTransfer.GetAllComments(100, true);
            TempData["Comments"] = "1";
            return View("Comments",comments);
        }
        public ActionResult DeleteComment(Guid NidComment)
        {
            dbTransfer = new DbTransfer();
            var cmt = dbTransfer.GetCommentByNidComment(NidComment);
            cmt.State = 2;
            dbTransfer.Update(cmt);
            if (dbTransfer.Save())
                return Json(new JsonResults() { HasValue = true, Message = "نظر با موفقیت حذف گردید" });
            else
                return Json(new JsonResults() { HasValue = false, Message = "خطا در دیتابیس" });
        }
        public ActionResult AcceptComment(Guid NidComment)
        {
            dbTransfer = new DbTransfer();
            var cmt = dbTransfer.GetCommentByNidComment(NidComment);
            cmt.State = 1;
            dbTransfer.Update(cmt);
            if (dbTransfer.Save())
                return Json(new JsonResults() { HasValue = true, Message = "نظر با موفقیت تایید شد" });
            else
                return Json(new JsonResults() { HasValue = false, Message = "خطا در تایید" });
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
        [HttpPost]
        public ActionResult UploadFiles()
        {
            List<string> Uploaded = new List<string>();
            if (Request.Files.Count != 0)
            {

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    var fileName = "Image_" + DateTime.Now.ToShortDateString().Replace('/','_') + "_" + DateTime.Now.ToShortTimeString().Replace(':', '_') + "_" + Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    file.SaveAs(path);
                    Uploaded.Add("http://localhost:9000/Uploads/" + fileName);
                }
                return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext,"_UploadedImages",Uploaded), Message = string.Join(",",Uploaded) });
            }
            else
                return Json(new JsonResults() {  HasValue = false});

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
        public string PicturesWrap { get; set; }
        public string Pictures { get; set; }
    }
    public class JsonBnTEdit
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Nid { get; set; }
    }
}