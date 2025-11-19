using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.App.Services
{
    public interface IDbRepository<T>
    {
        public IEnumerable<T> GetAll();
        public int Insert(T model);
        public int Update(T model);
        public int Delete(T model);
        public IEnumerable<T> Search(string property);
    }
}
