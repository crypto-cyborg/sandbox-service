using System;
using SandboxService.Core.Models;

namespace SandboxService.Application.Data.Dtos;

public class WalletCreateDto
{
    public Guid UserId { get; set; }

    public Currency Currency { get; set; }
}
