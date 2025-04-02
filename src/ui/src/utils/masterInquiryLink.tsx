import { Button } from "@mui/material";
import Link from "@mui/material/Link";

// Some callers want a MUI Link, some want a Button with a react navigate function
export const viewBadgeLinkRenderer = (badgeNumber: number, psnSuffix?: number, 
                                      navigateFunction?: (path: string) => void) => {
  if (!badgeNumber || badgeNumber > 800000) return badgeNumber;
  if (psnSuffix === undefined) psnSuffix = 0;
  
  const safeValue = psnSuffix > 0 ? badgeNumber.toString() + psnSuffix.toString() : badgeNumber.toString();
  const displayValue = psnSuffix > 0 ? `${badgeNumber}-${psnSuffix}` : badgeNumber;

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
};
