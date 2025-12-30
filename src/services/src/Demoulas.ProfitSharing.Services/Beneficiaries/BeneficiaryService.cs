using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Beneficiaries.Validators;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Beneficiaries;

public class BeneficiaryService : IBeneficiaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly CreateBeneficiaryRequestValidator _createBeneficiaryValidator;
    private readonly CreateBeneficiaryContactRequestValidator _createBeneficiaryContactValidator;
    private readonly UpdateBeneficiaryContactRequestValidator _updateBeneficiaryContactValidator;
    private readonly BeneficiaryDatabaseValidator _databaseValidator;

    public BeneficiaryService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
        _createBeneficiaryValidator = new CreateBeneficiaryRequestValidator();
        _createBeneficiaryContactValidator = new CreateBeneficiaryContactRequestValidator();
        _updateBeneficiaryContactValidator = new UpdateBeneficiaryContactRequestValidator();
        _databaseValidator = new BeneficiaryDatabaseValidator(demographicReaderService);
    }
    public async Task<CreateBeneficiaryResponse> CreateBeneficiary(CreateBeneficiaryRequest req, CancellationToken cancellationToken)
    {
        // Validate request using FluentValidation
        var validationResult = await _createBeneficiaryValidator.ValidateAsync(req, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Validate percentage constraints before creating beneficiary
        if (req.EmployeeBadgeNumber <= 0)
        {
            throw new ValidationException("Badge number must be greater than 0.");
        }

        if (req.Percentage <= 0 || req.Percentage > 100m)
        {
            throw new ValidationException("Percentage must be between 0 and 100%.");
        }

        // Check that sum of percentages doesn't exceed 100%
        var existingPercentageSum = await GetBeneficiaryPercentageSumAsync(req.EmployeeBadgeNumber, null, cancellationToken);
        if (existingPercentageSum >= 0 && (existingPercentageSum + req.Percentage) > 100m)
        {
            throw new ValidationException("The sum of all beneficiary percentages would exceed 100%.");
        }

        var rslt = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            // Validate database-dependent business rules
            var dbValidationModel = new BeneficiaryDatabaseValidationModel
            {
                BeneficiaryContactId = req.BeneficiaryContactId,
                EmployeeBadgeNumber = req.EmployeeBadgeNumber,
                Context = ctx
            };
            var dbValidationResult = await _databaseValidator.ValidateAsync(dbValidationModel, cancellationToken);

            if (!dbValidationResult.IsValid)
            {
                throw new ValidationException(dbValidationResult.Errors);
            }

            // At this point, validation has confirmed these entities exist
            var beneficiaryContact = (await ctx.BeneficiaryContacts.FirstOrDefaultAsync(x => x.Id == req.BeneficiaryContactId, cancellationToken))!;
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var demographic = (await demographicQuery.Where(x => x.BadgeNumber == req.EmployeeBadgeNumber).SingleOrDefaultAsync(cancellationToken))!;

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
                Percent = beneficiary.Percent
            };

            return resp;
        }, cancellationToken);

        return rslt;
    }

    public async Task<CreateBeneficiaryContactResponse> CreateBeneficiaryContact(CreateBeneficiaryContactRequest req, CancellationToken cancellationToken)
    {
        // Validate request using FluentValidation
        var validationResult = await _createBeneficiaryContactValidator.ValidateAsync(req, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var rslt = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            // Validate database-dependent business rules
            var dbValidationResult = await BeneficiaryDatabaseValidator.ValidateCreateBeneficiaryContactAsync(
                req.ContactSsn,
                ctx,
                cancellationToken);

            if (!dbValidationResult.IsValid)
            {
                throw new ValidationException(dbValidationResult.Errors);
            }
            var beneficiaryContact = new BeneficiaryContact()
            {
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

    public Task<UpdateBeneficiaryResponse> UpdateBeneficiary(UpdateBeneficiaryRequest req, CancellationToken cancellationToken)
    {
        var resp = _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            var beneficiary = await ctx.Beneficiaries.SingleAsync(x => x.Id == req.Id, cancellationToken);

            if (!string.IsNullOrEmpty(req.Relationship))
            {
                beneficiary.Relationship = req.Relationship;
            }

            if (req.Percentage.HasValue)
            {
                // Validate percentage constraints if percentage was updated
                if (req.Percentage.Value <= 0 || req.Percentage.Value > 100m)
                {
                    throw new ValidationException("Percentage must be between 0 and 100%.");
                }

                // Check that sum of percentages doesn't exceed 100%
                var existingPercentageSum = await GetBeneficiaryPercentageSumAsync(beneficiary.BadgeNumber, beneficiary.Id, cancellationToken);
                if (existingPercentageSum >= 0 && (existingPercentageSum + req.Percentage.Value) > 100m)
                {
                    throw new ValidationException("The sum of all beneficiary percentages would exceed 100%.");
                }

                beneficiary.Percent = req.Percentage.Value;
            }

            var response = new UpdateBeneficiaryResponse()
            {
                Id = beneficiary.Id,
                BeneficiaryContactId = beneficiary.BeneficiaryContactId,
                Relationship = beneficiary.Relationship,
                Percentage = beneficiary.Percent,
                DemographicId = beneficiary.DemographicId,
                BadgeNumber = beneficiary.BadgeNumber,
                Street1 = string.Empty,
                City = string.Empty,
                State = string.Empty,
                PostalCode = string.Empty,
                Ssn = string.Empty.MaskSsn(),
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

            return response;
        }, cancellationToken);

        return resp;
    }


    public Task<UpdateBeneficiaryContactResponse> UpdateBeneficiaryContact(UpdateBeneficiaryContactRequest req, CancellationToken cancellationToken)
    {
        var response = _dataContextFactory.UseWritableContext(async (ctx) =>
        {
            var resp = new UpdateBeneficiaryContactResponse()
            {
                Ssn = string.Empty.MaskSsn(),
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

    private async Task<BeneficiaryContact> SetBeneficiaryContactColumns(int beneficiaryContactId, UpdateBeneficiaryContactRequest req, ProfitSharingDbContext ctx, UpdateBeneficiaryContactResponse response, CancellationToken cancellationToken)
    {
        // Validate request using FluentValidation
        req.Id = beneficiaryContactId; // Ensure ID is set for validation
        var validationResult = await _updateBeneficiaryContactValidator.ValidateAsync(req, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var contact = await ctx.BeneficiaryContacts.Include(x => x.Address).Include(x => x.ContactInfo).SingleAsync(x => x.Id == beneficiaryContactId, cancellationToken);

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
        }
        if (!string.IsNullOrEmpty(req.LastName))
        {
            contact.ContactInfo!.LastName = req.LastName.Trim();
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
            contact.ContactInfo!.MiddleName = req.MiddleName;
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
            contact.ContactInfo!.PhoneNumber = req.PhoneNumber;
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
            contact.ContactInfo!.MobileNumber = req.MobileNumber;
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
            contact.ContactInfo!.EmailAddress = req.EmailAddress;
        }

        //Update the ModifiedAtUtc timestamp - now that ValueGeneratedOnUpdate is removed, this will work
        contact.ModifiedAtUtc = DateTimeOffset.UtcNow;

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
        response.ModifiedAtUtc = contact.ModifiedAtUtc;

        return contact;
    }

    public async Task DeleteBeneficiary(int id, CancellationToken cancellationToken)
    {
        _ = await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            var beneficiaryToDelete = await ctx.Beneficiaries.Include(x => x.Contact).Include(x => x.Contact!.ContactInfo).Include(x => x.Contact!.Address).SingleAsync(x => x.Id == id);
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
            var contactToDelete = await ctx.BeneficiaryContacts
                .Include(x => x.Beneficiaries)
                .Include(beneficiaryContact => beneficiaryContact.Address)
                .Include(beneficiaryContact => beneficiaryContact.ContactInfo)
                .SingleAsync(x => x.Id == id, cancellation);

            var deleteContact = true;
            Beneficiary? beneficiaryToDelete = null;
            if (contactToDelete.Beneficiaries?.Count == 1) //If contact is only associated with one beneficiary, check to see if we can delete it.
            {
                var firstBeneficiary = contactToDelete.Beneficiaries[0];
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
        if (balanceInfo != null && balanceInfo?.CurrentBalance != 0)
        {
            throw new InvalidOperationException("Balance is not zero, cannot delete beneficiary.");
        }

        if (await ctx.DistributionPayees.AnyAsync(x => x.Ssn == beneficiary.Contact!.Ssn, cancellationToken))
        {
            throw new InvalidOperationException("Beneficiary is a payee for a distribution, cannot delete beneficiary.");
        }

        return true;
    }

    private static async Task<bool> CanIDeleteThisBeneficiaryContact(int beneficiaryId, BeneficiaryContact contact, ProfitSharingDbContext ctx, CancellationToken token)
    {
        return !(await ctx.Beneficiaries.AnyAsync(b => b.BeneficiaryContactId == contact.Id && b.Id != beneficiaryId, token));
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

    /// <summary>
    /// Gets the total beneficiary percentage sum for a specific badge number, optionally excluding one beneficiary by ID.
    /// </summary>
    /// <param name="badgeNumber">The employee badge number</param>
    /// <param name="beneficiaryIdToExclude">Optional: Beneficiary ID to exclude (e.g., when updating)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total percentage sum, or -1 if badge not found</returns>
    public async Task<decimal> GetBeneficiaryPercentageSumAsync(int badgeNumber, int? beneficiaryIdToExclude, CancellationToken cancellationToken)
    {
        try
        {
            return await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = ctx.Beneficiaries.Where(x => x.BadgeNumber == badgeNumber);

                if (beneficiaryIdToExclude.HasValue)
                {
                    query = query.Where(x => x.Id != beneficiaryIdToExclude.Value);
                }

                return await query.SumAsync(x => x.Percent, cancellationToken);
            }, cancellationToken);
        }
        catch (Exception)
        {
            // Return -1 to indicate error/not found
            return -1m;
        }
    }

    /// <summary>
    /// Gets all beneficiaries for a specific badge number, optionally excluding one beneficiary by ID.
    /// Internal method used by other service methods that need full beneficiary data.
    /// </summary>
    /// <param name="badgeNumber">The employee badge number</param>
    /// <param name="beneficiaryIdToExclude">Optional: Beneficiary ID to exclude (e.g., when updating)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of beneficiaries</returns>
    internal Task<List<Beneficiary>> GetBeneficiariesForBadgeInternalAsync(int badgeNumber, int? beneficiaryIdToExclude, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = ctx.Beneficiaries.Where(x => x.BadgeNumber == badgeNumber);

            if (beneficiaryIdToExclude.HasValue)
            {
                query = query.Where(x => x.Id != beneficiaryIdToExclude.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }, cancellationToken);
    }
}

