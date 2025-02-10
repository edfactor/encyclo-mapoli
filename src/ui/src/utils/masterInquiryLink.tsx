// cellRenderers.ts (or any shared utility file)
import type { ICellRendererParams } from 'ag-grid-community';
import Link from '@mui/material/Link'; // If you're using MUI Link

export function viewBadgeRenderer({ value }: ICellRendererParams) {
  if (!value) {
    return null; // Return null if there's no value
  }

  return (
    <Link
      style={{ height: '20px', textDecoration: 'underline', textTransform: 'none' }}
  href={`/master-inquiry/${value}`} // MUI Link with `href`
>
  {value}
  </Link>
);
}
