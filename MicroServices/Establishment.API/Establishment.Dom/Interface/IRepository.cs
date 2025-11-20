using Establishment.Dom.Model;

namespace Establishment.Dom.Interface;

public interface IRepository
{
    Task<Result<int>> Insert(Model.Establishment t); 
    Task<Result<int>> Update(Model.Establishment  t);
    Task<Result<int>> Delete(Model.Establishment  t);

    Task<Result<Model.Establishment>> SelectById(int id);
    Task<Result<List<Model.Establishment>>> Select();
}