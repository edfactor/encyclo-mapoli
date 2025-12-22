import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { Typography } from "@mui/material";
import { useState } from "react";
import { numberToCurrency } from "smart-ui-library";
import { MasterUpdateCrossReferenceValidationResponse } from "@/types/validation/cross-reference-validation";

interface ProfitShareUpdateSummaryTableProps {
  totals: {
    beginningBalance: number;
    totalContribution: number;
    earnings: number;
    earnings2: number;
    forfeiture: number;
    distributions: number;
    military: number;
    endingBalance: number;
    allocations: number;
    paidAllocations: number;
    contributionPoints: number;
    earningPoints: number;
    maxPointsTotal: number;
  };
  validationResponse?: MasterUpdateCrossReferenceValidationResponse | null;
}

export const ProfitShareUpdateSummaryTable = ({ totals, validationResponse }: ProfitShareUpdateSummaryTableProps) => {
  const [openValidationField, setOpenValidationField] = useState<string | null>(null);

  const handleValidationToggle = (fieldName: string) => {
    setOpenValidationField(openValidationField === fieldName ? null : fieldName);
  };

  // Helper to get validation for a specific field
  const getFieldValidation = (fieldKey: string) => {
    if (!validationResponse) return null;

    for (const group of validationResponse.validationGroups) {
      const validation = group.validations.find((v) => v.fieldName === fieldKey);
      if (validation) return validation;
    }
    return null;
  };

  // Helper to render validation icon with popup
  const renderValidationIcon = (fieldKey: string, fieldDisplayName: string) => {
    const validation = getFieldValidation(fieldKey);
    if (!validation) return null;

    return (
      <div className="relative ml-1 inline-block">
        <InfoOutlinedIcon
          className={`cursor-pointer ${validation.isValid ? "text-green-500" : "text-orange-500"}`}
          fontSize="small"
          onClick={() => handleValidationToggle(fieldKey)}
        />
        {openValidationField === fieldKey && (
          <div className="absolute left-0 top-full z-[1000] mt-1 w-[350px] rounded border border-gray-300 bg-white shadow-lg">
            <div className="p-3">
              <div className="mb-2 flex items-center justify-between">
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: "bold" }}>
                  {fieldDisplayName}
                </Typography>
                <Typography
                  variant="caption"
                  sx={{
                    color: validation.isValid ? "success.main" : "warning.main",
                    fontWeight: "bold"
                  }}>
                  {validation.isValid ? "✓ Match" : "⚠ Mismatch"}
                </Typography>
              </div>
              <table className="w-full border-collapse text-sm">
                <thead>
                  <tr>
                    <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                    <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td className="border-b border-gray-100 px-2 py-1 text-left">Current (PAY444)</td>
                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                      {numberToCurrency(validation.currentValue || 0)}
                    </td>
                  </tr>
                  <tr>
                    <td className="border-b border-gray-100 px-2 py-1 text-left">Expected (PAY443)</td>
                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                      {numberToCurrency(validation.expectedValue || 0)}
                    </td>
                  </tr>
                  {!validation.isValid && (validation.variance || 0) !== 0 && (
                    <tr className="bg-orange-50">
                      <td className="px-2 py-1 text-left font-semibold text-orange-700">Variance</td>
                      <td className="px-2 py-1 text-right font-bold text-orange-700">
                        {numberToCurrency(validation.variance || 0)}
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
              {validation.message && (
                <Typography
                  variant="caption"
                  sx={{ mt: 1, display: "block", px: 1, color: "text.secondary" }}>
                  {validation.message}
                </Typography>
              )}
            </div>
          </div>
        )}
      </div>
    );
  };

  return (
    <div className="w-full">
      <table className="w-full border-collapse">
        <thead>
          <tr className="bg-gray-100">
            <th className="border border-gray-300 p-2 text-left"></th>
            <th className="border border-gray-300 p-2 text-center text-sm">
              <div className="flex items-center justify-center">
                <span>Beginning Balance</span>
                {validationResponse && renderValidationIcon("PAY444.BeginningBalance", "Beginning Balance")}
              </div>
            </th>
            <th className="border border-gray-300 p-2 text-center text-sm">
              <div className="flex items-center justify-center">
                <span>Contributions</span>
                {validationResponse && renderValidationIcon("PAY444.TotalContributions", "Contributions")}
              </div>
            </th>
            <th className="border border-gray-300 p-2 text-center text-sm">
              <div className="flex items-center justify-center">
                <span>Earnings</span>
                {validationResponse && renderValidationIcon("PAY444.TotalEarnings", "Earnings")}
              </div>
            </th>
            <th className="border border-gray-300 p-2 text-center text-sm">Earnings2</th>
            <th className="border border-gray-300 p-2 text-center text-sm">
              <div className="flex items-center justify-center">
                <span>Forfeitures</span>
                {validationResponse && renderValidationIcon("PAY444.TotalForfeitures", "Forfeitures")}
              </div>
            </th>
            <th className="border border-gray-300 p-2 text-center text-sm">
              <div className="flex items-center justify-center">
                <span>Distributions</span>
                {validationResponse && renderValidationIcon("PAY444.Distributions", "Distributions")}
              </div>
            </th>
            <th className="border border-gray-300 p-2 text-center text-sm">Military/Paid Allocation</th>
            <th className="border border-gray-300 p-2 text-center text-sm">Ending Balance</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td className="border border-gray-300 bg-gray-50 p-2 font-semibold">Total</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.beginningBalance || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.totalContribution || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.earnings || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.earnings2 || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.forfeiture || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.distributions || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.military || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.endingBalance || 0)}</td>
          </tr>
          <tr>
            <td className="border border-gray-300 bg-gray-50 p-2 font-semibold">Allocation</td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.allocations || 0)}</td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.maxPointsTotal || 0)}</td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.paidAllocations || 0)}</td>
            <td className="border border-gray-300 p-2 text-right">
              {numberToCurrency((totals.allocations || 0) + (totals.paidAllocations || 0))}
            </td>
          </tr>
          <tr>
            <td className="border border-gray-300 bg-gray-50 p-2 font-semibold">Point</td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2 text-right">
              {numberToCurrency(totals.contributionPoints || 0)}
            </td>
            <td className="border border-gray-300 p-2 text-right">{numberToCurrency(totals.earningPoints || 0)}</td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2"></td>
            <td className="border border-gray-300 p-2"></td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};
