using AutoMapper;
using SandboxService.Application.Data.Dtos;
using SandboxService.Core.Models;

namespace SandboxService.Application.Profiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<SanboxInitializeRequest, UserData>();
    }
}
