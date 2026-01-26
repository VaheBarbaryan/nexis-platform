using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Roles.Exceptions;

public class RoleCannotBeEmptyException() : DomainException("Role name cannot be empty.");