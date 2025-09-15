using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Extensions;
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
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
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
            var beneficiaryContact = await ctx.BeneficiaryContacts.FirstOrDefaultAsync(x=>x.Id == req.BeneficiaryContactId, cancellationToken);
            if (beneficiaryContact == default)
            {
                throw new InvalidOperationException("Beneficiary Contact does not exist");
            }
            
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var demographic = await demographicQuery.Where(x=>x.BadgeNumber == req.EmployeeBadgeNumber).SingleOrDefaultAsync(cancellationToken);
            if (demographic == default)
            {
                throw new InvalidOperationException("Employee Badge does not exist");
            }

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
            if (req.Percentage < 0 || req.Percentage > 100)
            {
                throw new InvalidOperationException("Invalid percentage");
            }

            //await ValidatePercentages(ctx, req.EmployeeBadgeNumber, req.Percentage, cancellationToken);
            
            var psnSuffix = await FindPsn(req, ctx, cancellationToken);
            var beneficiary = new Beneficiary
            {
                Id = 0,
                BadgeNumber = req.EmployeeBadgeNumber,
                PsnSuffix = psnSuffix,
                DemographicId = demographic.Id,
                Contact = beneficiaryContact,
                BeneficiaryContactId = req.BeneficiaryContactId,
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

            var resp = new CreateBeneficiaryResponse()
            {
                BeneficiaryId = beneficiary.Id,
                PsnSuffix = psnSuffix,
                EmployeeBadgeNumber = req.EmployeeBadgeNumber,
                DemographicId = demographic.Id,
                BeneficiaryContactId = beneficiaryContact.Id,
                Relationship = beneficiary.Relationship,
                KindId = beneficiary.KindId,
                Percent = beneficiary.Percent
            };
            
            return resp;
        }, cancellationToken);

        return rslt;
    }

    public async Task<CreateBeneficiaryContactResponse> CreateBeneficiaryContact(CreateBeneficiaryContactRequest req, CancellationToken cancellationToken)
    {
        var rslt = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            if (req.ContactSsn > 999999999)
            {
                throw new InvalidOperationException("Contact Ssn must be 9 digits");
            }
            if (await ctx.BeneficiaryContacts.AnyAsync(x => x.Ssn == req.ContactSsn, cancellationToken))
            {
                throw new InvalidOperationException("Contact Ssn already exists");
            }
            var beneficiaryContact = new BeneficiaryContact() {
                Id = 0,
                Ssn = req.ContactSsn,
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
                CreatedDate = DateTime.Now.ToDateOnly()
            };
            ctx.Add(beneficiaryContact);

            await ctx.SaveChangesAsync(cancellationToken);
            if (transaction != default)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return beneficiaryContact;
        }, cancellationToken);

        var response = new CreateBeneficiaryContactResponse
        {
            Id = rslt.Id,
            Ssn = rslt.Ssn.MaskSsn(),
            DateOfBirth = rslt.DateOfBirth,
            Street = rslt.Address.Street,
            Street2 = rslt.Address.Street2,
            Street3 = rslt.Address.Street3,
            Street4 = rslt.Address.Street4,
            City = rslt.Address.City ?? string.Empty,
            State = rslt.Address.State ?? string.Empty,
            PostalCode = rslt.Address.PostalCode ?? string.Empty,
            CountryIso = rslt.Address.CountryIso,
            FirstName = rslt.ContactInfo!.FirstName,
            LastName = rslt.ContactInfo.LastName,
            MiddleName = rslt.ContactInfo.MiddleName,
            PhoneNumber = rslt.ContactInfo.PhoneNumber,
            MobileNumber = rslt.ContactInfo.MobileNumber,
            EmailAddress = rslt.ContactInfo.EmailAddress
        };

        return response;
    }

    public async Task<UpdateBeneficiaryResponse> UpdateBeneficiary(UpdateBeneficiaryRequest req, CancellationToken cancellationToken)
    {
        var resp = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            var beneficiary = await ctx.Beneficiaries.SingleAsync(x => x.Id == req.Id, cancellationToken);

            if (req.KindId.HasValue)
            {
                beneficiary.KindId = req.KindId.Value;
            }

            if (!string.IsNullOrEmpty(req.Relationship))
            {
                beneficiary.Relationship = req.Relationship;
            }

            if (req.Percentage.HasValue)
            {
                beneficiary.Percent = req.Percentage.Value;
            }
            var response = new UpdateBeneficiaryResponse()
            {
                Id = beneficiary.Id,
                BeneficiaryContactId = beneficiary.BeneficiaryContactId,
                KindId = beneficiary.KindId,
                Relationship = beneficiary.Relationship,
                Percentage = beneficiary.Percent,
                DemographicId = beneficiary.DemographicId,
                BadgeNumber = beneficiary.BadgeNumber,
                Street1 = string.Empty,
                City = string.Empty,
                State = string.Empty,
                PostalCode = string.Empty,
                Ssn = string.Empty,
                DateOfBirth = DateOnly.MinValue,
                FullName = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
            };
            var contact = await SetBeneficiaryContactColumns(beneficiary.BeneficiaryContactId, req, ctx, response, cancellationToken);
            response.BeneficiaryContactId = contact.Id;
            response.Id = beneficiary.Id;

            await ctx.SaveChangesAsync(cancellationToken);
            if (transaction != null) 
            { 
                await transaction.CommitAsync(cancellationToken);
            }

            return Task.FromResult(response);
        }, cancellationToken);

        return await resp;
    }


    public Task<UpdateBeneficiaryContactResponse> UpdateBeneficiaryContact(UpdateBeneficiaryContactRequest req, CancellationToken cancellationToken)
    {
        var response = _dataContextFactory.UseWritableContext(async (ctx) =>
        {
            var resp = new UpdateBeneficiaryContactResponse()
            {
                Ssn = string.Empty,
                DateOfBirth = DateOnly.MinValue,
                Street1 = string.Empty,
                City = string.Empty,
                State = string.Empty,
                PostalCode = string.Empty,
                FullName = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                            };
            _ = await SetBeneficiaryContactColumns(req.Id, req, ctx, resp, cancellationToken);

            await ctx.SaveChangesAsync(cancellationToken);
          
            return resp;
        }, cancellationToken);
        return response;
    }

    private static async Task<BeneficiaryContact> SetBeneficiaryContactColumns(int beneficiaryContactId, UpdateBeneficiaryContactRequest req, ProfitSharingDbContext ctx, UpdateBeneficiaryContactResponse response, CancellationToken cancellationToken)
    {
        var contact = await ctx.BeneficiaryContacts.Include(x => x.Address).Include(x => x.ContactInfo).SingleAsync(x => x.Id == beneficiaryContactId, cancellationToken);
        if (req.ContactSsn is > 999999999)
        {
            throw new InvalidOperationException("Contact Ssn must be 9 digits");
        }

        if (req.ContactSsn.HasValue)
        {
            contact.Ssn = req.ContactSsn.Value;
        }
        if (req.DateOfBirth.HasValue)
        {
            contact.DateOfBirth = req.DateOfBirth.Value;
        }
        if (!string.IsNullOrEmpty(req.Street1))
        {
            contact.Address.Street = req.Street1;
        }
        if (req.Street2 != null)
        {
            if (req.Street2 == string.Empty)
            {
                req.Street2 = null;
            }
            else
            {
                req.Street2 = req.Street2.Trim();
            }
        }
        if (req.Street3 != null)
        {
            if (req.Street3 == string.Empty)
            {
                req.Street3 = null;
            }
            else
            {
                req.Street3 = req.Street3.Trim();
            }
        }
        if (req.Street4 != null)
        {
            if (req.Street4 == string.Empty)
            {
                req.Street4 = null;
            }
            else
            {
                req.Street4 = req.Street4.Trim();
            }
        }
        if (!string.IsNullOrEmpty(req.City))
        {
            contact.Address.City = req.City.Trim();
        }
        if (!string.IsNullOrEmpty(req.State))
        {
            contact.Address.State = req.State.Trim();
        }
        if (!string.IsNullOrEmpty(req.PostalCode))
        {
            contact.Address.PostalCode = req.PostalCode.Trim();
        }
        if (req.CountryIso != null)
        {
            if (req.CountryIso == string.Empty)
            {
                req.CountryIso = null;
            }
            else
            {
                req.CountryIso = req.CountryIso.Trim();
            }
        }
        if (!string.IsNullOrEmpty(req.FirstName))
        {
            contact.ContactInfo!.FirstName = req.FirstName.Trim();
            contact.ContactInfo!.FullName = $"{contact.ContactInfo!.LastName}, {req.FirstName}";
        }
        if (!string.IsNullOrEmpty(req.LastName))
        {
            contact.ContactInfo!.LastName = req.LastName.Trim();
            contact.ContactInfo!.FullName = $"{req.LastName}, {contact.ContactInfo!.FirstName}";
        }
        if (req.MiddleName != null)
        {
            if (req.MiddleName == string.Empty)
            {
                req.MiddleName = null;
            }
            else
            {
                req.MiddleName = req.MiddleName.Trim();
            }
        }
        if (req.PhoneNumber != null)
        {
            if (req.PhoneNumber == string.Empty)
            {
                req.PhoneNumber = null;
            }
            else
            {
                req.PhoneNumber = req.PhoneNumber.Trim();
            }
        }
        if (req.MobileNumber != null)
        {
            if (req.MobileNumber == string.Empty)
            {
                req.MobileNumber = null;
            }
            else
            {
                req.MobileNumber = req.MobileNumber.Trim();
            }
        }
        if (req.EmailAddress != null)
        {
            if (req.EmailAddress == string.Empty)
            {
                req.EmailAddress = null;
            }
            else
            {
                req.EmailAddress = req.EmailAddress.Trim();
            }
        }

        response.Id = contact.Id;
        response.Ssn = contact.Ssn.MaskSsn();
        response.DateOfBirth = contact.DateOfBirth;
        response.Street1 = contact.Address.Street;
        response.Street2 = contact.Address.Street2;
        response.Street3 = contact.Address.Street3;
        response.Street4 = contact.Address.Street4;
        response.City = contact.Address.City ?? string.Empty;
        response.State = contact.Address.State ?? string.Empty;
        response.PostalCode = contact.Address.PostalCode ?? string.Empty;
        response.CountryIso = contact.Address.CountryIso ?? string.Empty;
        response.FullName = contact.ContactInfo!.FullName ?? string.Empty;
        response.FirstName = contact.ContactInfo!.FirstName;
        response.LastName = contact.ContactInfo.LastName;
        response.MiddleName = contact.ContactInfo.MiddleName;
        response.PhoneNumber = contact.ContactInfo.PhoneNumber;
        response.MobileNumber = contact.ContactInfo.MobileNumber;
        response.EmailAddress = contact.ContactInfo.EmailAddress;

        return contact;
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
        if (balanceInfo!=null && balanceInfo?.CurrentBalance != 0) {
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
