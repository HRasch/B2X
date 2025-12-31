#!/usr/bin/env node
import fs from 'fs';
import path from 'path';

const repoRoot = path.resolve(process.cwd());
const featuresDir = path.join(repoRoot, 'docs', 'features');
const outFile = path.join(repoRoot, '.ai', 'knowledgebase', 'docs-product-index.md');

function listFeatureFiles() {
  if (!fs.existsSync(featuresDir)) return [];
  return fs.readdirSync(featuresDir)
    .filter(f => f.endsWith('.md'))
    .sort((a,b) => a.localeCompare(b));
}

function buildContent(files) {
  const header = `# Product Documentation Index (docs/)

This page points to product-related documentation found under the repository 
\`docs/\` folder. Use these links as the canonical starting point for product, feature, and compliance documentation.

Key product docs
 - **Quick reference**: [docs/QUICK_REFERENCE.md](docs/QUICK_REFERENCE.md)
 - **Product vision & features**: [docs/features/](docs/features/) — implementation notes and feature guides.
 - **Architecture & design**: [docs/architecture/INDEX.md](docs/architecture/INDEX.md) — architecture overviews, diagrams, and ADRs.
 - **User guides**: [docs/user-guides/README.md](docs/user-guides/README.md) — end-user and admin flows (multi-language subfolders).
 - **Guides for contributors**: [docs/guides/GETTING_STARTED.md](docs/guides/GETTING_STARTED.md) and [docs/guides/DEVELOPER_GUIDE.md](docs/guides/DEVELOPER_GUIDE.md).
 - **Compliance & legal**: [docs/compliance/EXECUTIVE_SUMMARY.md](docs/compliance/EXECUTIVE_SUMMARY.md) and related P0.* compliance tests and checklists.

Feature docs (docs/features/)
-----------------------------
Below is an indexed list of feature-level documentation currently present in \`docs/features/\`.

`;

  const list = files.map(f => `- [${f}](docs/features/${f})`).join('\n');

  const footer = `

If you want, this index can be generated automatically and updated via CI or a helper script.
`;

  return header + (list ? list + '\n' : '') + footer;
}

function main() {
  const files = listFeatureFiles();
  const content = buildContent(files);
  fs.writeFileSync(outFile, content, { encoding: 'utf8' });
  console.log(`Wrote ${outFile} with ${files.length} feature(s).`);
}

main();
