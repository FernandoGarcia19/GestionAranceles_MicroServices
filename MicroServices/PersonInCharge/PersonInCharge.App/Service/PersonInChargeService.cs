
using PersonInCharge.Dom.Interface;

namespace PersonInCharge.App.Service;

public class PersonInChargeService
{
    private readonly IRepository _repository;
    public PersonInChargeService(IRepository repository)
    {
        _repository = repository;
    }
    public async Task<int> Insert(Dom.Model.PersonInCharge t)
    {
        var id = await _repository.Insert(t);
        return id;
    }
    public async Task<List<Dom.Model.PersonInCharge>> Select()
    {
        return await _repository.Select();
    }

    public async Task<Dom.Model.PersonInCharge> SelectById(int id)
    {
        return await _repository.SelectById(id);
    }
    public async Task<int> Update(Dom.Model.PersonInCharge t)
    {
        var res = await _repository.Update(t);
        return res;
    }

    public async Task<int> Delete(Dom.Model.PersonInCharge t)
    {
        var res = await _repository.Delete(t);
        return res;
    }
}