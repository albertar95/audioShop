using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;

namespace AudioShopFrontend.ViewModels
{
    public class CategoryViewModel
    {
        public List<Category_Brands> CategoryBrands { get; set; }
        public List<Category_Types> CategoryTypes { get; set; }
        public List<Product> Products { get; set; }
    }
}