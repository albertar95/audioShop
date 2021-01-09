using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.Services
{
    interface IDataTransfer
    {
        //index
        List<CategoryLiteDTO> GetcategoryList();
        //category
        Category GetCategoryByNidCategory(int Nidcategory);
        Category GetCategoryByCategoryName(string CategoryName);
    }
}
