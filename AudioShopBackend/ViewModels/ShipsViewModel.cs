using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopBackend.Models;

namespace AudioShopBackend.ViewModels
{
    public class ShipsViewModel
    {
        public List<Ship> Done { get; set; }
        public List<Ship> Doing { get; set; }
        public bool IsDone { get; set; }
    }
}