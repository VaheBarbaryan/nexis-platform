using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Permissions.Exceptions;

public class PermissionCannotBeEmptyException() : DomainException("Permission cannot be empty.");