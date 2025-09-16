import React from "react";
import { NavigationDto } from "reduxstore/types";
import { ApiMessageAlert } from "smart-ui-library";
import usePrerequisiteNavigations from "../hooks/usePrerequisiteNavigations";
import { Messages } from "../utils/messageDictonary";

// Minimal subset to avoid tight coupling to smart-ui-library types
interface MessageUpdateLike {
  key: string;
  message: {
    type: "success" | "error" | "warning";
    title: string;
    message?: string;
  };
}

interface PrerequisiteGuardProps {
  navigationId?: number | null;
  messageTemplate?: MessageUpdateLike;
  /** If not provided, defaults to messageTemplate.key */
  commonKey?: string;
  /** Show ApiMessageAlert inline (default: true) */
  showAlert?: boolean;
  children: (ctx: {
    prerequisitesComplete: boolean;
    incompletePrerequisites: NavigationDto[];
    currentNavigation?: NavigationDto;
  }) => React.ReactNode;
}

/**
 * PrerequisiteGuard centralizes prerequisite message dispatch via usePrerequisiteNavigations
 * and renders ApiMessageAlert for the provided key. It exposes prerequisitesComplete to children
 * so callers can gate actions (e.g., disable Save) without duplicating wiring.
 */
const PrerequisiteGuard: React.FC<PrerequisiteGuardProps> = ({
  navigationId,
  messageTemplate = Messages.ProfitSharePrerequisiteIncomplete as unknown as MessageUpdateLike,
  commonKey,
  showAlert = true,
  children
}) => {
  const resolvedNavigationId =
    navigationId ?? (Number.parseInt(localStorage.getItem("navigationId") ?? "") || undefined);

  const { prerequisitesComplete, incompletePrerequisites, currentNavigation } = usePrerequisiteNavigations(
    resolvedNavigationId,
    {
      messageTemplate: messageTemplate as any
    }
  );

  const keyToUse = commonKey ?? messageTemplate.key;

  return (
    <>
      {showAlert && <ApiMessageAlert commonKey={keyToUse} />}
      {children({ prerequisitesComplete, incompletePrerequisites, currentNavigation })}
    </>
  );
};

export default PrerequisiteGuard;
