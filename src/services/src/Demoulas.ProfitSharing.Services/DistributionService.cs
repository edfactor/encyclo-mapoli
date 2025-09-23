using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
public sealed class DistributionService : IDistributionService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IAppUser? _appUser;
    private readonly TotalService _totalService;

    public DistributionService(IProfitSharingDataContextFactory dataContextFactory, IDemographicReaderService demographicReaderService, IAppUser? appUser, TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _appUser = appUser;
        _totalService = totalService;
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
                    var ssn = await ctx.Beneficiaries.Where(x => x.BadgeNumber == request.BadgeNumber.Value && x.PsnSuffix == request.PsnSuffix.Value).Select(x => x.Contact!.Ssn).FirstOrDefaultAsync(cancellationToken);
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

    public async Task<CreateOrUpdateDistributionResponse> CreateDistribution(CreateDistributionRequest request, CancellationToken cancellationToken)
    {

        ValidateDistributionRequest(request);

        return await _dataContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var dem = await demographic.Where(d => d.BadgeNumber == request.BadgeNumber).FirstOrDefaultAsync(cancellationToken);
            if (dem == null)
            {
                throw new InvalidOperationException("Badge number not found.");
            }

            var balance = await _totalService.GetVestingBalanceForSingleMemberAsync(Common.Contracts.Request.SearchBy.Ssn, dem.Ssn, (short)DateTime.Today.Year, cancellationToken);
            if (request.TaxCodeId == TaxCode.Constants.NormalDistribution.Id && dem.DateOfBirth.Age() > 64)
            {
                if (balance != default && balance.VestedBalance != request.GrossAmount && string.IsNullOrEmpty(request.Memo))
                {
                    request.Memo = "AGE>64 - OVERRIDE";
                }
            }

            if (request.GrossAmount > (balance?.VestedBalance ?? 0))
            {
                throw new InvalidOperationException($"Gross amount {request.GrossAmount:C} exceeds vested balance {(balance?.VestedBalance ?? 0):C}.");
            }

            if (request.FrequencyId == DistributionFrequency.Constants.RolloverDirect)
            {
                request.FederalTaxAmount = 0;
                request.FederalTaxPercentage = 0;
                request.StateTaxAmount = 0;
                request.StateTaxPercentage = 0;
                request.CheckAmount = request.GrossAmount;
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
                GrossAmount = request.GrossAmount,
                FederalTaxAmount = request.FederalTaxAmount,
                StateTaxAmount = request.StateTaxAmount,
                TaxCodeId = request.TaxCodeId,
                IsDeceased = request.IsDeceased,
                GenderId = request.GenderId,
                QualifiedDomesticRelationsOrder = request.IsQdro,
                Memo = request.Memo,
                RothIra = request.IsRothIra,
                CreatedAtUtc = DateTime.UtcNow,
                UserName = _appUser != null ? _appUser.UserName : "unknown",
                ThirdPartyPayeeAccount = request.ThirdPartyPayee?.Account
            };

            if (request.ThirdPartyPayee != null)
            {
                distribution.ThirdPartyPayee = new DistributionThirdPartyPayee
                {
                    Payee = request.ThirdPartyPayee.Payee,
                    Name = request.ThirdPartyPayee.Name,
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

            return new CreateOrUpdateDistributionResponse
            {
                Id = distribution.Id,
                BadgeNumber = request.BadgeNumber, // Include BadgeNumber from the original request
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
                    Account = distribution.ThirdPartyPayeeAccount,
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

    public async Task<Result<CreateOrUpdateDistributionResponse>> UpdateDistribution(UpdateDistributionRequest request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateDistributionRequest(request);
        if (!validationResult.IsSuccess)
        {
            return Result<CreateOrUpdateDistributionResponse>.Failure(validationResult.Error!);
        }

        return await _dataContextFactory.UseWritableContext(async ctx =>
        {
            var distribution = await ctx.Distributions
                .Include(d => d.ThirdPartyPayee)
                .ThenInclude(tp => tp!.Address) 
                .Where(d => d.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (distribution == null)
            {
                return Result<CreateOrUpdateDistributionResponse>.Failure(Error.DistributionNotFound);
            }
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var dem = await demographic.Where(d => d.BadgeNumber == request.BadgeNumber).FirstOrDefaultAsync(cancellationToken);
            if (dem == null)
            {
                return Result<CreateOrUpdateDistributionResponse>.Failure(Error.BadgeNumberNotFound);
            }
            var balance = await _totalService.GetVestingBalanceForSingleMemberAsync(Common.Contracts.Request.SearchBy.Ssn, dem.Ssn, (short)DateTime.Today.Year, cancellationToken);
            if (request.TaxCodeId == TaxCode.Constants.NormalDistribution.Id && dem.DateOfBirth.Age() > 64)
            {
                if (balance != default && balance.VestedBalance != request.GrossAmount && string.IsNullOrEmpty(request.Memo))
                {
                    request.Memo = "AGE>64 - OVERRIDE";
                }
            }
            var originalGrossAmount = distribution.GrossAmount;
            if (request.GrossAmount > (balance != default ? balance.VestedBalance - originalGrossAmount : originalGrossAmount))
            {
                var validationErrors = new Dictionary<string, string[]>
                {
                    [nameof(request.GrossAmount)] = [$"Gross amount {request.GrossAmount:C} exceeds vested balance {(balance?.VestedBalance ?? 0):C}."]
                };
                return Result<CreateOrUpdateDistributionResponse>.ValidationFailure(validationErrors);
            }

            if (request.FrequencyId == DistributionFrequency.Constants.RolloverDirect)
            {
                request.FederalTaxAmount = 0;
                request.FederalTaxPercentage = 0;
                request.StateTaxAmount = 0;
                request.StateTaxPercentage = 0;
                request.CheckAmount = request.GrossAmount;
            }

            distribution.StatusId = request.StatusId;
            distribution.FrequencyId = request.FrequencyId;
            if (request.ThirdPartyPayee != default)
            {
                distribution.ThirdPartyPayee = distribution.ThirdPartyPayee ?? new DistributionThirdPartyPayee()
                {
                    Address = new Data.Entities.Address() { Street = string.Empty }
                };
                distribution.ThirdPartyPayee!.Name = request.ThirdPartyPayee.Name;
                distribution.ThirdPartyPayee!.Payee = request.ThirdPartyPayee.Payee;
                distribution.ThirdPartyPayee!.Memo = request.ThirdPartyPayee.Memo;
                distribution.ThirdPartyPayee!.Address.Street = request.ThirdPartyPayee.Address.Street;
                distribution.ThirdPartyPayee!.Address.Street2 = request.ThirdPartyPayee.Address.Street2;
                distribution.ThirdPartyPayee!.Address.Street3 = request.ThirdPartyPayee.Address.Street3;
                distribution.ThirdPartyPayee!.Address.Street4 = request.ThirdPartyPayee.Address.Street4;
                distribution.ThirdPartyPayee!.Address.City = request.ThirdPartyPayee.Address.City;
                distribution.ThirdPartyPayee!.Address.State = request.ThirdPartyPayee.Address.State;
                distribution.ThirdPartyPayee!.Address.PostalCode = request.ThirdPartyPayee.Address.PostalCode;
                distribution.ThirdPartyPayee!.Address.CountryIso = request.ThirdPartyPayee.Address.CountryIso ?? "US";
                distribution.ThirdPartyPayeeAccount = request.ThirdPartyPayee?.Account;
            }
            else
            {
                distribution.ThirdPartyPayeeId = null;
            }
            distribution.ForTheBenefitOfAccountType = request.ForTheBenefitOfAccountType;
            distribution.ForTheBenefitOfPayee = request.ForTheBenefitOfPayee;
            distribution.Tax1099ForBeneficiary = request.Tax1099ForBeneficiary;
            distribution.Tax1099ForEmployee = request.Tax1099ForEmployee;
            distribution.GrossAmount = request.GrossAmount;
            distribution.FederalTaxAmount = request.FederalTaxAmount;
            distribution.StateTaxAmount = request.StateTaxAmount;
            distribution.TaxCodeId = request.TaxCodeId;
            distribution.IsDeceased = request.IsDeceased;
            distribution.GenderId = request.GenderId;
            distribution.QualifiedDomesticRelationsOrder = request.IsQdro;
            distribution.Memo = request.Memo;
            distribution.RothIra = request.IsRothIra;
            distribution.ModifiedAtUtc = DateTime.UtcNow;

            await ctx.SaveChangesAsync(cancellationToken);
            var response = new CreateOrUpdateDistributionResponse
            {
                Id = distribution.Id,
                BadgeNumber = request.BadgeNumber,
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
                    Account = distribution.ThirdPartyPayeeAccount,
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
            return Result<CreateOrUpdateDistributionResponse>.Success(response);
        }, cancellationToken);
    }

    private Result<bool> ValidateDistributionRequest(CreateDistributionRequest request)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request.BadgeNumber < 1 || request.BadgeNumber > 9_999_999)
        {
            validationErrors[nameof(request.BadgeNumber)] = ["BadgeNumber must be a 7-digit number."];
        }
        if (request.GrossAmount <= 0)
        {
            validationErrors[nameof(request.GrossAmount)] = ["Gross amount must be greater than zero."];
        }
        if (request.FederalTaxPercentage < 0 || request.FederalTaxPercentage > 100)
        {
            validationErrors[nameof(request.FederalTaxPercentage)] = ["Federal tax percentage must be between 0 and 100."];
        }
        if (request.StateTaxPercentage < 0 || request.StateTaxPercentage > 100)
        {
            validationErrors[nameof(request.StateTaxPercentage)] = ["State tax percentage must be between 0 and 100."];
        }
        if (request.ThirdPartyPayee != default && request.FrequencyId != DistributionFrequency.Constants.RolloverDirect)
        {
            validationErrors[nameof(request.ThirdPartyPayee)] = ["Third party payee can only be set for Rollover Direct frequency."];
        }

        return validationErrors.Count > 0 
            ? Result<bool>.ValidationFailure(validationErrors) 
            : Result<bool>.Success(true);
    }
}
