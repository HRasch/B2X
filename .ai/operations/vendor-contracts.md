Vendor Contracts & References

This file lists references to vendor contracts and the recommended storage location.

IMPORTANT: Do NOT store signed contracts or secrets in the public repository. Instead, store them in a secure document store (e.g., internal SharePoint, Confluence with restricted access, or secure S3 bucket with strict IAM policies).

Recommended fields per vendor:
- Vendor: Anthropic
  - Contract reference: Internal vault path or link (e.g., `vault://contracts/anthropic/b2connect`) — replace with your secure reference
  - Contact: Account manager name / email
  - Notes: API key scope restrictions, data retention opt-out status

- Vendor: Truffle Security (TruffleHog)
  - Contract reference: (if applicable)
  - Notes: TruffleHog OSS is AGPL-3.0; ensure CI policy accepts AGPL usage or use Docker/action with approval

Add additional vendors and secure references here as needed.
