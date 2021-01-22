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
        List<ProductDTO> GetLatestProducts(int pagesize = 10);
        List<ProductDTO> GetPopularProducts(int pagesize = 10);
        List<ProductDTO> GetSpecialProducts(int pagesize = 10);
        //product
        Product GetProductByID(Guid NidProduct);
        //User
        User GetUserByUsername(string Username);
        bool CheckUsername(string Username);
        bool AddUser(User User);
    }
}
