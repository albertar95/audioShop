using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.DTO;
using AudioShopFrontend.Models;

namespace AudioShopFrontend.ViewModels
{
    public class ProfileViewModel
    {
        public List<Order> Orders { get; set; }
        public User UserInfo { get; set; }
    }
}