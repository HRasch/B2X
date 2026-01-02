#!/bin/bash
# Enable PR Quality Gate - GitHub Branch Protection
# This script configures GitHub branch protection rules via GitHub CLI

set -e

REPO="B2Connect/b2connect"  # Update with your actual repo
BRANCH="main"

echo "🔐 Enabling PR Quality Gate for ${REPO}:${BRANCH}"

# Check if gh CLI is installed
if ! command -v gh &> /dev/null; then
    echo "❌ GitHub CLI not found. Install it first:"
    echo "   brew install gh"
    echo "   gh auth login"
    exit 1
fi

# Check if authenticated
if ! gh auth status &> /dev/null; then
    echo "❌ Not authenticated with GitHub. Run: gh auth login"
    exit 1
fi

echo "✅ GitHub CLI authenticated"

# Enable branch protection
echo "🔒 Configuring branch protection rules..."

gh api -X PUT "/repos/${REPO}/branches/${BRANCH}/protection" \
  --input - <<EOF
{
  "required_status_checks": {
    "strict": true,
    "contexts": [
      "fast-checks",
      "unit-tests",
      "integration-tests",
      "e2e-tests",
      "security-compliance",
      "quality-gate"
    ]
  },
  "enforce_admins": false,
  "required_pull_request_reviews": {
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": true,
    "required_approving_review_count": 2,
    "require_last_push_approval": true
  },
  "restrictions": null,
  "required_linear_history": true,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "required_conversation_resolution": true,
  "lock_branch": false,
  "allow_fork_syncing": true
}
EOF

echo "✅ Branch protection enabled!"
echo ""
echo "📋 Configuration:"
echo "  - Required status checks: ✅"
echo "  - Minimum 2 approvals: ✅"
echo "  - Code owner review: ✅"
echo "  - Dismiss stale reviews: ✅"
echo "  - Require linear history: ✅"
echo "  - Conversation resolution: ✅"
echo ""
echo "🎉 PR Quality Gate is now ACTIVE!"
echo ""
echo "Next steps:"
echo "  1. Create a test PR"
echo "  2. Verify all checks run"
echo "  3. Monitor metrics in .ai/status/"
