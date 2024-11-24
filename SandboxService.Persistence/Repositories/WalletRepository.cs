﻿using SandboxService.Core.Models;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class WalletRepository(SandboxContext context) : RepositoryBase<Wallet>(context) { }