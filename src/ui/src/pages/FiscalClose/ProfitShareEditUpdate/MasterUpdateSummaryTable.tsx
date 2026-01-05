 import { CrossReferenceValidation } from "@/types/validation";
import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { formatNumberWithComma, numberToCurrency } from "smart-ui-library";
import { ProfitShareUpdateTotals } from "../../../reduxstore/types";
import ValidationPopup, { ValidationField } from "./ValidationPopup";

interface MasterUpdateSummaryTableProps {
  totals: ProfitShareUpdateTotals;
  getFieldValidation: (fieldKey: string) => Partial<CrossReferenceValidation> | null;
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

    const validations = [];
    for (const key of keys) {
      const validation = getFieldValidation(key);
      validations.push(validation);
    }
    if (validations.every((v) => v === null)) {
      return null;
    }

    const isValid = validations.every((v) => v !== null && v.isValid);

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

  const ValidationPopupFields: ValidationField[] = [
    { 
      fieldKey: "TotalProfitSharingBalance", 
      title: "Beginning Balance",
      headers: ["Report", "Amount"],
      rows: [
        {
          label: "PAY444 (Current)",
          valueGetter: () => numberToCurrency(getFieldValidation("TotalProfitSharingBalance")?.currentValue || 0)
        },
        {
          label: "PAY443 (Expected)",
          valueGetter: () => numberToCurrency(getFieldValidation("TotalProfitSharingBalance")?.expectedValue || 0)
        }
      ]
    },
    {
      fieldKey: "TotalContributions",
      title: "Contributions",
      headers: ["Report", "Amount"],
      rows: [
        {
          label: "PAY444 (Current)",
          valueGetter: () => numberToCurrency(getFieldValidation("PAY443.TotalContributions")?.currentValue || 0)
        },
        {
          label: "PAY443 (Expected)",
          valueGetter: () => numberToCurrency(getFieldValidation("PAY443.TotalContributions")?.expectedValue || 0)
        }
      ]
    },
    {
      fieldKey: "TotalEarnings",
      title: "Earnings",
      headers: ["Report", "Amount"],
      rows: [
        {
          label: "PAY444 (Current)",
          valueGetter: () => numberToCurrency(getFieldValidation("PAY443.TotalEarnings")?.currentValue || 0)
        },
        {
          label: "PAY443 (Expected)",
          valueGetter: () => numberToCurrency(getFieldValidation("PAY443.TotalEarnings")?.expectedValue || 0)
        }
      ]
    },
    {
      fieldKey: "TotalForfeitures",
      title: "Forfeitures",
      headers: ["Report", "Amount"],
      rows: [
        {
          label: "PAY444 (Current)",
          valueGetter: () => numberToCurrency(getFieldValidation("PAY443.TotalForfeitures")?.currentValue || 0)
        },
        {
          label: "PAY443 (Expected)",
          valueGetter: () => numberToCurrency(getFieldValidation("PAY443.TotalForfeitures")?.expectedValue || 0)
        }
      ]
    },
    {
      fieldKey: "DistributionTotals",
      title: "Distributions",
      headers: ["Report", "Amount"],
      rows: [
        {
          label: "PAY444 (Current)",
          valueGetter: () => numberToCurrency(getFieldValidation("DistributionTotals")?.currentValue || 0)
        },
        {
          label: "PAY443 (Expected)",
          valueGetter: () => numberToCurrency(getFieldValidation("DistributionTotals")?.expectedValue || 0)
        },
        {
          label: "QPAY129 (Expected)",
          valueGetter: () => numberToCurrency(getFieldValidation("QPAY129_DistributionTotals")?.expectedValue || 0)
        }
      ]
    },
    {
      popupClassName: "w-[400px]",
      fieldKey: "NetAllocTransfer",
      title: "ALLOC/PAID ALLOC Transfer Balance",
      headers: ["Field", "Amount"],
      rows: [
        {
          condition: () => getFieldValidation("IncomingAllocations") !== null,
          label: "Incoming (ALLOC - code 6)",
          valueGetter: () => numberToCurrency(getFieldValidation("IncomingAllocations")?.currentValue || 0)
        },
        {
          condition: () => getFieldValidation("OutgoingAllocations") !== null,
          label: "Outgoing (PAID ALLOC - code 5)",
          valueGetter: () => numberToCurrency(getFieldValidation("OutgoingAllocations")?.currentValue || 0)
        },
        {
          getRowClass: () => "font-semibold",
          label: "Net Transfer (Should be $0.00)",
          valueGetter: () => numberToCurrency(getFieldValidation("NetAllocTransfer")?.currentValue || 0),
          getValueClass: () => getFieldValidation("NetAllocTransfer")?.isValid ? "text-green-600" : "text-orange-600",
        }
      ],
      messageGetter: () => getFieldValidation("NetAllocTransfer")?.message
    },
    {
      fieldKey: "TotalForfeitPoints",
      title: "Contribution Points",
      headers: ["Report", "Amount"],
      rows: [
        {
          label: "PAY444 (Current)",
          valueGetter: () => formatNumberWithComma(totals.contributionPoints || 0)
        },
        {
          label: "PAY443 (Expected)",
          valueGetter: () => formatNumberWithComma(getFieldValidation("TotalForfeitPoints")?.expectedValue || 0)
        }
      ]
    },
    {
      fieldKey: "TotalEarningPoints",
      title: "Earning Points",
      headers: ["Report", "Amount"],
      rows: [
        {
          label: "PAY444 (Current)",
          valueGetter: () => formatNumberWithComma(totals.earningPoints || 0)
        },
        {
          label: "PAY443 (Expected)",
          valueGetter: () => formatNumberWithComma(getFieldValidation("TotalEarningPoints")?.expectedValue || 0)
        }
      ]
    }
  ];

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
                  {renderHeaderValidationIcon(["DistributionTotals", "QPAY129_DistributionTotals"])}
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

      {ValidationPopupFields.map((field) => {
        if (openValidationField === field.fieldKey && getFieldValidation(field.fieldKey)) {
          return (
            <ValidationPopup
              key={field.fieldKey}
              field={field}
              openField={openValidationField}
              getFieldValidation={getFieldValidation}
              onClose={() => onValidationToggle(field.fieldKey)}
            />
          );
        }

        return null;
      })}
    </>
  );
};
