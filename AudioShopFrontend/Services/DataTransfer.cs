using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;

namespace AudioShopFrontend.Services
{
    public class DataTransfer : IDataTransfer
    {
        ASDbEntities db = new ASDbEntities();
        DataMapper mapper = new DataMapper();
        static string hashkey { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

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
            foreach (var cat in db.Categories.Where(p => p.IsSubmmited == true))
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

        public Product GetProductByID(Guid NidProduct)
        {
            return db.Products.Where(p => p.NidProduct == NidProduct).FirstOrDefault();
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

        public User GetUserByUsername(string Username)
        {
            try
            {
                return db.Users.Where(p => p.Username == Username).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string s = ex.InnerException.Message;
                return null;
            }
        }

        public static string Encrypt(string text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hashkey));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string Decrypt(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hashkey));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        public bool CheckUsername(string Username)
        {
            return db.Users.Where(p => p.Username == Username).Any();
        }

        public bool AddUser(User User)
        {
            db.Users.Add(User);
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public List<ProductDTO> GetUserFavorites(Guid UserId)
        {
            return null;
        }

        public ProductDTO GetProductDtoByID(Guid NidProduct)
        {
            return mapper.MapToProductDto(db.Products.Where(p => p.NidProduct == NidProduct).FirstOrDefault());
        }

        public List<ProductDTO> SearchProduct(string input, int Nidcategory = 0)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            if(Nidcategory != 0)
            { 
            foreach (var sr in db.Products.Where(p => p.Category.NidCategory == Nidcategory && p.ProductName.Contains(input)).Take(3))
            {
                result.Add(mapper.MapToProductDto(sr));
            }
            }
            else
            {
                foreach (var sr in db.Products.Where(p => p.ProductName.Contains(input)).Take(3))
                {
                    result.Add(mapper.MapToProductDto(sr));
                }
            }
            return result;
        }

        public User GetUserByNidUser(Guid NidUser)
        {
            try
            {
                return db.Users.Where(p => p.NidUser == NidUser).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Order> GetUsersOrder(Guid NidUser)
        {
            try
            {
                return db.Orders.Where(p => p.NidUser == NidUser).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}