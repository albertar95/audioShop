using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AudioShopBackend.Models;

namespace AudioShopBackend.Services
{
    public class DbTransfer : IDbTransfer
    {
        ASDbEntities db = new ASDbEntities();
        public void Add<T>(T entity) where T : class
        {
            db.Entry(entity).State = System.Data.Entity.EntityState.Added;
        }

        public void Delete<T>(T entity) where T : class
        {
            db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
        }

        public bool Save()
        {
            if (db.SaveChanges() > 0)
                return true;
            else
                return false;
        }

        public void Update<T>(T entity) where T : class
        {
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public List<Category_Brands> GetCategoryBrandsByNidCategory(int NidCategory, int PageSize = 10)
        {
            return db.Category_Brands.Where(p => p.NidCategory == NidCategory).Take(PageSize).ToList();
        }
        public int GetAddedCategory(string Name)
        {
            return db.Categories.Where(p => p.IsSubmmited == false && p.CategoryName == Name).FirstOrDefault().NidCategory;
        }
        public int GenerateNewNidCategory()
        {
            var res = db.Categories.OrderByDescending(p => p.NidCategory).FirstOrDefault();
            if (res != null)
                return res.NidCategory + 1;
            else
                return 1;
        }

        public List<Category_Types> GetCategoryTypesByNidCategory(int NidCategory, int PageSize = 10)
        {
            return db.Category_Types.Where(p => p.NidCategory == NidCategory).Take(PageSize).ToList();
        }

        public Category GetCategoryByNidCategory(int NidCategory)
        {
            return db.Categories.Where(p => p.NidCategory == NidCategory).FirstOrDefault();
        }

        public List<Category> GetAllCategories(int PageSize = 10, bool IncludeTypes = false, bool IncludeBrands = false)
        {
            return db.Categories.Where(p => p.IsSubmmited == true).Take(PageSize).ToList();
        }
    }
}