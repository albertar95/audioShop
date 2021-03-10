using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopBackend.Models;

namespace AudioShopBackend.ViewModels
{
    public class CategoryAndBrandViewModel
    {
        public List<Category_Brands> category_Brand { get; set; }
        public List<Category_Types> category_Type { get; set; }
        public bool IsBrand { get; set; }
        public int NidCategory { get; set; }
        public string CategoryName { get; set; }
    }
}