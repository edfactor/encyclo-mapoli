import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { RootState } from "reduxstore/store";
import { ImpersonationRoles } from "reduxstore/types";

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: ImpersonationRoles | ImpersonationRoles[];
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, requiredRoles }) => {
  const { impersonating } = useSelector((state: RootState) => state.security);
  const localStorageImpersonating = localStorage.getItem("impersonatingRole");
  const navigate = useNavigate();
  const location = useLocation();

  if (!requiredRoles) {
    return <>{children}</>;
  }

  const rolesArray = Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles];

  const hasRequiredRole = rolesArray.some((role) => impersonating === role || localStorageImpersonating === role);

  useEffect(() => {
    if (!hasRequiredRole) {
      const rolesList = rolesArray.join(", ");
      const searchParams = new URLSearchParams({
        requiredRoles: rolesList,
        page: location.pathname
      });
      navigate(`/unauthorized?${searchParams.toString()}`, { replace: true });
    }
  }, [hasRequiredRole, rolesArray, location.pathname, navigate]);

  return hasRequiredRole ? <>{children}</> : null;
};

export default ProtectedRoute;
