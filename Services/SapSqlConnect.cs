using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SapGateway.Services
{
    public class SapSqlConnect
    {
        private readonly IConfiguration _config;
        public SapSqlConnect(IConfiguration config) => _config = config;

        private string GetConnectionString(string company)
        {
            var dbMap = _config.GetSection("SapDbMap").Get<Dictionary<string, string>>();
            if (!dbMap.ContainsKey(company)) throw new Exception($"Invalid company code: {company}");

            var database = dbMap[company];
            var host = _config["SapSql:Host"]!;
            var user = _config["SapSql:User"]!;
            var password = _config["SapSql:Password"]!;

            return $"Server={host};Database={database};User Id={user};Password={password};TrustServerCertificate=True;";
        }

        public SqlConnection GetConnection(string company) => new SqlConnection(GetConnectionString(company));

        public async Task<List<Dictionary<string, object>>> QueryAsync(string company, string sql)
        {
            using var conn = GetConnection(company);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return result;
        }
    }
}
