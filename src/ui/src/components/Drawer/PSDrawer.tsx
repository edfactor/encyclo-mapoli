import { ChevronLeft, ExpandLess, ExpandMore } from "@mui/icons-material";
import {
  AppBar,
  Box,
  Button,
  Chip,
  Collapse,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  MenuItem,
  SvgIcon,
  Toolbar,
  Typography
} from "@mui/material";
import React, { useState } from "react";
import { Route, Routes, useNavigate } from "react-router-dom";

const SidebarIcon = (props: any) => (
  <SvgIcon
    {...props}
    viewBox="0 0 16 16"
    sx={{ fontSize: 20 }}>
    <path d="M6.823 7.823a.25.25 0 0 1 0 .354l-2.396 2.396A.25.25 0 0 1 4 10.396V5.604a.25.25 0 0 1 .427-.177Z" />
    <path d="M1.75 0h12.5C15.216 0 16 .784 16 1.75v12.5A1.75 1.75 0 0 1 14.25 16H1.75A1.75 1.75 0 0 1 0 14.25V1.75C0 .784.784 0 1.75 0ZM1.5 1.75v12.5c0 .138.112.25.25.25H9.5v-13H1.75a.25.25 0 0 0-.25.25ZM11 14.5h3.25a.25.25 0 0 0 .25-.25V1.75a.25.25 0 0 0-.25-.25H11Z" />
  </SvgIcon>
);

const drawerWidth = 320;

interface MenuLevel {
  title: string;
  pages: {
    title: string;
    subPages: {
      title: string;
      page: string;
    }[];
  }[];
}

const menuLevels: MenuLevel[] = [
  {
    title: "December",
    pages: [
      {
        title: "Clean Up Reports",
        subPages: [{ title: "Clean Up Reports", page: "Clean Up Reports" }]
      },
      {
        title: "Military and Rehire",
        subPages: [{ title: "Military and Rehire", page: "Military and Rehire" }]
      },
      {
        title: "Profit Share Loan Balance",
        subPages: [{ title: "Profit Share Loan Balance", page: "Profit Share Loan Balance" }]
      },
      {
        title: "Manage Executives",
        subPages: [{ title: "Manage Executives", page: "Manage Executives" }]
      },
      {
        title: "Terminations",
        subPages: [{ title: "Terminations", page: "Terminations" }]
      }
    ]
  },
  {
    title: "Fiscal Close",
    pages: [
      {
        title: "Payprofit Extract",
        subPages: [{ title: "Payprofit Extract", page: "Payprofit Extract" }]
      },
      {
        title: "YTD Wages Extract",
        subPages: [{ title: "YTD Wages Extract", page: "YTD Wages Extract" }]
      },
      {
        title: "Manage Executive Hours",
        subPages: [{ title: "Manage Executive Hours", page: "Manage Executive Hours" }]
      }
    ]
  },
  {
    title: "Post Frozen",
    pages: [
      {
        title: "Prof Share by Store",
        subPages: [{ title: "Prof Share by Store", page: "Prof Share by Store" }]
      },
      {
        title: "Print Profit Certs",
        subPages: [{ title: "Print Profit Certs", page: "Print Profit Certs" }]
      },
      {
        title: "Save Prof Paymastr",
        subPages: [{ title: "Save Prof Paymastr", page: "Save Prof Paymastr" }]
      }
    ]
  }
];

const PSDrawer = () => {
  const navigate = useNavigate();
  const [open, setOpen] = useState(true);
  const [expandedLevels, setExpandedLevels] = useState<{ [key: string]: boolean }>({});
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedLevel, setSelectedLevel] = useState<string | null>(null);
  const [activeSubmenu, setActiveSubmenu] = useState<string | null>(null);
  const menuOpen = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleDrawerToggle = () => {
    setOpen(!open);
    // Close all levels and clear active submenu when drawer closes
    if (open) {
      setExpandedLevels({});
      setActiveSubmenu(null);
    }
  };

  const handleLevelClick = (level: string) => {
    console.log("handleLevelClick", level);
    setActiveSubmenu(level);
  };

  const handleBackToMain = () => {
    setActiveSubmenu(null);
    setExpandedLevels({});
    setSelectedLevel(null);
  };

  const handleMenuItemClick = (level: string) => {
    if (!open) {
      setOpen(true);
    }
    console.log("handleMenuItemClick", level);
    const newExpandedLevels = Object.keys(expandedLevels).reduce(
      (acc, key) => {
        acc[key] = false;
        return acc;
      },
      {} as { [key: string]: boolean }
    );

    newExpandedLevels[level] = true;
    setExpandedLevels(newExpandedLevels);
    setSelectedLevel(level);
    setActiveSubmenu(level);
    handleClose();
  };

  const handlePageClick = (page: string) => {
    const slug = page.toLowerCase().replace(/[^a-z0-9]+/g, "-");
    navigate(`/${slug}`);
    console.log(`Navigating to ${slug}`);
  };

  const handleSubPageClick = (subPage: { title: string; page: string }) => {
    const slug = subPage.page.toLowerCase().replace(/[^a-z0-9]+/g, "-");
    navigate(`/${slug}`);
    console.log(`Navigating to ${slug}`);
  };

  return (
    <>
      {/* Toggle Button */}
      <Box
        sx={{
          position: "fixed",
          left: open ? "16px" : "12px",
          top: "179px",
          zIndex: (theme) => theme.zIndex.drawer + 1,
          display: "flex",
          alignItems: "center",
          gap: 1,
          justifyContent: "space-between",
          width: open ? "300px" : "auto",
          transition: (theme) =>
            theme.transitions.create("left", {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen
            })
        }}>
        {open && (
          <Typography
            variant="subtitle1"
            sx={{
              color: (theme) => theme.palette.text.primary,
              whiteSpace: "nowrap",
              fontWeight: "bold"
            }}>
            Profit Sharing Activities
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
        sx={{
          width: open ? drawerWidth : 64,
          flexShrink: 0,

          "& .MuiDrawer-paper": {
            width: open ? drawerWidth : 64,
            boxSizing: "border-box",
            overflowX: "hidden",
            "& > *": {
              overflowX: "hidden"
            },
            transition: (theme) =>
              theme.transitions.create("width", {
                easing: theme.transitions.easing.sharp,
                duration: theme.transitions.duration.enteringScreen
              })
          }
        }}>
        <Toolbar />
        <Box
          sx={{
            mt: "160px",
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
                  variant="subtitle1"
                  sx={{ fontWeight: "bold" }}>
                  {activeSubmenu}
                </Typography>
              </ListItemButton>
              <List>
                {menuLevels
                  .find((l) => l.title === activeSubmenu)
                  ?.pages.map((page) => (
                    <React.Fragment key={page.title}>
                      <ListItemButton
                        onClick={() => {
                          const newExpandedLevels = { ...expandedLevels };
                          newExpandedLevels[page.title] = !expandedLevels[page.title];
                          setExpandedLevels(newExpandedLevels);
                        }}
                        sx={{
                          pl: 2,
                          display: "flex",
                          justifyContent: "space-between",
                          py: 0.5,
                          minHeight: 0
                        }}>
                        <Box sx={{ display: "flex", alignItems: "center" }}>
                          <ListItemText
                            primary={page.title}
                            sx={{
                              margin: 0,
                              "& .MuiTypography-root": {
                                fontSize: "0.875rem"
                              }
                            }}
                          />
                        </Box>
                        {expandedLevels[page.title] ? <ExpandLess /> : <ExpandMore />}
                      </ListItemButton>
                      <Collapse
                        in={expandedLevels[page.title]}
                        timeout="auto"
                        unmountOnExit>
                        <List
                          component="div"
                          disablePadding>
                          {page.subPages.map((subPage) => (
                            <ListItemButton
                              key={subPage.title}
                              sx={{
                                pl: 4,
                                display: "flex",
                                justifyContent: "space-between",
                                py: 0.5,
                                minHeight: 0
                              }}
                              onClick={() => handleSubPageClick(subPage)}>
                              <Box sx={{ display: "flex", alignItems: "center" }}>
                                <ListItemText
                                  primary={subPage.title}
                                  sx={{
                                    margin: 0,
                                    "& .MuiTypography-root": {
                                      fontSize: "0.875rem"
                                    }
                                  }}
                                />
                              </Box>
                              <Chip
                                label="Started"
                                size="small"
                                sx={{
                                  height: "20px",
                                  backgroundColor: (theme) => theme.palette.primary.main,
                                  color: "white",
                                  "& .MuiChip-label": {
                                    px: 1,
                                    fontSize: "0.75rem"
                                  }
                                }}
                              />
                            </ListItemButton>
                          ))}
                        </List>
                      </Collapse>
                    </React.Fragment>
                  ))}
              </List>
            </>
          ) : (
            <>
              <Divider sx={{ mb: 1 }} />
              <List>
                {menuLevels.map((level, index) => (
                  <React.Fragment key={level.title}>
                    <ListItem disablePadding>
                      <ListItemButton
                        onClick={() => handleLevelClick(level.title)}
                        sx={{
                          backgroundColor: expandedLevels[level.title]
                            ? (theme) => theme.palette.primary.main
                            : "transparent",
                          "&:hover": {
                            backgroundColor: expandedLevels[level.title]
                              ? (theme) => theme.palette.primary.dark
                              : (theme) => theme.palette.action.hover
                          },
                          "& .MuiListItemText-primary": {
                            color: expandedLevels[level.title] ? "white" : (theme) => theme.palette.text.primary
                          },
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center"
                        }}>
                        {open && (
                          <>
                            <ListItemText
                              primary={level.title}
                              secondary={`1 of ${level.pages.length} completed`}
                              secondaryTypographyProps={{
                                sx: {
                                  color: expandedLevels[level.title]
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
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: `calc(100% - ${open ? drawerWidth : 64}px)`,
          transition: (theme) =>
            theme.transitions.create("width", {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen
            })
        }}>
        <Toolbar />
        <Routes>
          {/* Default Route */}
          <Route
            path="/"
            element={<Typography paragraph>This is where the content.</Typography>}
          />
        </Routes>
      </Box>
    </>
  );
};

export default PSDrawer;
