using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.ViewModels
{
    public class IndexViewModel
    {
        public List<CategoryLiteDTO> Categories { get; set; }
        public List<ProductDTO> LatestProducts { get; set; }
        public List<ProductDTO> SpecialProducts { get; set; }
        public List<ProductDTO> PopularProducts { get; set; }
    }
}