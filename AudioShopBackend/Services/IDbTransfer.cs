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
        //sub-section : Brand 
        List<Category_Brands> GetCategoryBrandsByNidCategory(int NidCategory,int PageSize);
        //sub-section : type 
        List<Category_Types> GetCategoryTypesByNidCategory(int NidCategory, int PageSize);
    }
}
