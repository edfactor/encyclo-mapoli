import { ChevronLeft, Close, ExpandLess, ExpandMore } from "@mui/icons-material";
import {
  Alert,
  Box,
  Chip,
  Collapse,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  SelectChangeEvent,
  SvgIcon,
  SvgIconProps,
  Typography
} from "@mui/material";
import React, { FC, useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { clearActiveSubMenu, closeDrawer, openDrawer, setActiveSubMenu } from "reduxstore/slices/generalSlice";
import {
  checkDecemberParamsAndGridsProfitYears,
  checkFiscalCloseParamsAndGridsProfitYears,
  setSelectedProfitYearForDecemberActivities,
  setSelectedProfitYearForFiscalClose
} from "reduxstore/slices/yearsEndSlice";
import { drawerTitle, menuLevels } from "../../MenuData";
import { drawerClosedWidth, drawerOpenWidth, MENU_LABELS } from "../../constants";
import ProfitYearSelector from "components/ProfitYearSelector/ProfitYearSelector";
import { RootState } from "reduxstore/store";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { ICommon } from "smart-ui-library";
import { NavigationResponseDto } from "reduxstore/types";
import { setCurrentNavigationId } from "reduxstore/slices/navigationSlice";

// Define the highlight color as a constant
const HIGHLIGHT_COLOR = "#0258A5";

const SidebarIcon = (props: SvgIconProps) => (
  <SvgIcon
    {...props}
    viewBox="0 0 16 16"
    sx={{ fontSize: 20 }}>
    <path d="M6.823 7.823a.25.25 0 0 1 0 .354l-2.396 2.396A.25.25 0 0 1 4 10.396V5.604a.25.25 0 0 1 .427-.177Z" />
    <path d="M1.75 0h12.5C15.216 0 16 .784 16 1.75v12.5A1.75 1.75 0 0 1 14.25 16H1.75A1.75 1.75 0 0 1 0 14.25V1.75C0 .784.784 0 1.75 0ZM1.5 1.75v12.5c0 .138.112.25.25.25H9.5v-13H1.75a.25.25 0 0 0-.25.25ZM11 14.5h3.25a.25.25 0 0 0 .25-.25V1.75a.25.25 0 0 0-.25-.25H11Z" />
  </SvgIcon>
);

export interface PSDrawerProps extends ICommon {
  navigationData?: NavigationResponseDto;
}

const PSDrawer: FC<PSDrawerProps> = ({ navigationData }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { isDrawerOpen: drawerOpen, activeSubmenu } = useSelector((state: RootState) => state.general);
  const { selectedProfitYearForDecemberActivities, selectedProfitYearForFiscalClose } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [expandedLevels, setExpandedLevels] = useState<{ [key: string]: boolean }>({});
  const [selectedLevel, setSelectedLevel] = useState<string | null>(null);
  const [showDecemberBanner, setShowDecemberBanner] = useState(true);
  const [showFiscalCloseBanner, setShowFiscalCloseBanner] = useState(true);
  const currentPath = location.pathname; // Directly use location.pathname instead of state
  const pathRef = useRef(currentPath); // Use ref to track previous path
  const dispatch = useDispatch();
  const profitYear = useDecemberFlowProfitYear();
  const fiscalFlowProfitYear = useFiscalCloseProfitYear();
  const expandedOnceRef = useRef<{ [key: string]: boolean }>({});

  const hasThirdLevel = (level: string, secondLevel: string) => {
    const hasSome = menuLevels(navigationData).some(
      (l) => l.mainTitle === level && l.topPage.some((y) => y.topTitle === secondLevel && y.subPages.length > 0)
    );
    return hasSome;
  };

  // The format of the captions is New Report Name (Legacy Name)
  const getNewReportName = (caption: string): string => {
    if (!caption.includes("(")) {
      return caption;
    }
    return caption.split(" (")[0];
  };

  // The format of the captions is New Report Name (Legacy Name)
  const getLegacyReportName = (caption: string): string => {
    const legacyReportName = caption.split(/[()]/)[1];
    if (legacyReportName === caption) {
      return "";
    }
    return legacyReportName;
  };

  const handleDrawerToggle = () => {
    if (drawerOpen) {
      dispatch(closeDrawer());
    } else {
      dispatch(openDrawer());
    }
    // Close all levels and clear active submenu when drawer closes
    if (drawerOpen) {
      setExpandedLevels({});
      setSelectedLevel(null);
      dispatch(clearActiveSubMenu());
    }
  };

  const handleLevelClick = (level: string) => {
    dispatch(setActiveSubMenu(level));
  };

  const handleBackToMain = () => {
    dispatch(clearActiveSubMenu());
    setExpandedLevels({});
    setSelectedLevel(null);
  };

  const settingCurrentNavigation = (navigationId?: number) => {
    if (navigationId) {
      localStorage.setItem("navigationId", navigationId.toString());
    }
  };

  const handlePageClick = (route: string, navigationId: number | null) => {
    settingCurrentNavigation(navigationId ?? undefined);
    navigate(`/${route}`);
    console.log(`Top page Navigating to ${route}`);
  };

  const handleSubPageClick = (subRoute: string, navigationId: number | null) => {
    settingCurrentNavigation(navigationId ?? undefined);
    navigate(`/${subRoute}`);
    console.log(`Sub page Navigating to ${subRoute}`);
  };

  const handleDecemberProfitYearChange = (event: SelectChangeEvent) => {
    dispatch(setSelectedProfitYearForDecemberActivities(Number(event.target.value)));
    dispatch(checkDecemberParamsAndGridsProfitYears(Number(event.target.value)));
  };

  const handleFiscalCloseProfitYearChange = (event: SelectChangeEvent) => {
    dispatch(setSelectedProfitYearForFiscalClose(Number(event.target.value)));
    dispatch(checkFiscalCloseParamsAndGridsProfitYears(Number(event.target.value)));
  };

  // Check if route is active
  const isRouteActive = (route: string): boolean => {
    if (!route) return false;
    return currentPath === `/${route}`;
  };

  // Auto-expand menu containing active route only when path changes
  useEffect(() => {
    // Only run this effect if the path has changed
    if (pathRef.current !== currentPath) {
      pathRef.current = currentPath; // Update ref to current path

      if (activeSubmenu) {
        const menuLevel = menuLevels(navigationData).find((l) => l.mainTitle === activeSubmenu);
        if (menuLevel) {
          // Find top-level page containing the current route
          const activePage = menuLevel.topPage.find(
            (page) =>
              (page.topRoute && isRouteActive(page.topRoute)) ||
              page.subPages.some((subPage) => subPage.subRoute && isRouteActive(subPage.subRoute))
          );

          if (activePage && !expandedOnceRef.current[activePage.topTitle]) {
            // Only expand if we haven't expanded this section before
            setExpandedLevels((prev) => ({ ...prev, [activePage.topTitle]: true }));
            expandedOnceRef.current[activePage.topTitle] = true;
          }
        }
      }
    }
  }, [currentPath, activeSubmenu, navigationData, isRouteActive]);

  return (
    <>
      {/* Toggle Button */}
      <Box
        key={"TitleBarAboveDrawer"}
        sx={{
          position: "fixed",
          left: drawerOpen ? "16px" : "12px",
          top: "179px",
          zIndex: (theme) => theme.zIndex.drawer + 100,
          display: "flex",
          alignItems: "center",
          gap: 1,
          justifyContent: "space-between",
          width: drawerOpen ? "300px" : "auto",
          transition: (theme) =>
            theme.transitions.create("left", {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen
            })
        }}>
        {drawerOpen && (
          <Typography
            variant="h6"
            sx={{
              color: (theme) => theme.palette.text.primary,
              whiteSpace: "nowrap",
              fontWeight: "bold"
            }}>
            {drawerTitle}
          </Typography>
        )}
        <IconButton
          onClick={handleDrawerToggle}
          sx={{
            backgroundColor: "transparent",
            "&:hover": {
              backgroundColor: (theme) => theme.palette.action.hover
            }
          }}>
          <SidebarIcon />
        </IconButton>
      </Box>
      <Drawer
        variant="permanent"
        id="DrawerItself"
        sx={{
          width: drawerOpen ? drawerOpenWidth : drawerClosedWidth,
          display: drawerOpen ? "block" : "none",
          flexShrink: 0,

          // THIS IS WHAT STYLES THE AQUAMARINE COLORED SPACE
          "& .MuiDrawer-paper": {
            width: drawerOpen ? drawerOpenWidth : drawerClosedWidth,

            boxSizing: "border-box",
            overflowX: "hidden",
            "& > *": {
              overflowX: "hidden"
            },
            transition: "all 225ms"
          }
        }}>
        <Box
          id="DrawerContent"
          sx={{
            mt: "215px",
            overflowY: "hidden",
            "&:hover": {
              overflowY: "auto"
            },
            "&::-webkit-scrollbar": {
              width: "8px"
            },
            "&::-webkit-scrollbar-thumb": {
              backgroundColor: "transparent",
              borderRadius: "4px"
            },
            "&:hover::-webkit-scrollbar-thumb": {
              backgroundColor: (theme) => theme.palette.grey[300]
            }
          }}>
          {activeSubmenu ? (
            <>
              <ListItemButton
                onClick={handleBackToMain}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 1,
                  borderBottom: 1,
                  borderColor: "divider",
                  "&:hover": {
                    backgroundColor: (theme) => theme.palette.action.hover
                  }
                }}>
                <ChevronLeft />
                <Typography
                  variant="body2"
                  sx={{ fontWeight: "bold" }}>
                  {activeSubmenu}
                </Typography>
              </ListItemButton>
              {activeSubmenu === MENU_LABELS.DECEMBER_ACTIVITIES && (
                <div>
                  {showDecemberBanner && (
                    <Alert
                      severity="info"
                      sx={{ fontSize: "0.8rem" }} // Shrink font size
                      action={
                        <IconButton
                          aria-label="close"
                          color="inherit"
                          size="small"
                          onClick={() => setShowDecemberBanner(false)}>
                          <Close fontSize="inherit" />
                        </IconButton>
                      }>
                      Sets accounting calendar year
                    </Alert>
                  )}
                  <div style={{ padding: "24px" }}>
                    <ProfitYearSelector
                      selectedProfitYear={selectedProfitYearForDecemberActivities}
                      handleChange={handleDecemberProfitYearChange}
                      defaultValue={profitYear?.toString()}
                    />
                  </div>
                </div>
              )}

              {activeSubmenu === MENU_LABELS.FISCAL_CLOSE && (
                <div>
                  {showFiscalCloseBanner && (
                    <Alert
                      severity="info"
                      action={
                        <IconButton
                          aria-label="close"
                          color="inherit"
                          size="small"
                          onClick={() => setShowFiscalCloseBanner(false)}>
                          <Close fontSize="inherit" />
                        </IconButton>
                      }>
                      Sets accounting calendar year
                    </Alert>
                  )}
                  <div style={{ padding: "24px" }}>
                    <ProfitYearSelector
                      selectedProfitYear={selectedProfitYearForFiscalClose}
                      handleChange={handleFiscalCloseProfitYearChange}
                      defaultValue={fiscalFlowProfitYear?.toString()}
                    />
                  </div>
                </div>
              )}
              <List>
                {menuLevels(navigationData)
                  .find((l) => l.mainTitle === activeSubmenu)
                  ?.topPage.filter((page) => !page.disabled)
                  .map((page, index) => {
                    const isTopPageActive = page.topRoute && isRouteActive(page.topRoute);
                    const hasActiveSubPage = page.subPages.some(
                      (subPage) => subPage.subRoute && isRouteActive(subPage.subRoute)
                    );

                    return (
                      <React.Fragment key={page.topTitle + index}>
                        {/* Need to decide if this is a link or set of menus */}
                        {hasThirdLevel(activeSubmenu, page.topTitle) ? (
                          <>
                            <ListItemButton
                              key={"" + index + page.topTitle}
                              onClick={() => {
                                const newExpandedLevels = { ...expandedLevels };
                                newExpandedLevels[page.topTitle] = !expandedLevels[page.topTitle];
                                setExpandedLevels(newExpandedLevels);
                              }}
                              sx={{
                                pl: 2,
                                display: "flex",
                                justifyContent: "space-between",
                                py: 1.75,
                                minHeight: 0,
                                backgroundColor: isTopPageActive // Only check direct match
                                  ? `${HIGHLIGHT_COLOR}15`
                                  : "transparent",
                                borderLeft: isTopPageActive // Only check direct match
                                  ? `4px solid ${HIGHLIGHT_COLOR}`
                                  : "4px solid transparent",
                                "&:hover": {
                                  backgroundColor: isTopPageActive
                                    ? `${HIGHLIGHT_COLOR}25`
                                    : (theme) => theme.palette.action.hover
                                }
                              }}>
                              <Box sx={{ display: "flex", alignItems: "center" }}>
                                <ListItemText
                                  primary={getNewReportName(page.topTitle)}
                                  secondary={getLegacyReportName(page.topTitle)}
                                  secondaryTypographyProps={{
                                    sx: {
                                      fontSize: "0.75rem",
                                      color: (theme) => theme.palette.text.secondary
                                    }
                                  }}
                                  sx={{
                                    margin: 0,
                                    "& .MuiTypography-root": {
                                      fontSize: "0.875rem",
                                      fontWeight: hasActiveSubPage ? "bold" : "normal",
                                      color: hasActiveSubPage ? HIGHLIGHT_COLOR : "inherit"
                                    }
                                  }}
                                />
                              </Box>

                              {expandedLevels[page.topTitle] ? <ExpandLess /> : <ExpandMore />}
                            </ListItemButton>
                            <Collapse
                              in={expandedLevels[page.topTitle]}
                              timeout="auto"
                              unmountOnExit>
                              <List
                                component="div"
                                disablePadding>
                                {page.subPages
                                  .filter((page) => !page.disabled)
                                  .map((subPage) => {
                                    const isSubPageActive = subPage.subRoute && isRouteActive(subPage.subRoute);

                                    return (
                                      <ListItemButton
                                        key={page.topTitle + subPage.subTitle}
                                        sx={{
                                          pl: 4,
                                          display: "flex",
                                          justifyContent: "space-between",
                                          py: 1,
                                          minHeight: 0,
                                          backgroundColor: isSubPageActive ? `${HIGHLIGHT_COLOR}15` : "transparent",
                                          borderLeft: isSubPageActive
                                            ? `4px solid ${HIGHLIGHT_COLOR}`
                                            : "4px solid transparent",
                                          "&:hover": {
                                            backgroundColor: isSubPageActive
                                              ? `${HIGHLIGHT_COLOR}25`
                                              : (theme) => theme.palette.action.hover
                                          }
                                        }}
                                        onClick={() =>
                                          handleSubPageClick(subPage.subRoute ?? "", subPage.navigationId ?? null)
                                        }>
                                        <Box sx={{ display: "flex", alignItems: "center" }}>
                                          <ListItemText
                                            primary={getNewReportName(subPage.subTitle || "")}
                                            secondary={getLegacyReportName(subPage.subTitle || "")}
                                            secondaryTypographyProps={{
                                              sx: {
                                                fontSize: "0.75rem",
                                                color: (theme) => theme.palette.text.secondary
                                              }
                                            }}
                                            primaryTypographyProps={{
                                              variant: "body2",
                                              sx: {
                                                fontWeight: isSubPageActive ? "bold" : "normal",
                                                color: isSubPageActive ? HIGHLIGHT_COLOR : "inherit"
                                              }
                                            }}
                                            sx={{
                                              margin: 0
                                            }}
                                          />
                                        </Box>
                                        <Box sx={{ display: "flex", alignItems: "right", gap: 1 }}>
                                          <Chip
                                            variant="outlined"
                                            label={subPage.statusName}
                                            className="border-gray-700 text-gray-700"
                                            size="small"
                                            sx={{
                                              backgroundColor:
                                                subPage.statusName === "In Progress"
                                                  ? "#E6F4EA" // subtle green
                                                  : subPage.statusName === "On Hold"
                                                    ? "#FFF9E5" // subtle yellow
                                                    : undefined,
                                              color:
                                                subPage.statusName === "In Progress"
                                                  ? "#22543D" // dark green text
                                                  : subPage.statusName === "On Hold"
                                                    ? "#8D6B04" // medium/dark yellow text
                                                    : undefined,
                                              fontWeight: 500
                                            }}
                                          />
                                        </Box>
                                      </ListItemButton>
                                    );
                                  })}
                              </List>
                            </Collapse>
                          </>
                        ) : (
                          <>
                            <ListItemButton
                              key={page.topTitle}
                              onClick={() => handlePageClick(page.topRoute ?? "", page.navigationId ?? null)}
                              sx={{
                                pl: 2,
                                display: "flex",
                                justifyContent: "space-between",
                                py: 1,
                                minHeight: 0,
                                backgroundColor: isTopPageActive ? `${HIGHLIGHT_COLOR}15` : "transparent",
                                borderLeft: isTopPageActive ? `4px solid ${HIGHLIGHT_COLOR}` : "4px solid transparent",
                                "&:hover": {
                                  backgroundColor: isTopPageActive
                                    ? `${HIGHLIGHT_COLOR}25`
                                    : (theme) => theme.palette.action.hover
                                }
                              }}>
                              <Box sx={{ display: "flex", alignItems: "center" }}>
                                <ListItemText
                                  primary={getNewReportName(page.topTitle || "")}
                                  secondary={getLegacyReportName(page.topTitle || "")}
                                  secondaryTypographyProps={{
                                    sx: {
                                      fontSize: "0.75rem",
                                      color: (theme) => theme.palette.text.secondary
                                    }
                                  }}
                                  sx={{
                                    margin: 0,
                                    "& .MuiTypography-root": {
                                      fontSize: "0.875rem",
                                      fontWeight: isTopPageActive ? "bold" : "normal"
                                    }
                                  }}
                                />
                              </Box>
                              <Box sx={{ display: "flex", alignItems: "right", gap: 1 }}>
                                <Chip
                                  variant="outlined"
                                  label={page.statusName}
                                  className="border-gray-700 text-gray-700"
                                  size="small"
                                  sx={{
                                    backgroundColor:
                                      page.statusName === "In Progress"
                                        ? "#E6F4EA" // subtle green
                                        : page.statusName === "On Hold"
                                          ? "#FFF9E5" // subtle yellow
                                          : undefined,
                                    color:
                                      page.statusName === "In Progress"
                                        ? "#22543D" // dark green text
                                        : page.statusName === "On Hold"
                                          ? "#8D6B04" // medium/dark yellow text
                                          : undefined,
                                    fontWeight: 500
                                  }}
                                />
                              </Box>
                            </ListItemButton>
                          </>
                        )}
                      </React.Fragment>
                    );
                  })}
              </List>
            </>
          ) : (
            <>
              <Divider sx={{ mb: 1 }} />
              <List>
                {menuLevels(navigationData).map((level, index) => {
                  // Check if this top-level menu contains the active route
                  const hasActiveRoute = level.topPage.some(
                    (page) =>
                      (page.topRoute && isRouteActive(page.topRoute)) ||
                      page.subPages.some((subPage) => subPage.subRoute && isRouteActive(subPage.subRoute))
                  );

                  return (
                    <React.Fragment key={level.mainTitle + index}>
                      <ListItem disablePadding>
                        <ListItemButton
                          onClick={() => handleLevelClick(level.mainTitle)}
                          sx={{
                            backgroundColor: "transparent", // No highlighting based on children
                            borderLeft: "4px solid transparent",
                            "&:hover": {
                              backgroundColor: (theme) => theme.palette.action.hover
                            },
                            "& .MuiListItemText-primary": {
                              color: (theme) => theme.palette.text.primary,
                              fontWeight: "normal"
                            },
                            display: "flex",
                            justifyContent: "space-between",
                            alignItems: "center"
                          }}>
                          {drawerOpen && (
                            <>
                              <ListItemText
                                primary={level.mainTitle}
                                secondary={`${level.topPage.filter((page) => page.statusName?.toLowerCase() === "complete").length} of ${level.topPage.length} completed`}
                                primaryTypographyProps={{
                                  variant: "h6"
                                }}
                                secondaryTypographyProps={{
                                  sx: {
                                    variant: "body2",
                                    color: hasActiveRoute
                                      ? (theme) => theme.palette.text.secondary
                                      : (theme) => theme.palette.text.secondary,
                                    fontSize: "0.75rem"
                                  }
                                }}
                              />
                            </>
                          )}
                        </ListItemButton>
                      </ListItem>
                    </React.Fragment>
                  );
                })}
              </List>
            </>
          )}
        </Box>
      </Drawer>
    </>
  );
};

export default PSDrawer;
