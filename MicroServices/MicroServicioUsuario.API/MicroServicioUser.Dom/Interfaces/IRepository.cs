using MicroServicioUser.Dom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroServicioUser.Dom.Patterns;

namespace MicroServicioUser.Dom.Interfaces
{
    public interface IRepository
    {
        public Task<Result<int>> Delete(int id);

        public Task<Result<IEnumerable<User>>> GetAll();
        public Task<Result<int>> Insert(User model);
        public Task<Result<int>> Update(User model);
        public Task<Result<IEnumerable<User>>> Search(string property);

        public Task<Result<User?>> GetById(int id);

        public Task<Result<User?>> GetByUsername(string username);
    }
}
