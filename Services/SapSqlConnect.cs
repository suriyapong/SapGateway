using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SapGateway.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

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

            //return $"Server={host};Database={database};User Id={user};Password={password};TrustServerCertificate=True;";
            return $"Server={host};Initial Catalog={database};User Id={user};Password={password};Encrypt=False;TrustServerCertificate=True;";
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

        public async Task<List<SAPBudgetModel>> GetAllBudget(string company, int year)
        {
            try
            {
                string conStr = GetConnectionString(company);

                if (await TestConnectionAsync(conStr)) { 
                    IDbConnection db = new SqlConnection(conStr);
                    const string sql = @"
                            select 
                            T0.AcctCode
                            , T0.AcctName
                            , T0.OcrCode
                            , T1.[PR-Draft]  + T1.PR + T1.[PO-Draft] + T1.PO + T1.GRPO + T1.AP  + T1.Accounting as 'total'
                            , T0.Budget as 'budgetTotal'
                            from HMC_BGBUDGET T0
		                            left join HMC_BGACTUAL T1 on T0.AcctCode = T1.AcctCode
					                            and T0.OcrCode = T1.OcrCode
                                                and T1.[Year] = @Year
                            where T0.BGYear = @Year";
                    var result = await db.QueryAsync<SAPBudgetModel>(sql, new { Year = year });
                    return result.ToList();
                }
                else
                {
                    throw new HttpRequestException($"SAP not connect");
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"SAP API request failed with status {ex.Message}");
            }
        }

        public async Task<List<SAPBudgetDetailModel>> GetAllBudgetDetail(string company, int year)
        {
            try
            {
                string conStr = GetConnectionString(company);

                if (await TestConnectionAsync(conStr)) {
                    IDbConnection db = new SqlConnection(conStr);
                    const string sql = @"
                        SELECT TOP (1000) [Year]
                          ,[AcctCode]
                          ,[AcctName]
                          ,[OcrCode]
                          ,[PR-Draft] as PRDraft
                          ,[PR]
                          ,[PO-Draft] as PODraft
                          ,[PO]
                          ,[GRPO]
                          ,[AP]
                          ,[Accounting]
                        FROM [NovaEmpire].[dbo].[HMC_BGACTUAL] WHERE Year = @Year";
                    var result = await db.QueryAsync<SAPBudgetDetailModel>(sql, new { Year = year });
                    return result.ToList();
                }
                else
                {
                    throw new HttpRequestException($"SAP not connect");
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"SAP API request failed with status {ex.Message}");
            }
        }

        private async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}