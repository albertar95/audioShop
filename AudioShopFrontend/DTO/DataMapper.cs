using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.DTO
{
    public class DataMapper
    {
        private readonly IMapper _mapper;
        public CategoryLiteDTO MapToCategoryLite(Category category)
        {
            try
            {
                return _mapper.Map<CategoryLiteDTO>(category);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}