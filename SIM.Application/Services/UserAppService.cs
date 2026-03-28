using FluentValidation;
using SIM.Application.Abstractions.Services;
using SIM.Application.Exceptions;
using SIM.Application.ViewModels.Users;
using SIM.Domain.Abstractions;
using SIM.Domain.Constants;
using SIM.Domain.Entities;

namespace SIM.Application.Services;

public class UserAppService(
    IValidator<CreateUserViewModel> createValidator,
    IRepository<UserProfile> userProfileRepository,
    IRepository<Organization> organizationRepository,
    IUnitOfWork unitOfWork) : IUserAppService
{
    public async Task<UserViewModel> CreateAsync(
        CreateUserViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var organization = await organizationRepository.GetByIdAsync(vm.OrganizationId, cancellationToken)
            ?? throw new BusinessLogicException(ValidationMessages.OrganizationNotFound);

        var userProfile = UserProfile.Create(vm.SupabaseUserId, vm.FullName, vm.Email, vm.Role, organization.Id, vm.UnitId);

        await userProfileRepository.AddAsync(userProfile, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToViewModel(userProfile);
    }

    public async Task<UserViewModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(id, cancellationToken);
        return userProfile is null ? null : MapToViewModel(userProfile);
    }

    private static UserViewModel MapToViewModel(UserProfile u) =>
        new(u.Id, u.FullName, u.Email, u.Role, u.OrganizationId, u.UnitId, u.CreatedAt, u.IsActive);
}
