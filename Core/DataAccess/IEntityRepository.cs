using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    //where ve sonrası generic constraint olarak bilinir
    //T:class daki class referans tip olabileceği anlamına gelir
    //,IEntity kısmı ise IEntity ya da onu kullanan bir referans olmalı nalamına gelir
    //,new() kısmı new'lenebilir olmalı demek, böylece IEntity kendisi kullanılmaz onu implemente edenler olabilir demek
    public interface IEntityRepository<T> where T:class,IEntity,new()
    {
        //List<T> GetAll();
        //aşağıdaki expression da filter=null yukarıdaki hepsini getirle aynı işi görüyor,
        //yani linq sorgusu olmadan çağırılırsa hepsi gelir
        List<T> GetAll(Expression<Func<T,bool>> filter=null);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
