using LanguageExt.Common;
using Newtonsoft.Json;
using SandboxService.Application.Data.Dtos;

namespace SandboxService.Application.ServiceClients;

public class UserServiceClient(HttpClient httpClient)
{
    public async Task<GetUserResponse> GetUser(Guid id)
    {
        var response = await httpClient.GetAsync($"users/{id}");
        var json = await response.Content.ReadAsStringAsync();

        var entity = JsonConvert.DeserializeObject<GetUserResponse>(json);

        return entity!;
    }
}
