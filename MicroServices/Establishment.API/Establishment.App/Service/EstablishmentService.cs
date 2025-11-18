using Establishment.Dom.Interface;

namespace Establishment.App.Service;

public class EstablishmentService
{
    private readonly IRepository _repository;

    public EstablishmentService(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<int> Insert(Dom.Model.Establishment t)
    {
        var id = await _repository.Insert(t);
        return id;
    }

    public async Task<List<Dom.Model.Establishment>> Select()
    {
        var res = await _repository.Select();
        return res;
    }
    
    public async Task<Dom.Model.Establishment> SelectById(int id)
    {
        var res = await _repository.SelectById(id);
        return res;
    }
    
    public async Task<int> Update(Dom.Model.Establishment t)
    {
        var res = await _repository.Update(t);
        return res;
    }
    
    public async Task<int> Delete(Dom.Model.Establishment t)
    {
        var res = await _repository.Delete(t);
        return res;
    }
}