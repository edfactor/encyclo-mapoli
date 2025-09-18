using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
public sealed class DistributionService : IDistributionService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IAppUser? _appUser;

    public DistributionService(IProfitSharingDataContextFactory dataContextFactory, IDemographicReaderService demographicReaderService, IAppUser? appUser)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _appUser = appUser;
    }

    public async Task<PaginatedResponseDto<DistributionSearchResponse>> SearchAsync(DistributionSearchRequest request, CancellationToken cancellationToken)
    {
        var data = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var query = from dist in ctx.Distributions
                        join freq in ctx.DistributionFrequencies on dist.FrequencyId equals freq.Id
                        join status in ctx.DistributionStatuses on dist.StatusId equals status.Id
                        join tax in ctx.TaxCodes on dist.TaxCodeId equals tax.Id
                        join demLj in demographic on dist.Ssn equals demLj.Ssn into demGroup
                        from dem in demGroup.DefaultIfEmpty()
                        join benLj in ctx.Beneficiaries on dist.Ssn equals benLj.Contact!.Ssn into benGroup
                        from ben in benGroup.DefaultIfEmpty()
                        select new 
                        { 
                            dist.Ssn,
                            BadgeNumber = dem != null ? (int?)dem.BadgeNumber : null,
                            DemFullName = dem != null ? dem.ContactInfo.FullName : null,
                            BeneFullName = ben != null ? ben.Contact!.ContactInfo.FullName : null,
                            dist.FrequencyId,
                            Frequency = freq,
                            dist.StatusId,
                            Status = status,
                            dist.TaxCodeId,
                            TaxCode = tax,
                            dist.GrossAmount,
                            dist.FederalTaxAmount,
                            dist.StateTaxAmount,
                            dist.CheckAmount,
                            IsExecutive = dem != null && dem.PayFrequencyId == PayFrequency.Constants.Monthly
                        };

            int searchSsn;
            if (!string.IsNullOrWhiteSpace(request.Ssn) && int.TryParse(request.Ssn, out searchSsn))
            {
                query = query.Where(d => d.Ssn == searchSsn);
            }

            if (request.BadgeNumber.HasValue)
            {
                if (request.PsnSuffix.HasValue)
                {
                    var ssn = await ctx.Beneficiaries.Where(x=>x.BadgeNumber == request.BadgeNumber.Value && x.PsnSuffix == request.PsnSuffix.Value).Select(x=>x.Contact!.Ssn).FirstOrDefaultAsync(cancellationToken);
                    if (ssn == default)
                    {
                        throw new InvalidOperationException("Badge number and PSN suffix combination not found.");
                    }
                    query = query.Where(d => d.Ssn == ssn);
                } 
                else
                {
                    var ssn = await demographic.Where(x => x.BadgeNumber == request.BadgeNumber.Value).Select(x => x.Ssn).FirstOrDefaultAsync(cancellationToken);
                    if (ssn == default)
                    {
                        throw new InvalidOperationException("Badge number not found.");
                    }
                    query = query.Where(d => d.Ssn == ssn);
                }
                
            }

            if (request.DistributionFrequencyId.HasValue)
            {
                query = query.Where(d => d.FrequencyId == request.DistributionFrequencyId.Value);
            }

            if (request.DistributionStatusId.HasValue)
            {
                query = query.Where(d => d.StatusId == request.DistributionStatusId.Value);
            }

            if (request.TaxCodeId.HasValue)
            {
                query = query.Where(d => d.TaxCodeId == request.TaxCodeId.Value);
            }

            if (request.MinGrossAmount.HasValue)
            {
                query = query.Where(d => d.GrossAmount >= request.MinGrossAmount.Value);
            }

            if (request.MaxGrossAmount.HasValue)
            {
                query = query.Where(d => d.GrossAmount <= request.MaxGrossAmount.Value);
            }

            if (request.MinCheckAmount.HasValue)
            {
                query = query.Where(d => d.CheckAmount >= request.MinCheckAmount.Value);
            }

            if (request.MaxCheckAmount.HasValue)
            {
                query = query.Where(d => d.CheckAmount <= request.MaxCheckAmount.Value);
            }

            return await query.ToPaginationResultsAsync(request, cancellationToken);
        });

        var result = new PaginatedResponseDto<DistributionSearchResponse>(request)
        {
            Total = data.Total,
            Results = data.Results.Select(d => new DistributionSearchResponse
            {
                Ssn = d.Ssn.MaskSsn(),
                BadgeNumber = d.BadgeNumber,
                FullName = d.DemFullName ?? d.BeneFullName,
                FrequencyId = d.FrequencyId,
                FrequencyName = d.Frequency!.Name,
                StatusId = d.StatusId,
                StatusName = d.Status!.Name,
                TaxCodeId = d.TaxCodeId,
                TaxCodeName = d.TaxCode!.Name,
                GrossAmount = d.GrossAmount,
                FederalTax = d.FederalTaxAmount,
                StateTax = d.StateTaxAmount,
                CheckAmount = d.CheckAmount,
                IsExecutive = d.IsExecutive
            }).ToList()
        };

        return result;
    }

    public async Task<CreateDistributionResponse> CreateDistribution(CreateDistributionRequest request, CancellationToken cancellationToken)
    {
        
        if (request.GrossAmount <= 0)
        {
            throw new ArgumentException("Gross amount must be greater than zero.", nameof(request.GrossAmount));
        }
        if (request.FederalTaxPercentage < 0 || request.FederalTaxPercentage > 100)
        {
            throw new ArgumentException("Federal tax percentage must be between 0 and 100.", nameof(request.FederalTaxPercentage));
        }
        if (request.StateTaxPercentage < 0 || request.StateTaxPercentage > 100)
        {
            throw new ArgumentException("State tax percentage must be between 0 and 100.", nameof(request.StateTaxPercentage));
        }
        return await _dataContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var dem = await demographic.Where(d => d.BadgeNumber == request.BadgeNumber).FirstOrDefaultAsync(cancellationToken);
            if (dem == null)
            {
                throw new InvalidOperationException("Badge number not found.");
            }
            var maxSequence = await ctx.Distributions.Where(d => d.Ssn == dem.Ssn).MaxAsync(d => (byte?)d.PaymentSequence) ?? 0;
            var distribution = new Distribution
            {
                Ssn = dem.Ssn,
                EmployeeName = dem.ContactInfo!.FullName ?? "",
                PaymentSequence = (byte)(maxSequence + 1),
                StatusId = request.StatusId,
                FrequencyId = request.FrequencyId,
                PayeeId = request.PayeeId,
                ForTheBenefitOfPayee = request.ForTheBenefitOfPayee,
                ForTheBenefitOfAccountType = request.ForTheBenefitOfAccountType,
                Tax1099ForEmployee = request.Tax1099ForEmployee,
                Tax1099ForBeneficiary = request.Tax1099ForBeneficiary,
                FederalTaxPercentage = request.FederalTaxPercentage,
                StateTaxPercentage = request.StateTaxPercentage,
                GrossAmount = request.GrossAmount,
                FederalTaxAmount = request.FederalTaxAmount,
                StateTaxAmount = request.StateTaxAmount,
                CheckAmount = request.CheckAmount,
                TaxCodeId = request.TaxCodeId,
                IsDeceased = request.IsDeceased,
                GenderId = request.GenderId,
                QualifiedDomesticRelationsOrder = request.IsQdro,
                Memo = request.Memo,
                RothIra = request.IsRothIra,
                CreatedAtUtc = DateTime.UtcNow,
                UserName = _appUser != null ? _appUser.UserName : "unknown"
            };

            if (request.ThirdPartyPayee != null)
            {
                distribution.ThirdPartyPayee = new DistributionThirdPartyPayee
                {
                    Payee = request.ThirdPartyPayee.Payee,
                    Name = request.ThirdPartyPayee.Name,
                    Account = request.ThirdPartyPayee.Account,
                    Address = new Data.Entities.Address()
                    {
                        Street = request.ThirdPartyPayee.Address.Street,
                        Street2 = request.ThirdPartyPayee.Address.Street2,
                        Street3 = request.ThirdPartyPayee.Address.Street3,
                        Street4 = request.ThirdPartyPayee.Address.Street4,
                        City = request.ThirdPartyPayee.Address.City,
                        State = request.ThirdPartyPayee.Address.State,
                        PostalCode = request.ThirdPartyPayee.Address.PostalCode,
                        CountryIso = request.ThirdPartyPayee.Address.CountryIso ?? "US",
                    },
                    Memo = request.ThirdPartyPayee.Memo
                };
            }
            await ctx.Distributions.AddAsync(distribution, cancellationToken);
            await ctx.SaveChangesAsync(cancellationToken);

            return new CreateDistributionResponse
            {
                Id = distribution.Id,
                MaskSsn = distribution.Ssn.MaskSsn(),
                PaymentSequence = distribution.PaymentSequence,
                StatusId = distribution.StatusId,
                FrequencyId = distribution.FrequencyId,
                PayeeId = distribution.PayeeId,
                ForTheBenefitOfPayee = distribution.ForTheBenefitOfPayee,
                ForTheBenefitOfAccountType = distribution.ForTheBenefitOfAccountType,
                Tax1099ForEmployee = distribution.Tax1099ForEmployee,
                Tax1099ForBeneficiary = distribution.Tax1099ForBeneficiary,
                FederalTaxPercentage = distribution.FederalTaxPercentage,
                StateTaxPercentage = distribution.StateTaxPercentage,
                GrossAmount = distribution.GrossAmount,
                FederalTaxAmount = distribution.FederalTaxAmount,
                StateTaxAmount = distribution.StateTaxAmount,
                CheckAmount = distribution.CheckAmount,
                TaxCodeId = distribution.TaxCodeId,
                IsDeceased = distribution.IsDeceased,
                GenderId = distribution.GenderId,
                IsQdro = distribution.QualifiedDomesticRelationsOrder,
                Memo = distribution.Memo,
                IsRothIra = distribution.RothIra,
                ThirdPartyPayee = new ThirdPartyPayee()
                {
                    Account = distribution.ThirdPartyPayee?.Account,
                    Name = distribution.ThirdPartyPayee?.Name,
                    Payee = distribution.ThirdPartyPayee?.Payee,
                    Memo = distribution.ThirdPartyPayee?.Memo,
                    Address = new Common.Contracts.Request.Distributions.Address()
                    {
                        Street = distribution.ThirdPartyPayee?.Address?.Street ?? string.Empty,
                        Street2 = distribution.ThirdPartyPayee?.Address?.Street2,
                        Street3 = distribution.ThirdPartyPayee?.Address?.Street3,
                        Street4 = distribution.ThirdPartyPayee?.Address?.Street4,
                        City = distribution.ThirdPartyPayee?.Address?.City,
                        State = distribution.ThirdPartyPayee?.Address?.State,
                        PostalCode = distribution.ThirdPartyPayee?.Address?.PostalCode,
                        CountryIso = distribution.ThirdPartyPayee?.Address?.CountryIso ?? "US",
                    }
                }
            };
        }, cancellationToken);
}
}
