

using MicroServicioUser.Dom.Patterns;
namespace MicroServicioUser.Dom.Interfaces;

public interface IRepositoryService<TModel>
{
    public  Task<Result<IEnumerable<TModel>>> GetAll();
    public Task<Result<int>> Insert(TModel model);
    public Task<Result<int>> Update(TModel model);
    public Task<Result<int>> Delete(int id);
    public Task<Result<TModel>> GetById(int id);
    public Task<Result<IEnumerable<TModel>>> Search(string property);
}