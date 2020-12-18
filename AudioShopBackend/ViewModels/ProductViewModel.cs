using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopBackend.Models;

namespace AudioShopBackend.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }
        public List<Category_Brands> Category_Brands { get; set; }
        public List<Category_Types> category_Types { get; set; }
    }
}