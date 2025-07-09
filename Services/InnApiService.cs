using System;
using System.Threading.Tasks;
using Dadata;
using Dadata.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TelegramInnBot.Models;

namespace TelegramInnBot.Services
{
    public class InnApiService
    {
        private readonly SuggestClientAsync _apiClient;
        private readonly ILogger<InnApiService> _logger;

        public InnApiService(IConfiguration configuration, ILogger<InnApiService> logger)
        {
            _logger = logger;

            var token = configuration["InnApi:ApiKey"];
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("InnApi:ApiKey не указан в appsettings.json");
            }

            _apiClient = new SuggestClientAsync(token);
        }

        public virtual async Task<InnResponse?> GetCompanyInfoAsync(string inn)
        {
            try
            {
                var request = new FindPartyRequest(query: inn)
                {
                    branch_type = PartyBranchType.MAIN
                };

                var response = await _apiClient.FindParty(request);

                if (response.suggestions == null || response.suggestions.Count == 0)
                {
                    _logger.LogInformation("Компания по ИНН {Inn} не найдена", inn);
                    return null;
                }

                var data = response.suggestions[0].data;

                return new InnResponse
                {
                    Inn = data.inn,
                    Name = data.name?.full_with_opf ?? string.Empty,
                    Address = data.address?.value ?? string.Empty,
                    SortName = data.name?.@full ?? data.name?.@short ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запросе к API DaData");
                return null;
            }
        }
    }
}
