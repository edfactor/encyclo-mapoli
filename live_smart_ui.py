#!/usr/bin/env python3
"""
Script to enable live development of smart-ui-library within a smart application.

This script:
1. Creates a symbolic link from smart-profit-sharing/src to smart-ui/src/library
2. Updates tsconfig.app.json to include the library directory
3. Updates vite.config.ts to deduplicate React and add extensions
4. Replaces all smart-ui-library imports with direct imports from the library source
"""

import os
import re
import sys
from pathlib import Path
from typing import Dict, List, Tuple

# Paths that you should customize for your environment
SMART_UI_LIBRARY_PATH = "/Users/elyons/code/smart-ui/src/library"

SMART_APPLICATION_SRC_PATH = "/Users/elyons/code/smart-profit-sharing/src"

SMART_APP_TO_LIBRARY_SYMLINK_NAME = "library"

SYMLINK_PATH = os.path.join(SMART_APPLICATION_SRC_PATH, SMART_APP_TO_LIBRARY_SYMLINK_NAME)

SMART_APPLICATION_TSCONFIG_PATH = "/Users/elyons/code/smart-profit-sharing/src/ui/tsconfig.app.json"
SMART_APPLICATION_UI_SRC_PATH = "/Users/elyons/code/smart-profit-sharing/src/ui/src"
SMART_APPLICATION_VITE_CONFIG_PATH = "/Users/elyons/code/smart-profit-sharing/src/ui/vite.config.ts"

def create_export_mapping() -> Dict[str, str]:
    """
    Create a mapping of exported names to their relative import paths.
    Returns a dictionary like: {"DSMAccordion": "../../../../library/components/DSMAccordion/DSMAccordion"}
    """
    mapping = {}

    # Component exports
    component_exports = {
        "DSMCurrencyCellEditor": "components/AgGridHelpers/DSMCurrencyCellEditor/DSMCurrencyCellEditor",
        "DSMSelectCellEditor": "components/AgGridHelpers/SelectCell/DSMSelectCellEditor",
        "DSMSelectCellRenderer": "components/AgGridHelpers/SelectCell/DSMSelectCellRenderer",
        "ApiMessageAlert": "components/ApiMessageAlert/ApiMessageAlert",
        "Breadcrumb": "components/Breadcrumbs/Breadcrumb",
        "DSMAccordion": "components/DSMAccordion/DSMAccordion",
        "DSMAutocomplete": "components/DSMAutocomplete/DSMAutocomplete",
        "DSMCurrencyInput": "components/DSMCurrencyInput/DSMCurrencyInput",
        "DSMErrorBoundary": "components/DSMErrorBoundary/DSMErrorBoundary",
        "DSMGrid": "components/DSMGrid/DSMGrid",
        "DSMLayout": "components/DSMLayout/DSMLayout",
        "WelcomeDisplay": "components/DSMLayout/WelcomeDisplay",
        "BreadcrumbItem": "components/DynamicBreadcrumbs/BreadcrumbItem",
        "DynamicBreadcrumbs": "components/DynamicBreadcrumbs/DynamicBreadcrumbs",
        "EnvironmentBanner": "components/EnvironmentBanner/EnvironmentBanner",
        "IEnvironmentBannerProps": "components/EnvironmentBanner/EnvironmentBanner",
        "ErrorBoundary": "components/ErrorBoundary/ErrorBoundary",
        "ErrorFallback": "components/ErrorFallback/ErrorFallback",
        "GuardedComponent": "components/GuardedRouteComponent/GuardedComponent",
        "LoadingSpinner": "components/LoadingSpinner/LoadingSpinner",
        "ImpersonationMultiSelect": "components/MenuBar/ImpersonationMultiSelect",
        "MenuBar": "components/MenuBar/MenuBar",
        "ActionButtons": "components/ModalDialog/ActionButtons",
        "SmartModal": "components/ModalDialog/SmartModal",
        "NotFound404": "components/NotFound404/NotFound404",
        "Page": "components/Page/Page",
        "Pagination": "components/Pagination/Pagination",
        "PreferencesModal": "components/PreferencesModal/PreferencesModal",
        "SearchAndReset": "components/SearchAndReset/SearchAndReset",
        "SectionTitle": "components/SectionTitle/SectionTitle",
        "ToastServiceUtils": "components/ToastService/ToastService",
        "ToastServiceProvider": "components/ToastService/ToastServiceProvider",
        "TotalsGrid": "components/TotalsGrid/TotalsGrid",
        "TotalsRow": "components/TotalsRow/TotalsRow",
    }

    # Utility exports
    utility_exports = {
        "yyyyMMDDToMMDDYYYY": "utils/dateUtils",
        "extractErrorDetails": "utils/errorUtils",
        "downloadFile": "utils/fileUtils",
        "numberToCurrency": "utils/formattingUtils",
        "CurrencyInputValueToNumber": "utils/formattingUtils",
        "formatNumberWithComma": "utils/formattingUtils",
        "hasValue": "utils/generalUtils",
    }

    # Redux exports
    redux_exports = {
        "messageSlice": "redux/messageSlice",
        "setMessage": "redux/messageSlice",
        "MessageUpdate": "redux/messageSlice",
        "rtkQueryErrorToastMiddleware": "redux/rtkQueryErrorToastMiddleware",
    }

    # Theme exports
    theme_exports = {
        "colors": "theme/colors",
        "themeOptions": "theme/themeOptions",
    }

    # Common types
    type_exports = {
        "ICommon": "components/ICommon",
        "ISortParams": "types",
        "Paged": "types",
        "PaginationParams": "types",
        "RouteData": "types",
        "RouteCategory": "types",
        "PathNameMap": "types",
    }

    # Hook exports
    hook_exports = {
        "useNumpadEnterAsTab": "hooks/useEnterAsTab",
    }

    # Combine all exports
    all_exports = {
        **component_exports,
        **utility_exports,
        **redux_exports,
        **theme_exports,
        **type_exports,
        **hook_exports,
    }

    # Store just the path part (relative prefix will be calculated per file)
    for name, path in all_exports.items():
        mapping[name] = path

    return mapping


def create_symlink():
    """Create symbolic link from smart-profit-sharing/src to smart-ui/src/library"""
    print(f"Creating symbolic link: {SYMLINK_PATH} -> {SMART_UI_LIBRARY_PATH}")

    # Remove existing symlink or directory if it exists
    if os.path.islink(SYMLINK_PATH):
        os.unlink(SYMLINK_PATH)
        print(f"  Removed existing symlink")
    elif os.path.exists(SYMLINK_PATH):
        print(f"  WARNING: {SYMLINK_PATH} exists and is not a symlink. Skipping symlink creation.")
        return False

    # Create the symlink
    os.symlink(SMART_UI_LIBRARY_PATH, SYMLINK_PATH)
    print(f"  ✓ Symlink created successfully")
    return True


def update_tsconfig():
    """Update tsconfig.app.json to include the library directory"""
    print(f"\nUpdating {SMART_APPLICATION_TSCONFIG_PATH}")

    with open(SMART_APPLICATION_TSCONFIG_PATH, 'r') as f:
        content = f.read()

    # Replace the include line
    original_line = '  "include": ["src"]'
    new_line = '  "include": ["src", "../library"]'

    if original_line in content:
        content = content.replace(original_line, new_line)

        with open(SMART_APPLICATION_TSCONFIG_PATH, 'w') as f:
            f.write(content)

        print(f"  ✓ Updated include path to: {new_line}")
        return True
    elif new_line in content:
        print(f"  ✓ Include path already correct")
        return True
    else:
        print(f"  ⚠ Could not find expected include line in tsconfig.app.json")
        return False


def update_vite_config():
    """Update vite.config.ts to deduplicate React and add extensions"""
    print(f"\nUpdating {SMART_APPLICATION_VITE_CONFIG_PATH}")

    with open(SMART_APPLICATION_VITE_CONFIG_PATH, 'r') as f:
        content = f.read()

    # Check if already updated
    if "// Deduplicate React and related packages" in content:
        print(f"  ✓ Vite config already updated")
        return True

    # Replace the styles line with the full block
    original_line = '        styles: path.resolve(__dirname, "./src/styles")'
    new_block = '''        styles: path.resolve(__dirname, "./src/styles"),
        // Deduplicate React and related packages to prevent multiple instances
        react: path.resolve(__dirname, "./node_modules/react"),
        "react-dom": path.resolve(__dirname, "./node_modules/react-dom"),
        "@mui/material": path.resolve(__dirname, "./node_modules/@mui/material"),
        "@emotion/react": path.resolve(__dirname, "./node_modules/@emotion/react"),
        "@emotion/styled": path.resolve(__dirname, "./node_modules/@emotion/styled"),
        "ag-grid-community": path.resolve(__dirname, "./node_modules/ag-grid-community"),
        "ag-grid-react": path.resolve(__dirname, "./node_modules/ag-grid-react")'''

    if original_line in content:
        content = content.replace(original_line, new_block)

        # Also add extensions if not present
        if 'extensions:' not in content:
            # Find the closing brace of alias and add extensions before it
            content = content.replace(
                '      }\n    },',
                '      },\n      extensions: [".ts", ".tsx", ".js", ".jsx", ".json"]\n    },'
            )

        with open(SMART_APPLICATION_VITE_CONFIG_PATH, 'w') as f:
            f.write(content)

        print(f"  ✓ Added React deduplication aliases")
        print(f"  ✓ Added file extensions configuration")
        return True
    else:
        print(f"  ⚠ Could not find expected styles line in vite.config.ts")
        return False


def find_import_files() -> List[Path]:
    """Find all TypeScript files that import from smart-ui-library"""
    print(f"\nSearching for files importing from 'smart-ui-library' in {SMART_APPLICATION_UI_SRC_PATH}")

    files_with_imports = []
    ui_src = Path(SMART_APPLICATION_UI_SRC_PATH)

    for file_path in ui_src.rglob("*.ts*"):
        if file_path.suffix in ['.ts', '.tsx']:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
                if 'from "smart-ui-library"' in content or "from 'smart-ui-library'" in content:
                    files_with_imports.append(file_path)

    print(f"  Found {len(files_with_imports)} files with smart-ui-library imports")
    return files_with_imports


def parse_import_statement(line: str) -> Tuple[List[str], bool]:
    """
    Parse an import statement and extract imported names.
    Returns (list of imports, is_type_import)
    """
    # Match import statements
    # Pattern: import { ... } from "smart-ui-library"
    pattern = r'import\s+(?:type\s+)?{([^}]+)}\s+from\s+["\']smart-ui-library["\']'
    match = re.search(pattern, line)

    if not match:
        return [], False

    is_type_import = 'import type' in line
    imports_str = match.group(1)

    # Split by comma and clean up
    imports = [imp.strip() for imp in imports_str.split(',')]

    return imports, is_type_import


def calculate_relative_path(from_file: Path, depth: int = None) -> str:
    """
    Calculate the relative path prefix based on file depth.
    If depth is None, calculate it from the file path.
    """
    if depth is None:
        # Calculate depth from ui/src
        ui_src = Path(SMART_APPLICATION_UI_SRC_PATH)
        try:
            relative = from_file.relative_to(ui_src)
            depth = len(relative.parent.parts)
        except ValueError:
            depth = 0

    # From ui/src to src is ../../ (2 levels up)
    # Then add one more ../ for each subdirectory level
    base_depth = 2
    total_depth = base_depth + depth

    return "../" * total_depth


def replace_imports_in_file(file_path: Path, export_mapping: Dict[str, str]):
    """Replace smart-ui-library imports with direct imports in a single file"""
    print(f"  Processing: {file_path.relative_to(SMART_APPLICATION_UI_SRC_PATH)}")

    with open(file_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    new_lines = []
    changes_made = False

    for line in lines:
        if 'from "smart-ui-library"' in line or "from 'smart-ui-library'" in line:
            imports, is_type_import = parse_import_statement(line)

            if imports:
                # Calculate relative path depth for this file
                relative_prefix = calculate_relative_path(file_path)

                # Create individual import statements
                type_prefix = "type " if is_type_import else ""
                for imp in imports:
                    if imp in export_mapping:
                        # Get the path and build full import path
                        lib_path = export_mapping[imp]
                        full_path = f"{relative_prefix}library/{lib_path}"

                        new_line = f'import {{ {type_prefix}{imp} }} from "{full_path}";\n'
                        new_lines.append(new_line)
                        changes_made = True
                    else:
                        print(f"    ⚠ Warning: No mapping found for '{imp}'")
                        new_lines.append(line)
            else:
                # Couldn't parse, keep original
                new_lines.append(line)
        else:
            new_lines.append(line)

    if changes_made:
        with open(file_path, 'w', encoding='utf-8') as f:
            f.writelines(new_lines)
        print(f"    ✓ Updated imports")
    else:
        print(f"    - No changes needed")


def replace_all_imports(export_mapping: Dict[str, str]):
    """Find and replace all smart-ui-library imports"""
    print("\nReplacing imports in all files...")

    files = find_import_files()

    for file_path in files:
        replace_imports_in_file(file_path, export_mapping)

    print(f"\n✓ Processed {len(files)} files")


def main():
    """Main execution function"""
    print("=" * 70)
    print("Smart UI Live Development Setup")
    print("=" * 70)

    # Step 1: Create export mapping
    print("\nStep 1: Creating export mapping...")
    export_mapping = create_export_mapping()
    print(f"  ✓ Created mapping for {len(export_mapping)} exports")

    # Step 2: Create symbolic link
    print("\nStep 2: Creating symbolic link...")
    if not create_symlink():
        print("\n⚠ Symlink creation failed. Please resolve manually.")
        return 1

    # Step 3: Update tsconfig
    print("\nStep 3: Updating tsconfig.app.json...")
    if not update_tsconfig():
        print("\n⚠ tsconfig update had issues. Please verify manually.")

    # Step 4: Update vite.config.ts
    print("\nStep 4: Updating vite.config.ts...")
    if not update_vite_config():
        print("\n⚠ vite.config.ts update had issues. Please verify manually.")

    # Step 5: Replace imports
    print("\nStep 5: Replacing smart-ui-library imports...")
    replace_all_imports(export_mapping)

    print("\n" + "=" * 70)
    print("✓ Setup complete!")
    print("=" * 70)
    print("\nNext steps:")
    print("1. Restart your TypeScript server / IDE")
    print("2. Any changes to smart-ui library will now be reflected immediately")
    print("3. You may need to restart your dev server")

    return 0


if __name__ == "__main__":
    sys.exit(main())
