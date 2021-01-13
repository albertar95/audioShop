using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;
using AudioShopFrontend.App_Start;

namespace AudioShopFrontend.DTO
{
    public class DataMapper
    {
         static MapperConfiguration config = MapperConfig.Configure();

        //build the mapper
         IMapper mapper = config.CreateMapper();
        public  CategoryLiteDTO MapToCategoryLite(Category category)
        {
            try
            {
                return mapper.Map<CategoryLiteDTO>(category);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public ProductDTO MapToProductDto(Product product)
        {
            try
            {
                return mapper.Map<ProductDTO>(product);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}