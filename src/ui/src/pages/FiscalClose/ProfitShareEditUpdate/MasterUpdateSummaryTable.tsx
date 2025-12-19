import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { Typography } from "@mui/material";
import { formatNumberWithComma, numberToCurrency } from "smart-ui-library";
import { ProfitShareUpdateTotals } from "../../../reduxstore/types";

interface ValidationResult {
  isValid: boolean;
  currentValue: number;
  expectedValue: number;
  variance?: number;
  message?: string;
}

interface MasterUpdateSummaryTableProps {
  totals: ProfitShareUpdateTotals;
  getFieldValidation: (fieldKey: string) => ValidationResult | null;
  openValidationField: string | null;
  onValidationToggle: (fieldName: string) => void;
}

/**
 * Unified Summary (PAY444) table component that consolidates all profit sharing update totals
 * into a single table with row labels (Total/Allocation/Point) appearing once instead of repeated
 * in separate TotalsGrid components. Validation icons are integrated into column headers.
 */
export const MasterUpdateSummaryTable: React.FC<MasterUpdateSummaryTableProps> = ({
  totals,
  getFieldValidation,
  openValidationField,
  onValidationToggle
}) => {
  /**
   * Renders a validation icon in a column header with proper styling
   */
  const renderHeaderValidationIcon = (keysToValidate: string | string[]) => {
    var keys = Array.isArray(keysToValidate) ? keysToValidate : [keysToValidate];
    
    const validations=[];
    for (const key of keys) {
      const validation = getFieldValidation(key);
      validations.push(validation);
    }
    if (validations.every(v => v === null)) {
      return null;
    }
    
    const isValid = validations.every(v => v !== null && v.isValid);

    return (
      <div
        className="inline-block cursor-pointer"
        onClick={() => onValidationToggle(keys[0])}>
        <InfoOutlinedIcon
          className={`${isValid ? "text-green-500" : "text-orange-500"}`}
          fontSize="small"
        />
      </div>
    );
  };

  return (
    <>
      {/* Unified Summary Table */}
      <div className="px-[24px] py-4">
        <table className="w-full border-collapse">
          <thead>
            <tr className="border-b-2 border-gray-300">
              <th className="px-3 py-2 text-left text-sm font-semibold"></th>

              {/* Beginning Balance */}
              <th className="px-3 py-2 text-right text-sm font-semibold">
                <div className="flex items-center justify-end gap-1">
                  <span>Beginning Balance</span>
                  {renderHeaderValidationIcon("TotalProfitSharingBalance")}
                </div>
              </th>

              {/* Contributions */}
              <th className="px-3 py-2 text-right text-sm font-semibold">
                <div className="flex items-center justify-end gap-1">
                  <span>Contributions</span>
                </div>
              </th>

              {/* Earnings */}
              <th className="px-3 py-2 text-right text-sm font-semibold">
                <div className="flex items-center justify-end gap-1">
                  <span>Earnings</span>
                </div>
              </th>

              {/* Earnings2 */}
              <th className="px-3 py-2 text-right text-sm font-semibold">Earnings2</th>

              {/* Forfeitures */}
              <th className="px-3 py-2 text-right text-sm font-semibold">
                <div className="flex items-center justify-end gap-1">
                  <span>Forfeitures</span>
                </div>
              </th>

              {/* Distributions */}
              <th className="px-3 py-2 text-right text-sm font-semibold">
                <div className="flex items-center justify-end gap-1">
                  <span>Distributions</span>
                  {renderHeaderValidationIcon(["DistributionTotals","QPAY129_DistributionTotals"])}
                </div>
              </th>

              {/* Military/Paid Allocation */}
              <th className="px-3 py-2 text-right text-sm font-semibold">
                <div className="flex items-center justify-end gap-1">
                  <span>Military/Paid Allocation</span>
                  {renderHeaderValidationIcon("NetAllocTransfer")}
                </div>
              </th>

              {/* Ending Balance */}
              <th className="px-3 py-2 text-right text-sm font-semibold">Ending Balance</th>
            </tr>
          </thead>
          <tbody>
            {/* Total Row */}
            <tr className="border-b border-gray-200">
              <td className="px-3 py-2 text-left font-medium">Total</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.beginningBalance || 0)}</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.totalContribution || 0)}</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.earnings || 0)}</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.earnings2 || 0)}</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.forfeiture || 0)}</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.distributions || 0)}</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.military || 0)}</td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.endingBalance || 0)}</td>
            </tr>

            {/* Allocation Row */}
            <tr className="border-b border-gray-200">
              <td className="px-3 py-2 text-left font-medium">Allocation</td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.allocations || 0)}</td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.maxPointsTotal || 0)}</td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right">{numberToCurrency(totals.paidAllocations || 0)}</td>
              <td className="px-3 py-2 text-right">
                {numberToCurrency((totals.allocations || 0) + (totals.paidAllocations || 0))}
              </td>
            </tr>

            {/* Point Row */}
            <tr className="border-b border-gray-200">
              <td className="px-3 py-2 text-left font-medium">Point</td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right">
                {renderHeaderValidationIcon("TotalForfeitPoints")}&nbsp;
                {formatNumberWithComma(totals.contributionPoints || 0)}
              </td>
              <td className="px-3 py-2 text-right">
                {renderHeaderValidationIcon("TotalEarningPoints")}&nbsp;
                {formatNumberWithComma(totals.earningPoints || 0)}
              </td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right"></td>
              <td className="px-3 py-2 text-right"></td>
            </tr>
          </tbody>
        </table>
      </div>

      {/* Validation Popups - positioned fixed and centered on screen */}

      {/* Beginning Balance Validation Popup */}
      {openValidationField === "TotalProfitSharingBalance" && getFieldValidation("TotalProfitSharingBalance") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[350px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              Beginning Balance
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                  <td className="border-b border-gray-100 px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("TotalProfitSharingBalance")?.currentValue || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("TotalProfitSharingBalance")?.expectedValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Contributions Validation Popup */}
      {openValidationField === "TotalContributions" && getFieldValidation("TotalContributions") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[350px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              Contributions
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                  <td className="border-b border-gray-100 px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("PAY443.TotalContributions")?.currentValue || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("PAY443.TotalContributions")?.expectedValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Earnings Validation Popup */}
      {openValidationField === "TotalEarnings" && getFieldValidation("TotalEarnings") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[350px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              Earnings
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                  <td className="border-b border-gray-100 px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("PAY443.TotalEarnings")?.currentValue || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("PAY443.TotalEarnings")?.expectedValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Forfeitures Validation Popup */}
      {openValidationField === "TotalForfeitures" && getFieldValidation("TotalForfeitures") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[350px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              Forfeitures
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                  <td className="border-b border-gray-100 px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("PAY443.TotalForfeitures")?.currentValue || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("PAY443.TotalForfeitures")?.expectedValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Distributions Validation Popup */}
      {openValidationField === "DistributionTotals" && getFieldValidation("DistributionTotals") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[350px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              Distributions
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                  <td className="border-b border-gray-100 px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("DistributionTotals")?.currentValue || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("DistributionTotals")?.expectedValue || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">QPAY129 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {numberToCurrency(getFieldValidation("QPAY129_DistributionTotals")?.expectedValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ALLOC/PAID ALLOC Transfer Balance Validation Popup */}
      {openValidationField === "NetAllocTransfer" && getFieldValidation("NetAllocTransfer") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[400px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              ALLOC/PAID ALLOC Transfer Balance
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Field</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                {getFieldValidation("IncomingAllocations") && (
                  <tr>
                    <td className="border-b border-gray-100 px-2 py-1 text-left">Incoming (ALLOC - code 6)</td>
                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                      {numberToCurrency(getFieldValidation("IncomingAllocations")?.currentValue || 0)}
                    </td>
                  </tr>
                )}
                {getFieldValidation("OutgoingAllocations") && (
                  <tr>
                    <td className="border-b border-gray-100 px-2 py-1 text-left">Outgoing (PAID ALLOC - code 5)</td>
                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                      {numberToCurrency(getFieldValidation("OutgoingAllocations")?.currentValue || 0)}
                    </td>
                  </tr>
                )}
                <tr className="font-semibold">
                  <td className="px-2 py-1 text-left">Net Transfer (Should be $0.00)</td>
                  <td
                    className={`px-2 py-1 text-right ${getFieldValidation("NetAllocTransfer")?.isValid ? "text-green-600" : "text-orange-600"}`}>
                    {numberToCurrency(getFieldValidation("NetAllocTransfer")?.currentValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
            {getFieldValidation("NetAllocTransfer")?.message && (
              <div className="mt-2 rounded bg-gray-50 p-2 text-sm">
                <strong>Note:</strong> {getFieldValidation("NetAllocTransfer")?.message}
              </div>
            )}
          </div>
        </div>
      )}

      {/* Forfeiture points Validation Popup */}
      {openValidationField === "TotalForfeitPoints" && getFieldValidation("TotalForfeitPoints") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[350px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              Contribution Points
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                  <td className="border-b border-gray-100 px-2 py-1 text-right">
                    {formatNumberWithComma(totals.contributionPoints || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {formatNumberWithComma(getFieldValidation("TotalForfeitPoints")?.expectedValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Forfeiture points Validation Popup */}
      {openValidationField === "TotalEarningPoints" && getFieldValidation("TotalEarningPoints") && (
        <div className="fixed left-1/2 top-1/2 z-[1000] max-h-[300px] w-[350px] -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg">
          <div className="p-2 px-4 pb-4">
            <Typography
              variant="subtitle2"
              sx={{ p: 1 }}>
              Earning Points
            </Typography>
            <table className="w-full border-collapse text-[0.95rem]">
              <thead>
                <tr>
                  <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                  <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                  <td className="border-b border-gray-100 px-2 py-1 text-right">
                    {formatNumberWithComma(totals.earningPoints || 0)}
                  </td>
                </tr>
                <tr>
                  <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                  <td className="px-2 py-1 text-right">
                    {formatNumberWithComma(getFieldValidation("TotalEarningPoints")?.expectedValue || 0)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}
    </>
  );
};
