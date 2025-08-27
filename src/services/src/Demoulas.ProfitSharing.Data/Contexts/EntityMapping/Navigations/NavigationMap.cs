using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;
internal sealed class NavigationMap : IEntityTypeConfiguration<Navigation>
{
    public void Configure(EntityTypeBuilder<Navigation> builder)
    {
        _ = builder.ToTable("NAVIGATION");
        _ = builder.HasKey(t => t.Id);
        _ = builder.Property(x => x.Id).HasColumnName("ID").ValueGeneratedNever();
        _ = builder.Property(x => x.Url).HasColumnName("URL").HasMaxLength(200);
        _ = builder.Property(x => x.Title).HasColumnName("TITLE").HasMaxLength(100);
        _ = builder.Property(x => x.SubTitle).HasColumnName("SUB_TITLE").HasMaxLength(70);
        _ = builder.Property(x => x.Icon).HasColumnName("ICON").HasMaxLength(200);
        _ = builder.Property(x => x.OrderNumber).HasColumnName("ORDER_NUMBER");
        _ = builder.Property(x => x.StatusId).HasColumnName("STATUS_ID");
        _ = builder.Property(x => x.ParentId).HasColumnName("PARENT_ID");
        _ = builder.Property(x => x.Disabled).HasColumnName("DISABLED");

        builder.HasOne(m => m.NavigationStatus)
            .WithMany(x => x.Navigations)
            .HasForeignKey(x => x.StatusId);

        builder.HasOne(m => m.Parent)
            .WithMany(x=>x.Items)
            .HasForeignKey(x => x.ParentId);

        builder.HasMany(x => x.RequiredRoles)
            .WithMany()
            .UsingEntity(x=>x.ToTable(name:"NAVIGATION_ASSIGNED_ROLES"));

        // Self-referencing many-to-many for prerequisites
        builder
            .HasMany(n => n.PrerequisiteNavigations!)
            .WithMany(n => n.DependentNavigations!)
            .UsingEntity<Dictionary<short, short>>(
                "NAVIGATION_PREREQUISITES",
                j => j
                    .HasOne<Navigation>()
                    .WithMany()
                    .HasForeignKey("PREREQUISITE_ID")
                    .HasConstraintName("FK_NAV_PREREQ_PREREQUISITE")
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne<Navigation>()
                    .WithMany()
                    .HasForeignKey("NAVIGATION_ID")
                    .HasConstraintName("FK_NAV_PREREQ_DEPENDENT")
                    .OnDelete(DeleteBehavior.NoAction)
            )
            .ToTable("NAVIGATION_PREREQUISITES");

        // Seed data based on add-navigation-data.sql
        builder.HasData(
             // Main menu items
             new Navigation
             {
                 Id = Navigation.Constants.Unknown,
                 ParentId = null,
                 Title = "Unknown",
                 SubTitle = "",
                 Url = "",
                 StatusId = 1,
                 OrderNumber = 1,
                 Icon = "",
                 Disabled = false
             },
            new Navigation
            {
                Id = Navigation.Constants.Inquiries,
                ParentId = null,
                Title = "INQUIRIES",
                SubTitle = "",
                Url = "",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.Beneficiaries,
                ParentId = null,
                Title = "BENEFICIARIES",
                SubTitle = "",
                Url = "",
                StatusId = 1,
                OrderNumber = 2,
                Icon = "",
                Disabled = true
            },
            new Navigation
            {
                Id = Navigation.Constants.Distributions,
                ParentId = null,
                Title = "DISTRIBUTIONS",
                SubTitle = "",
                Url = "",
                StatusId = 1,
                OrderNumber = 3,
                Icon = "",
                Disabled = true
            },
            new Navigation
            {
                Id = Navigation.Constants.Reconciliation,
                ParentId = null,
                Title = "RECONCILIATION",
                SubTitle = "",
                Url = "",
                StatusId = 1,
                OrderNumber = 4,
                Icon = "",
                Disabled = true
            },
            new Navigation
            {
                Id = Navigation.Constants.YearEnd,
                ParentId = null,
                Title = "YEAR END",
                SubTitle = "",
                Url = "",
                StatusId = 1,
                OrderNumber = 5,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ItDevOps,
                ParentId = null,
                Title = "IT DEVOPS",
                SubTitle = "",
                Url = "",
                StatusId = 1,
                OrderNumber = 6,
                Icon = "",
                Disabled = false
            },

            // Inquiries sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.MasterInquiry,
                ParentId = Navigation.Constants.Inquiries,
                Title = "MASTER INQUIRY",
                SubTitle = "",
                Url = "master-inquiry",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },

            // IT Operations sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.DemographicFreeze,
                ParentId = Navigation.Constants.ItDevOps,
                Title = "Demographic Freeze",
                SubTitle = "",
                Url = "demographic-freeze",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },

            // Year End sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.DecemberActivities,
                ParentId = Navigation.Constants.YearEnd,
                Title = "December Activities",
                SubTitle = "",
                Url = "december-process-accordion",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.FiscalClose,
                ParentId = Navigation.Constants.YearEnd,
                Title = "Fiscal Close",
                SubTitle = "",
                Url = "fiscal-close",
                StatusId = 1,
                OrderNumber = 2,
                Icon = "",
                Disabled = false
            },

            // December Activities sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.CleanupReports,
                ParentId = Navigation.Constants.DecemberActivities,
                Title = "Clean up Reports",
                SubTitle = "",
                Url = "",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.Unforfeit,
                ParentId = Navigation.Constants.DecemberActivities,
                Title = "Unforfeit",
                SubTitle = "QPREV-PROF",
                Url = "unforfeitures",
                StatusId = 1,
                OrderNumber = 2,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.MilitaryContributions,
                ParentId = Navigation.Constants.DecemberActivities,
                Title = "Military Contributions",
                SubTitle = "008-13",
                Url = "military-entry-and-modification",
                StatusId = 1,
                OrderNumber = 3,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.Terminations,
                ParentId = Navigation.Constants.DecemberActivities,
                Title = "Terminations",
                SubTitle = "QPAY066",
                Url = "prof-term",
                StatusId = 1,
                OrderNumber = 4,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.Forfeitures,
                ParentId = Navigation.Constants.DecemberActivities,
                Title = "Forfeitures",
                SubTitle = "008-12",
                Url = "forfeitures-adjustment",
                StatusId = 1,
                OrderNumber = 5,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.DistributionsAndForfeitures,
                ParentId = Navigation.Constants.DecemberActivities,
                Title = "Distributions and Forfeitures",
                SubTitle = "QPAY129",
                Url = "distributions-and-forfeitures",
                StatusId = 1,
                OrderNumber = 6,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfitShareReport,
                ParentId = Navigation.Constants.DecemberActivities,
                Title = "Profit Share Report",
                SubTitle = "PAY426",
                Url = "profit-share-report",
                StatusId = 1,
                OrderNumber = 9,
                Icon = "",
                Disabled = false
            },

            // Clean up Reports sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.DemographicBadgesNotInPayProfit,
                ParentId = Navigation.Constants.CleanupReports,
                Title = "Demographic Badges Not In PayProfit",
                SubTitle = "",
                Url = "demographic-badges-not-in-payprofit",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.DuplicateSSNsInDemographics,
                ParentId = Navigation.Constants.CleanupReports,
                Title = "Duplicate SSNs in Demographics",
                SubTitle = "",
                Url = "duplicate-ssns-demographics",
                StatusId = 1,
                OrderNumber = 2,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.NegativeETVA,
                ParentId = Navigation.Constants.CleanupReports,
                Title = "Negative ETVA",
                SubTitle = "",
                Url = "negative-etva-for-ssns-on-payprofit",
                StatusId = 1,
                OrderNumber = 3,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.DuplicateNamesAndBirthdays,
                ParentId = Navigation.Constants.CleanupReports,
                Title = "Duplicate Names and Birthdays",
                SubTitle = "",
                Url = "duplicate-names-and-birthdays",
                StatusId = 1,
                OrderNumber = 4,
                Icon = "",
                Disabled = false
            },

            // Fiscal Close sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.ManageExecutiveHours,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Manage Executive Hours",
                SubTitle = "PROF-DOLLAR-EXEC-EXTRACT, TPR008-09",
                Url = "manage-executive-hours-and-dollars",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.YTDWagesExtract,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "YTD Wages Extract",
                SubTitle = "PROF-DOLLAR-EXTRACT",
                Url = "ytd-wages-extract",
                StatusId = 1,
                OrderNumber = 2,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfitShareReportEditRun,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Profit Share Report (Edit Run)",
                SubTitle = "PAY426",
                Url = "pay426n",
                StatusId = 1,
                OrderNumber = 3,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfitShareReportFinalRun,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Profit Share Report (Final Run)",
                SubTitle = "PAY426",
                Url = "profit-share-report",
                StatusId = 1,
                OrderNumber = 4,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.GetEligibleEmployees,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Get Eligible Employees",
                SubTitle = "GET-ELIGIBLE-EMPS",
                Url = "eligible-employees",
                StatusId = 1,
                OrderNumber = 5,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfitShareForfeit,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Profit Share Forfeit",
                SubTitle = "PAY443",
                Url = "forfeit",
                StatusId = 1,
                OrderNumber = 6,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.MasterUpdate,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Master Update",
                SubTitle = "PAY444|PAY447",
                Url = "profit-share-update",
                StatusId = 1,
                OrderNumber = 7,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfitMasterUpdate,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Profit Master Update",
                SubTitle = "PAY460, PROFTLD",
                Url = "profit-master-update",
                StatusId = 1,
                OrderNumber = 8,
                Icon = "",
                Disabled = true
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfPayMasterUpdate,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Prof PayMaster Update",
                SubTitle = "PAY450",
                Url = "pay450-summary",
                StatusId = 1,
                OrderNumber = 10,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfControlSheet,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Prof Control Sheet",
                SubTitle = "PROF-CNTRL-SHEET",
                Url = "prof-control-sheet",
                StatusId = 1,
                OrderNumber = 11,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfShareReportByAge,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Prof Share Report By Age",
                SubTitle = "Prof130",
                Url = "",
                StatusId = 1,
                OrderNumber = 12,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfShareGrossRpt,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Prof Share Gross Rpt",
                SubTitle = "QPAY501",
                Url = "profit-share-gross-report",
                StatusId = 1,
                OrderNumber = 13,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ProfShareByStore,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Prof Share by Store",
                SubTitle = "QPAY066TA",
                Url = "",
                StatusId = 1,
                OrderNumber = 14,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.PrintProfitCerts,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Print Profit Certs",
                SubTitle = "PAYCERT",
                Url = "print-profit-certs",
                StatusId = 1,
                OrderNumber = 15,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.SaveProfPaymstr,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "Save Prof Paymstr",
                SubTitle = "",
                Url = "save-prof-paymstr",
                StatusId = 1,
                OrderNumber = 16,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.QPAY066AdHocReports,
                ParentId = Navigation.Constants.FiscalClose,
                Title = "QPAY066* Ad Hoc Reports",
                SubTitle = "QPAY066*",
                Url = "qpay066-adhoc",
                StatusId = 1,
                OrderNumber = 17,
                Icon = "",
                Disabled = false
            },

            // Prof Share Report By Age sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.DistributionsByAge,
                ParentId = Navigation.Constants.ProfShareReportByAge,
                Title = "DISTRIBUTIONS BY AGE",
                SubTitle = "PROF130",
                Url = "distributions-by-age",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ContributionsByAge,
                ParentId = Navigation.Constants.ProfShareReportByAge,
                Title = "CONTRIBUTIONS BY AGE",
                SubTitle = "PROF130",
                Url = "contributions-by-age",
                StatusId = 1,
                OrderNumber = 2,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.ForfeituresByAge,
                ParentId = Navigation.Constants.ProfShareReportByAge,
                Title = "FORFEITURES BY AGE",
                SubTitle = "PROF130",
                Url = "forfeitures-by-age",
                StatusId = 1,
                OrderNumber = 3,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.BalanceByAge,
                ParentId = Navigation.Constants.ProfShareReportByAge,
                Title = "BALANCE BY AGE",
                SubTitle = "PROF130B",
                Url = "balance-by-age",
                StatusId = 1,
                OrderNumber = 4,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.VestedAmountsByAge,
                ParentId = Navigation.Constants.ProfShareReportByAge,
                Title = "VESTED AMOUNTS BY AGE",
                SubTitle = "PROF130V",
                Url = "vested-amounts-by-age",
                StatusId = 1,
                OrderNumber = 5,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.BalanceByYears,
                ParentId = Navigation.Constants.ProfShareReportByAge,
                Title = "BALANCE BY YEARS",
                SubTitle = "PROF130Y",
                Url = "balance-by-years",
                StatusId = 1,
                OrderNumber = 6,
                Icon = "",
                Disabled = false
            },

            // Prof Share by Store sub-menu items
            new Navigation
            {
                Id = Navigation.Constants.QPAY066Under21,
                ParentId = Navigation.Constants.ProfShareByStore,
                Title = "QPAY066-UNDR21",
                SubTitle = "",
                Url = "qpay066-under21",
                StatusId = 1,
                OrderNumber = 1,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.QPAY066TAUnder21,
                ParentId = Navigation.Constants.ProfShareByStore,
                Title = "QPAY066TA-UNDR21",
                SubTitle = "",
                Url = "qpay066ta-under21",
                StatusId = 1,
                OrderNumber = 2,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.QPAY066TA,
                ParentId = Navigation.Constants.ProfShareByStore,
                Title = "QPAY066TA",
                SubTitle = "",
                Url = "qpay066ta",
                StatusId = 1,
                OrderNumber = 3,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.QNEWPROFLBL,
                ParentId = Navigation.Constants.ProfShareByStore,
                Title = "QNEWPROFLBL",
                SubTitle = "",
                Url = "new-ps-labels",
                StatusId = 1,
                OrderNumber = 4,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.PROFNEW,
                ParentId = Navigation.Constants.ProfShareByStore,
                Title = "PROFNEW",
                SubTitle = "",
                Url = "profnew",
                StatusId = 1,
                OrderNumber = 5,
                Icon = "",
                Disabled = false
            },
            new Navigation
            {
                Id = Navigation.Constants.PROFALL,
                ParentId = Navigation.Constants.ProfShareByStore,
                Title = "PROFALL",
                SubTitle = "",
                Url = "profall",
                StatusId = 1,
                OrderNumber = 6,
                Icon = "",
                Disabled = false
            }
        );
    }
}
