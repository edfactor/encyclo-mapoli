import { Button } from "@mui/material";
import Link from "@mui/material/Link";
import { JSX } from "react";

export function viewBadgeLinkRenderer(
  badgeNumber: number,
  navigateFunction?: (path: string) => void
): JSX.Element | number;
export function viewBadgeLinkRenderer(
  badgeNumber: number,
  psnSuffix: number,
  navigateFunction?: (path: string) => void
): JSX.Element | number;
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
        className="h-5 solid underline normal-case"
        href={`/master-inquiry/${safeValue}`}>
        {displayValue}
      </Link>
    );
  } else {
    return (
      <Button
        href={`/master-inquiry/${safeValue}`}
        variant="text"
        onClick={() => navigateFunction(`/master-inquiry/${safeValue}`)}>
        {displayValue}
      </Button>
    );
  }
}
