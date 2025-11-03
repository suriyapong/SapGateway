using SapGateway.Models;
using SapGateway.Services;
using System.Globalization;
using System.Net.Http;

namespace SapGateway.Endpoints
{
    public static class CurencyBotEndpoints
    {
        public static void MapCurencyBotEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/curencybot/{company}");

            group.MapPost("/updatecurency", HandleInsertCurrency);
            group.MapGet("/info", HandleGetInfo);
        }

        private static async Task<IResult> HandleInsertCurrency(string company, CurrencyUpdateRequest body, SapServiceLayerClient sl)
        {
            try
            {
                if (body == null) return Results.BadRequest(new { message = "Request body is required" });

                CurrencyRateModel currencyBank = await GetCurrencyFromBankById(body.Period, body.CurrencyId);
                var res = await sl.InsertCurrencyData(company, body.SAPCurrencyId, body.Period, body.IsBuyingRate ? Convert.ToDouble(currencyBank.BuyingTransfer) : Convert.ToDouble(currencyBank.Selling));
                return Results.Ok(new { Message = $"Update curency {company}" });
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> HandleGetInfo(IConfiguration config)
        {
            var map = config.GetSection("SapDbMap:novax").Value;
             return Results.Ok(new { Message = $"{map}" + "Time : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Time Zone : " + TimeZoneInfo.Local.ToString() });
        }

        private static async Task<CurrencyRateModel> GetCurrencyFromBankById(string date, string currencyId)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-IBM-Client-Id", "fb62d2f3-364f-4b98-a7b1-2001c7518fa2"); ;

            DateTime dateTime = DateTime.Parse(date, CultureInfo.InvariantCulture).AddDays(-1);

            const int maxRetries = 7; // prevent infinite loop
            int attempt = 0;
            var en = new System.Globalization.CultureInfo("en-US");

            while (attempt < maxRetries)
            {
                try
                {
                    var response = await httpClient.GetAsync($"https://apigw1.bot.or.th/bot/public/Stat-ExchangeRate/v2/DAILY_AVG_EXG_RATE/?start_period={dateTime.ToString("yyyy-MM-dd", en)}&end_period={dateTime.ToString("yyyy-MM-dd", en)}");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }

                    var contents = await response.Content.ReadFromJsonAsync<ExchageRateModel>();

                    if (contents?.result?.data?.data_detail != null)
                    {
                        // If all records have a valid period, return
                        if (contents.result.data.data_detail.All(x => !string.IsNullOrEmpty(x.period)))
                        {
                            if (contents.result.data.data_detail.Any(x => x.currency_id == currencyId))
                            {
                                var BOTRate = contents.result.data.data_detail.FirstOrDefault(x => x.currency_id == currencyId);
                                if (currencyId == "JPY")
                                {
                                    return new CurrencyRateModel
                                    {
                                        Period = BOTRate.period,
                                        CurrencyId = BOTRate.currency_id,
                                        Selling = (Convert.ToDouble(BOTRate.selling) / 100).ToString("F4"),
                                        BuyingTransfer = (Convert.ToDouble(BOTRate.buying_transfer) / 100).ToString("F4")
                                    };
                                }
                                else if (currencyId == "IDR")
                                {
                                    return new CurrencyRateModel
                                    {
                                        Period = BOTRate.period,
                                        CurrencyId = BOTRate.currency_id,
                                        Selling = (Convert.ToDouble(BOTRate.selling) / 1000).ToString("F4"),
                                        BuyingTransfer = (Convert.ToDouble(BOTRate.buying_transfer) / 1000).ToString("F4")
                                    };
                                }
                                else
                                {
                                    return new CurrencyRateModel
                                    {
                                        Period = BOTRate.period,
                                        CurrencyId = BOTRate.currency_id,
                                        Selling = BOTRate.selling,
                                        BuyingTransfer = BOTRate.buying_transfer
                                    };
                                }
                            }
                        }
                    }

                    // Try previous day
                    dateTime = dateTime.AddDays(-1);
                    attempt++;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching exchange rate", ex);
                }
            }
            return null;
        }
    }

    public record CurrencyUpdateRequest(
        string Period,
        Boolean IsBuyingRate,
        string CurrencyId,
        string SAPCurrencyId
    );
}
