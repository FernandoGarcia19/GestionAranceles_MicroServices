namespace PersonInCharge.Dom.Interface;

using PersonInCharge.Dom.Model;

public interface IRepository
{
    Task<Result<int>> Insert(Model.PersonInCharge t); 
    Task<Result<int>> Update(Model.PersonInCharge  t);
    Task<Result<int>> Delete(Model.PersonInCharge  t);

    Task<Result<Model.PersonInCharge>> SelectById(int id);
    Task<Result<List<Model.PersonInCharge>>> Select();
    public Task<Result<IEnumerable<Model.PersonInCharge>>> Search(string property);

}