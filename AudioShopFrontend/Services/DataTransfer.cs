using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.Services
{
    public class DataTransfer : IDataTransfer
    {
        ASDbEntities db = new ASDbEntities();
        DataMapper mapper = new DataMapper();

        public Category GetCategoryByCategoryName(string CategoryName)
        {
            return db.Categories.Where(p => p.CategoryName == CategoryName).FirstOrDefault();
        }

        public Category GetCategoryByNidCategory(int Nidcategory)
        {
            return db.Categories.Where(p => p.NidCategory == Nidcategory).FirstOrDefault();
        }

        public List<CategoryLiteDTO> GetcategoryList()
        {
            List<CategoryLiteDTO> result = new List<CategoryLiteDTO>();
            foreach (var cat in db.Categories.Where(p => p.Issubmmited == true))
            {
                result.Add(mapper.MapToCategoryLite(cat));
            }
            return result;
        }
    }
}