using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.DTO;
using AudioShopFrontend.Models;

namespace AudioShopFrontend.App_Start
{
    public class MapperConfig
    {
        public static MapperConfiguration Configure()
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => {
                cfg.CreateMap<Category, CategoryLiteDTO>();
                cfg.CreateMap<Product, ProductDTO>();
            });

            return mapperConfiguration;
        }
    }
}