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
                    Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddMinutes(30);
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
            return View();
        }
        public ActionResult MyFavorites()
        {
            HttpCookie myuserCookie = Request.Cookies["AudioShopLogin"];
            //HttpCookie myuserCookie2 = System.Web.HttpContext.Current.Response.Cookies["AudioShopLogin"];
            if(myuserCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(myuserCookie.Value);
                string niduser = ticket.UserData;
                string username = ticket.Name;
            }
            return View();
        }
        public ActionResult AddProductToFavorites(Guid NidProduct)
        {
            HttpCookie myuserCookie = Response.Cookies["AudioShopLogin"];
            int count = 0;
            //HttpCookie myuserCookie2 = System.Web.HttpContext.Current.Response.Cookies["AudioShopLogin"];
            if (myuserCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(myuserCookie.Value);
                string niduser = ticket.UserData;
                HttpCookie favoritesCookie = Request.Cookies["AudioShopFavorites"];
                if(favoritesCookie != null)
                {
                    string[] favs = favoritesCookie.Value.Split(',');
                    if(!favs.Contains(NidProduct.ToString()))
                    {
                        Response.Cookies["AudioShopFavorites"].Value += "," + NidProduct.ToString();
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
                    count = 1;
                }
            }
            return Json(new JsonResults() {  HasValue = true,Html = count.ToString()});
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