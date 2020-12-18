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
            try
            {
                if (db.SaveChanges() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {

                return false;
            }
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

        public bool CheckForBrandByNidcategory(int NidCategory)
        {
            return db.Category_Brands.Where(p => p.NidCategory == NidCategory).Any();
        }

        public bool CheckForTypeByNidcategory(int NidCategory)
        {
            return db.Category_Types.Where(p => p.NidCategory == NidCategory).Any();
        }

        public bool CheckForProductByNidcategory(int NidCategory)
        {
            return db.Products.Where(p => p.NidCategory == NidCategory).Any();
        }

        public bool CheckForProductByNidBrand(Guid NidBrand)
        {
            return db.Products.Where(p => p.NidBrand == NidBrand).Any();
        }

        public bool CheckForProductByNidType(Guid NidType)
        {
            return db.Products.Where(p => p.NidType == NidType).Any();
        }

        public Category_Brands GetCategoryBrandByNidBrand(Guid NidBrand)
        {
            return db.Category_Brands.Where(p => p.NidBrand == NidBrand).FirstOrDefault();
        }

        public Category_Types GetCategoryTypeByNidType(Guid NidType)
        {
            return db.Category_Types.Where(p => p.NidType == NidType).FirstOrDefault();
        }

        public List<Product> GetAllProducts(int pagesize = 10)
        {
            return db.Products.Where(p => p.State == 0).Take(pagesize).ToList();
        }

        public Product GetProductByProductId(Guid NidProduct)
        {
            return db.Products.Where(p => p.NidProduct == NidProduct).FirstOrDefault();
        }

        public bool CheckForOrderByNidProduct(Guid NidProduct)
        {
            return db.Orders.Where(p => p.NidProduct == NidProduct).Any();
        }
    }
}