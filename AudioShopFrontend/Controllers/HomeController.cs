﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AudioShopFrontend.Services;
using AudioShopFrontend.DTO;
using AudioShopFrontend.ViewModels;
using AudioShopFrontend.Models;

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
        public ActionResult SubmitLogin(string Username,string Password,bool isPersistant)
        {
            dataTransfer = new DataTransfer();
            var User = dataTransfer.GetUserByUsername(Username);
            if(User != null)
            {
                if (DataTransfer.Encrypt(Password) == User.Password)
                    return Json(new JsonResults() {  HasValue =true});
                else
                    return Json(new JsonResults() { HasValue = false,Message = "incorrect password" });
            }else
                return Json(new JsonResults() { HasValue = false,Message = "user not found" });
        }
        public ActionResult SubmitRegister(User User)
        {
            return RedirectToAction("Index");
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