// cellRenderers.ts (or any shared utility file)
import type { ICellRendererParams } from 'ag-grid-community';
import Link from '@mui/material/Link'; // If you're using MUI Link

export function viewBadgeRenderer({ value }: ICellRendererParams) {
  if (!value) return null;

  const safeValue = value.toString(); // Ensure it's a string

  return (
    <Link
      style={{ height: '20px', textDecoration: 'underline', textTransform: 'none' }}
      href={`/master-inquiry/${safeValue}`}
    >
      {safeValue}
    </Link>
  );
}

