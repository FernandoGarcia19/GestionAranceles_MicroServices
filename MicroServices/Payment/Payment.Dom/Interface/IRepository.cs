namespace Payment.Dom.Interface;

using Payment.Dom.Model;

public interface IRepository
{
    Task<Result<int>> Insert(Model.Payment t); 
    Task<Result<int>> Update(Model.Payment t);
    Task<Result<int>> Delete(Model.Payment t);

    Task<Result<Model.Payment>> SelectById(int id);
    Task<Result<List<Model.Payment>>> Select();
    public Task<Result<IEnumerable<Model.Payment>>> Search(string property);
}
