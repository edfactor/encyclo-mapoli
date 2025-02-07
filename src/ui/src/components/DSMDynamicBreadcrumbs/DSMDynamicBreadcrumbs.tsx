import { Breadcrumbs, Link } from '@mui/material';
import { useNavigate, useLocation, Location } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { getReadablePathName } from 'utils/getReadablePathName';
import { HISTORY_KEY } from '../../constants';
import { BreadcrumbItem } from './DSMBreadcrumbItem';

interface DSMDynamicBreadcrumbsProps {
  separator?: string;
  customItems?: BreadcrumbItem[];
}

// Prevents login from showing up as first breadcrumb after Auth Redirect
const EXCLUDED_PATHS = ['/login', '/login/callback'];

const DSMDynamicBreadcrumbs: React.FC<DSMDynamicBreadcrumbsProps> = ({
  separator = '/',
  customItems
}) => {
  const navigate = useNavigate();
  const location = useLocation();
  const [navigationHistory, setNavigationHistory] = useState<Location[]>([]);

  useEffect(() => {
    if (EXCLUDED_PATHS.some(path => location.pathname.startsWith(path))) {
      return;
    }
    const currentPaths = navigationHistory.map(h => h.pathname);
    
    if (!currentPaths.includes(location.pathname)) {
      const newHistory = [...navigationHistory, location];
      console.log('New history:', newHistory.map(h => h.pathname));
      setNavigationHistory(newHistory);
      localStorage.setItem(HISTORY_KEY, JSON.stringify(newHistory));
    }
  }, [location.pathname]);

  const handleClick = (path: string, index: number) => {
    if (path !== location.pathname) {
      const newHistory = navigationHistory.slice(0, index + 1);
      setNavigationHistory(newHistory);
      localStorage.setItem(HISTORY_KEY, JSON.stringify(newHistory));
      navigate(path);
    }
  };

  const buildBreadcrumbItems = (): BreadcrumbItem[] => {
    if (customItems) return customItems.slice(-3);

    return navigationHistory.slice(-3).map((location, index, array) => ({
      label: getReadablePathName(location.pathname),
      path: location.pathname,
      isActive: index === array.length - 1
    }));
  };

  const items = buildBreadcrumbItems();

  return (
    <Breadcrumbs separator={separator} maxItems={3}>
      {items.map((item, index) => {
        const isCurrentPath = item.path === location.pathname;
        
        if (item.isActive || isCurrentPath) {
          return (
            <div key={index} className="text-dsm-secondary">
              {item.label}
            </div>
          );
        }

        return (
          <Link
            key={index}
            color="inherit"
            underline="hover"
            onClick={() => handleClick(item.path, index)}
          >
            {item.label}
          </Link>
        );
      })}
    </Breadcrumbs>
  );
};

export default DSMDynamicBreadcrumbs;