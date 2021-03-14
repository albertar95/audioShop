using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AudioShopBackend.Models;
using System.Security.Cryptography;
using System.Text;

namespace AudioShopBackend.Services
{
    public class DbTransfer : IDbTransfer
    {
        static string hashkey { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";
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
            return db.Categories.Where(p => p.IsSubmmited == true).OrderByDescending(q => q.NidCategory).Take(PageSize).ToList();
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
            return db.Products.Where(p => p.State == 0).OrderByDescending(q => q.CreateDate).Take(pagesize).ToList();
        }

        public Product GetProductByProductId(Guid NidProduct)
        {
            return db.Products.Where(p => p.NidProduct == NidProduct).FirstOrDefault();
        }

        public bool CheckForOrderByNidProduct(Guid NidProduct)
        {
            return db.Orders.Where(p => p.NidProduct == NidProduct).Any();
        }

        public List<Order> GetAllOrders(int pagesize = 10)
        {
            return db.Orders.Where(p => p.state == 0).OrderByDescending(q => q.CreateDate).Take(pagesize).ToList();
        }

        public Order GetOrderByNidOrder(Guid NidOrder)
        {
            return db.Orders.Where(p => p.NidOrder == NidOrder && p.state == 0).FirstOrDefault();
        }

        public List<Ship> GetAllDoneShips(int pagesize = 10)
        {
            return db.Ships.Where(p => p.State == 1).OrderByDescending(q => q.CreateDate).Take(pagesize).ToList();
        }

        public List<Ship> GetAllDoingShips(int pagesize = 10)
        {
            return db.Ships.Where(p => p.State == 0).OrderByDescending(q => q.CreateDate).Take(pagesize).ToList();
        }

        public Ship GetShipByNidShip(Guid NidShip)
        {
            return db.Ships.Where(p => p.NidShip == NidShip).FirstOrDefault();
        }

        public User GetUserByNidUser(Guid NidUser)
        {
            return db.Users.Where(p => p.NidUser == NidUser).FirstOrDefault();
        }

        public List<User> GetAllUsers(int PageSize = 10)
        {
            return db.Users.Where(p => p.Enabled == true).OrderByDescending(q => q.CreateDate).Take(10).ToList();
        }

        public bool CheckForUserExistance(string Username)
        {
            return db.Users.Where(p => p.Username == Username).Any();
        }

        public Comment GetCommentByNidComment(Guid NidComment)
        {
            return db.Comments.Where(p => p.NidComment == NidComment).FirstOrDefault();
        }

        public List<Comment> GetAllComments(int Pagesize = 10,bool Accepted = false)
        {
            if(Accepted)
            return db.Comments.Where(p => p.State == 1).OrderByDescending(q => q.CreateDate).Take(Pagesize).ToList();
            else
                return db.Comments.Where(p => p.State == 0).Take(Pagesize).ToList();
        }

        public Tuple<byte,User> Authenticate(string Username, string Password)
        {
            if(db.Users.Where(p => p.Username.Trim().ToLower() == Username.Trim().ToLower()).Any())
            {
                var tmpUser = db.Users.Where(p => p.Username.Trim().ToLower() == Username.Trim().ToLower()).FirstOrDefault();
                if (tmpUser.Password == Encrypt(Password))
                    return new Tuple<byte, User>(0,tmpUser);
                else
                    return new Tuple<byte, User>(1, null);
            }else
                return new Tuple<byte, User>(2, null);
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
    }
}