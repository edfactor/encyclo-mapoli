
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
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
                    SSN = 0,
                    DateOfBirth = employee.DateOfBirth,

                    FirstName = employee.Name.FirstName,
                    LastName = employee.Name.LastName,
                    FullName = employee.Name.DisplayName,
                    StoreNumber = 0,
                    DepartmentId = Department.Constants.Beer_And_Wine,
                    PayClassificationId = PayClassification.Constants.ApprMeatCutters,
                    HireDate = employee.Name.EffectiveStartDate.ToDateOnly(),
                    EmploymentTypeCode = EmploymentType.Constants.PartTime,
                    PayFrequencyId = PayFrequency.Constants.Weekly,
                    EmploymentStatusId = EmploymentStatus.Constants.Active,
                    GenderCode = Gender.Constants.Other,


                    ContactInfo = new ContactInfoRequestDto
                    {
                        PhoneNumber = employee.Phone?.PhoneNumber
                    },
                    Address = new AddressRequestDto
                    {
                        Street = employee.Address.AddressLine1,
                        Street2 = employee.Address.AddressLine2,
                        Street3 = employee.Address.AddressLine3,
                        Street4 = employee.Address.AddressLine4,
                        City = employee.Address.TownOrCity,
                        State = employee.Address.State,
                        PostalCode = employee.Address.PostalCode,
                        CountryISO = employee.Address.Country
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
        await builder.AddDatabaseServices(list, true, true);
        Services.Extensions.ServicesExtension.AddProjectServices(builder);
        builder.Services.AddSingleton<DemographicsService>();
        var provider = builder.Services.BuildServiceProvider();

        var service = provider.GetRequiredService<OracleDemographicsService>();

        var employees = service.GetAllEmployees();

        var dto = ConvertToDto(employees);

        var demographicsService = provider.GetRequiredService<DemographicsService>();
        await demographicsService.AddDemographicsStream(dto, 5, CancellationToken.None);

        await Task.CompletedTask;

        Console.ReadLine();
    }

   
}
