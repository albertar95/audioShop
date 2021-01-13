using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;
using AutoMapper;

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

        public List<ProductDTO> GetLatestProducts(int pagesize = 10)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            foreach (var pro in db.Products.Where(p => p.State == 0).OrderByDescending(q => q.CreateDate).Take(pagesize))
            {
                result.Add(mapper.MapToProductDto(pro));
            }
            return result;
        }

        public List<ProductDTO> GetPopularProducts(int pagesize = 10)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            foreach (var pro in db.Products.Where(p => p.State == 0).OrderByDescending(q => q.CreateDate).Take(pagesize))
            {
                result.Add(mapper.MapToProductDto(pro));
            }
            return result;
        }

        public List<ProductDTO> GetSpecialProducts(int pagesize = 10)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            foreach (var pro in db.Products.Where(p => p.State == 0).OrderByDescending(q => q.CreateDate).Take(pagesize))
            {
                result.Add(mapper.MapToProductDto(pro));
            }
            return result;
        }
    }
}