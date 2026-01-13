using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Commands;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Handles history tracking and entity updates for demographics.
/// Returns commands for transaction-safe execution.
/// </summary>
public sealed class DemographicHistoryService : IDemographicHistoryService
{
    private readonly ILogger<DemographicHistoryService> _logger;

    public DemographicHistoryService(ILogger<DemographicHistoryService> logger)
    {
        _logger = logger;
    }

    public (int UpdatedCount, List<IDemographicCommand> Commands) PrepareUpdateExistingWithHistory(
        List<Demographic> existing,
        List<Demographic> incoming)
    {
        var incomingLookup = incoming.ToDictionary(e => e.OracleHcmId);
        var updatedCount = 0;
        var commands = new List<IDemographicCommand>();

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
            commands.Add(new AddHistoryCommand(historyRecord));

            // Update existing demographic with new values
            UpdateDemographicValues(existingItem, incomingItem);
            commands.Add(new UpdateDemographicCommand(existingItem));
            updatedCount++;
        }

        if (updatedCount > 0)
        {
            _logger.LogDebug("Prepared {Count} demographics for update with history tracking", updatedCount);
        }

        return (updatedCount, commands);
    }

    public (int InsertedCount, List<IDemographicCommand> Commands) PrepareInsertNewWithHistory(
        List<Demographic> newDemographics)
    {
        if (newDemographics.Count == 0)
        {
            return (0, new List<IDemographicCommand>());
        }

        var commands = new List<IDemographicCommand>();

        foreach (var demographic in newDemographics)
        {
            commands.Add(new AddDemographicCommand(demographic));

            // Create initial history record
            var historyRecord = CreateHistoryRecord(demographic);
            commands.Add(new AddHistoryCommand(historyRecord));
        }

        _logger.LogDebug("Prepared {Count} new demographics for insertion with initial history", newDemographics.Count);
        return (newDemographics.Count, commands);
    }

    public List<Demographic> DetectSsnChanges(
        List<Demographic> existing,
        Dictionary<long, Demographic> incomingByOracleId)
    {
        var ssnChangedDemographics = existing
            .Where(e => incomingByOracleId.TryGetValue(e.OracleHcmId, out var incoming) && e.Ssn != incoming.Ssn)
            .ToList();

        if (ssnChangedDemographics.Count > 0)
        {
            _logger.LogInformation(
                "Detected {Count} SSN changes",
                ssnChangedDemographics.Count);
        }

        return ssnChangedDemographics;
    }

    public List<IDemographicCommand> PrepareSsnUpdateCommands(
        List<Demographic> ssnChangedDemographics,
        Dictionary<long, Demographic> incomingByOracleId)
    {
        var commands = new List<IDemographicCommand>();

        foreach (var existing in ssnChangedDemographics)
        {
            if (!incomingByOracleId.TryGetValue(existing.OracleHcmId, out var incoming))
            {
                continue;
            }

            var oldSsn = existing.Ssn;
            var newSsn = incoming.Ssn;

            // Update related BeneficiaryContacts using old SSN
            commands.Add(new UpdateBeneficiaryContactsSsnCommand(oldSsn, newSsn));

            // Update related ProfitDetails using old SSN
            commands.Add(new UpdateProfitDetailsSsnCommand(oldSsn, newSsn));

            _logger.LogDebug(
                "Prepared SSN update commands for OracleHcmId {OracleHcmId}: {OldSsn} -> {NewSsn}",
                existing.OracleHcmId, oldSsn, newSsn);
        }

        return commands;
    }

    #region Private Helpers

    private static bool DetectChanges(Demographic existing, Demographic incoming)
    {
        return existing.Ssn != incoming.Ssn ||
               existing.BadgeNumber != incoming.BadgeNumber ||
               existing.ContactInfo.FirstName != incoming.ContactInfo.FirstName ||
               existing.ContactInfo.MiddleName != incoming.ContactInfo.MiddleName ||
               existing.ContactInfo.LastName != incoming.ContactInfo.LastName ||
               existing.GenderId != incoming.GenderId ||
               existing.DateOfBirth != incoming.DateOfBirth ||
               existing.HireDate != incoming.HireDate ||
               existing.TerminationDate != incoming.TerminationDate ||
               existing.EmploymentStatusId != incoming.EmploymentStatusId ||
               existing.ContactInfo.PhoneNumber != incoming.ContactInfo.PhoneNumber ||
               existing.ContactInfo.MobileNumber != incoming.ContactInfo.MobileNumber ||
               existing.Address.Street != incoming.Address.Street ||
               existing.Address.Street2 != incoming.Address.Street2 ||
               existing.Address.City != incoming.Address.City ||
               existing.Address.State != incoming.Address.State ||
               existing.Address.PostalCode != incoming.Address.PostalCode ||
               existing.ContactInfo.EmailAddress != incoming.ContactInfo.EmailAddress;
    }

    private static DemographicHistory CreateHistoryRecord(Demographic demographic)
    {
        var history = DemographicHistory.FromDemographic(demographic);
        history.ValidFrom = DateTimeOffset.UtcNow;
        history.ValidTo = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Local);
        return history;
    }

    private static void UpdateDemographicValues(Demographic existing, Demographic incoming)
    {
        existing.Ssn = incoming.Ssn;
        existing.BadgeNumber = incoming.BadgeNumber;
        existing.ContactInfo.FirstName = incoming.ContactInfo.FirstName;
        existing.ContactInfo.MiddleName = incoming.ContactInfo.MiddleName;
        existing.ContactInfo.LastName = incoming.ContactInfo.LastName;
        existing.GenderId = incoming.GenderId;
        existing.DateOfBirth = incoming.DateOfBirth;
        existing.HireDate = incoming.HireDate;
        existing.TerminationDate = incoming.TerminationDate;
        existing.EmploymentStatusId = incoming.EmploymentStatusId;
        existing.ContactInfo.PhoneNumber = incoming.ContactInfo.PhoneNumber;
        existing.ContactInfo.MobileNumber = incoming.ContactInfo.MobileNumber;
        existing.Address.Street = incoming.Address.Street;
        existing.Address.Street2 = incoming.Address.Street2;
        existing.Address.City = incoming.Address.City;
        existing.Address.State = incoming.Address.State;
        existing.Address.PostalCode = incoming.Address.PostalCode;
        existing.ContactInfo.EmailAddress = incoming.ContactInfo.EmailAddress;
        existing.ModifiedAtUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}
