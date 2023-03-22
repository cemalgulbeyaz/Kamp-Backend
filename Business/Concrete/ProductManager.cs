using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.CCS;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService; //Bir Dal içinde başka bir Dal çağırılmaz ondan Service olanı çağrılır
        public ProductManager(IProductDal productDal, ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }

        //[SecuredOperation("product.add,admin")]
        //[ValidationAspect(typeof(ProductValidator))]
        //[CacheRemoveAspect("IProductService.Get")] // methodu başarı ile tamamlanırsa paterne uyan tüm cache'ler silinir
        public IResult Add(Product product)
        {
            //bussines codes

            #region Eski kodlar
            ////if (product.UnitPrice<=0)
            ////{
            ////    return new ErrorDataResult(Messages.UnitPriceInvalid);
            ////}
            //if (product.ProductName.Length < 2)
            //{
            //    return new ErrorResult(Messages.ProductNameInvalid);
            //}

            //var context = new ValidationContext<Product>(product);
            //ProductValidator productValidator = new ProductValidator();
            //var result = productValidator.Validate(context);
            //if (!result.IsValid)
            //{
            //    throw new ValidationException(result.Errors);
            //}
            // Core projesideki ValidationTool a gitti

            //ValidationTool.Validate(new ProductValidator(), product);
            // Methodun tepesine gitti.

            //// Bir kategoride en fazla 10 ürün olabilir kuralı
            //if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success
            //    && CheckIfProductNameExist(product.ProductName).Success)
            //{
            //    _productDal.Add(product);
            //    return new Result(true, Messages.ProductAdded);
            //}
            #endregion

            //IResult result = BusinessRules.Run(CheckIfProductCountOfCategoryCorrect(product.CategoryId),
            //    CheckIfProductNameExist(product.ProductName),
            //    CheckIfCategoryLimitExceded());

            //if (result != null)
            //{
            //    return result;
            //}

            _productDal.Add(product);
            return new Result(true, Messages.ProductAdded);
        }

        //[CacheAspect]
        public IDataResult<List<Product>> GetAll()
        {
            //İş kontrol kodları olur. If koşullarıyla

            //if (DateTime.Now.Hour == 22)
            //{
            //    // saat 22:00-23:00 arası bakım çalışması nedeniyle listeleme
            //    // özelliği devre dışı kalıyor gibi bir senaryo
            //    // sadece test etmek için sonrasında iptal
            //    return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            //}

            return new SuccessDataResult<List<Product>> 
                (
                _productDal.GetAll(), 
                Messages.ProductsListed
                );
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>
                (
                _productDal.GetAll(p => p.CategoryId == id), 
                Messages.ProductsListed
                );

            //return _productDal.GetAll(p => p.CategoryId == id);
        }

        public IDataResult<List<Product>> GetAllByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>
                (
                _productDal.GetAll(p => p.UnitPrice >= min && p.UnitPrice <= max),
                Messages.ProductsListed
                );
            //return _productDal.GetAll(p => p.UnitPrice >= min && p.UnitPrice <= max);
        }

        [CacheAspect]
        //[PerformanceAspect(5)] // Methodun işletilmesi 5sn den uzun sürerse debug console'una log atıyor
        public IDataResult<Product> GetById(int product_Id)
        {
            return new SuccessDataResult<Product>
                (
                _productDal.Get(p => p.ProductId == product_Id),
                Messages.ProductsListed
                );
            //return _productDal.Get(p => p.ProductId == product_Id);
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>
                (
                _productDal.GetProductDetails(),
                Messages.ProductsListed
                );
            //return _productDal.GetProductDetails();
        }
        public IDataResult<List<ProductWithCategoryDto>> GetAllProductsWithCategory()
        {
            return new SuccessDataResult<List<ProductWithCategoryDto>>
                (
                _productDal.GetAllProductsWithCategory(),
                Messages.ProductsListed
                );
            //return _productDal.GetProductDetails();
        }

        //[ValidationAspect(typeof(ProductValidator))]
        //[CacheRemoveAspect("IProductService.Get")] // methodu başarı ile tamamlanırsa paterne uyan tüm cache'ler silinir
        public IResult Update(Product product)
        {
            //return new SuccessDataResult<List<ProductDetailDto>>
            //    (
            //    _productDal.GetProductDetails(),
            //    Messages.ProductsListed
            //    );

            _productDal.Update(product);
            return new Result(true, Messages.ProductUpdated);
        }

        public IResult Delete(Product product)
        {
            _productDal.Delete(product);
            return new Result(true, Messages.ProductDeleted);
        }

        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result >= 10)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }
        private IResult CheckIfProductNameExist(string productName)
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExit);
            }
            return new SuccessResult();
        }
        private IResult CheckIfCategoryLimitExceded()
        {
            var result = _categoryService.GetAll();
            if (result.Data.Count>15)
            {
                return new ErrorResult(Messages.CategoryLimitExceded);
            }
            return new SuccessResult();
        }

        //[TransactionScopeAspect] // Methodu Transaction kontrollü işletir
        public IResult AddTransactionalTest(Product product)
        {
            Add(product);
            if (product.UnitPrice < 10)
                throw new Exception("");

            Add(product);
            return null;
        }
    }
}
