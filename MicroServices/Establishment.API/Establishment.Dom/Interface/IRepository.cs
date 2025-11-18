namespace Establishment.Dom.Interface;

public interface IRepository
{
    Task<int> Insert(Model.Establishment t); 
    Task<int> Update(Model.Establishment  t);
    Task<int> Delete(Model.Establishment  t);

    Task<Model.Establishment > SelectById(int id);
    Task<List<Model.Establishment >> Select();
}