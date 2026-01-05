import { CrossReferenceValidation } from "@/types/validation";
import Close from "@mui/icons-material/Close";
import { Typography } from "@mui/material";
import React from "react";

export interface ValidationRow {
  label: string;
  valueGetter: () => React.ReactNode;
  condition?: () => boolean;
  getRowClass?: () => string;
  getValueClass?: () => string;
}

export interface ValidationField {
  fieldKey: string;
  title: string;
  headers: string[];
  rows: ValidationRow[];
  messageGetter?: () => string | null | undefined;
  popupClassName?: string;
}

interface ValidationPopupProps {
  field: ValidationField;
  openField: string | null;
  getFieldValidation: (fieldKey: string) => Partial<CrossReferenceValidation> | null;
  onClose: () => void;
}

const ValidationPopup: React.FC<ValidationPopupProps> = ({ field, openField, getFieldValidation, onClose }) => {
  if (openField !== field.fieldKey) return null;
  if (!getFieldValidation(field.fieldKey)) return null;

  const popupWidth = field.popupClassName ?? "w-[350px]";

  return (
    <div className={`fixed left-1/2 top-1/2 z-[1000] max-h-[300px] ${popupWidth} -translate-x-1/2 -translate-y-1/2 overflow-auto rounded border border-gray-300 bg-white shadow-lg`}>
      <div className="p-2 px-4 pb-4">
        <div className="flex items-center justify-between">
          <Typography variant="subtitle2" sx={{ p: 1 }}>{field.title}</Typography>
          <div className="inline-block cursor-pointer" onClick={onClose}>
            <Close fontSize="small" />
          </div>
        </div>

        <table className="w-full border-collapse text-[0.95rem]">
          <thead>
            <tr>
              {field.headers.map((header) => (
                <th key={header} className="border-b border-gray-300 px-2 py-1 text-left font-semibold">{header}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {field.rows.map((row, index) => {
              if (row.condition && !row.condition()) return null;
              const rowClass = row.getRowClass ? row.getRowClass() : "";
              const valueClass = row.getValueClass ? row.getValueClass() : "";

              return (
                <tr key={index} className={rowClass}>
                  <td className={`px-2 py-1 text-left${index < field.rows.length ? " border-b border-gray-100" : ""}`}>
                    {row.label}
                  </td>
                  <td className={`px-2 py-1 text-right ${valueClass}`}>
                    {row.valueGetter()}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>

        {field.messageGetter && field.messageGetter() && (
          <div className="mt-2 rounded bg-gray-50 p-2 text-sm">
            <strong>Note:</strong> {field.messageGetter()}
          </div>
        )}
      </div>
    </div>
  );
};

export default ValidationPopup;
