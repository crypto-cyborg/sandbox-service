using AutoMapper;
using SandboxService.Core.Interfaces.Services;
using SandboxService.Core.Models;

namespace SandboxService.Application.Profiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<SandboxInitializeRequest, User>();
    }
}
