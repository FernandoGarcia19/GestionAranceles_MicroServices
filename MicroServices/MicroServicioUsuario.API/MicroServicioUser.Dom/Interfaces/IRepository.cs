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
        Task<int> Insert(User t);
        Task<int> Update(User t);
        Task<int> Delete(User t);
        Task<List<User>> Select();
        Task<List<User>> Search(string p);
    }
}
