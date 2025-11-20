using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceCategory.Domain.Interfaces
{
    public interface IRepository<TModel>
    {
        Task<int> Insert(TModel model);
        Task<int> Update(TModel model);
        Task<int> Delete(TModel model);
        Task<List<TModel>> Select();
        Task<List<TModel>> Search(string property);
        Task<TModel> SelectById(int id);
    }
}
