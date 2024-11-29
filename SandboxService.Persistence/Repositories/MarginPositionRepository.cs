using SandboxService.Core.Models;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class MarginPositionRepository(SandboxContext context) : RepositoryBase<MarginPosition>(context) { }