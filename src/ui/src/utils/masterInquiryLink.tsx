import { Button } from "@mui/material";
import Link from "@mui/material/Link";

// Some callers want a MUI Link, some want a Button with a react navigate function
export const viewBadgeLinkRenderer = (badgeNumber: number, navigateFunction?: (path: string) => void) => {
  if (!badgeNumber || badgeNumber < 99999 || badgeNumber > 9999999) return "";
  const safeValue = badgeNumber.toString(); // Ens

  if (navigateFunction === undefined) {
    return (
      <Link
        className="h-5 solid underline normal-case"
        href={`/master-inquiry/${safeValue}`}>
        {safeValue}
      </Link>
    );
  } else {
    return (
      badgeNumber && (
        <Button
          variant="text"
          onClick={() => navigateFunction(`/master-inquiry/${safeValue}`)}>
          {safeValue}
        </Button>
      )
    );
  }
};
