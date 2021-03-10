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
        Product GetProductByProductId(Guid NidProduct);
        //orders
        List<Order> GetAllOrders(int pagesize = 10);
        Order GetOrderByNidOrder(Guid NidOrder);
        //ships
        List<Ship> GetAllDoneShips(int pagesize = 10);
        List<Ship> GetAllDoingShips(int pagesize = 10);
        Ship GetShipByNidShip(Guid NidShip);
        //users
        User GetUserByNidUser(Guid NidUser);
        List<User> GetAllUsers(int PageSize = 10);
        //comments
        Comment GetCommentByNidComment(Guid NidComment);
        List<Comment> GetAllComments(int Pagesize = 10, bool Accepted = false);
        //login
        Tuple<byte,User> Authenticate(string Username,string Password);
        //generals
        bool CheckForBrandByNidcategory(int NidCategory);
        bool CheckForTypeByNidcategory(int NidCategory);
        bool CheckForProductByNidcategory(int NidCategory);
        bool CheckForProductByNidBrand(Guid NidBrand);
        bool CheckForProductByNidType(Guid NidType);
        bool CheckForOrderByNidProduct(Guid NidProduct);
        bool CheckForUserExistance(string Username);
    }
}
