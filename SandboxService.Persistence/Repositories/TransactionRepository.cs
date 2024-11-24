using SandboxService.Core.Models;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class TransactionRepository(SandboxContext context) : RepositoryBase<Transaction>(context) { }