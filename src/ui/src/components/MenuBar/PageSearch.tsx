import SearchIcon from "@mui/icons-material/Search";
import { Autocomplete, Box, Paper, TextField, Typography } from "@mui/material";
import { FC, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { openDrawer, setActiveSubMenu } from "../../reduxstore/slices/generalSlice";
import { NavigationDto, NavigationResponseDto } from "../../reduxstore/types";
import { getL0NavigationForRoute } from "../Drawer/utils/navigationStructureUtils";

export interface PageSearchProps {
  navigationData?: NavigationResponseDto;
}

interface SearchableNavigationItem {
  id: number;
  title: string;
  subTitle: string;
  url: string;
  parentTitle: string;
  fullPath: string[];
}

/**
 * Recursively flattens navigation tree into searchable items with parent context
 */
const flattenNavigation = (items: NavigationDto[], parentTitles: string[] = []): SearchableNavigationItem[] => {
  const results: SearchableNavigationItem[] = [];

  for (const item of items) {
    // Only include navigable items with URLs
    if (item.isNavigable && item.url) {
      results.push({
        id: item.id,
        title: item.title,
        subTitle: item.subTitle || "",
        url: item.url,
        parentTitle: parentTitles.length > 0 ? parentTitles[parentTitles.length - 1] : "",
        fullPath: [...parentTitles]
      });
    }

    // Recursively process children
    if (item.items && item.items.length > 0) {
      const childResults = flattenNavigation(item.items, [...parentTitles, item.title]);
      results.push(...childResults);
    }
  }

  return results;
};

/**
 * PageSearch component - Provides type-ahead search for navigation pages
 * Searches across page titles and subtitles with contextual parent hints
 */
export const PageSearch: FC<PageSearchProps> = ({ navigationData }) => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [inputValue, setInputValue] = useState("");
  const [isOpen, setIsOpen] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  // Flatten navigation data into searchable items
  const searchableItems = useMemo(() => {
    if (!navigationData?.navigation) return [];
    return flattenNavigation(navigationData.navigation);
  }, [navigationData]);

  // Filter items based on search input
  const filteredItems = useMemo(() => {
    if (!inputValue.trim()) return [];

    const searchTerm = inputValue.toLowerCase();
    return searchableItems.filter(
      (item) => item.title.toLowerCase().includes(searchTerm) || item.subTitle.toLowerCase().includes(searchTerm)
    );
  }, [inputValue, searchableItems]);

  // Handle selection
  const handleSelect = (_event: React.SyntheticEvent, value: string | SearchableNavigationItem | null) => {
    if (value && typeof value !== "string") {
      const absolutePath = value.url.startsWith("/") ? value.url : `/${value.url}`;
      
      // Find the L0 parent section for this page
      const l0Section = getL0NavigationForRoute(navigationData, absolutePath);
      
      // If found, set the active submenu and open drawer
      if (l0Section) {
        dispatch(setActiveSubMenu(l0Section.title));
        dispatch(openDrawer());
      }
      
      // Navigate to the page
      navigate(absolutePath);
      
      // Clear search
      setInputValue("");
      setIsOpen(false);
    }
  };

  // Handle input change
  const handleInputChange = (_event: React.SyntheticEvent, newInputValue: string) => {
    setInputValue(newInputValue);
    setIsOpen(newInputValue.trim().length > 0);
  };

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = () => {
      setIsOpen(false);
    };

    if (isOpen) {
      document.addEventListener("click", handleClickOutside);
      return () => document.removeEventListener("click", handleClickOutside);
    }
  }, [isOpen]);

  return (
    <Autocomplete
      freeSolo
      open={isOpen}
      onOpen={() => setIsOpen(inputValue.trim().length > 0)}
      onClose={() => setIsOpen(false)}
      options={filteredItems}
      value={null}
      inputValue={inputValue}
      onInputChange={handleInputChange}
      onChange={handleSelect}
      getOptionLabel={(option) => (typeof option === "string" ? option : option.title)}
      filterOptions={(x) => x} // We handle filtering ourselves
      disablePortal // Keep dropdown in the same DOM hierarchy for testing
      renderInput={(params) => (
        <TextField
          {...params}
          inputRef={inputRef}
          placeholder="Search pages..."
          variant="outlined"
          size="small"
          sx={{
            width: 280,
            "& .MuiOutlinedInput-root": {
              backgroundColor: "rgba(255, 255, 255, 0.15)",
              color: "white",
              fontSize: "0.875rem",
              "& fieldset": {
                borderColor: "rgba(255, 255, 255, 0.3)"
              },
              "&:hover fieldset": {
                borderColor: "rgba(255, 255, 255, 0.5)"
              },
              "&.Mui-focused fieldset": {
                borderColor: "rgba(255, 255, 255, 0.7)"
              }
            },
            "& .MuiOutlinedInput-input": {
              color: "white",
              "&::placeholder": {
                color: "rgba(255, 255, 255, 0.7)",
                opacity: 1
              }
            },
            "& .MuiSvgIcon-root": {
              color: "rgba(255, 255, 255, 0.7)"
            }
          }}
          InputProps={{
            ...params.InputProps,
            startAdornment: <SearchIcon sx={{ color: "rgba(255, 255, 255, 0.7)", mr: 1 }} />
          }}
        />
      )}
      renderOption={(props, option) => (
        <li
          {...props}
          key={option.id}>
          <Box sx={{ width: "100%", py: 0.5 }}>
            <Typography
              variant="body2"
              sx={{ fontWeight: 500 }}>
              {option.title}
            </Typography>
            {option.subTitle && (
              <Typography
                variant="caption"
                sx={{ color: "text.secondary", display: "block" }}>
                {option.subTitle}
              </Typography>
            )}
            {option.parentTitle && (
              <Typography
                variant="caption"
                sx={{
                  color: "text.disabled",
                  display: "block",
                  fontStyle: "italic",
                  mt: 0.25
                }}>
                in {option.fullPath.length > 1 ? `${option.fullPath.join(" > ")} > ` : ""}
                {option.parentTitle}
              </Typography>
            )}
          </Box>
        </li>
      )}
      PaperComponent={(props) => (
        <Paper
          {...props}
          sx={{
            mt: 1,
            maxHeight: 400,
            overflow: "auto"
          }}
        />
      )}
      noOptionsText={inputValue.trim() ? "No pages found" : "Start typing to search..."}
      data-testid="page-search"
    />
  );
};

export default PageSearch;
