

using MicroServicioUser.Dom.Patterns;
namespace MicroServicioUser.Dom.Interfaces;

public interface IRepositoryService<TModel>
{
    public Result<IEnumerable<TModel>> GetAll();
    public Result<TModel> Insert(TModel model);
    public Result<TModel> Update(TModel model);
    public Result<TModel> Delete(TModel model);
    public Result<IEnumerable<TModel>> Search(string property);
}