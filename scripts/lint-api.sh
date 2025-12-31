#!/bin/bash
# API Linting Script - Cross-platform alternative (bash/zsh)
# Purpose: Validate OpenAPI spec against Zalando RESTful Guidelines
# 
# Usage:
#   ./scripts/lint-api.sh                                          # Auto-download from QA
#   ./scripts/lint-api.sh --openapi ./openapi.json                # Lint local file
#   ./scripts/lint-api.sh --qa-url https://qa-api.example.com    # Custom QA URL
#   ./scripts/lint-api.sh --output ./reports/api-lint.xml        # Save JUnit report
#
# Requirements:
#   - Node.js 22+
#   - npx (comes with npm 5.2+)

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Default values
OPENAPI_PATH=""
QA_BASE_URL="${QA_BASE_URL:-}"
OUTPUT_FORMAT="stylish"
OUTPUT_FILE=""
FAIL_ON_WARNINGS=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --openapi)
            OPENAPI_PATH="$2"
            shift 2
            ;;
        --qa-url)
            QA_BASE_URL="$2"
            shift 2
            ;;
        --output)
            OUTPUT_FILE="$2"
            OUTPUT_FORMAT="junit"
            shift 2
            ;;
        --fail-on-warnings)
            FAIL_ON_WARNINGS=true
            shift
            ;;
        --help|-h)
            echo "API Linting Script - Validates OpenAPI spec against Zalando RESTful Guidelines"
            echo ""
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  --openapi PATH              Path to local OpenAPI spec file"
            echo "  --qa-url URL                Base URL for QA environment (e.g., https://qa-api.example.com)"
            echo "  --output FILE               Output file path for JUnit report"
            echo "  --fail-on-warnings          Treat warnings as errors"
            echo "  --help, -h                  Show this help message"
            echo ""
            echo "Examples:"
            echo "  $0                                                     # Auto-download from QA"
            echo "  $0 --openapi ./openapi.json                          # Lint local file"
            echo "  $0 --qa-url https://qa-api.example.com               # Custom QA URL"
            echo "  $0 --output ./reports/api-lint.xml                   # Save JUnit report"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

echo -e "${CYAN}API LINTING - Zalando RESTful Guidelines${NC}"
echo -e "${CYAN}=============================================${NC}"
echo ""

# Step 1: Determine OpenAPI spec source
if [ -z "$OPENAPI_PATH" ]; then
    if [ -z "$QA_BASE_URL" ]; then
        echo -e "${RED}Error: No OpenAPI path provided and QA_BASE_URL not set${NC}"
        echo "Set QA_BASE_URL environment variable or provide --openapi or --qa-url"
        exit 1
    fi

    OPENAPI_PATH="openapi-qa.json"
    URI="$QA_BASE_URL/swagger/Release%201.0/swagger.json"

    echo -e "${CYAN}[1] Downloading OpenAPI spec from QA${NC}"
    echo -e "${YELLOW}URL: $URI${NC}"

    if ! curl -s -k -o "$OPENAPI_PATH" "$URI"; then
        echo -e "${RED}Failed to download OpenAPI spec from QA${NC}"
        exit 1
    fi
    echo -e "${GREEN}[OK] OpenAPI spec downloaded successfully${NC}"
else
    echo -e "${CYAN}[1] Using provided OpenAPI spec${NC}"
    echo -e "${YELLOW}Path: $OPENAPI_PATH${NC}"

    if [ ! -f "$OPENAPI_PATH" ]; then
        echo -e "${RED}Error: OpenAPI spec file not found: $OPENAPI_PATH${NC}"
        exit 1
    fi
fi

echo ""

# Step 2: Ensure Spectral CLI is available
echo -e "${CYAN}[2] Checking Spectral CLI installation${NC}"
if command -v spectral &> /dev/null; then
    SPECTRAL_VERSION=$(spectral --version 2>/dev/null || echo "unknown")
    echo -e "${GREEN}[OK] Spectral CLI version: $SPECTRAL_VERSION${NC}"
else
    echo -e "${YELLOW}Installing Spectral CLI via npx...${NC}"
    if ! npx @stoplight/spectral-cli --version &> /dev/null; then
        echo -e "${RED}Failed to install Spectral CLI${NC}"
        exit 1
    fi
    echo -e "${GREEN}[OK] Spectral CLI ready (using npx)${NC}"
fi

echo ""

# Step 3: Run Spectral linting
echo -e "${CYAN}[3] Running API linting with Spectral${NC}"
echo ""

# Create reports directory if needed
if [ -n "$OUTPUT_FILE" ]; then
    REPORT_DIR=$(dirname "$OUTPUT_FILE")
    mkdir -p "$REPORT_DIR"
fi

# Run linting (always show stylish output to console)
if npx @stoplight/spectral-cli lint "$OPENAPI_PATH" --format stylish; then
    LINT_EXIT_CODE=0
else
    LINT_EXIT_CODE=$?
fi

# Save JUnit output if requested
if [ -n "$OUTPUT_FILE" ]; then
    echo ""
    echo -e "${CYAN}[4] Generating JUnit output${NC}"
    if npx @stoplight/spectral-cli lint "$OPENAPI_PATH" --format junit --output "$OUTPUT_FILE"; then
        echo -e "${GREEN}[OK] JUnit report saved to: $OUTPUT_FILE${NC}"
    fi
fi

echo ""
echo -e "${CYAN}API LINTING SUMMARY${NC}"
echo -e "${CYAN}=======================${NC}"

if [ $LINT_EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}[PASS] API linting PASSED - No errors found${NC}"
    exit 0
else
    echo -e "${RED}[FAIL] API linting FAILED - Review errors above${NC}"
    if [ -n "$OUTPUT_FILE" ]; then
        echo -e "${YELLOW}JUnit report saved to: $OUTPUT_FILE${NC}"
    fi
    exit 1
fi
