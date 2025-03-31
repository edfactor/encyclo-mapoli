import { ChevronLeft, ExpandLess, ExpandMore } from "@mui/icons-material";
import {
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
  SvgIcon,
  Typography
} from "@mui/material";
import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { clearActiveSubMenu, closeDrawer, openDrawer, setActiveSubMenu } from "reduxstore/slices/generalSlice";
import { drawerTitle, menuLevels } from "../../MenuData";
import { drawerClosedWidth, drawerOpenWidth } from "../../constants";

import { SvgIconProps } from "@mui/material";
import { RootState } from "reduxstore/store";

const SidebarIcon = (props: SvgIconProps) => (
  <SvgIcon
    {...props}
    viewBox="0 0 16 16"
    sx={{ fontSize: 20 }}>
    <path d="M6.823 7.823a.25.25 0 0 1 0 .354l-2.396 2.396A.25.25 0 0 1 4 10.396V5.604a.25.25 0 0 1 .427-.177Z" />
    <path d="M1.75 0h12.5C15.216 0 16 .784 16 1.75v12.5A1.75 1.75 0 0 1 14.25 16H1.75A1.75 1.75 0 0 1 0 14.25V1.75C0 .784.784 0 1.75 0ZM1.5 1.75v12.5c0 .138.112.25.25.25H9.5v-13H1.75a.25.25 0 0 0-.25.25ZM11 14.5h3.25a.25.25 0 0 0 .25-.25V1.75a.25.25 0 0 0-.25-.25H11Z" />
  </SvgIcon>
);

const PSDrawer = () => {
  const navigate = useNavigate();
  const { isDrawerOpen: drawerOpen, activeSubmenu } = useSelector((state: RootState) => state.general);
  const [expandedLevels, setExpandedLevels] = useState<{ [key: string]: boolean }>({});
  const [selectedLevel, setSelectedLevel] = useState<string | null>(null);
  const dispatch = useDispatch();

  const hasThirdLevel = (level: string, secondLevel: string) => {
    const hasSome = menuLevels.some(
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

  const handlePageClick = (route: string) => {
    navigate(`/${route}`);
    console.log(`Top page Navigating to ${route}`);
  };

  const handleSubPageClick = (subRoute: string) => {
    navigate(`/${subRoute}`);
    console.log(`Sub page Navigating to ${subRoute}`);
  };

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
              <List>
                {menuLevels
                  .find((l) => l.mainTitle === activeSubmenu)
                  ?.topPage.map((page, index) => (
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
                              minHeight: 0
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
                                    fontSize: "0.875rem"
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
                              {page.subPages.map((subPage) => (
                                <ListItemButton
                                  key={page.topTitle + subPage.subTitle}
                                  sx={{
                                    pl: 4,
                                    display: "flex",
                                    justifyContent: "space-between",
                                    py: 1,
                                    minHeight: 0
                                  }}
                                  onClick={() => handleSubPageClick(subPage.subRoute ?? "")}>
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
                                        variant: "body2"
                                      }}
                                      sx={{
                                        margin: 0
                                      }}
                                    />
                                  </Box>
                                  <Box sx={{ display: "flex", alignItems: "right", gap: 1 }}>
                                    <Chip
                                      variant="outlined"
                                      label={"Not Started"}
                                      className="text-gray-700 border-gray-700"
                                      size="small"
                                    />
                                  </Box>
                                </ListItemButton>
                              ))}
                            </List>
                          </Collapse>
                        </>
                      ) : (
                        <>
                          <ListItemButton
                            key={page.topTitle}
                            onClick={() => handlePageClick(page.topRoute ?? "")}
                            sx={{
                              pl: 2,
                              display: "flex",
                              justifyContent: "space-between",
                              py: 1,
                              minHeight: 0
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
                                    fontSize: "0.875rem"
                                  }
                                }}
                              />
                            </Box>
                            <Box sx={{ display: "flex", alignItems: "right", gap: 1 }}>
                              <Chip
                                variant="outlined"
                                label={"Not Started"}
                                className="text-gray-700 border-gray-700"
                                size="small"
                              />
                            </Box>
                          </ListItemButton>
                        </>
                      )}
                    </React.Fragment>
                  ))}
              </List>
            </>
          ) : (
            <>
              <Divider sx={{ mb: 1 }} />
              <List>
                {menuLevels.map((level, index) => (
                  <React.Fragment key={level.mainTitle + index}>
                    <ListItem disablePadding>
                      <ListItemButton
                        onClick={() => handleLevelClick(level.mainTitle)}
                        sx={{
                          backgroundColor: expandedLevels[level.mainTitle]
                            ? (theme) => theme.palette.primary.main
                            : "transparent",
                          "&:hover": {
                            backgroundColor: expandedLevels[level.mainTitle]
                              ? (theme) => theme.palette.primary.dark
                              : (theme) => theme.palette.action.hover
                          },
                          "& .MuiListItemText-primary": {
                            color: expandedLevels[level.mainTitle] ? "white" : (theme) => theme.palette.text.primary
                          },
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center"
                        }}>
                        {drawerOpen && (
                          <>
                            <ListItemText
                              primary={level.mainTitle}
                              secondary={`1 of ${level.topPage.length - 1} completed`}
                              primaryTypographyProps={{
                                variant: "h6"
                              }}
                              secondaryTypographyProps={{
                                sx: {
                                  variant: "body2",
                                  color: expandedLevels[level.mainTitle]
                                    ? "rgba(255, 255, 255, 0.7)"
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
                ))}
              </List>
            </>
          )}
        </Box>
      </Drawer>
    </>
  );
};

export default PSDrawer;
