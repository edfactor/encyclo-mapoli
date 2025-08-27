import React, { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { ImpersonationRoles } from "reduxstore/types";

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: ImpersonationRoles | ImpersonationRoles[];
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, requiredRoles }) => {
  const localStorageImpersonating: string[] = JSON.parse(localStorage.getItem("impersonatingRoles") || "[]");
  const navigate = useNavigate();
  const location = useLocation();

  if (!requiredRoles) {
    return <>{children}</>;
  }

  const rolesArray = Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles];

  const hasRequiredRole = rolesArray.some((role) => localStorageImpersonating.includes(role));

  console.log("ProtectedRoute - hasRequiredRole:", hasRequiredRole);

  useEffect(() => {
    if (!hasRequiredRole) {
      const rolesList = rolesArray.join(", ");
      const searchParams = new URLSearchParams({
        requiredRoles: rolesList,
        page: location.pathname,
        reason: "role_restricted"
      });
      navigate(`/unauthorized?${searchParams.toString()}`, { replace: true });
    }
  }, [hasRequiredRole, rolesArray, location.pathname, navigate]);

  return hasRequiredRole ? <>{children}</> : null;
};

export default ProtectedRoute;
