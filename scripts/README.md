# scripts

This folder contains small helper scripts used by CI or developers.

add-jira-comment.ps1
- Posts a comment to a Jira issue using Jira Cloud REST API.
- Usage: see header in the script. Keep `JIRA_API_TOKEN` in CI secrets.

Security note: prefer using the organization's Atlassian MCP integration for automated Jira interactions where possible. If your CI cannot use MCP, store API tokens in the pipeline's protected variables.
