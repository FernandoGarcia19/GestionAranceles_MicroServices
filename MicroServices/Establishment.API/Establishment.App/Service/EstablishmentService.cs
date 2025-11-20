using Establishment.Dom.Interface;
using Establishment.Dom.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Establishment.App.Service;

public class EstablishmentService
{
    private readonly IRepository _repository;

    public EstablishmentService(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<int>> Insert(Dom.Model.Establishment t)
    {
        var res = await _repository.Insert(t);
        return res;
    }

    public async Task<Result<List<Dom.Model.Establishment>>> Select()
    {
        var res = await _repository.Select();
        return res;
    }
    
    public async Task<Result<Dom.Model.Establishment>> SelectById(int id)
    {
        var res = await _repository.SelectById(id);
        return res;
    }
    
    public async Task<Result<int>> Update(Dom.Model.Establishment t)
    {
        var res = await _repository.Update(t);
        return res;
    }
    
    public async Task<Result<int>> Delete(Dom.Model.Establishment t)
    {
        var res = await _repository.Delete(t);
        return res;
    }
    
    public async Task<Result<IEnumerable<Dom.Model.Establishment>>> Search(string property)
    {
        return await _repository.Search(property);
    }
}