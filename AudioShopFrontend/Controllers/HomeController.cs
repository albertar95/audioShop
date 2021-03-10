using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AudioShopFrontend.Services;
using AudioShopFrontend.DTO;
using AudioShopFrontend.ViewModels;
using AudioShopFrontend.Models;
using System.Web.Security;
using System.IO;

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
            ivm.LatestProducts = dataTransfer.GetLatestProducts();
            ivm.PopularProducts = dataTransfer.GetPopularProducts();//to edit
            ivm.SpecialProducts = dataTransfer.GetSpecialProducts();//to edit
            return View(ivm);
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
        public ActionResult Generals(int Typo)
        {
            List<ProductDTO> Products = new List<ProductDTO>();
            dataTransfer = new DataTransfer();
            switch (Typo)
            {
                case 1:
                    Products = dataTransfer.GetLatestProducts();
                    break;
                case 2:
                    Products = dataTransfer.GetSpecialProducts();
                    break;
                case 3:
                    Products = dataTransfer.GetSpecialProducts();
                    break;
                default:
                    break;
            }
            return View(Products);
        }
        public ActionResult Product(Guid NidProduct)
        {
            dataTransfer = new DataTransfer();
            var product = dataTransfer.GetProductByID(NidProduct);
            return View(product);
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult SubmitLogin(string Username,string Password,bool isPersistant)
        {
            dataTransfer = new DataTransfer();
            var User = dataTransfer.GetUserByUsername(Username);
            if(User != null)
            {
                if (DataTransfer.Encrypt(Password) == User.Password)
                {
                    FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(1, User.Username, DateTime.Now, DateTime.Now.AddMinutes(30), isPersistant, User.NidUser.ToString(), FormsAuthentication.FormsCookiePath);
                    string encTicket = FormsAuthentication.Encrypt(Ticket);
                    Response.Cookies.Add(new HttpCookie("AudioShopLogin", encTicket));
                    //Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddMinutes(30);
                    //Response.Cookies["AudioShopLogin"].HttpOnly = true;
                    //Response.Cookies["AudioShopLogin"].Secure = true;
                    return Json(new JsonResults() { HasValue = true });
                }
                else
                    return Json(new JsonResults() { HasValue = false, Message = "incorrect password" });
            }else
                return Json(new JsonResults() { HasValue = false,Message = "user not found" });
        }
        public ActionResult SubmitRegister(User User)
        {
            dataTransfer = new DataTransfer();
            if(!dataTransfer.CheckUsername(User.Username))
            {
                User.CreateDate = DateTime.Now;
                User.Enabled = true;
                User.IsAdmin = false;
                User.NidUser = Guid.NewGuid();
                User.Password = DataTransfer.Encrypt(User.Password);
                if(dataTransfer.AddUser(User))
                    return RedirectToAction("Index");
                else
                {
                    TempData["RegisterError"] = "error in database";
                    return View("Register");
                }
            }
            else
            {
                TempData["RegisterError"] = "username already exists";
                return RedirectToAction("Register");
            }
            
        }
        public ActionResult MyCart()
        {
            List<ProductDTO> prods = new List<ProductDTO>();
            dataTransfer = new DataTransfer();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData;
                if (Request.Cookies.AllKeys.Contains("AudioShopCart"))
                {
                    string[] cart = Request.Cookies["AudioShopCart"].Value.Split(',');
                    foreach (var f in cart)
                    {
                        prods.Add(dataTransfer.GetProductDtoByID(Guid.Parse(f)));
                    }
                }
            }
            return View(prods);
        }
        public ActionResult MyFavorites()
        {
            List<ProductDTO> prods = new List<ProductDTO>();
            dataTransfer = new DataTransfer();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData;
                if (Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                {
                    string[] fav = Request.Cookies["AudioShopFavorites"].Value.Split(',');
                    foreach (var f in fav)
                    {
                        prods.Add(dataTransfer.GetProductDtoByID(Guid.Parse(f)));
                    }
                }
            }
            return View(prods);
        }
        public ActionResult AddProductToFavorites(Guid NidProduct)
        {
            int count = 0;
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData;
                if(Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                {
                    string[] favs = Request.Cookies["AudioShopFavorites"].Value.Split(',');
                    if(!favs.Contains(NidProduct.ToString()))
                    {
                        Response.Cookies["AudioShopFavorites"].Value = Request.Cookies["AudioShopFavorites"].Value + "," + NidProduct.ToString();
                        count = favs.Count() + 1;
                    }
                    else
                    {
                        count = favs.Count();
                    }
                }
                else
                {
                    HttpCookie newCookie = new HttpCookie("AudioShopFavorites", NidProduct.ToString());
                    Response.Cookies.Add(newCookie);
                    count = 1;
                }
            }
            return Json(new JsonResults() {  HasValue = true,Html = count.ToString()});
        }
        public ActionResult AddProductToCart(Guid NidProduct)
        {
            int count = 0;
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData;
                if (Request.Cookies.AllKeys.Contains("AudioShopCart"))
                {
                    string[] carts = Request.Cookies["AudioShopCart"].Value.Split(',');
                    if (!carts.Contains(NidProduct.ToString()))
                    {
                        Response.Cookies["AudioShopCart"].Value = Request.Cookies["AudioShopCart"].Value + "," + NidProduct.ToString();
                        count = carts.Count() + 1;
                    }
                    else
                    {
                        count = carts.Count();
                    }
                }
                else
                {
                    HttpCookie newCookie = new HttpCookie("AudioShopCart", NidProduct.ToString());
                    Response.Cookies.Add(newCookie);
                    count = 1;
                }
            }
            return Json(new JsonResults() { HasValue = true, Html = count.ToString() });
        }
        public ActionResult MyProfile()
        {
            ProfileViewModel pvm = new ProfileViewModel();
            List<Order> Orders = new List<Order>();
            Models.User User = new User();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                dataTransfer = new DataTransfer();
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string NidUser = ticket.UserData;
                User = dataTransfer.GetUserByNidUser(Guid.Parse(NidUser));
                Orders = dataTransfer.GetUsersOrder(Guid.Parse(NidUser));
            }
            pvm.Orders = Orders;
            pvm.UserInfo = User;
            return View(pvm);
        }
        public ActionResult ChangePassword(string CurrentPassword,string NewPassword)
        {
            string message = "";
            bool IsUpdated = false;
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                dataTransfer = new DataTransfer();
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string NidUser = ticket.UserData;
                var CurrentUser = dataTransfer.GetUserByNidUser(Guid.Parse(NidUser));
                if (CurrentUser != null)
                {
                    if (CurrentUser.Password == DataTransfer.Encrypt(CurrentPassword))
                    {
                        CurrentUser.Password = DataTransfer.Encrypt(NewPassword);
                        if (dataTransfer.UpdateUser(CurrentUser))
                        {
                            message = "password updated successfully";
                            IsUpdated = true;
                        }
                        else
                            message = "error in database";
                    }
                    else
                        message = "current password doesnt meet!";
                }
                else
                    message = "user not found";
            }
            else
                message = "user not logined";

            return Json(new JsonResults() { HasValue = IsUpdated, Message = message });
        }
        public ActionResult ChangeAddress(string NewAddress)
        {
            string message = "";
            bool IsUpdated = false;
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                dataTransfer = new DataTransfer();
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string NidUser = ticket.UserData;
                var CurrentUser = dataTransfer.GetUserByNidUser(Guid.Parse(NidUser));
                if (CurrentUser != null)
                {
                    CurrentUser.Address = NewAddress;
                    if (dataTransfer.UpdateUser(CurrentUser))
                    {
                        message = "done";
                        IsUpdated = true;
                    }
                }
                else
                    message = "user not found";
            }
            else
                message = "user not logined";
            return Json(new JsonResults() { HasValue = IsUpdated, Message = message });
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult Blog()
        {
            return View();
        }
        public ActionResult SearchThis(string Text,int Nidcategory)
        {
            dataTransfer = new DataTransfer();
            var res = dataTransfer.SearchProduct(Text, Nidcategory);
            if(res.Count != 0)
            {
                return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_SearchResult", res) });
            }
            else
            {
                return Json(new JsonResults() { HasValue = false});
            }
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