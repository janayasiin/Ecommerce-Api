using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductService(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task CreateProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();

            if (request.MainImage != null)
            {
                var imagePath = await _fileService.UploadASync(request.MainImage);
                product.MainImage = imagePath;
            }
            if (product.Images != null) {

                foreach (var image in request.SubImages)
                {
                    var imagePath = await _fileService.UploadASync(image);
                    product.Images.Add(new ProductImage { ImagePath = imagePath });
                }

            }
            await _productRepository.CreateAsync(product);
        }
        public async Task<List<ProductResponse>> GetAllProductsAsync()
        {

            var products = await _productRepository.GetAllAsync(p => p.Status == EntityStatus.Active, new string[]
            {
            nameof(Product.Translations) , nameof(Product.CreatedBy) , "Images"
            });

            return products.Adapt<List<ProductResponse>>();



        }

        public async Task<ProductResponse?> GetProduct(Expression<Func<Product, bool>> filter)
        {
            var product = await _productRepository.GetOne(filter, new string[]
        {
            nameof(Product.Translations) , nameof(Product.CreatedBy)
        });

            if (product == null) return null;
            return product.Adapt<ProductResponse>();
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _productRepository.GetOne(c => c.Id == id,
                new string[] { nameof(Product.Images) });
            if (product == null) return false;

            _fileService.Delete(product.MainImage);

            if (product.Images != null)
            {
                foreach (var image in product.Images)
                {
                    _fileService.Delete(image.ImagePath);
                }

            }



            return await _productRepository.DeleteAsync
                   (product);
        }

        public async Task<bool> UpdateProduct(int id, ProductUpdateRequest request)
        {
            var product = await _productRepository.GetOne(p => p.Id == id, new string[] { nameof(Product.Translations) ,
                nameof(Product.Images)});

            if (product == null) return false;
            request.Adapt(product);
            if (request.Translations != null)
            {
                foreach (var translationrequest in request.Translations)
                {
                    var existing = product.Translations.FirstOrDefault
                        (t => t.Language == translationrequest.Language);
                    if (existing != null)
                    {
                        if (translationrequest.Name != null)
                        {
                            existing.Name = translationrequest.Name;
                        }
                        if (translationrequest.Description != null)
                        {
                            existing.Description = translationrequest.Description;
                        }



                    }
                    else
                    {
                        return false;
                    }


                }
            }
                var oldImage = product.MainImage;
                if (request.MainImage != null)
                {
                    _fileService.Delete(oldImage);
                    product.MainImage = await _fileService.UploadASync(request.MainImage);
                }
                else
                {
                    product.MainImage = oldImage;
                }

                if (request.SubImages != null)
                {
                    foreach (var image in product.Images)
                    {
                        _fileService.Delete(image.ImagePath);



                    }
                    product.Images.Clear();

                    foreach (var image in request.SubImages)
                    {
                        var imagePath = await _fileService.UploadASync(image);

                        product.Images.Add(new ProductImage { ImagePath = imagePath });

                    }
                }





                if (request.NewImages != null)
                {
                    foreach (var image in request.NewImages)
                    {
                        var imagePath = await _fileService.UploadASync(image);
                        product.Images.Add(new ProductImage { ImagePath = imagePath });
                    }
                }








                return await _productRepository.UpdateAsync(product);

            }






        public async Task<bool> ToggleStatus(int id)
        {
            var product = await _productRepository.GetOne(p=>p.Id==id);
            if (product == null) return false;
            product.Status=product.Status==EntityStatus.Active? EntityStatus.Inactive : EntityStatus.Active;
             return await _productRepository.UpdateAsync(product);
        }
    }
}
