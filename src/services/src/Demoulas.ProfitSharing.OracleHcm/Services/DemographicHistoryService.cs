using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Repositories;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Handles history tracking and entity updates for demographics.
/// Provides change detection, history record creation, and related entity updates.
/// </summary>
public sealed class DemographicHistoryService : IDemographicHistoryService
{
    private readonly IDemographicsRepository _repository;
    private readonly ILogger<DemographicHistoryService> _logger;

    public DemographicHistoryService(
        IDemographicsRepository repository,
        ILogger<DemographicHistoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<int> UpdateExistingWithHistoryAsync(
        List<Demographic> existing,
        List<Demographic> incoming,
        CancellationToken ct)
    {
        var incomingLookup = incoming.ToDictionary(e => e.OracleHcmId);
        var updatedCount = 0;

        foreach (var existingItem in existing)
        {
            if (!incomingLookup.TryGetValue(existingItem.OracleHcmId, out var incomingItem))
            {
                continue;
            }

            var hasChanges = DetectChanges(existingItem, incomingItem);
            if (!hasChanges)
            {
                continue;
            }

            // Create history record before updating
            var historyRecord = CreateHistoryRecord(existingItem);
            await _repository.AddHistoryRecordAsync(historyRecord, ct);

            // Update existing demographic with new values
            UpdateDemographicValues(existingItem, incomingItem);
            updatedCount++;
        }

        if (updatedCount > 0)
        {
            _logger.LogDebug("Updated {Count} demographics with history tracking", updatedCount);
        }

        return updatedCount;
    }

    public async Task<int> InsertNewWithHistoryAsync(
        List<Demographic> newDemographics,
        CancellationToken ct)
    {
        if (newDemographics.Count == 0)
        {
            return 0;
        }

        foreach (var demographic in newDemographics)
        {
            await _repository.AddAsync(demographic, ct);

            // Create initial history record
            var historyRecord = CreateHistoryRecord(demographic);
            await _repository.AddHistoryRecordAsync(historyRecord, ct);
        }

        _logger.LogDebug("Inserted {Count} new demographics with initial history", newDemographics.Count);
        return newDemographics.Count;
    }

    public async Task UpdateRelatedEntitiesForSsnChangeAsync(
        List<Demographic> ssnChangedDemographics,
        CancellationToken ct)
    {
        if (ssnChangedDemographics.Count == 0)
        {
            return;
        }

        foreach (var demographic in ssnChangedDemographics)
        {
            // Update BeneficiaryContacts SSN
            await _repository.UpdateBeneficiaryContactsSsnAsync(
                demographic.Id, demographic.Ssn, ct);

            // Update ProfitDetails SSN
            await _repository.UpdateProfitDetailsSsnAsync(
                demographic.Id, demographic.Ssn, ct);
        }

        _logger.LogInformation(
            "Updated related entities for {Count} SSN changes",
            ssnChangedDemographics.Count);
    }

    #region Private Helpers

    private static bool DetectChanges(Demographic existing, Demographic incoming)
    {
        return existing.Ssn != incoming.Ssn ||
               existing.BadgeNumber != incoming.BadgeNumber ||
               existing.FirstName != incoming.FirstName ||
               existing.MiddleInitial != incoming.MiddleInitial ||
               existing.LastName != incoming.LastName ||
               existing.Gender != incoming.Gender ||
               existing.BirthDate != incoming.BirthDate ||
               existing.HireDate != incoming.HireDate ||
               existing.TerminationDate != incoming.TerminationDate ||
               existing.EmploymentStatus != incoming.EmploymentStatus ||
               existing.HomePhoneNumber != incoming.HomePhoneNumber ||
               existing.CellPhoneNumber != incoming.CellPhoneNumber ||
               existing.AddressLine1 != incoming.AddressLine1 ||
               existing.AddressLine2 != incoming.AddressLine2 ||
               existing.City != incoming.City ||
               existing.State != incoming.State ||
               existing.ZipCode != incoming.ZipCode ||
               existing.EmailAddress != incoming.EmailAddress ||
               existing.Military != incoming.Military ||
               existing.MaritalStatus != incoming.MaritalStatus;
    }

    private static DemographicsHistory CreateHistoryRecord(Demographic demographic)
    {
        return new DemographicsHistory
        {
            DemographicId = demographic.Id,
            OracleHcmId = demographic.OracleHcmId,
            Ssn = demographic.Ssn,
            BadgeNumber = demographic.BadgeNumber,
            FirstName = demographic.FirstName,
            MiddleInitial = demographic.MiddleInitial,
            LastName = demographic.LastName,
            Gender = demographic.Gender,
            BirthDate = demographic.BirthDate,
            HireDate = demographic.HireDate,
            TerminationDate = demographic.TerminationDate,
            EmploymentStatus = demographic.EmploymentStatus,
            HomePhoneNumber = demographic.HomePhoneNumber,
            CellPhoneNumber = demographic.CellPhoneNumber,
            AddressLine1 = demographic.AddressLine1,
            AddressLine2 = demographic.AddressLine2,
            City = demographic.City,
            State = demographic.State,
            ZipCode = demographic.ZipCode,
            EmailAddress = demographic.EmailAddress,
            Military = demographic.Military,
            MaritalStatus = demographic.MaritalStatus,
            ValidFrom = DateTime.UtcNow,
            ValidTo = null
        };
    }

    private static void UpdateDemographicValues(Demographic existing, Demographic incoming)
    {
        existing.Ssn = incoming.Ssn;
        existing.BadgeNumber = incoming.BadgeNumber;
        existing.FirstName = incoming.FirstName;
        existing.MiddleInitial = incoming.MiddleInitial;
        existing.LastName = incoming.LastName;
        existing.Gender = incoming.Gender;
        existing.BirthDate = incoming.BirthDate;
        existing.HireDate = incoming.HireDate;
        existing.TerminationDate = incoming.TerminationDate;
        existing.EmploymentStatus = incoming.EmploymentStatus;
        existing.HomePhoneNumber = incoming.HomePhoneNumber;
        existing.CellPhoneNumber = incoming.CellPhoneNumber;
        existing.AddressLine1 = incoming.AddressLine1;
        existing.AddressLine2 = incoming.AddressLine2;
        existing.City = incoming.City;
        existing.State = incoming.State;
        existing.ZipCode = incoming.ZipCode;
        existing.EmailAddress = incoming.EmailAddress;
        existing.Military = incoming.Military;
        existing.MaritalStatus = incoming.MaritalStatus;
        existing.LastModifiedDate = DateTime.UtcNow;
    }

    #endregion
}
