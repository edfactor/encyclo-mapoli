import { Description as DescriptionIcon } from "@mui/icons-material";
import { Box, Divider, List, ListItem, ListItemButton, ListItemText, Paper, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { Page } from "smart-ui-library";

// Documentation files metadata
const documentationFiles = [
  {
    key: "year-end-testability",
    title: "Year-End Testability & Acceptance Criteria",
    filename: "Year-End-Testability-And-Acceptance-Criteria.md",
    description: "Comprehensive testing strategy and acceptance criteria for year-end processing"
  },
  {
    key: "distribution-requirements",
    title: "Distribution Processing Requirements",
    filename: "Distribution-Processing-Requirements.md",
    description: "High-level functional requirements for distribution processing"
  },
  {
    key: "distribution-business-need",
    title: "Distribution Business Need & Security",
    filename: "Distribution-Business-Need-Security-Process.md",
    description: "Business objectives, security concerns, and process documentation"
  },
  {
    key: "distribution-architecture",
    title: "Distribution Touchpoints & Architecture",
    filename: "Distribution-Touchpoints-Architecture.md",
    description: "System integration points and architectural considerations"
  },
  {
    key: "distribution-screens",
    title: "Distribution Screens & Process Requirements",
    filename: "Distribution-Screens-Process-Requirements.md",
    description: "User interface screens and detailed process flows"
  },
  {
    key: "distribution-testability",
    title: "Distribution Testability & Acceptance Criteria",
    filename: "Distribution-Testability-Acceptance-Criteria.md",
    description: "Testing framework and acceptance criteria for distribution processing"
  },
  {
    key: "telemetry-guide",
    title: "Telemetry Implementation Guide",
    filename: "TELEMETRY_GUIDE.md",
    description:
      "Comprehensive 75+ page reference covering developers, QA, and DevOps with architecture, implementation patterns, metrics reference, security guidelines, configuration, and troubleshooting"
  },
  {
    key: "telemetry-quick-reference",
    title: "Telemetry Quick Reference",
    filename: "TELEMETRY_QUICK_REFERENCE.md",
    description:
      "Developer cheat sheet with 3-step implementation process, copy-paste examples, business metrics patterns, and troubleshooting checklist"
  },
  {
    key: "telemetry-devops-guide",
    title: "Telemetry DevOps Guide",
    filename: "TELEMETRY_DEVOPS_GUIDE.md",
    description:
      "Production operations guide with deployment checklist, monitoring setup (Prometheus/Grafana), security configuration, alert rules, and disaster recovery procedures"
  },
  {
    key: "telemetry-overview",
    title: "Telemetry Overview",
    filename: "TELEMETRY.md",
    description: "High-level overview of telemetry implementation and system architecture"
  },
  {
    key: "read-only-functionality",
    title: "Read-Only Functionality Guide",
    filename: "READ_ONLY_FUNCTIONALITY.md",
    description:
      "Complete guide to read-only role implementation covering architecture, implementation patterns, testing, and maintenance for ITDEVOPS and AUDITOR roles"
  },
  {
    key: "read-only-quick-reference",
    title: "Read-Only Quick Reference",
    filename: "READ_ONLY_QUICK_REFERENCE.md",
    description:
      "Developer cheat sheet with copy-paste code examples, implementation checklist, common patterns, and troubleshooting guide for read-only features"
  },
  {
    key: "ps-1623-summary",
    title: "PS-1623 Read-Only Implementation Summary",
    filename: "PS-1623_READ_ONLY_SUMMARY.md",
    description:
      "Executive summary of PS-1623 read-only role implementation including status tracking, impact assessment, and deployment verification steps"
  },
  {
    key: "duplicate-names-birthdays",
    title: "Duplicate Names & Birthdays Report",
    filename: "Duplicate-Names-And-Birthdays-Report.md",
    description: "Implementation notes and operational guidance for the Duplicate Names and Birthdays cleanup report"
  },
  {
    key: "military-contribution-qa-guide",
    title: "Military Contribution QA Testing Guide",
    filename: "MILITARY_CONTRIBUTION_QA_GUIDE.md",
    description:
      "Comprehensive QA testing guide for military contribution business logic, validation rules, test scenarios, and expected behaviors"
  },
  {
    key: "terminated-employee-developer-guide",
    title: "Terminated Employee Report - Developer Guide",
    filename: "TERMINATED_EMPLOYEE_REPORT_DEVELOPER_GUIDE.md",
    description:
      "Complete developer guide for the Terminated Employee & Beneficiary Report implementation including data flow, calculation logic, vesting schedules, transaction processing, and code examples"
  },
  {
    key: "terminated-employee-analysis",
    title: "Terminated Employee Report - Discrepancy Analysis",
    filename: "TERMINATED_EMPLOYEE_REPORT_ANALYSIS.md",
    description:
      "Detailed analysis comparing SMART vs READY implementations, documenting known discrepancies, root causes, financial impacts, and investigation priorities for QA and stakeholders"
  },
  {
    key: "terminations-business-guide",
    title: "Terminations Business Guide",
    filename: "TERMINATIONS_BUSINESS_GUIDE.md",
    description:
      "Comprehensive business guide for stakeholders and QA teams explaining termination processing, vesting calculations, beneficiary handling, filtering rules, and common scenarios"
  },
  {
    key: "report-crossreference-matrix",
    title: "Report Cross-Reference Matrix",
    filename: "REPORT_CROSSREFERENCE_MATRIX.md",
    description:
      "Comprehensive matrix documenting which values across different Profit Sharing reports should match for data integrity validation, including PAY444, PAY443, QPAY129, QPAY066, and PAY426 series reports with implementation guidelines for checksum validation"
  },
  {
    key: "report-crossreference-quick",
    title: "Report Cross-Reference Quick Guide",
    filename: "REPORT_CROSSREFERENCE_QUICK.md",
    description:
      "Visual quick reference guide showing report value matching patterns, validation priority order, and field names for API validation - ideal for developers implementing cross-report validation"
  }
];

const Documentation: React.FC = () => {
  const [selectedDoc, setSelectedDoc] = useState<string | null>(null);
  const [documentContent, setDocumentContent] = useState<string>("");
  const [loading, setLoading] = useState(false);
  const location = useLocation();

  // Check for URL query parameter to pre-select a document
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const docKey = params.get("doc");
    if (docKey && documentationFiles.some((d) => d.key === docKey)) {
      setSelectedDoc(docKey);
    }
  }, [location.search]);

  // Load document content
  const loadDocument = async (filename: string) => {
    setLoading(true);
    try {
      const response = await fetch(`/docs/${filename}`);
      if (response.ok) {
        const content = await response.text();
        setDocumentContent(content);
      } else {
        setDocumentContent(
          `# Document Loading\n\nDocument "${filename}" is available but could not be loaded.\n\nStatus: ${response.status} ${response.statusText}`
        );
      }
    } catch (error) {
      setDocumentContent(
        `# Document Loading Error\n\nUnable to load "${filename}". \n\nError: ${error instanceof Error ? error.message : "Unknown error"}\n\nThe document files are available in the public/docs folder.`
      );
    }
    setLoading(false);
  };

  useEffect(() => {
    if (selectedDoc) {
      const doc = documentationFiles.find((d) => d.key === selectedDoc);
      if (doc) {
        loadDocument(doc.filename);
      }
    }
  }, [selectedDoc]);

  // Enhanced markdown-like rendering (basic implementation)
  const renderMarkdown = (content: string) => {
    const lines = content.split("\n");
    const elements: React.ReactNode[] = [];
    let inCodeBlock = false;
    let codeBlockContent: string[] = [];
    let inTable = false;
    let tableRows: string[] = [];

    lines.forEach((line, index) => {
      // Handle code blocks
      if (line.trim().startsWith("```")) {
        if (inCodeBlock) {
          // End code block
          elements.push(
            <Paper
              key={index}
              sx={{ p: 2, mt: 2, mb: 2, bgcolor: "grey.100" }}>
              <Typography
                component="pre"
                sx={{ fontFamily: "monospace", fontSize: "0.875rem", whiteSpace: "pre-wrap" }}>
                {codeBlockContent.join("\n")}
              </Typography>
            </Paper>
          );
          codeBlockContent = [];
          inCodeBlock = false;
        } else {
          // Start code block
          inCodeBlock = true;
        }
        return;
      }

      if (inCodeBlock) {
        codeBlockContent.push(line);
        return;
      }

      // Handle table rows
      if (line.includes("|") && line.trim().startsWith("|")) {
        if (!inTable) {
          inTable = true;
          tableRows = [];
        }
        tableRows.push(line);
        return;
      } else if (inTable && !line.includes("|")) {
        // End table
        if (tableRows.length > 0) {
          elements.push(
            <Paper
              key={index}
              sx={{ mt: 2, mb: 2, overflow: "hidden" }}>
              <Box sx={{ overflowX: "auto" }}>
                <Box
                  component="table"
                  sx={{ width: "100%", borderCollapse: "collapse" }}>
                  <tbody>
                    {tableRows.map((row, rowIndex) => {
                      const cells = row.split("|").filter((cell) => cell.trim() !== "");
                      const isHeader = rowIndex === 0;
                      const isHeaderSeparator = row.includes("---");

                      if (isHeaderSeparator) return null;

                      return (
                        <Box
                          component="tr"
                          key={rowIndex}>
                          {cells.map((cell, cellIndex) => {
                            const Component = isHeader ? "th" : "td";
                            return (
                              <Box
                                component={Component}
                                key={cellIndex}
                                sx={{
                                  p: "8px 12px",
                                  border: "1px solid #ddd",
                                  bgcolor: isHeader ? "grey.100" : "white",
                                  fontWeight: isHeader ? "bold" : "normal"
                                }}>
                                {cell.trim()}
                              </Box>
                            );
                          })}
                        </Box>
                      );
                    })}
                  </tbody>
                </Box>
              </Box>
            </Paper>
          );
        }
        tableRows = [];
        inTable = false;
      }

      // Handle headers
      if (line.startsWith("# ")) {
        elements.push(
          <Typography
            key={index}
            variant="h3"
            sx={{ mt: 4, mb: 2, fontWeight: "bold", color: "primary.main" }}>
            {line.substring(2)}
          </Typography>
        );
      } else if (line.startsWith("## ")) {
        elements.push(
          <Typography
            key={index}
            variant="h4"
            sx={{ mt: 3, mb: 2, fontWeight: "bold", color: "primary.dark" }}>
            {line.substring(3)}
          </Typography>
        );
      } else if (line.startsWith("### ")) {
        elements.push(
          <Typography
            key={index}
            variant="h5"
            sx={{ mt: 2, mb: 1, fontWeight: "bold" }}>
            {line.substring(4)}
          </Typography>
        );
      } else if (line.startsWith("#### ")) {
        elements.push(
          <Typography
            key={index}
            variant="h6"
            sx={{ mt: 2, mb: 1, fontWeight: "bold" }}>
            {line.substring(5)}
          </Typography>
        );
      } else if (line.startsWith("**") && line.endsWith("**") && line.length > 4) {
        elements.push(
          <Typography
            key={index}
            variant="body1"
            sx={{ mt: 1, fontWeight: "bold" }}>
            {line.substring(2, line.length - 2)}
          </Typography>
        );
      } else if (line.trim().startsWith("- [ ]")) {
        elements.push(
          <Typography
            key={index}
            variant="body2"
            sx={{ ml: 2, mt: 0.5, display: "flex", alignItems: "center" }}>
            <Box
              component="span"
              sx={{ mr: 1 }}>
              ☐
            </Box>
            {line.substring(line.indexOf("- [ ]") + 5)}
          </Typography>
        );
      } else if (line.trim().startsWith("- [x]") || line.trim().startsWith("- [X]")) {
        elements.push(
          <Typography
            key={index}
            variant="body2"
            sx={{ ml: 2, mt: 0.5, display: "flex", alignItems: "center" }}>
            <Box
              component="span"
              sx={{ mr: 1, color: "success.main" }}>
              ☑
            </Box>
            {line.substring(line.indexOf("- [") + 5)}
          </Typography>
        );
      } else if (line.trim().startsWith("- ")) {
        const content = line.substring(line.indexOf("- ") + 2);
        elements.push(
          <Typography
            key={index}
            variant="body2"
            sx={{ ml: 2, mt: 0.5 }}>
            • {content}
          </Typography>
        );
      } else if (line.trim().match(/^\d+\. /)) {
        const content = line.substring(line.indexOf(". ") + 2);
        elements.push(
          <Typography
            key={index}
            variant="body2"
            sx={{ ml: 2, mt: 0.5 }}>
            {line.match(/^\s*(\d+)\./)?.[1]}. {content}
          </Typography>
        );
      } else if (line.trim().startsWith("> ")) {
        elements.push(
          <Paper
            key={index}
            sx={{ pl: 2, py: 1, mt: 1, bgcolor: "info.light", borderLeft: "4px solid", borderColor: "info.main" }}>
            <Typography
              variant="body2"
              sx={{ fontStyle: "italic" }}>
              {line.substring(line.indexOf("> ") + 2)}
            </Typography>
          </Paper>
        );
      } else if (line.trim().startsWith("---")) {
        elements.push(
          <Divider
            key={index}
            sx={{ my: 2 }}
          />
        );
      } else if (line.trim() !== "") {
        // Handle inline code and bold text
        let processedLine = line;

        // Replace `code` with styled text
        processedLine = processedLine.replace(/`([^`]+)`/g, (match, code) => `[CODE]${code}[/CODE]`);

        // Replace **bold** with styled text
        processedLine = processedLine.replace(/\*\*([^*]+)\*\*/g, (match, bold) => `[BOLD]${bold}[/BOLD]`);

        // Split and render with styles
        const parts = processedLine.split(/(\[CODE\][^\[\]]+\[\/CODE\]|\[BOLD\][^\[\]]+\[\/BOLD\])/);

        elements.push(
          <Typography
            key={index}
            variant="body1"
            sx={{ mt: 1, lineHeight: 1.6 }}>
            {parts.map((part, partIndex) => {
              if (part.startsWith("[CODE]") && part.endsWith("[/CODE]")) {
                return (
                  <Box
                    key={partIndex}
                    component="span"
                    sx={{
                      fontFamily: "monospace",
                      bgcolor: "grey.100",
                      px: 0.5,
                      py: 0.25,
                      borderRadius: 0.5,
                      fontSize: "0.875em"
                    }}>
                    {part.substring(6, part.length - 7)}
                  </Box>
                );
              } else if (part.startsWith("[BOLD]") && part.endsWith("[/BOLD]")) {
                return (
                  <Box
                    key={partIndex}
                    component="span"
                    sx={{ fontWeight: "bold" }}>
                    {part.substring(6, part.length - 7)}
                  </Box>
                );
              }
              return part;
            })}
          </Typography>
        );
      } else {
        elements.push(
          <Box
            key={index}
            sx={{ height: "8px" }}
          />
        );
      }
    });

    return elements;
  };

  return (
    <Page label="Documentation">
      <Box sx={{ display: "flex", height: "100%", p: 2 }}>
        {/* Document List Sidebar */}
        <Paper
          elevation={2}
          sx={{
            width: 350,
            mr: 2,
            p: 2,
            maxHeight: "calc(100vh - 200px)",
            overflow: "auto"
          }}>
          <Typography
            variant="h6"
            sx={{ mb: 2, display: "flex", alignItems: "center" }}>
            <DescriptionIcon sx={{ mr: 1 }} />
            Documentation
          </Typography>
          <Divider sx={{ mb: 2 }} />

          <List disablePadding>
            {documentationFiles.map((doc) => (
              <ListItem
                key={doc.key}
                disablePadding
                sx={{ mb: 1 }}>
                <ListItemButton
                  selected={selectedDoc === doc.key}
                  onClick={() => setSelectedDoc(doc.key)}
                  sx={{
                    borderRadius: 1,
                    "&.Mui-selected": {
                      backgroundColor: "primary.light",
                      color: "primary.contrastText",
                      "&:hover": {
                        backgroundColor: "primary.main"
                      }
                    }
                  }}>
                  <ListItemText
                    primary={
                      <Typography
                        variant="body2"
                        fontWeight="medium">
                        {doc.title}
                      </Typography>
                    }
                    secondary={
                      <Typography
                        variant="caption"
                        color="text.secondary">
                        {doc.description}
                      </Typography>
                    }
                  />
                </ListItemButton>
              </ListItem>
            ))}
          </List>
        </Paper>

        {/* Document Content Area */}
        <Paper
          elevation={2}
          sx={{
            flex: 1,
            p: 3,
            maxHeight: "calc(100vh - 200px)",
            overflow: "auto"
          }}>
          {!selectedDoc ? (
            <Box sx={{ textAlign: "center", mt: 8 }}>
              <DescriptionIcon sx={{ fontSize: 64, color: "text.secondary", mb: 2 }} />
              <Typography
                variant="h5"
                color="text.secondary"
                gutterBottom>
                Select a Document
              </Typography>
              <Typography
                variant="body1"
                color="text.secondary">
                Choose a documentation file from the sidebar to view its contents.
              </Typography>
            </Box>
          ) : loading ? (
            <Box sx={{ textAlign: "center", mt: 8 }}>
              <Typography
                variant="h6"
                color="text.secondary">
                Loading document...
              </Typography>
            </Box>
          ) : (
            <Box>{renderMarkdown(documentContent)}</Box>
          )}
        </Paper>
      </Box>
    </Page>
  );
};

export default Documentation;
