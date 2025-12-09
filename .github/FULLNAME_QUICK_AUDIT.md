# Quick Audit: Finding FullName Pattern Stragglers

## 🔍 Search Commands You Can Run Now

### Backend Stragglers

**Find Response DTOs with "Name" property (excluding legitimate uses):**

```powershell
# PowerShell
Get-ChildItem -Path "src/services/src/Demoulas.ProfitSharing.Common/Contracts/Response" -Filter "*Response.cs" -Recurse |
  ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match 'public\s+string\s+Name\s*(?:{|=|;)' -and
        $content -notmatch 'FullName|FrequencyName|StatusName|TaxCodeName|KindName|TypeName|EmploymentTypeName|DepartmentName|PayClassificationName|RelationshipName') {
      Write-Host "❌ $($_.FullName)" -ForegroundColor Red
    }
  }
```

**Find services assigning FullName without ComputeFullNameWithInitial:**

```powershell
# PowerShell
Get-ChildItem -Path "src/services/src/Demoulas.ProfitSharing.Services" -Filter "*Service.cs" -Recurse |
  ForEach-Object {
    $content = Get-Content $_.FullName
    $lineNum = 0
    $content | ForEach-Object {
      $lineNum++
      if ($_ -match 'FullName\s*=' -and
          $_ -notmatch 'ComputeFullNameWithInitial' -and
          $_ -notmatch '\.FullName\s*\?' -and
          $_ -notmatch '//') {
        Write-Host "$($_.FullName):$lineNum - $($_.Trim())" -ForegroundColor Red
      }
    }
  }
```

### Frontend Stragglers

**Find .name property access on person objects:**

```bash
# Bash/PowerShell
grep -r "\.name\b" src/ui/src/pages --include="*.tsx" --include="*.ts" |
  grep -v "headerName\|displayName\|kindName\|statusName\|frequencyName\|typeName\|roleName\|pathName\|taxCodeName" |
  grep -v "__test__\|\.test\." |
  grep -v "params\.data\|checkedValue"
```

**Find manual firstName/lastName concatenation:**

```bash
# Bash
grep -r -E "(firstName|lastName)\s*\+|firstName.*lastName|lastName.*firstName" \
  src/ui/src/pages --include="*.tsx" --include="*.ts" |
  grep -v "//\|❌\|WRONG\|comment" |
  grep -v "__test__\|\.test\."
```

**Find components NOT using fullName in display:**

```bash
# Look for BeneficiaryDetail, EmployeeDetails, etc. without fullName
grep -r "selectedMember\.\(firstName\|lastName\)|memberDetails\.\(firstName\|lastName\)" \
  src/ui/src/pages --include="*.tsx" --include="*.ts"
```

## Summary of Completed Updates

### ✅ Services Updated

1. **ExecutiveHoursAndDollarsService** - Lines 84-94

   - Uses `DtoCommonExtensions.ComputeFullNameWithInitial()`
   - Format: "LastName, FirstName M"

2. **BreakdownReportService** - Lines 746-748

   - Uses inline `ComputeFullNameWithInitial()` with null-safe pattern
   - Handles cases with/without middle name

3. **BeneficiaryInquiryService** - GetBeneficiaryDetail method

   - Database query path: Uses `ComputeFullNameWithInitial()` with individual name parts
   - In-memory path: Uses computed FullName from upstream service
   - Both paths ensure consistent middle initial formatting

4. **DistributionService** - SearchAsync method
   - Fetches individual name parts: `DemLastName`, `DemFirstName`, `DemMiddleName`
   - Fetches beneficiary parts: `BeneLastName`, `BeneFirstName`, `BeneMiddleName`
   - Computes FullName using `ComputeFullNameWithInitial()`
   - Prefers Demographic (employee) name with Beneficiary fallback

### ✅ DTOs Updated

**Backend (C#)**

- ExecutiveHoursAndDollarsResponse: `FullName` property
- BreakdownByStoreEmployeeResponse: `FullName` property
- EmployeeDetails: `FullName` property
- BeneficiaryDetail: `FullName` property
- BeneficiaryDetailResponse: `FullName` property (replaces `Name`)
- DistributionSearchResponse: `FullName` property
- BeneficiaryDto: `FullName` property

**Frontend (TypeScript)**

- EmployeeDetails: `fullName` property
- BeneficiaryDetail: `fullName` property
- DistributionSearchResponse: `fullName` property
- BeneficiaryDto: `fullName` property

### ✅ Components Updated

Frontend pages updated to use `fullName`:

- MasterInquiryMemberDetails.tsx
- EditDistribution.tsx
- AddDistribution.tsx
- ForfeituresAdjustment.ts
- MilitaryContribution.tsx
- BeneficiariesListGridColumns.ts
- BeneficiaryOfGridColumns.tsx
- MemberDetailsPanel.tsx (Beneficiaries page)

## 📊 Pattern Statistics

- **Endpoints using ComputeFullNameWithInitial**: 4
- **Response DTOs with FullName**: 8
- **Frontend components using fullName**: 8+
- **Manual concatenations eliminated**: 7

## ⚠️ Still Need Checking

After running the search commands above, check:

1. Any other Response DTOs with "Name" that aren't lookup tables
2. Any new Services added since this work
3. Any components displaying names that might have been missed
4. Any new pages or features requiring person names

## Next Steps

1. **Run the search commands** from "Quick Audit" section above
2. **Fix any violations** found using the guidance in FULLNAME_CONSOLIDATION_GUIDE.md
3. **Setup pre-commit hooks** from FULLNAME_PATTERN_PREVENTION.md
4. **Add to PR checklist** to catch future violations
5. **Document any exceptions** if found (e.g., for lookup table names)

## Documentation Files Created

- `.github/FULLNAME_CONSOLIDATION_GUIDE.md` - Comprehensive implementation guide
- `.github/FULLNAME_PATTERN_PREVENTION.md` - Pre-commit checks and CI/CD integration
- `.github/FULLNAME_QUICK_AUDIT.md` - This file

---

**Questions?**

- Check FULLNAME_CONSOLIDATION_GUIDE.md for implementation patterns
- Review completed examples in services for reference implementations
- See FULLNAME_PATTERN_PREVENTION.md for automated detection setup
