Update Vision Issues helper
===========================

This helper script finds open issues labeled `vision` in this repository and posts a suggested rewrite (or updates the issue body when run with `--apply`).

Prerequisites
-------------
- Node.js 18+ installed
- A GitHub personal access token with `repo` scope (or `public_repo` for public repos)

Run (suggest only)
------------------
```bash
export GITHUB_TOKEN=ghp_xxx
export REPO_OWNER=your-org-or-user
export REPO_NAME=B2X
node scripts/update-vision-issues.js
```

Run (apply changes)
---------------------
```bash
export GITHUB_TOKEN=ghp_xxx
export REPO_OWNER=your-org-or-user
export REPO_NAME=B2X
node scripts/update-vision-issues.js --apply
```

What it changes
----------------
- Escapes known agent role handles (e.g., `@TechLead`) into inline code to avoid accidental mentions.
- Ensures HTML comment markers are followed by an extra blank line where found.
- Normalizes code-fence backticks.

Safety
------
- By default the script posts suggestion comments; only update issue bodies when you include `--apply`.
- Review comments before applying.

Notes
-----
- The list of role handles is configurable in the script. Update `roleHandles` if your project uses different role handles.
