using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SandboxService.Persistence.Contexts;

public class SandboxContextDapper
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public SandboxContextDapper(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SandboxDapper");
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
