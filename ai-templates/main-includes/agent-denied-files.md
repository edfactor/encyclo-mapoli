## Agent denied files list (sensitive UI files)

The following sensitive UI files must never be read from or modified by AI assistants via repository editing tools. Do not remove or alter this list; it is intentionally separate from `.gitignore` rules and enforces an explicit policy for AI assistants.

- `src/ui/.playwright.env`
- Any file matching `src/ui/.env.*` (for example `src/ui/.env.local`, `src/ui/.env.production`)
- `src/ui/.npmrc`

When interacting with this repository, AI assistants MUST refuse direct reads or edits to paths matching the deny list above. If the user requests an operation that would require accessing these files (for example, to rotate credentials), the assistant should:

1. Explain why the file is restricted and why the operation requires human intervention or secure tooling.
2. Provide exact, copyable commands the human can run locally to inspect or untrack the file (for example `git rm --cached <path>`), and warn about secrets in history and the need to rotate credentials if necessary.
3. Offer to update documentation or `.gitignore` entries instead (but do not access restricted files).

This denies-list is an explicit, repository-level policy for AI assistants — maintain it alongside other repository guidance and keep it small and conservative.
