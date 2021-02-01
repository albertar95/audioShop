using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AudioShopFrontend.ViewModels
{
    public class SharedLayotViewModel
    {
        public static int GetUserFavoritesCount()
        {
            try
            {
                if(HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin") && HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                {
                    return HttpContext.Current.Request.Cookies["AudioShopFavorites"].Value.Split(',').Count();
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }
        public static int GetUserCartCount()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin") && HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopCart"))
                {
                    return HttpContext.Current.Request.Cookies["AudioShopCart"].Value.Split(',').Count();
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

    }

}