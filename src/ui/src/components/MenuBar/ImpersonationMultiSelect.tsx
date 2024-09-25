import { SelectChangeEvent } from "@mui/material";
import MultipleSelectCheckmarks from "components/MultiSelect/MultiSelect";
import { Role } from "Okta/role";

import { useState } from "react";

const ImpersonationMultiSelect = () => {
  const getActiveRoles = () => {
    const key = "SMART-AR_Impersonation=";
    const cookie = document.cookie;
    const impersonationCookie = cookie
      .split(";")
      .map((c) => c.trim())
      .filter((c) => c.startsWith(key))
      .pop();

    if (impersonationCookie) {
      const roleString = impersonationCookie.slice(key.length);
      const isMultiValued = roleString.indexOf("|") > -1;

      if (isMultiValued) {
        return roleString.split("|").filter((v) => v !== Role.IMPERSONATION);
      }

      if (roleString.length > 0) {
        return [roleString];
      }
    }
    return [];
  };

  const getAllRoles = (): string[] => {
    let roles = Object.values(Role) as string[];
    roles = roles.filter((r) => r !== Role.IMPERSONATION);
    return roles;
  };
  const [roles, setRoles] = useState<string[]>(getActiveRoles());
  const allRoles = getAllRoles();

  const handleImpersonationChange = (event: SelectChangeEvent<typeof roles>) => {
    const {
      target: { value }
    } = event;

    const currentRoles = typeof value === "string" ? value.split(",") : value;
    setRoles(currentRoles);
    const cookieRoles = currentRoles.join("|");
    document.cookie = `SMART-AR_Impersonation=${cookieRoles}`;
  };

  const handleClose = () => {
    window.location.reload();
  };

  return (
    <MultipleSelectCheckmarks
      label="Impersonation"
      options={allRoles || []}
      handleChange={handleImpersonationChange}
      handleClose={handleClose}
      value={roles}
    />
  );
};

export default ImpersonationMultiSelect;
