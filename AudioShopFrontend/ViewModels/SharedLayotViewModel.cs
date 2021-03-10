using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.DTO;

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
        public static List<ProductDTO> GetCurrentCartItems()
        {
            List<ProductDTO> result = new List<ProductDTO>();
            if(HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin") && HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopCart"))
            {
                Services.DataTransfer dataTransfer = new Services.DataTransfer();
                var cookieValue = HttpContext.Current.Request.Cookies["AudioShopCart"].Value;
                string[] products = cookieValue.Split(',');
                foreach (var pro in products)
                {
                    result.Add(dataTransfer.GetProductDtoByID(Guid.Parse(pro)));
                }
            }
            return result;
        }
        public static List<CategoryLiteDTO> GetCategories()
        {
            try
            {
                Services.DataTransfer dataTransfer = new Services.DataTransfer();
                return dataTransfer.GetcategoryList();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

}