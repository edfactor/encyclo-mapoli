
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using Demoulas.ProfitSharing.Services;
using Demoulas.StoreInfo.Entities.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;

namespace Demoulas.ProfitSharing.OracleHcm.Cli;

public class Program
{
    public static async Task Main()
    {
        async IAsyncEnumerable< DemographicsRequestDto> ConvertToDto(IAsyncEnumerable<OracleEmployee?> asyncEnumerable)
        {
            await foreach (OracleEmployee? employee in asyncEnumerable)
            {
                if (employee == null)
                {
                    continue;
                }

                yield return new DemographicsRequestDto
                {
                    OracleHcmId = employee.PersonId,
                    BadgeNumber = employee.BadgeNumber,
                    SSN = employee.PersonId,
                    DateOfBirth = employee.DateOfBirth,

                    FirstName = "employee.Name.FirstName",
                    LastName = "",
                    StoreNumber = 0,
                    DepartmentId = 0,
                    PayClassificationId = 0,
                    HireDate = DateOnly.FromDateTime(employee.CreationDate.DateTime),
                    EmploymentTypeCode = EmploymentType.Constants.PartTime,
                    PayFrequencyId = PayFrequency.Constants.Weekly,
                    EmploymentStatusId = EmploymentStatus.Constants.Active,
                    GenderCode = Gender.Constants.Other,

                    ContactInfo = new ContactInfoRequestDto()
                    {
                        EmailAddress = ""
                    },
                    Address = new AddressRequestDto
                    {
                        Street = "",
                        City = "",
                        State = "",
                        PostalCode = "",
                        CountryISO = ""
                    }
                };
            }
        }

        IHostApplicationBuilder builder = new HostApplicationBuilder();

        builder.Services.AddHttpClient<OracleDemographicsService>((services, client) =>
        {
            var config = services.GetRequiredService<OracleHcmConfig>();

            byte[] bytes = Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}");
            var encodedAuth = Convert.ToBase64String(bytes);

            client.BaseAddress = new Uri(config.Url, UriKind.Absolute);
            client.DefaultRequestHeaders.Add("REST-Framework-Version", config.RestFrameworkVersion);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
        }).AddStandardResilienceHandler(options =>
        {
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
            options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
        });
        builder.Configuration.AddUserSecrets<Program>();

        List<ContextFactoryRequest> list = new List<ContextFactoryRequest>
        {
            ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing"),
            ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"),
            ContextFactoryRequest.Initialize<StoreInfoDbContext>("StoreInfo")
        };
        builder.AddDatabaseServices(list, true, true);
        Services.Extensions.ServicesExtension.AddProjectServices(builder);
        builder.Services.AddSingleton<DemographicsService>();
        var provider = builder.Services.BuildServiceProvider();

        var service = provider.GetRequiredService<OracleDemographicsService>();

        var employees = service.GetAllEmployees();

        var dto = ConvertToDto(employees);

        var demographicsService = provider.GetRequiredService<DemographicsService>();
        await demographicsService.AddDemographicsStream(dto, byte.MaxValue, CancellationToken.None);

        Console.ReadLine();
    }

   
}
