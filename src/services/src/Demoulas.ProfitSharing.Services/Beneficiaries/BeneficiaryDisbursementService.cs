using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Beneficiaries;

public sealed class BeneficiaryDisbursementService : IBeneficiaryDisbursementService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public BeneficiaryDisbursementService(IProfitSharingDataContextFactory dataContextFactory, IDemographicReaderService demographicReaderService, TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public async Task<Result<bool>> DisburseFundsToBeneficiaries(BeneficiaryDisbursementRequest request, CancellationToken cancellationToken)
    {
        var rslt = await _dataContextFactory.UseWritableContext(async context =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(context, false);
            int disburserSsn = 0;
            var profitYear = (short)DateTime.Now.Year;
            int disburserDemographicId = 0;
            long disburserOracleHcmId = 0;

            // Validations
            // -- Does the Disburser exist
            if (request.PsnSuffix.HasValue)
            {
                // Validate with suffix
                var beneficiary = await context.Beneficiaries.Include(x => x.Contact).FirstOrDefaultAsync(b => b.BadgeNumber == request.BadgeNumber && b.PsnSuffix == request.PsnSuffix, cancellationToken);
                if (beneficiary == default)
                {
                    return Result<bool>.Failure(Error.DisburserDoesNotExist);
                }
                var member = await demographics.FirstAsync(d => d.BadgeNumber == request.BadgeNumber, cancellationToken);
                disburserSsn = member.Ssn;
                disburserOracleHcmId = member.OracleHcmId;
                disburserDemographicId = member.Id;
            }
            else
            {
                // Validate without suffix
                var member = await demographics.FirstOrDefaultAsync(d => d.BadgeNumber == request.BadgeNumber, cancellationToken);
                if (member == default)
                {
                    return Result<bool>.Failure(Error.DisburserDoesNotExist);
                }

                // -- If disbursement type is death, is the disburser deceased
                if (request.IsDeceased && member.TerminationCodeId != TerminationCode.Constants.Deceased)
                {
                    return Result<bool>.Failure(Error.DisburserIsStillMarkedAlive);
                }

                disburserSsn = member.Ssn;
                disburserOracleHcmId = member.OracleHcmId;
                disburserDemographicId = member.Id;
            }

            // -- Does each Beneficiary exist
            foreach (var beneficiary in request.Beneficiaries)
            {
                // Validate with suffix
                if (await context.Beneficiaries.AnyAsync(b => b.BadgeNumber == request.BadgeNumber && b.PsnSuffix == beneficiary.PsnSuffix, cancellationToken) == false)
                {
                    return Result<bool>.Failure(Error.BeneficiaryDoesNotExist($"{request.BadgeNumber}-{beneficiary.PsnSuffix}"));
                }
            }

            if (request.Beneficiaries.Sum(x => x.Percentage ?? 0) > 100)
            {
                return Result<bool>.Failure(Error.PercentageMoreThan100);
            }

            if (request.Beneficiaries.Any(x => x.Percentage.HasValue) && request.Beneficiaries.Any(x => x.Amount.HasValue))
            {
                return Result<bool>.Failure(Error.CantMixPercentageAndAmount);
            }

            if (request.Beneficiaries.Any(x => (x.Percentage ?? 0) < 0) || request.Beneficiaries.Any(x => (x.Amount ?? 0) < 0))
            {
                return Result<bool>.Failure(Error.PercentageAndAmountsMustBePositive);
            }

            var balanceSet = await _totalService.GetTotalBalanceSet(context, profitYear).Where(x => x.Ssn == disburserSsn).FirstOrDefaultAsync(cancellationToken);
            var balance = balanceSet?.TotalAmount ?? 0;
            var totalRequestedDisbursement = request.Beneficiaries.Sum(x => x.Amount ?? 0) + request.Beneficiaries.Sum(x => balance * ((x.Percentage ?? 0) / 100));

            if (totalRequestedDisbursement > balance)
            {
                return Result<bool>.Failure(Error.NotEnoughFundsToCoverAmounts);
            }

            if (totalRequestedDisbursement != balance && request.IsDeceased)
            {
                return Result<bool>.Failure(Error.RemainingAmountToDisburse(totalRequestedDisbursement - balance));
            }

            foreach (var b in request.Beneficiaries)
            {
                var beneficiaryMember = await context.Beneficiaries.Include(x => x.Contact).FirstAsync(bn => bn.BadgeNumber == request.BadgeNumber && bn.PsnSuffix == b.PsnSuffix);
                var beneficiaryAsEmployee = await demographics.FirstOrDefaultAsync(d => d.Ssn == beneficiaryMember.Contact!.Ssn, cancellationToken);

                var amount = b.Amount ?? balance * ((b.Percentage ?? 0) / 100);

                var transferFromProfitDetail = new ProfitDetail
                {
                    Ssn = disburserSsn,
                    ProfitYear = profitYear,
                    ProfitCodeId = ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                    Forfeiture = amount,
                    MonthToDate = (byte)DateTime.Now.Month,
                    YearToDate = (short)DateTime.Now.Year,
                    CommentTypeId = request.IsDeceased ? CommentType.Constants.TransferOut : CommentType.Constants.QdroOut,
                    Remark = $"{(request.IsDeceased ? "XREF>" : "QDRO>")}{request.BadgeNumber}{request.PsnSuffix?.ToString("0000") ?? "0000"}",
                    CommentRelatedOracleHcmId = disburserOracleHcmId,
                    CommentRelatedPsnSuffix = b.PsnSuffix,
                };

                var transferToProfitDetail = new ProfitDetail
                {
                    Ssn = beneficiaryMember.Contact!.Ssn,
                    ProfitYear = profitYear,
                    ProfitCodeId = ProfitCode.Constants.IncomingQdroBeneficiary.Id,
                    Contribution = amount,
                    MonthToDate = (byte)DateTime.Now.Month,
                    YearToDate = (short)DateTime.Now.Year,
                    CommentTypeId = request.IsDeceased ? CommentType.Constants.TransferIn : CommentType.Constants.QdroIn,
                    Remark = $"{(request.IsDeceased ? "XREF<" : "QDRO<")}{request.BadgeNumber}{request.PsnSuffix?.ToString("0000") ?? "0000"}",
                    CommentRelatedOracleHcmId = disburserOracleHcmId,
                    CommentRelatedPsnSuffix = request.PsnSuffix,
                };

                // If beneficiary is an employee, add amount to ETVA
                if (beneficiaryAsEmployee != default)
                {
                    var beneficiaryPayProfit = await context.PayProfits.FirstAsync(x => x.DemographicId == beneficiaryAsEmployee.Id && x.ProfitYear == profitYear, cancellationToken);
                    beneficiaryPayProfit.Etva += amount;
                }

                await context.ProfitDetails.AddAsync(transferFromProfitDetail, cancellationToken);
                await context.ProfitDetails.AddAsync(transferToProfitDetail, cancellationToken);
            }

            //If balance - amount > etva amount, adjust ETVA accordingly
            var disburserPayProfit = await context.PayProfits.FirstAsync(x => x.DemographicId == disburserDemographicId && x.ProfitYear == profitYear, cancellationToken);
            var nonEtvaAmount = balance - disburserPayProfit.Etva;
            if (nonEtvaAmount - totalRequestedDisbursement < 0)
            {
                disburserPayProfit.Etva += Math.Max(-disburserPayProfit.Etva, nonEtvaAmount - totalRequestedDisbursement);
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }, cancellationToken);

        return rslt;

    }
}
