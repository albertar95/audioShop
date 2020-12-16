using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioShopBackend.Models;

namespace AudioShopBackend.Services
{
    interface IDbTransfer
    {
        //general
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        bool Save();

        //Categories
        Category GetCategoryByNidCategory(int NidCategory);
        List<Category> GetAllCategories(int PageSize = 10,bool IncludeTypes = false, bool IncludeBrands = false);
        //sub-section : Brand 
        List<Category_Brands> GetCategoryBrandsByNidCategory(int NidCategory,int PageSize);
        Category_Brands GetCategoryBrandByNidBrand(Guid NidBrand);
        //sub-section : type 
        List<Category_Types> GetCategoryTypesByNidCategory(int NidCategory, int PageSize);
        Category_Types GetCategoryTypeByNidType(Guid NidType);

        //products
        List<Product> GetAllProducts(int pagesize = 10);
        //generals
        bool CheckForBrandByNidcategory(int NidCategory);
        bool CheckForTypeByNidcategory(int NidCategory);
        bool CheckForProductByNidcategory(int NidCategory);
        bool CheckForProductByNidBrand(Guid NidBrand);
        bool CheckForProductByNidType(Guid NidType);
    }
}
