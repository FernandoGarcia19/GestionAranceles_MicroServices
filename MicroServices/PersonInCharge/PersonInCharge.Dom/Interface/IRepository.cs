
namespace PersonInCharge.Dom.Interface;

public interface IRepository
{
    Task<int> Insert(Model.PersonInCharge t); 
    Task<int> Update(Model.PersonInCharge  t);
    Task<int> Delete(Model.PersonInCharge  t);

    Task<Model.PersonInCharge > SelectById(int id);
    Task<List<Model.PersonInCharge >> Select();

}