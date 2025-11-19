using MicroServicioUser.Dom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.Dom.Interfaces
{
    public interface IRepository
    {
        public Task<int> Delete(int id);

        public Task<IEnumerable<User>> GetAll();
        public Task<int> Insert(User model);
        public Task<int> Update(User model);
        public Task<IEnumerable<User>> Search(string property);

        public Task<User?> GetById(int id);

        public Task<User?> GetByUsername(string username);
    }
}
