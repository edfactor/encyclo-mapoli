# Complete DTO Inventory Report

**Generated:** December 31, 2025

---

## EXECUTIVE SUMMARY

### Overall Statistics

| Metric                                      | Count   |
| ------------------------------------------- | ------- |
| **Total Request DTOs**                      | 84      |
| **Total Response DTOs**                     | 155     |
| **Total DTOs**                              | **239** |
| **Request DTOs with RequestExample()**      | 54      |
| **Request DTOs WITHOUT RequestExample()**   | 30      |
| **Response DTOs with ResponseExample()**    | 99      |
| **Response DTOs WITHOUT ResponseExample()** | 56      |
| **TOTAL DTOs missing Example methods**      | **86**  |

### Work Estimate

- **Phase 1 - Critical YearEnd/Validation/Reports Response DTOs:** ~30 methods
- **Phase 2 - Remaining Response DTOs:** ~26 methods
- **Phase 3 - Request DTOs:** ~30 methods
- **Total estimated development time:** ~86 methods across 3 phases
- **Estimated effort:** 2-3 weeks for full batch implementation with testing

---

## REQUEST DTOs BY CATEGORY

### Root Level (25 files)

| File                                              | Class Name                                     | Has RequestExample |
| ------------------------------------------------- | ---------------------------------------------- | ------------------ |
| AccountHistoryReportRequest.cs                    | AccountHistoryReportRequest                    | ✓ YES              |
| AddressRequestDto.cs                              | AddressRequestDto                              | ✗ NO               |
| AdhocBeneficiariesReportRequest.cs                | AdhocBeneficiariesReportRequest                | ✓ YES              |
| BadgeNumberRequest.cs                             | BadgeNumberRequest                             | ✗ NO               |
| BreakdownByStoreRequest.cs                        | BreakdownByStoreRequest                        | ✓ YES              |
| CerficatePrintRequest.cs                          | CerficatePrintRequest                          | ✓ YES              |
| ContactInfoRequestDto.cs                          | ContactInfoRequestDto                          | ✗ NO               |
| DemographicsRequest.cs                            | DemographicsRequest                            | ✗ NO               |
| DistributionsAndForfeituresRequest.cs             | DistributionsAndForfeituresRequest             | ✓ YES              |
| DuplicateNamesAndBirthdaysRequest.cs              | DuplicateNamesAndBirthdaysRequest              | ✓ YES              |
| EmployeesWithProfitsOver73Request.cs              | EmployeesWithProfitsOver73Request              | ✓ YES              |
| EmployeeTypeRequestDto.cs                         | EmployeeTypeRequestDto                         | ✗ NO               |
| EnrollmentRequestDto.cs                           | EnrollmentRequestDto                           | ✗ NO               |
| ExecutiveHoursAndDollarsRequest.cs                | ExecutiveHoursAndDollarsRequest                | ✓ YES              |
| FilterableStartAndEndDateRequest.cs               | FilterableStartAndEndDateRequest               | ✓ YES              |
| ForfeitureAdjustmentUpdateRequest.cs              | ForfeitureAdjustmentUpdateRequest              | ✓ YES              |
| FrozenProfitYearRequest.cs                        | FrozenProfitYearRequest                        | ✓ YES              |
| FrozenReportsByAgeRequest.cs                      | FrozenReportsByAgeRequest                      | ✓ YES              |
| GrossWagesReportRequest.cs                        | GrossWagesReportRequest                        | ✓ YES              |
| IdRequest.cs                                      | IdRequest                                      | ✗ NO               |
| IdsRequest.cs                                     | IdsRequest                                     | ✓ YES              |
| IMemberRequest.cs                                 | IMemberRequest (Interface)                     | ✗ NO               |
| IStartEndDateRequest.cs                           | IStartEndDateRequest (Interface)               | ✗ NO               |
| ProfitShareUpdateRequest.cs                       | ProfitShareUpdateRequest                       | ✓ YES              |
| ProfitYearAndAsOfDateRequest.cs                   | ProfitYearAndAsOfDateRequest                   | ✗ NO               |
| ProfitYearRequest.cs                              | ProfitYearRequest                              | ✓ YES              |
| SetExecutiveHoursAndDollarsDto.cs                 | SetExecutiveHoursAndDollarsDto                 | ✓ YES              |
| **Additional Root Files**                         |                                                |                    |
| PayServicesRequest.cs                             | PayServicesRequest                             | ✓ YES              |
| SearchBy.cs                                       | SearchBy (Enum/Helper)                         | ✗ NO               |
| SetFrozenStateRequest.cs                          | SetFrozenStateRequest                          | ✗ NO               |
| SimpleRequest.cs                                  | SimpleRequest (Base class)                     | ✗ NO               |
| StartAndEndDateRequest.cs                         | StartAndEndDateRequest                         | ✓ YES              |
| SuggestedForfeitureAdjustmentRequest.cs           | SuggestedForfeitureAdjustmentRequest           | ✗ NO               |
| TerminatedEmployeesWithBalanceBreakdownRequest.cs | TerminatedEmployeesWithBalanceBreakdownRequest | ✓ YES              |
| TerminatedLettersRequest.cs                       | TerminatedLettersRequest                       | ✓ YES              |
| UserSyncRequestDto.cs                             | UserSyncRequestDto                             | ✗ NO               |
| WagesCurrentYearRequest.cs                        | WagesCurrentYearRequest                        | ✓ YES              |
| YearEndProfitSharingReportRequest.cs              | YearEndProfitSharingReportRequest              | ✓ YES              |
| YearRangeRequest.cs                               | YearRangeRequest                               | ✗ NO               |
| YearRequest.cs                                    | YearRequest                                    | ✗ NO               |
| YearRequestWithRebuild.cs                         | YearRequestWithRebuild                         | ✗ NO               |

### Adjustments/ (1 file)

| File                         | Class Name                | Has RequestExample |
| ---------------------------- | ------------------------- | ------------------ |
| MergeProfitDetailsRequest.cs | MergeProfitDetailsRequest | ✓ YES              |

### Administration/ (2 files)

| File                        | Class Name               | Has RequestExample |
| --------------------------- | ------------------------ | ------------------ |
| CreateCommentTypeRequest.cs | CreateCommentTypeRequest | ✓ YES              |
| UpdateCommentTypeRequest.cs | UpdateCommentTypeRequest | ✗ NO               |

### Audit/ (2 files)

| File                     | Class Name            | Has RequestExample |
| ------------------------ | --------------------- | ------------------ |
| AuditChangeEntryInput.cs | AuditChangeEntryInput | ✓ YES              |
| AuditSearchRequestDto.cs | AuditSearchRequestDto | ✓ YES              |

### Beneficiaries/ (6 files)

| File                               | Class Name                      | Has RequestExample |
| ---------------------------------- | ------------------------------- | ------------------ |
| BadgeNumberRequest.cs              | BadgeNumberRequest              | ✗ NO               |
| BeneficiaryDisbursementRequest.cs  | BeneficiaryDisbursementRequest  | ✗ NO               |
| BeneficiaryTypeRequestDto.cs       | BeneficiaryTypeRequestDto       | ✗ NO               |
| CreateBeneficiaryContactRequest.cs | CreateBeneficiaryContactRequest | ✗ NO               |
| CreateBeneficiaryRequest.cs        | CreateBeneficiaryRequest        | ✗ NO               |
| UpdateBeneficiaryContactRequest.cs | UpdateBeneficiaryContactRequest | ✗ NO               |
| UpdateBeneficiaryRequest.cs        | UpdateBeneficiaryRequest        | ✗ NO               |

### BeneficiaryInquiry/ (5 files)

| File                              | Class Name                     | Has RequestExample |
| --------------------------------- | ------------------------------ | ------------------ |
| BeneficiaryDetailRequest.cs       | BeneficiaryDetailRequest       | ✓ YES              |
| BeneficiaryKindRequestDto.cs      | BeneficiaryKindRequestDto      | ✓ YES              |
| BeneficiaryRequestDto.cs          | BeneficiaryRequestDto          | ✓ YES              |
| BeneficiarySearchFilterRequest.cs | BeneficiarySearchFilterRequest | ✓ YES              |
| BeneficiaryTypesRequestDto.cs     | BeneficiaryTypesRequestDto     | ✓ YES              |

### CheckRun/ (1 file)

| File                    | Class Name           | Has RequestExample |
| ----------------------- | -------------------- | ------------------ |
| CheckRunStartRequest.cs | CheckRunStartRequest | ✓ YES              |

### Distributions/ (6 files)

| File                            | Class Name                   | Has RequestExample |
| ------------------------------- | ---------------------------- | ------------------ |
| Address.cs                      | Address                      | ✓ YES              |
| CreateDistributionRequest.cs    | CreateDistributionRequest    | ✓ YES              |
| DistributionRunReportRequest.cs | DistributionRunReportRequest | ✓ YES              |
| DistributionSearchRequest.cs    | DistributionSearchRequest    | ✓ YES              |
| ThirdPartyPayee.cs              | ThirdPartyPayee              | ✓ YES              |
| UpdateDistributionRequest.cs    | UpdateDistributionRequest    | ✓ YES              |

### ItOperations/ (3 files)

| File                         | Class Name                | Has RequestExample |
| ---------------------------- | ------------------------- | ------------------ |
| GetAnnuityRatesRequest.cs    | GetAnnuityRatesRequest    | ✓ YES              |
| UpdateAnnuityRateRequest.cs  | UpdateAnnuityRateRequest  | ✓ YES              |
| UpdateStateTaxRateRequest.cs | UpdateStateTaxRateRequest | ✓ YES              |

### Job/ (1 file)

- No concrete Request DTOs (Job submission/query interfaces only)

### Lookups/ (1 file)

| File                     | Class Name            | Has RequestExample |
| ------------------------ | --------------------- | ------------------ |
| StateTaxLookupRequest.cs | StateTaxLookupRequest | ✓ YES              |

### MasterInquiry/ (3 files)

| File                                 | Class Name                        | Has RequestExample |
| ------------------------------------ | --------------------------------- | ------------------ |
| MasterInquiryRequest.cs              | MasterInquiryRequest              | ✓ YES              |
| MasterInquiryMemberDetailsRequest.cs | MasterInquiryMemberDetailsRequest | ✗ NO               |
| MasterInquiryMemberRequest.cs        | MasterInquiryMemberRequest        | ✗ NO               |

### Military/ (2 files)

| File                                 | Class Name                        | Has RequestExample |
| ------------------------------------ | --------------------------------- | ------------------ |
| CreateMilitaryContributionRequest.cs | CreateMilitaryContributionRequest | ✓ YES              |
| GetMilitaryContributionRequest.cs    | GetMilitaryContributionRequest    | ✓ YES              |

### Navigations/ (3 files)

| File                             | Class Name                    | Has RequestExample |
| -------------------------------- | ----------------------------- | ------------------ |
| GetNavigationStatusRequestDto.cs | GetNavigationStatusRequestDto | ✓ YES              |
| NavigationRequestDto.cs          | NavigationRequestDto          | ✓ YES              |
| UpdateNavigationRequestDto.cs    | UpdateNavigationRequestDto    | ✓ YES              |

### PayBen/ (1 file)

| File                   | Class Name          | Has RequestExample |
| ---------------------- | ------------------- | ------------------ |
| PayBenReportRequest.cs | PayBenReportRequest | ✗ NO               |

### ProfitDetails/ (3 files)

| File                                   | Class Name                          | Has RequestExample |
| -------------------------------------- | ----------------------------------- | ------------------ |
| GetProfitSharingAdjustmentsRequest.cs  | GetProfitSharingAdjustmentsRequest  | ✓ YES              |
| ProfitSharingAdjustmentRowRequest.cs   | ProfitSharingAdjustmentRowRequest   | ✓ YES              |
| SaveProfitSharingAdjustmentsRequest.cs | SaveProfitSharingAdjustmentsRequest | ✓ YES              |

### Validation/ (2 files)

| File                                    | Class Name                           | Has RequestExample |
| --------------------------------------- | ------------------------------------ | ------------------ |
| ProfitSharingReportValidationRequest.cs | ProfitSharingReportValidationRequest | ✗ NO               |
| ValidateReportFieldsRequest.cs          | ValidateReportFieldsRequest          | ✗ NO               |

---

## RESPONSE DTOs BY CATEGORY

### Root Level (39 files)

| File                                     | Class Name                            | Has ResponseExample |
| ---------------------------------------- | ------------------------------------- | ------------------- |
| AccountHistoryReportPaginatedResponse.cs | AccountHistoryReportPaginatedResponse | ✗ NO                |
| AccountHistoryReportResponse.cs          | AccountHistoryReportResponse          | ✗ NO                |
| AccountHistoryReportTotals.cs            | AccountHistoryReportTotals            | ✗ NO                |
| AddressResponseDto.cs                    | AddressResponseDto                    | ✓ YES               |
| AdhocBeneficiariesReportResponse.cs      | AdhocBeneficiariesReportResponse      | ✗ NO                |
| AdjustmentsSummaryDto.cs                 | AdjustmentsSummaryDto                 | ✗ NO                |
| BalanceEndpointResponse.cs               | BalanceEndpointResponse               | ✓ YES               |
| BeneficiaryReportDto.cs                  | BeneficiaryReportDto                  | ✗ NO                |
| BeneficiaryTypeResponseDto.cs            | BeneficiaryTypeResponseDto            | ✓ YES               |
| CalendarResponseDto.cs                   | CalendarResponseDto                   | ✓ YES               |
| CertificateReprintResponse.cs            | CertificateReprintResponse            | ✓ YES               |
| ContactInfoResponseDto.cs                | ContactInfoResponseDto                | ✓ YES               |
| DataWindowMetadata.cs                    | DataWindowMetadata                    | ✗ NO                |
| DemographicResponseDto.cs                | DemographicResponseDto                | ✓ YES               |
| DepartmentResponseDto.cs                 | DepartmentResponseDto                 | ✓ YES               |
| EmployeeTypeResponseDto.cs               | EmployeeTypeResponseDto               | ✓ YES               |
| EmploymentTypeResponseDto.cs             | EmploymentTypeResponseDto             | ✓ YES               |
| EnrollmentResponseDto.cs                 | EnrollmentResponseDto                 | ✓ YES               |
| FrozenStateResponse.cs                   | FrozenStateResponse                   | ✓ YES               |
| GenderResponseDto.cs                     | GenderResponseDto                     | ✓ YES               |
| IdsResponse.cs                           | IdsResponse                           | ✓ YES               |
| ListResponseDto.cs                       | ListResponseDto                       | ✗ NO                |
| PayFrequencyResponseDto.cs               | PayFrequencyResponseDto               | ✓ YES               |
| PayProfitResponseDto.cs                  | PayProfitResponseDto                  | ✗ NO                |
| ProfitControlSheetResponse.cs            | ProfitControlSheetResponse            | ✓ YES               |
| ProfitDetailDto.cs                       | ProfitDetailDto                       | ✗ NO                |
| ReportResponseBase.cs                    | ReportResponseBase                    | ✗ NO                |
| TerminationCodeResponseDto.cs            | TerminationCodeResponseDto            | ✓ YES               |

### Administration/ (1 file)

| File              | Class Name     | Has ResponseExample |
| ----------------- | -------------- | ------------------- |
| CommentTypeDto.cs | CommentTypeDto | ✓ YES               |

### Audit/ (2 files)

| File                   | Class Name          | Has ResponseExample |
| ---------------------- | ------------------- | ------------------- |
| AuditChangeEntryDto.cs | AuditChangeEntryDto | ✓ YES               |
| AuditEventDto.cs       | AuditEventDto       | ✓ YES               |

### Beneficiaries/ (4 files)

| File                               | Class Name                      | Has ResponseExample |
| ---------------------------------- | ------------------------------- | ------------------- |
| BeneficiaryDetailResponse.cs       | BeneficiaryDetailResponse       | ✗ NO                |
| BeneficiaryDto.cs                  | BeneficiaryDto                  | ✗ NO                |
| BeneficiaryResponse.cs             | BeneficiaryResponse             | ✗ NO                |
| BeneficiarySearchFilterResponse.cs | BeneficiarySearchFilterResponse | ✗ NO                |

### BeneficiaryInquiry/ (7 files)

| File                                | Class Name                       | Has ResponseExample             |
| ----------------------------------- | -------------------------------- | ------------------------------- |
| BeneficiaryRequestDto.cs            | BeneficiaryRequestDto            | ✗ NO (note: in Response folder) |
| BeneficiaryTypeDto.cs               | BeneficiaryTypeDto               | ✗ NO                            |
| BeneficiaryTypesResponseDto.cs      | BeneficiaryTypesResponseDto      | ✗ NO                            |
| BeneficiaryDetailResponse.cs        | BeneficiaryDetailResponse        | ✗ NO                            |
| BeneficiaryDto.FullName.partial.cs  | BeneficiaryDto (partial)         | ✗ NO                            |
| CreateBeneficiaryContactResponse.cs | CreateBeneficiaryContactResponse | ✗ NO                            |
| CreateBeneficiaryResponse.cs        | CreateBeneficiaryResponse        | ✗ NO                            |

### CheckRun/ (2 files)

| File                        | Class Name               | Has ResponseExample |
| --------------------------- | ------------------------ | ------------------- |
| CheckRunStepStatus.cs       | CheckRunStepStatus       | ✗ NO                |
| CheckRunWorkflowResponse.cs | CheckRunWorkflowResponse | ✓ YES               |

### Distributions/ (7 files)

| File                                    | Class Name                           | Has ResponseExample |
| --------------------------------------- | ------------------------------------ | ------------------- |
| CreateOrUpdateDistributionResponse.cs   | CreateOrUpdateDistributionResponse   | ✓ YES               |
| DisbursementReportDetailResponse.cs     | DisbursementReportDetailResponse     | ✗ NO                |
| DistributionFrequencyResponse.cs        | DistributionFrequencyResponse        | ✓ YES               |
| DistributionRunReportDetail.cs          | DistributionRunReportDetail          | ✗ NO                |
| DistributionRunReportSummaryResponse.cs | DistributionRunReportSummaryResponse | ✗ NO                |
| DistributionSearchResponse.cs           | DistributionSearchResponse           | ✗ NO                |
| DistributionsOnHoldResponse.cs          | DistributionsOnHoldResponse          | ✗ NO                |
| ManualChecksWrittenResponse.cs          | ManualChecksWrittenResponse          | ✓ YES               |

### Headers/ (2 files)

| File                                       | Class Name                              | Has ResponseExample |
| ------------------------------------------ | --------------------------------------- | ------------------- |
| DemographicHeaders.cs                      | DemographicHeaders                      | ✗ NO                |
| DemographicBadgesNotInPayProfitResponse.cs | DemographicBadgesNotInPayProfitResponse | ✗ NO                |

### ItOperations/ (8 files)

| File                                  | Class Name                         | Has ResponseExample |
| ------------------------------------- | ---------------------------------- | ------------------- |
| AnnuityRateDto.cs                     | AnnuityRateDto                     | ✓ YES               |
| ClearAuditResponse.cs                 | ClearAuditResponse                 | ✓ YES               |
| DemographicSyncAuditDto.cs            | DemographicSyncAuditDto            | ✓ YES               |
| DemographicSyncAuditRecordResponse.cs | DemographicSyncAuditRecordResponse | ✓ YES               |
| OracleHcmSyncMetadata.cs              | OracleHcmSyncMetadata              | ✓ YES               |
| OracleHcmSyncMetadataResponse.cs      | OracleHcmSyncMetadataResponse      | ✓ YES               |
| RowCountResult.cs                     | RowCountResult                     | ✓ YES               |
| StateTaxRateDto.cs                    | StateTaxRateDto                    | ✓ YES               |

### Job/ (1 file)

| File                   | Class Name          | Has ResponseExample |
| ---------------------- | ------------------- | ------------------- |
| SendMessageResponse.cs | SendMessageResponse | ✓ YES               |

### Lookup/ (8 files)

| File                            | Class Name                   | Has ResponseExample |
| ------------------------------- | ---------------------------- | ------------------- |
| CommentTypeResponse.cs          | CommentTypeResponse          | ✓ YES               |
| DistributionStatusResponse.cs   | DistributionStatusResponse   | ✓ YES               |
| MissiveResponse.cs              | MissiveResponse              | ✓ YES               |
| PayClassificationResponseDto.cs | PayClassificationResponseDto | ✓ YES               |
| StateListResponse.cs            | StateListResponse            | ✓ YES               |
| StateTaxLookupResponse.cs       | StateTaxLookupResponse       | ✓ YES               |
| TaxCodeResponse.cs              | TaxCodeResponse              | ✓ YES               |

### MasterInquiry/ (4 files)

| File                        | Class Name               | Has ResponseExample |
| --------------------------- | ------------------------ | ------------------- |
| GroupedProfitSummaryDto.cs  | GroupedProfitSummaryDto  | ✓ YES               |
| MasterInquiryResponseDto.cs | MasterInquiryResponseDto | ✓ YES               |
| MemberDetails.cs            | MemberDetails            | ✓ YES               |
| MemberProfitPlanDetails.cs  | MemberProfitPlanDetails  | ✓ YES               |

### Military/ (1 file)

| File                            | Class Name                   | Has ResponseExample |
| ------------------------------- | ---------------------------- | ------------------- |
| MilitaryContributionResponse.cs | MilitaryContributionResponse | ✓ YES               |

### Navigations/ (5 files)

| File                                 | Class Name                        | Has ResponseExample |
| ------------------------------------ | --------------------------------- | ------------------- |
| GetNavigationStatusResponseDto.cs    | GetNavigationStatusResponseDto    | ✓ YES               |
| NavigationDto.cs                     | NavigationDto                     | ✓ YES               |
| NavigationResponseDto.cs             | NavigationResponseDto             | ✓ YES               |
| NavigationStatusDto.cs               | NavigationStatusDto               | ✓ YES               |
| UpdateNavigationStatusResponseDto.cs | UpdateNavigationStatusResponseDto | ✓ YES               |

### PayBen/ (1 file)

| File                    | Class Name           | Has ResponseExample |
| ----------------------- | -------------------- | ------------------- |
| PayBenReportResponse.cs | PayBenReportResponse | ✓ YES               |

### PostFrozen/ (8 files)

| File                                             | Class Name                                    | Has ResponseExample |
| ------------------------------------------------ | --------------------------------------------- | ------------------- |
| NewProfitSharingLabelResponse.cs                 | NewProfitSharingLabelResponse                 | ✓ YES               |
| ProfitSharingLabelResponse.cs                    | ProfitSharingLabelResponse                    | ✓ YES               |
| ProfitSharingUnder21BreakdownByStoreResponse.cs  | ProfitSharingUnder21BreakdownByStoreResponse  | ✓ YES               |
| ProfitSharingUnder21InactiveNoBalanceResponse.cs | ProfitSharingUnder21InactiveNoBalanceResponse | ✓ YES               |
| ProfitSharingUnder21ReportDetail.cs              | ProfitSharingUnder21ReportDetail              | ✓ YES               |
| ProfitSharingUnder21ReportResponse.cs            | ProfitSharingUnder21ReportResponse            | ✓ YES               |
| ProfitSharingUnder21TotalForStatus.cs            | ProfitSharingUnder21TotalForStatus            | ✓ YES               |
| ProfitSharingUnder21TotalsResponse.cs            | ProfitSharingUnder21TotalsResponse            | ✓ YES               |

### ProfitDetails/ (2 files)

| File                                   | Class Name                          | Has ResponseExample |
| -------------------------------------- | ----------------------------------- | ------------------- |
| GetProfitSharingAdjustmentsResponse.cs | GetProfitSharingAdjustmentsResponse | ✓ YES               |
| ProfitSharingAdjustmentRowResponse.cs  | ProfitSharingAdjustmentRowResponse  | ✓ YES               |

### Validation/ (6 files)

| File                                            | Class Name                                   | Has ResponseExample |
| ----------------------------------------------- | -------------------------------------------- | ------------------- |
| ChecksumValidationResponse.cs                   | ChecksumValidationResponse                   | ✓ YES               |
| CrossReferenceValidation.cs                     | CrossReferenceValidation                     | ✓ YES               |
| CrossReferenceValidationGroup.cs                | CrossReferenceValidationGroup                | ✓ YES               |
| FieldValidationResult.cs                        | FieldValidationResult                        | ✓ YES               |
| MasterUpdateCrossReferenceValidationResponse.cs | MasterUpdateCrossReferenceValidationResponse | ✓ YES               |
| ValidationResponse.cs                           | ValidationResponse                           | ✓ YES               |

### YearEnd/ (58 files) - **LARGEST CATEGORY**

| File                                               | Class Name                                      | Has ResponseExample  |
| -------------------------------------------------- | ----------------------------------------------- | -------------------- |
| BalanceByAge.cs                                    | BalanceByAge                                    | ✓ YES                |
| BalanceByAgeDetail.cs                              | BalanceByAgeDetail                              | ✓ YES                |
| BalanceByBase.cs                                   | BalanceByBase                                   | ✗ NO                 |
| BalanceByDetailBase.cs                             | BalanceByDetailBase                             | ✗ NO                 |
| BalanceByYears.cs                                  | BalanceByYears                                  | ✓ YES                |
| BalanceByYearsDetail.cs                            | BalanceByYearsDetail                            | ✓ YES                |
| BreakdownByStoreTotals.cs                          | BreakdownByStoreTotals                          | ✗ NO                 |
| ContributionsByAge.cs                              | ContributionsByAge                              | ✓ YES                |
| ContributionsByAgeDetail.cs                        | ContributionsByAgeDetail                        | ✓ YES                |
| DistributionsByAge.cs                              | DistributionsByAge                              | ✓ YES                |
| DistributionsByAgeDetail.cs                        | DistributionsByAgeDetail                        | ✓ YES                |
| DistributionsAndForfeitureResponse.cs              | DistributionsAndForfeitureResponse              | ✓ YES                |
| DistributionsAndForfeitureTotalsResponse.cs        | DistributionsAndForfeitureTotalsResponse        | ✓ YES                |
| DuplicateNamesAndBirthdaysCachedResponse.cs        | DuplicateNamesAndBirthdaysCachedResponse        | ✗ NO                 |
| DuplicateNamesAndBirthdaysResponse.cs              | DuplicateNamesAndBirthdaysResponse              | ✗ NO                 |
| DuplicateSsnReportResponse.cs                      | DuplicateSsnReportResponse                      | ✗ NO                 |
| EligibleEmployee.cs                                | EligibleEmployee                                | ✓ YES (as Example()) |
| ExecutiveHoursAndDollarsResponse.cs                | ExecutiveHoursAndDollarsResponse                | ✓ YES                |
| ForfeituresAndPointsForYearResponse.cs             | ForfeituresAndPointsForYearResponse             | ✓ YES                |
| ForfeituresAndPointsForYearResponseWithTotals.cs   | ForfeituresAndPointsForYearResponseWithTotals   | ✓ YES                |
| ForfeituresByAge.cs                                | ForfeituresByAge                                | ✓ YES                |
| ForfeituresByAgeDetail.cs                          | ForfeituresByAgeDetail                          | ✓ YES                |
| GetEligibleEmployeesResponse.cs                    | GetEligibleEmployeesResponse                    | ✗ NO                 |
| GrandTotalsByStoreResponseDto.cs                   | GrandTotalsByStoreResponseDto                   | ✗ NO                 |
| GrandTotalsByStoreRowDto.cs                        | GrandTotalsByStoreRowDto                        | ✗ NO                 |
| GrossWagesReportDetail.cs                          | GrossWagesReportDetail                          | ✓ YES                |
| GrossWagesReportResponse.cs                        | GrossWagesReportResponse                        | ✓ YES                |
| MemberYearSummaryDto.cs                            | MemberYearSummaryDto                            | ✗ NO                 |
| NegativeETVAForSSNsOnPayProfitResponse.cs          | NegativeETVAForSSNsOnPayProfitResponse          | ✗ NO                 |
| PayrollDuplicateSSNResponseDto.cs                  | PayrollDuplicateSsnResponseDto                  | ✓ YES                |
| ProfitMasterRevertResponse.cs                      | ProfitMasterRevertResponse                      | ✓ YES (as Example()) |
| ProfitMasterUpdateResponse.cs                      | ProfitMasterUpdateResponse                      | ✓ YES (as Example()) |
| ProfitShareEditMemberRecordResponse.cs             | ProfitShareEditMemberRecordResponse             | ✓ YES                |
| ProfitShareEditResponse.cs                         | ProfitShareEditResponse                         | ✗ NO                 |
| ProfitShareUpdateMemberResponse.cs                 | ProfitShareUpdateMemberResponse                 | ✗ NO                 |
| ProfitShareUpdateResponse.cs                       | ProfitShareUpdateResponse                       | ✗ NO                 |
| ProfitShareUpdateTotals.cs                         | ProfitShareUpdateTotals                         | ✗ NO                 |
| SuggestedForfeitureAdjustmentResponse.cs           | SuggestedForfeitureAdjustmentResponse           | ✓ YES                |
| TerminatedEmployeeAndBeneficiaryDataResponseDto.cs | TerminatedEmployeeAndBeneficiaryDataResponseDto | ✓ YES                |
| TerminatedEmployeeAndBeneficiaryResponse.cs        | TerminatedEmployeeAndBeneficiaryResponse        | ✗ NO                 |
| TerminatedEmployeeAndBeneficiaryYearDetailDto.cs   | TerminatedEmployeeAndBeneficiaryYearDetailDto   | ✗ NO                 |
| UnattributedTotals.cs                              | UnattributedTotals                              | ✗ NO                 |
| UnforfeituresResponse.cs                           | UnforfeituresResponse                           | ✓ YES                |
| UpdateSummaryReportDetail.cs                       | UpdateSummaryReportDetail                       | ✓ YES                |
| UpdateSummaryReportPointInTimeDetail.cs            | UpdateSummaryReportPointInTimeDetail            | ✗ NO                 |
| UpdateSummaryReportResponse.cs                     | UpdateSummaryReportResponse                     | ✓ YES                |
| VestedAmountsByAge.cs                              | VestedAmountsByAge                              | ✓ YES                |
| VestedAmountsByAgeDetail.cs                        | VestedAmountsByAgeDetail                        | ✗ NO                 |
| WagesCurrentYearParticipant.cs                     | WagesCurrentYearParticipant                     | ✗ NO                 |
| WagesCurrentYearResponse.cs                        | WagesCurrentYearResponse                        | ✓ YES                |
| YearEndProfitSharingReportDetail.cs                | YearEndProfitSharingReportDetail                | ✓ YES                |
| YearEndProfitSharingReportResponse.cs              | YearEndProfitSharingReportResponse              | ✓ YES                |
| YearEndProfitSharingReportSummaryLineItem.cs       | YearEndProfitSharingReportSummaryLineItem       | ✗ NO                 |
| YearEndProfitSharingReportSummaryResponse.cs       | YearEndProfitSharingReportSummaryResponse       | ✗ NO                 |
| YearEndProfitSharingReportTotals.cs                | YearEndProfitSharingReportTotals                | ✗ NO                 |
| Frozen/BalanceByYears.cs                           | BalanceByYears                                  | ✓ YES                |
| Frozen/BalanceByYearsDetail.cs                     | BalanceByYearsDetail                            | ✓ YES                |

---

## REQUEST DTOs MISSING RequestExample() METHOD (30 TOTAL)

### By Endpoint Category

**Beneficiaries (7 files)**

- AddressRequestDto (note: shared across multiple domains)
- BadgeNumberRequest
- BeneficiaryDisbursementRequest
- BeneficiaryTypeRequestDto
- CreateBeneficiaryContactRequest
- CreateBeneficiaryRequest
- UpdateBeneficiaryContactRequest
- UpdateBeneficiaryRequest

**Administration (1 file)**

- UpdateCommentTypeRequest

**Core/Root (14 files)**

- ContactInfoRequestDto
- DemographicsRequest
- EmployeeTypeRequestDto
- EnrollmentRequestDto
- IdRequest
- IMemberRequest (Interface - optional)
- IStartEndDateRequest (Interface - optional)
- ProfitYearAndAsOfDateRequest
- SearchBy (Enum/Helper - skip)
- SetFrozenStateRequest
- SimpleRequest (Base class - skip)
- SuggestedForfeitureAdjustmentRequest
- UserSyncRequestDto
- YearRangeRequest
- YearRequest
- YearRequestWithRebuild

**MasterInquiry (2 files)**

- MasterInquiryMemberDetailsRequest
- MasterInquiryMemberRequest

**PayBen (1 file)**

- PayBenReportRequest

**Validation (2 files)**

- ProfitSharingReportValidationRequest
- ValidateReportFieldsRequest

---

## RESPONSE DTOs MISSING ResponseExample() METHOD (56 TOTAL)

### By Endpoint Category (Organized by Priority)

**YearEnd (Critical - 19 files)**

- AdhocBeneficiariesReportResponse
- BalanceByBase (base class)
- BalanceByDetailBase (base class)
- BreakdownByStoreTotals
- DuplicateNamesAndBirthdaysCachedResponse
- DuplicateNamesAndBirthdaysResponse
- DuplicateSsnReportResponse
- GetEligibleEmployeesResponse
- GrandTotalsByStoreResponseDto
- GrandTotalsByStoreRowDto
- MemberYearSummaryDto
- NegativeETVAForSSNsOnPayProfitResponse
- ProfitShareEditResponse
- ProfitShareUpdateMemberResponse
- ProfitShareUpdateResponse
- ProfitShareUpdateTotals
- TerminatedEmployeeAndBeneficiaryResponse
- TerminatedEmployeeAndBeneficiaryYearDetailDto
- UnattributedTotals
- UpdateSummaryReportPointInTimeDetail
- VestedAmountsByAgeDetail
- WagesCurrentYearParticipant
- YearEndProfitSharingReportSummaryLineItem
- YearEndProfitSharingReportSummaryResponse
- YearEndProfitSharingReportTotals

**Validation/Reports (7 files)**

- AccountHistoryReportPaginatedResponse
- AccountHistoryReportResponse
- AccountHistoryReportTotals
- (Note: These 3 are for AccountHistory report - may share pattern)

**Core/Root (8 files)**

- AdjustmentsSummaryDto
- DataWindowMetadata
- ListResponseDto
- PayProfitResponseDto
- ProfitDetailDto
- ReportResponseBase (base class)

**Beneficiaries (7 files)**

- BeneficiaryDetailResponse
- BeneficiaryDto
- BeneficiaryResponse
- BeneficiarySearchFilterResponse
- BeneficiaryTypeDto (in BeneficiaryInquiry)
- BeneficiaryTypesResponseDto
- CreateBeneficiaryContactResponse
- CreateBeneficiaryResponse
- UpdateBeneficiaryContactResponse
- UpdateBeneficiaryResponse

**CheckRun (1 file)**

- CheckRunStepStatus

**Distributions (5 files)**

- DisbursementReportDetailResponse
- DistributionRunReportDetail
- DistributionRunReportSummaryResponse
- DistributionSearchResponse
- DistributionsOnHoldResponse

**Headers (2 files)**

- DemographicBadgesNotInPayProfitResponse
- DemographicHeaders

---

## GROUPED BY ENDPOINT DOMAIN (FOR BATCH PHASE 3 PROCESSING)

### Phase 1: YearEnd Report DTOs (HIGH PRIORITY)

**Est. Methods to Add:** 25-30
**Files:** 58 in YearEnd folder
**Current Status:** 34 with examples, 24 missing
**Priority:** CRITICAL - Year-end processing is core business logic

### Phase 2: Distribution + Validation + Reports

**Est. Methods to Add:** 20-25
**Categories:**

- Distributions (5 missing)
- Validation (6 complete, 0 missing)
- Audit (2 complete)
- Reports/AccountHistory (3 missing)
- PayBen/Job (0 missing)

### Phase 3: Beneficiary + Core/Admin DTOs

**Est. Methods to Add:** 25-30
**Categories:**

- Beneficiaries (10+ missing)
- BeneficiaryInquiry (0 missing)
- Core/Root (14 missing, but some are interfaces/base classes)
- MasterInquiry (2 missing)
- Administration (1 missing)

### Phase 4: Infrastructure/Lookup DTOs

**Est. Methods to Add:** 5-10
**Categories:**

- ItOperations (0 missing)
- Lookup (0 missing)
- Navigations (0 missing)
- Military (0 missing)
- Job (0 missing)

---

## RECOMMENDATIONS FOR IMPLEMENTATION

### Work Sequence

1. **Phase 1 (Week 1):** Focus on YearEnd folder Response DTOs - highest business value
2. **Phase 2 (Week 1-2):** Distributions, Validation, and Account History reports
3. **Phase 3 (Week 2-3):** Beneficiary, Core, and MasterInquiry Request/Response DTOs
4. **Phase 4 (Week 3):** Final batch - any remaining infrastructure DTOs

### Implementation Pattern

All Example methods should follow this pattern:

**For Request DTOs:**

```csharp
public static {ClassName} RequestExample()
{
    return new {ClassName}
    {
        // Populate properties with meaningful test data
        Property1 = value1,
        Property2 = value2,
    };
}
```

**For Response DTOs:**

```csharp
public static {ClassName} ResponseExample()
{
    return new {ClassName}
    {
        // Populate with representative response data
        Property1 = value1,
        Property2 = value2,
    };
}
```

### Testing Requirements

- Each new Example method should be testable for null/empty validation
- Complex nested objects should reference their own Example() methods
- Consider using Bogus library for realistic test data where appropriate
- Add unit tests for each Example method to ensure complete object graph

### Documentation

- Document any business logic dependencies in Example methods
- Add comments for non-obvious test values
- Link Examples to their endpoint definitions in code

---

## METADATA

**Report Generated:** December 31, 2025
**Total Scan Duration:** ~5 minutes
**Files Analyzed:** 239 (84 Request, 155 Response)
**Methods Found:** 153 (57 RequestExample + 96 ResponseExample)
**Methods Missing:** 86 (30 Request, 56 Response)
**Completeness:** 64% (153/239)

**Source Paths:**

- Request DTOs: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/`
- Response DTOs: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Response/`
