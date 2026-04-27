using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Mapping
{
    public class MapsterConfig
    {
        public static void MapsterConfigRegister()
        {
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig().Map(dest => dest.category_Id,
                source => source.Id).
                Map(dest => dest.UserCreated,
                source => source.CreatedBy.UserName).Map(dest => dest.Name, source => source.
                Translations.Where(t => t.Language == CultureInfo.CurrentCulture.Name).Select(t => t.Name).FirstOrDefault());



            TypeAdapterConfig<DAL.Models.Product, ProductResponse>.NewConfig().
              Map(dest => dest.UserCreated,
              source => source.CreatedBy.UserName).Map(dest => dest.Name, source => source.
              Translations.Where(t => t.Language == CultureInfo.CurrentCulture.Name).Select(t => t.Name).FirstOrDefault()).Map(dest => dest.MainImage, source
              => $"https://localhost:7175/images/{source.MainImage}").
              Map(dest => dest.SubImages, src => src.Images.Select(i => $"https://localhost:7175/images/{i.ImagePath}"));
            

            TypeAdapterConfig<ProductUpdateRequest, DAL.Models.Product>.NewConfig().IgnoreNullValues(true);

            TypeAdapterConfig<Brand, BrandResponse>.NewConfig().
              Map(dest => dest.UserCreated,
              source => source.CreatedBy.UserName).Map(dest => dest.Name, source => source.
              Translations.Where(t => t.Language == CultureInfo.CurrentCulture.Name).Select(t => t.Name).FirstOrDefault()).Map(dest => dest.Logo, source
              => $"https://localhost:7175/images/{source.Logo}");

            TypeAdapterConfig<Cart , CartResponse>.NewConfig().Map(dest=>dest.ProductName , source=>source.Product.Translations.Where(
                t=>t.Language ==CultureInfo.CurrentCulture.Name).Select(t=>t.Name).FirstOrDefault()).Map(dest=>dest.Price , source=>source.Product.Price
                ).Map(dest=>dest.ProductImage, source
              => $"https://localhost:7175/images/{source.Product.MainImage}");

        }
    }
}
