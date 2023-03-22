using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        ICategoryDal _categoryDal;

        public CategoryManager(ICategoryDal categoryDal)
        {
            _categoryDal = categoryDal;
        }

        public IDataResult<List<Category>> GetAll()
        {
            //İş kodları
            return new SuccessDataResult<List<Category>>(_categoryDal.GetAll());
        }

        public IDataResult<Category> GetById(int categoryId)
        {
            return new SuccessDataResult<Category>(_categoryDal.Get(c => c.CategoryId == categoryId));
        }

        public IResult Add(Category category)
        {

            _categoryDal.Add(category);
            return new Result(true, Messages.CategoryAdded);
        }

        public IResult Update(Category category)
        {

            _categoryDal.Update(category);
            return new Result(true, Messages.CategoryUpdated);
        }

        //public IResult Delete(int categoryId)
        //{
        //    var categoryToDelete = _categoryDal.Get(c => c.CategoryId == categoryId);

        //    _categoryDal.Delete(categoryToDelete);
        //    return new Result(true, Messages.CategoryUpdated);
        //}
        public IResult Delete(Category category)
        {
            _categoryDal.Delete(category);
            return new Result(true, Messages.CategoryDeleted);
        }

    }
}
