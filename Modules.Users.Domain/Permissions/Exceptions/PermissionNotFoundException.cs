using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Permissions.Exceptions;

public class PermissionNotFoundException(string name)
    : DomainException($"Permission '{name}' not found while building system roles");