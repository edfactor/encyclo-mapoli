import { useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { NavigationDto } from "reduxstore/types";
import { setMessage } from "smart-ui-library";
import { NAVIGATION_STATUS } from "../constants";

// Minimal subset of MessageUpdate to avoid tight coupling
interface MessageUpdateLike {
  key: string;
  message: {
    type: "success" | "error"; // warning not currently supported by slice
    title: string;
    message?: string;
  };
}

interface MessageConfig {
  messageTemplate: MessageUpdateLike;
  build?: (template: MessageUpdateLike, incomplete: NavigationDto[], current?: NavigationDto) => MessageUpdateLike;
}

export interface UsePrerequisiteNavigationsResult {
  prerequisitesComplete: boolean;
  incompletePrerequisites: NavigationDto[];
  currentNavigation?: NavigationDto;
}

/**
 * INTERNAL: Reusable hook to evaluate prerequisite navigation completion status.
 *
 * This hook dispatches a message (via setMessage) when prerequisites are incomplete,
 * using the provided message template and key. Prefer using the higher-level
 * PrerequisiteGuard component, which wraps this hook and renders ApiMessageAlert
 * for you.
 *
 * Recommended usage for pages: <PrerequisiteGuard> rather than calling this hook directly.
 * This remains exported for internal composition and testing.
 */
export const usePrerequisiteNavigations = (
  navigationId: number | null | undefined,
  messageConfig?: MessageConfig
): UsePrerequisiteNavigationsResult => {
  const navigationData = useSelector((state: RootState) => state.navigation.navigationData);
  const [prerequisitesComplete, setPrerequisitesComplete] = useState<boolean>(true);
  const [incompletePrerequisites, setIncompletePrerequisites] = useState<NavigationDto[]>([]);
  const [currentNavigation, setCurrentNavigation] = useState<NavigationDto | undefined>(undefined);
  const lastIdsRef = useRef<string>("");
  const dispatch = useDispatch();

  const findNavigationById = (navigationArray: NavigationDto[] | undefined, id?: number): NavigationDto | undefined => {
    if (!navigationArray || id == null) return undefined;
    for (const item of navigationArray) {
      if (item.id === id) return item;
      if (item.items && item.items.length > 0) {
        const found = findNavigationById(item.items, id);
        if (found) return found;
      }
    }
    return undefined;
  };

  useEffect(() => {
    if (!navigationId) return;
    const obj = findNavigationById(navigationData?.navigation, navigationId);
    setCurrentNavigation(obj);
    if (obj && obj.prerequisiteNavigations && obj.prerequisiteNavigations.length > 0) {
      const incomplete = obj.prerequisiteNavigations.filter((p) => p.statusId !== NAVIGATION_STATUS.COMPLETE);
      const allComplete = incomplete.length === 0;
      setPrerequisitesComplete(allComplete);
      setIncompletePrerequisites(incomplete);

      if (!allComplete) {
        const key = incomplete
          .map((i) => i.id)
          .sort()
          .join(",");
        if (key !== lastIdsRef.current && messageConfig) {
          lastIdsRef.current = key;

          // Build the message payload ensuring it has the shape { key, message: { type, title, message } }
          let built: MessageUpdateLike = messageConfig.messageTemplate;
          if (messageConfig.build) {
            built = messageConfig.build(messageConfig.messageTemplate, incomplete, obj);
          } else {
            const first = incomplete[0];
            const text =
              incomplete.length === 1
                ? `${first.title} is '${first.statusName ?? "Not Complete"}' and must be 'Complete' before saving.`
                : `Prerequisites incomplete: ${incomplete
                    .map((i) => `${i.title} (${i.statusName ?? "Not Complete"})`)
                    .join(", ")}. Each must be 'Complete' before saving.`;

            built = {
              ...messageConfig.messageTemplate,
              message: {
                ...messageConfig.messageTemplate.message,
                message: text
              }
            };
          }

          // Ensure final payload contains `key` and `message` at top level to match setMessage expectations
          const finalPayload = {
            key: built.key,
            message: {
              type: built.message.type,
              title: built.message.title,
              message: built.message.message || ""
            }
          };

          dispatch(setMessage(finalPayload as any));
        }
      }
    } else {
      setPrerequisitesComplete(true);
      setIncompletePrerequisites([]);
    }
  }, [navigationId, navigationData, dispatch]);

  return { prerequisitesComplete, incompletePrerequisites, currentNavigation };
};

export default usePrerequisiteNavigations;
