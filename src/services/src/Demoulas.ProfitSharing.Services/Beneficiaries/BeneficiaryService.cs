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
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Beneficiaries;
public class BeneficiaryService : IBeneficiaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public BeneficiaryService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
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

            var resp = new CreateBeneficiaryResponse();
            var beneficiaryContact = await GetOrCreateBeneficiaryContact(req, ctx, cancellationToken);

            resp.ContactExisted = beneficiaryContact.Id != 0;

            var demographic = await ctx.Demographics.Where(x=>x.BadgeNumber == req.EmployeeBadgeNumber).FirstOrDefaultAsync(cancellationToken);
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
