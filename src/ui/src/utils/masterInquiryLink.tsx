import { Button } from "@mui/material";
import Link from "@mui/material/Link";
import { JSX } from "react";

export function viewBadgeLinkRenderer(
  badgeNumber: number,
  navigateFunction?: (path: string) => void
): JSX.Element | number;
// eslint-disable-next-line no-redeclare
export function viewBadgeLinkRenderer(
  badgeNumber: number,
  psnSuffix: number,
  navigateFunction?: (path: string) => void
): JSX.Element | number;
// eslint-disable-next-line no-redeclare
export function viewBadgeLinkRenderer(
  badgeNumber: number,
  param2?: number | ((path: string) => void),
  param3?: (path: string) => void
): JSX.Element | number {
  if (!badgeNumber || badgeNumber > 99999999999) return badgeNumber;

  let psnSuffix = 0;
  let navigateFunction: ((path: string) => void) | undefined = undefined;

  // Determine which parameters are which
  if (typeof param2 === "function") {
    navigateFunction = param2;
  } else if (typeof param2 === "number") {
    psnSuffix = param2;
    navigateFunction = param3;
  }

  const safeValue = psnSuffix > 0 ? badgeNumber.toString() + psnSuffix.toString() : badgeNumber.toString();
  const displayValue = psnSuffix > 0 ? `${badgeNumber}${psnSuffix}` : badgeNumber;

  if (navigateFunction === undefined) {
    return (
      <Link
        className="solid h-5 normal-case underline"
        href={`/master-inquiry/${safeValue}`}>
        {displayValue}
      </Link>
    );
  } else {
    return (
      <Button
        href={`/master-inquiry/${safeValue}`}
        className="px-0 underline"
        variant="text"
        onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
          e.preventDefault();
          navigateFunction(`/master-inquiry/${safeValue}`);
        }}>
        {displayValue}
      </Button>
    );
  }
}
