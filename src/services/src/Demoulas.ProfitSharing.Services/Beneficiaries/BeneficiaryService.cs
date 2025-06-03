using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Beneficiaries;
public class BeneficiaryService : IBeneficiaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public BeneficiaryService(IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }
    public async Task<CreateBeneficiaryResponse> CreateBeneficiary(CreateBeneficiaryRequest req, CancellationToken cancellationToken)
    {
        var rslt = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            if (req.FirstLevelBeneficiaryNumber.HasValue && (req.FirstLevelBeneficiaryNumber < 0 || req.FirstLevelBeneficiaryNumber > 9))
            {
                throw new InvalidOperationException("FirstLevelBeneficiaryNumber must be between 1 and 9");
            }

            if (req.SecondLevelBeneficiaryNumber.HasValue && (req.SecondLevelBeneficiaryNumber < 0 || req.SecondLevelBeneficiaryNumber > 9))
            {
                throw new InvalidOperationException("SecondLevelBeneficiaryNumber must be between 1 and 9");
            }

            if (req.ThirdLevelBeneficiaryNumber.HasValue && (req.ThirdLevelBeneficiaryNumber < 0 || req.ThirdLevelBeneficiaryNumber > 9))
            {
                throw new InvalidOperationException("ThirdLevelBeneficiaryNumber must be between 1 and 9");
            }

            //await ValidatePercentages(ctx, req.EmployeeBadgeNumber, req.Percentage, cancellationToken);

            var resp = new CreateBeneficiaryResponse();
            var beneficiaryContact = await GetOrCreateBeneficiaryContact(req, ctx, cancellationToken);

            resp.ContactExisted = beneficiaryContact.Id != 0;
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            
            var demographic = await demographics.Where(x=>x.BadgeNumber == req.EmployeeBadgeNumber).FirstOrDefaultAsync(cancellationToken);
            if (demographic == default)
            {
                throw new InvalidOperationException("EmployeeBadgeNumber is invalid");
            }
            
            var psnSuffix = await FindPsn(req, ctx, cancellationToken);
            var beneficiary = new Beneficiary
            {
                Id = 0,
                BadgeNumber = req.EmployeeBadgeNumber,
                PsnSuffix = psnSuffix,
                DemographicId = demographic!.Id,
                Contact = beneficiaryContact,
                BeneficiaryContactId = beneficiaryContact.Id,
                Relationship = req.Relationship,
                KindId = req.KindId,
                Percent = req.Percentage
            };

            ctx.Add(beneficiary);
            await ctx.SaveChangesAsync(cancellationToken);
            if (transaction != default)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            resp.PsnSuffix = psnSuffix;
            resp.BeneficiaryId = beneficiary.Id;
            
            return resp;
        }, cancellationToken);

        return rslt;
    }

    public async Task UpdateBeneficiary(UpdateBeneficiaryRequest req, CancellationToken cancellationToken)
    {
        _ = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            var beneficiary = await ctx.Beneficiaries.Include(x=>x.Contact).Include(x=>x.Contact!.ContactInfo).Include(x=>x.Contact!.Address).SingleAsync(x => x.Id == req.Id, cancellationToken);

            beneficiary.Contact!.ContactInfo.FirstName = req.FirstName;
            beneficiary.Contact!.ContactInfo.LastName = req.LastName;
            beneficiary.Contact!.Address.Street = req.Street;
            beneficiary.Contact!.Address.Street2 = req.Street2;
            beneficiary.Contact!.Address.Street3 = req.Street3;
            beneficiary.Contact!.Address.Street4 = req.Street4;
            beneficiary.Contact!.Address.City = req.City;
            beneficiary.Contact!.Address.State = req.State;
            beneficiary.Contact!.Address.PostalCode = req.PostalCode;
            beneficiary.Contact!.Address.CountryIso = req.CountryIso;
            beneficiary.KindId = req.KindId;

            if (!string.IsNullOrEmpty(req.Relationship))
            {
                beneficiary.Relationship = req.Relationship;
            }

            if (req.BeneficiarySsn.HasValue)
            {
                beneficiary.Contact.Ssn = req.BeneficiarySsn.Value;
            }

            await ctx.SaveChangesAsync(cancellationToken);
            if (transaction != null) 
            { 
                await transaction.CommitAsync(cancellationToken);
            }

            return Task.FromResult(true);
        }, cancellationToken);
    }

    public async Task DeleteBeneficiary(int id, CancellationToken cancellationToken)
    {
        _ = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            var beneficiaryToDelete = await ctx.Beneficiaries.Include(x=>x.Contact).Include(x=>x.Contact!.ContactInfo).Include(x=>x.Contact!.Address).SingleAsync(x=>x.Id == id);
            if (await CanIDeleteThisBeneficiary(beneficiaryToDelete, ctx, cancellationToken))
            {
                var deleteContact = false;
                if (beneficiaryToDelete.Contact != null)
                {
                    deleteContact = await CanIDeleteThisBeneficiaryContact(id, beneficiaryToDelete!.Contact, ctx, cancellationToken);
                }

                ctx.Beneficiaries.Remove(beneficiaryToDelete);
                if (deleteContact && beneficiaryToDelete.Contact != null)
                {
                    if (beneficiaryToDelete!.Contact.Address != null)
                    {
                        ctx.Remove(beneficiaryToDelete!.Contact.Address);
                    }
                    if (beneficiaryToDelete!.Contact.ContactInfo != null)
                    {
                        ctx.Remove(beneficiaryToDelete!.Contact.ContactInfo);
                    }
                    ctx.BeneficiaryContacts.Remove(beneficiaryToDelete.Contact);
                }
            }

            await ctx.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }, cancellationToken);
    }

    public async Task DeleteBeneficiaryContact(int id, CancellationToken cancellation)
    {
        _ = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            var contactToDelete = await ctx.BeneficiaryContacts.Include(x => x.Beneficiaries).SingleAsync(x => x.Id == id, cancellation);
            var deleteContact = true;
            Beneficiary? beneficiaryToDelete = null;
            if (contactToDelete.Beneficiaries?.Count == 1) //If contact is only associated with one beneficiary, check to see if we can delete it.
            {
                var firstBeneficiary = contactToDelete.Beneficiaries.First();
                deleteContact = await CanIDeleteThisBeneficiary(firstBeneficiary, ctx, cancellation);
                beneficiaryToDelete = firstBeneficiary;
            }
            if (deleteContact)
            {
                if (beneficiaryToDelete != null)
                {
                    ctx.Beneficiaries.Remove(beneficiaryToDelete);
                }
                if (contactToDelete.Address != null)
                {
                    ctx.Remove(contactToDelete.Address);
                }
                if (contactToDelete.ContactInfo != null)
                {
                    ctx.Remove(contactToDelete.ContactInfo);
                }
                ctx.BeneficiaryContacts.Remove(contactToDelete);
                await ctx.SaveChangesAsync(cancellation);
                await transaction.CommitAsync(cancellation);
            }
            return true;
        }, cancellation);
    }

    private async Task<bool> CanIDeleteThisBeneficiary(Beneficiary beneficiary, ProfitSharingDbContext ctx, CancellationToken cancellationToken)
    {
        var balanceInfo = await _totalService.GetVestingBalanceForSingleMemberAsync(Common.Contracts.Request.SearchBy.Ssn, beneficiary.Contact!.Ssn, (short)DateTime.Now.Year, cancellationToken);
        if (balanceInfo?.CurrentBalance != 0) {
            throw new InvalidOperationException("Balance is not zero, cannot delete beneficiary.");
        }

        if (await ctx.DistributionPayees.AnyAsync(x => x.Ssn == beneficiary.Contact!.Ssn, cancellationToken))
        {
            throw new InvalidOperationException("Beneficiary is a payee for a distribution, cannot delete beneficiary.");
        }

        return true;
    }

    private async Task<bool> CanIDeleteThisBeneficiaryContact(int beneficiaryId, BeneficiaryContact contact, ProfitSharingDbContext ctx, CancellationToken token)
    {
        return !(await ctx.Beneficiaries.AnyAsync(b => b.BeneficiaryContactId == contact.Id && b.Id != beneficiaryId, token));
    }
    private async Task ValidatePercentages(ProfitSharingDbContext ctx, int badgeNumber, byte proposedPctOfNewBeneficiary, CancellationToken token)
    {
        var beneficiaries = await ctx.Beneficiaries.Where(x=>x.DemographicId == badgeNumber).OrderBy(x=>x.Psn).ToListAsync(token);
        var rootBeneficiaries = new List<Beneficiary>();

        foreach (var beneficiary in beneficiaries)
        {
            int childMask = 10;
            if (beneficiary.PsnSuffix % 100 == 0 && beneficiary.PsnSuffix % 1000 != 0)
            {
                childMask = 100;
            } else if (beneficiary.PsnSuffix % 1000 == 0)
            {
                childMask = 1000;
            }
            if (!beneficiaries.Any(x=>x.PsnSuffix > beneficiary.PsnSuffix && x.PsnSuffix < beneficiary.PsnSuffix + childMask))
            {
                rootBeneficiaries.Add(beneficiary);
            }
        }

        if (rootBeneficiaries.Sum(x=>x.Percent) + proposedPctOfNewBeneficiary > 100)
        {
            throw new InvalidOperationException("Total percentage for employee would be more than 100%");
        }
        
        

    }

    private static async Task<BeneficiaryContact> GetOrCreateBeneficiaryContact(CreateBeneficiaryRequest req, ProfitSharingDbContext ctx, CancellationToken token)
    {
        var beneficiaryContact = await ctx.BeneficiaryContacts.Where(x=>x.Ssn == req.BeneficiarySsn).FirstOrDefaultAsync(token);
        if (beneficiaryContact != default)
        {
            return beneficiaryContact;
        }

        beneficiaryContact = new BeneficiaryContact()
        {
            Id = 0,
            Ssn = req.BeneficiarySsn,
            DateOfBirth = req.DateOfBirth,
            Address = new Address()
            {
                Street = req.Street,
                Street2 = req.Street2,
                Street3 = req.Street3,
                Street4 = req.Street4,
                City = req.City,
                State = req.State,
                PostalCode = req.PostalCode,
                CountryIso = req.CountryIso,
            },
            ContactInfo = new ContactInfo()
            {
                FullName = $"{req.LastName}, {req.FirstName}",
                FirstName = req.FirstName,
                LastName = req.LastName,
                MiddleName = req.MiddleName,
                PhoneNumber = req.PhoneNumber,
                MobileNumber = req.MobileNumber,
                EmailAddress = req.EmailAddress,
            },
            CreatedDate = DateTime.Now.ToDateOnly(),
        };
        ctx.Add(beneficiaryContact);

        return beneficiaryContact;
    }

    private static async Task<short> FindPsn(CreateBeneficiaryRequest req, ProfitSharingDbContext ctx, CancellationToken token)
    {
        int minPsn = 0;
        short psnRange = 10000; 
        if (req.ThirdLevelBeneficiaryNumber.HasValue && req.ThirdLevelBeneficiaryNumber.Value > 0)
        {
            minPsn = 
                ((req.FirstLevelBeneficiaryNumber ?? 0) * 1000) +
                ((req.SecondLevelBeneficiaryNumber ?? 0) * 100) +
                ((req.ThirdLevelBeneficiaryNumber ?? 0) * 10);
            psnRange = 10;
        }
        else if (req.SecondLevelBeneficiaryNumber.HasValue && req.SecondLevelBeneficiaryNumber.Value > 0)
        {
            minPsn =
                ((req.FirstLevelBeneficiaryNumber ?? 0) * 1000) +
                ((req.SecondLevelBeneficiaryNumber ?? 0) * 100);
            psnRange = 100;
        }
        else if (req.FirstLevelBeneficiaryNumber.HasValue && req.FirstLevelBeneficiaryNumber.Value > 0)
        {
            minPsn = 
                (req.FirstLevelBeneficiaryNumber.Value * 1000);
            psnRange = 1000;
        }

        short currentMaxPsn = 0;
        var qry = (from bc in ctx.Beneficiaries
                   where bc.BadgeNumber == req.EmployeeBadgeNumber
                      && bc.PsnSuffix > minPsn
                      && bc.PsnSuffix < minPsn + psnRange
                   select bc.PsnSuffix);
        if (await qry.AnyAsync(token))
        {
            currentMaxPsn = (short)((await qry.MaxAsync(token)) - minPsn);
        }

        return (short)(minPsn + currentMaxPsn + (psnRange / 10));
    }
}
