using System.ComponentModel.DataAnnotations;
using Payment.Dom.Interface;
using Payment.Dom.Model;

namespace Payment.App.Service;

public class PaymentService
{
    private readonly IRepository _repository;
    public PaymentService(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<int>> Insert(Dom.Model.Payment t)
    {
        if (t == null)
            return Result<int>.Failure("InvalidInput: body is null");

        var validationErrors = ValidateModel(t);
        if (validationErrors.Any())
            return Result<int>.Failure(validationErrors.ToArray());

        // Validate items
        if (t.Items == null || !t.Items.Any())
            return Result<int>.Failure("InvalidInput: payment must have at least one item");

        foreach (var item in t.Items)
        {
            var itemErrors = ValidateItem(item);
            if (itemErrors.Any())
                return Result<int>.Failure(itemErrors.ToArray());
        }

        var repoRes = await _repository.Insert(t);
        if (repoRes.IsFailure)
            return Result<int>.Failure(repoRes.Errors.ToArray());

        return Result<int>.Success(repoRes.Value);
    }
    
    public async Task<Result<List<Dom.Model.Payment>>> Select()
    {
        var repoRes = await _repository.Select();
        if (repoRes.IsFailure)
            return Result<List<Dom.Model.Payment>>.Failure(repoRes.Errors.ToArray());

        return Result<List<Dom.Model.Payment>>.Success(repoRes.Value);
    }

    public async Task<Result<Dom.Model.Payment>> SelectById(int id)
    {
        if (id <= 0)
            return Result<Dom.Model.Payment>.Failure("InvalidInput: id must be greater than zero");

        var repoRes = await _repository.SelectById(id);
        if (repoRes.IsFailure)
            return Result<Dom.Model.Payment>.Failure(repoRes.Errors.ToArray());

        return Result<Dom.Model.Payment>.Success(repoRes.Value);
    }
    
    public async Task<Result<int>> Update(Dom.Model.Payment t)
    {
        if (t == null)
            return Result<int>.Failure("InvalidInput: body is null");

        if (t.Id <= 0)
            return Result<int>.Failure("InvalidInput: id must be greater than zero");

        var validationErrors = ValidateModel(t);
        if (validationErrors.Any())
            return Result<int>.Failure(validationErrors.ToArray());

        // Validate items
        if (t.Items == null || !t.Items.Any())
            return Result<int>.Failure("InvalidInput: payment must have at least one item");

        foreach (var item in t.Items)
        {
            var itemErrors = ValidateItem(item);
            if (itemErrors.Any())
                return Result<int>.Failure(itemErrors.ToArray());
        }

        var repoRes = await _repository.Update(t);
        if (repoRes.IsFailure)
            return Result<int>.Failure(repoRes.Errors.ToArray());

        return Result<int>.Success(repoRes.Value);
    }

    public async Task<Result<int>> Delete(Dom.Model.Payment t)
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
    
    public async Task<Result<IEnumerable<Dom.Model.Payment>>> Search(string property)
    {
        return await _repository.Search(property);
    }

    private List<string> ValidateModel(Dom.Model.Payment model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, serviceProvider: null, items: null);
        Validator.TryValidateObject(model, ctx, validationResults, validateAllProperties: true);
        return validationResults.Select(r => r.ErrorMessage ?? "").Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
    }

    private List<string> ValidateItem(CategoryPayment item)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(item, serviceProvider: null, items: null);
        Validator.TryValidateObject(item, ctx, validationResults, validateAllProperties: true);
        return validationResults.Select(r => r.ErrorMessage ?? "").Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
    }
}
