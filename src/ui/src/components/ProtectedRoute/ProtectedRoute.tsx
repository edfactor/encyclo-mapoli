import React, { useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { RootState } from "reduxstore/store";
import { ImpersonationRoles } from "reduxstore/types";
import { checkUserGroupsForRole } from "../../utils/roleUtils";

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: ImpersonationRoles | ImpersonationRoles[];
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, requiredRoles }) => {
  const { impersonating, userGroups } = useSelector((state: RootState) => state.security);
  const navigate = useNavigate();
  const location = useLocation();

  const rolesArray = useMemo<ImpersonationRoles[]>(
    () => (requiredRoles ? (Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles]) : []),
    [requiredRoles]
  );
  const hasRequiredRole =
    rolesArray.length > 0 &&
    rolesArray.some(
      (role) =>
        impersonating.includes(role) || checkUserGroupsForRole(userGroups ?? [], role) || userGroups.includes(role)
    );

  useEffect(() => {
    if (!requiredRoles) {
      return;
    }

    if (!hasRequiredRole) {
      const rolesList = rolesArray.join(", ");
      const searchParams = new URLSearchParams({
        requiredRoles: rolesList,
        page: location.pathname,
        reason: "role_restricted"
      });
      navigate(`/unauthorized?${searchParams.toString()}`, { replace: true });
    }
  }, [hasRequiredRole, rolesArray, location.pathname, navigate, requiredRoles]);

  if (!requiredRoles) {
    return <>{children}</>;
  }

  return hasRequiredRole ? <>{children}</> : null;
};

export default ProtectedRoute;
