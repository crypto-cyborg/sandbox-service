using SandboxService.Core.Models;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class AccountRepository(SandboxContext context) : RepositoryBase<Account>(context) { }