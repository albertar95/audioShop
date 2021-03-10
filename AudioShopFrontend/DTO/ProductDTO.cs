using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AudioShopFrontend.DTO
{
    public class ProductDTO
    {
        public System.Guid NidProduct { get; set; }
        public string ProductName { get; set; }
        public int NidCategory { get; set; }
        public System.Guid NidBrand { get; set; }
        public System.Guid NidType { get; set; }
        public string Pictures { get; set; }
        public decimal Price { get; set; }
        public byte State { get; set; }
    }
}