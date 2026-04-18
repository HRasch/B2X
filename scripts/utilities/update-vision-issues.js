#!/usr/bin/env node
/*
Script: update-vision-issues.js

Usage:
  GITHUB_TOKEN=ghp_xxx node scripts/update-vision-issues.js [--apply]

Behavior:
 - Finds open issues in the current repo with label `vision`.
 - Generates a suggested rewrite that escapes agent role handles (e.g., @TechLead -> `@TechLead`) to avoid accidental mentions and applies small formatting fixes.
 - By default posts a comment with the proposed rewrite. With `--apply` it updates the issue body directly.

This script is a helper to be run locally by maintainers (requires Node >= 18).
*/

import { Octokit } from "@octokit/rest";
import process from "process";

const token = process.env.GITHUB_TOKEN || process.env.GH_TOKEN;
if (!token) {
  console.error("Error: GITHUB_TOKEN or GH_TOKEN environment variable is required.");
  process.exit(1);
}

const repoOwner = process.env.REPO_OWNER || process.env.GITHUB_REPOSITORY?.split('/')?.[0];
const repoName = process.env.REPO_NAME || process.env.GITHUB_REPOSITORY?.split('/')?.[1];
if (!repoOwner || !repoName) {
  console.error('Error: set REPO_OWNER and REPO_NAME env vars, or run from a GitHub Actions environment with GITHUB_REPOSITORY.');
  process.exit(1);
}

const octokit = new Octokit({ auth: token });

const roleHandles = [
  'TechLead','Frontend','Backend','QA','Security','DevOps','ProductOwner','Architect','GitManager','UI','UX','SEO','SARAH'
];

function escapeHandles(text) {
  if (!text) return text;
  let out = text;
  for (const h of roleHandles) {
    // replace @Handle with inline code to avoid mentions
    const re = new RegExp(`@${h}\\b`, 'g');
    out = out.replace(re, '`@' + h + '`');
  }
  return out;
}

function ensureMarkerSpacing(text) {
  if (!text) return text;
  // ensure HTML comment markers are followed by an extra blank line
  return text.replace(/(<!--\s*[A-Z0-9_-]+\s*-->)(?!\n\n)/gi, '$1\n\n');
}

function generateRewrite(body) {
  let out = body || '';
  out = escapeHandles(out);
  out = ensureMarkerSpacing(out);
  // normalize triple backticks spacing (ensure fenced blocks are preserved)
  out = out.replace(/`{3,}/g, '```');
  return out;
}

async function findVisionIssues() {
  const issues = [];
  for await (const response of octokit.paginate.iterator(octokit.rest.issues.listForRepo, {
    owner: repoOwner,
    repo: repoName,
    state: 'open',
    labels: 'vision',
    per_page: 100
  })) {
    issues.push(...response.data);
  }
  return issues;
}

async function main() {
  const apply = process.argv.includes('--apply');
  console.log(`Repository: ${repoOwner}/${repoName}`);
  const issues = await findVisionIssues();
  console.log(`Found ${issues.length} open issue(s) labeled 'vision'.`);

  for (const issue of issues) {
    const original = issue.body || '';
    const suggested = generateRewrite(original);

    if (original.trim() === suggested.trim()) {
      console.log(`#${issue.number}: no change suggested.`);
      continue;
    }

    const commentBody = ['Proposed rewrite to follow GFM guidance:', '', '---', '', 'Original:', '', '```', original || '(empty)', '```', '', 'Suggested:', '', '```', suggested, '```', '', 'If you approve apply with `--apply`.'].join('\n');

    if (apply) {
      await octokit.rest.issues.update({ owner: repoOwner, repo: repoName, issue_number: issue.number, body: suggested });
      console.log(`#${issue.number}: issue body updated.`);
    } else {
      await octokit.rest.issues.createComment({ owner: repoOwner, repo: repoName, issue_number: issue.number, body: commentBody });
      console.log(`#${issue.number}: posted suggestion comment.`);
    }
  }
}

main().catch(err => { console.error(err); process.exit(1); });
