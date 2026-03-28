using FluentValidation;
using SIM.Application.Abstractions.Services;
using SIM.Application.Exceptions;
using SIM.Application.ViewModels.Organizations;
using SIM.Domain.Abstractions;
using SIM.Domain.Entities;

namespace SIM.Application.Services;

public class OrganizationAppService(
    IValidator<CreateOrganizationViewModel> createValidator,
    IRepository<Organization> repository,
    IUnitOfWork unitOfWork) : IOrganizationAppService
{
    public async Task<OrganizationViewModel> CreateAsync(
        CreateOrganizationViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var organization = Organization.Create(vm.Name, vm.Cnpj, vm.Type);

        await repository.AddAsync(organization, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToViewModel(organization);
    }

    public async Task<OrganizationViewModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var organization = await repository.GetByIdAsync(id, cancellationToken);
        return organization is null ? null : MapToViewModel(organization);
    }

    public async Task<IReadOnlyList<OrganizationViewModel>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var organizations = await repository.GetAllAsync(cancellationToken);
        return organizations.Select(MapToViewModel).ToList();
    }

    private static OrganizationViewModel MapToViewModel(Organization org) =>
        new(org.Id, org.Name, org.Cnpj, org.Type, org.CreatedAt, org.IsActive);
}
