using System.ComponentModel.DataAnnotations;
using PersonInCharge.Dom.Interface;
using PersonInCharge.Dom.Model;

namespace PersonInCharge.App.Service;

public class PersonInChargeService
{
    private readonly IRepository _repository;
    public PersonInChargeService(IRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result<int>> Insert(Dom.Model.PersonInCharge t)
    {
        if (t == null)
            return Result<int>.Failure("InvalidInput: body is null");

        var validationErrors = ValidateModel(t);
        if (validationErrors.Any())
            return Result<int>.Failure(validationErrors.ToArray());

        var repoRes = await _repository.Insert(t);
        if (repoRes.IsFailure)
            return Result<int>.Failure(repoRes.Errors.ToArray());

        return Result<int>.Success(repoRes.Value);
    }
    public async Task<Result<List<Dom.Model.PersonInCharge>>> Select()
    {
        var repoRes = await _repository.Select();
        if (repoRes.IsFailure)
            return Result<List<Dom.Model.PersonInCharge>>.Failure(repoRes.Errors.ToArray());

        return Result<List<Dom.Model.PersonInCharge>>.Success(repoRes.Value);
    }

    public async Task<Result<Dom.Model.PersonInCharge>> SelectById(int id)
    {
        if (id <= 0)
            return Result<Dom.Model.PersonInCharge>.Failure("InvalidInput: id must be greater than zero");

        var repoRes = await _repository.SelectById(id);
        if (repoRes.IsFailure)
            return Result<Dom.Model.PersonInCharge>.Failure(repoRes.Errors.ToArray());

        return Result<Dom.Model.PersonInCharge>.Success(repoRes.Value);
    }
    public async Task<Result<int>> Update(Dom.Model.PersonInCharge t)
    {
        if (t == null)
            return Result<int>.Failure("InvalidInput: body is null");

        if (t.Id <= 0)
            return Result<int>.Failure("InvalidInput: id must be greater than zero");

        var validationErrors = ValidateModel(t);
        if (validationErrors.Any())
            return Result<int>.Failure(validationErrors.ToArray());

        var repoRes = await _repository.Update(t);
        if (repoRes.IsFailure)
            return Result<int>.Failure(repoRes.Errors.ToArray());

        return Result<int>.Success(repoRes.Value);
    }

    public async Task<Result<int>> Delete(Dom.Model.PersonInCharge t)
    {
        if (t == null)
            return Result<int>.Failure("InvalidInput: body is null");

        if (t.Id <= 0)
            return Result<int>.Failure("InvalidInput: id must be greater than zero");

        var repoRes = await _repository.Delete(t);
        if (repoRes.IsFailure)
            return Result<int>.Failure(repoRes.Errors.ToArray());

        return Result<int>.Success(repoRes.Value);
    }

    private List<string> ValidateModel(Dom.Model.PersonInCharge model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, serviceProvider: null, items: null);
        Validator.TryValidateObject(model, ctx, validationResults, validateAllProperties: true);
        return validationResults.Select(r => r.ErrorMessage ?? "").Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
    }
}