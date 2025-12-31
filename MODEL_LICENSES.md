MODEL LICENSES & USAGE (AI Models)

Purpose
- Inventory AI models referenced in this repository and provide links to provider terms, license/usage notes, and a short compliance checklist for commercial usage.

Models referenced in repository

- `claude-sonnet-4` / `claude-sonnet-4.5` / `Claude Haiku 4.5`
  - Provider: Anthropic
  - Source in repo: .ai/collaboration/AGENT_TEAM_REGISTRY.md, .ai/status/* and other agent docs reference these models for internal agent orchestration.
  - Provider terms / docs:
    - Terms of Service: https://www.anthropic.com/terms
    - Policy / Acceptable Use: https://www.anthropic.com/policies
    - API docs: https://www.anthropic.com/docs (verify current links with provider)
  - License model: These are service offerings governed by commercial Terms of Service and API agreements (not permissive open-source licenses). Commercial use is allowed under Anthropic API terms when you have an appropriate API subscription/contract; check vendor contract for specific usage rights and data handling.

Notes about other AI model references
- The repository contains agent documentation that names many agent models (agent registry). If additional provider models are introduced, add them to this document.
- If you are using any OpenAI models (not currently found in repo search), record provider and links similarly:
  - OpenAI Terms: https://openai.com/policies
  - OpenAI API Terms: https://openai.com/terms

Third-party tools with license implications
- TruffleHog (secret scanner) — used in CI via GitHub Action or Docker image
  - Repo: https://github.com/trufflesecurity/trufflehog
  - License: AGPL-3.0 (TruffleHog v3+)
  - Note: AGPL is a copyleft license that can impose obligations if you distribute modified copies of AGPL-licensed code. Using TruffleHog as a CI action or running its Docker image does NOT automatically relicense your repository, but if you vendor AGPL code into the repo or redistribute it, you must comply with AGPL terms.

Compliance checklist (recommended actions)

1) Confirm commercial rights and contracts
- For each provider (Anthropic, OpenAI, etc.) obtain and document the applicable commercial terms (API contract or subscription) that permit commercial use.
- Store proof of purchase/contract in a secure internal location (not in the repo), and link it from `.ai/compliance/` if desired.

2) Data protection and prompts
- Do not commit API keys, secrets, or system prompts to the repository. Use environment variables and CI secrets for keys.
- Review provider data usage policies: determine whether prompts and responses are retained by the model provider and whether you need to opt out of data retention (if supported).
- Avoid sending PII or sensitive customer data to models unless covered by contract and data protection controls.

3) Attribution and documentation
- Add a short note to `README.md` describing AI model usage and pointing to `MODEL_LICENSES.md`.
- Record which parts of the system use models (agent orchestration, knowledgebase generation, etc.).

4) Do not include model weights or proprietary code
- Never commit model weights, snapshots, or vendor SDKs that include redistributable model files unless the license explicitly permits it.

5) Third-party license handling
- For components under copyleft licenses (e.g., AGPL TruffleHog), avoid vendoring their source into this repository. Using them via Docker or as remote actions in CI is generally safe, but consult legal if you plan to embed or modify and redistribute.

6) Security scans & CI
- Keep the TruffleHog usage pinned to a specific tag (we pinned to `v3.92.4`) to make auditing easier.
- If your org policy forbids AGPL usage in CI, switch to alternative secret scanners with permissive licenses.

7) Periodic review
- Add a periodic review task (quarterly) in `.ai/status/` to re-check provider terms and update this file.

What I changed
- Added this file `MODEL_LICENSES.md` at the repository root to document models and compliance steps.

Suggested next steps I can do for you
- Add a short note to `README.md` linking to this file.
- Add an entry in `.ai/compliance/` with copies/links to vendor contracts (secure references only).
- Run a repository scan to ensure no API keys or prompt files were committed.

If you want me to proceed with any of the suggested next steps, tell me which ones and I will implement them and record progress in the todo list.
