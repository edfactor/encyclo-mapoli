// cellRenderers.ts (or any shared utility file)
import Link from "@mui/material/Link"; // If you're using MUI Link

export function viewBadgeRenderer(badgeNumber: number) {
  // Badge number must be between 5 and 7 digits
  if (!badgeNumber || badgeNumber < 99999 || badgeNumber > 9999999) return "";

  const safeValue = badgeNumber.toString(); // Ensure it's a string

  return (
    <Link
      className="h-5 solid underline normal-case"
      href={`/master-inquiry/${safeValue}`}>
      {safeValue}
    </Link>
  );
}
