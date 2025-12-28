#!/bin/bash

# GitHub Project Dashboard Creation Script for B2Connect
# Creates a Kanban board project for sprint organization
# Usage: ./create-github-project.sh

set -e

# Color output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 B2Connect GitHub Project Dashboard Setup${NC}"
echo "=================================================="
echo ""

# Check if gh CLI is installed
if ! command -v gh &> /dev/null; then
    echo -e "${RED}❌ GitHub CLI (gh) is not installed${NC}"
    echo "Install from: https://cli.github.com/"
    exit 1
fi

# Get repository info
REPO=$(gh repo view --json nameWithOwner -q '.nameWithOwner')
if [ -z "$REPO" ]; then
    echo -e "${RED}❌ Not in a GitHub repository${NC}"
    exit 1
fi

echo -e "${GREEN}✓ Repository: $REPO${NC}"
echo ""

# Create the project
echo -e "${BLUE}📋 Creating GitHub Project...${NC}"

PROJECT_NAME="B2Connect Roadmap (Phases 0-3)"
PROJECT_DESCRIPTION="Multi-phase compliance and feature implementation roadmap using sprint-based kanban workflow"

# Extract owner and repo
OWNER=$(echo $REPO | cut -d'/' -f1)
REPO_NAME=$(echo $REPO | cut -d'/' -f2)

# Use GraphQL to create project without template (will be table by default)
# Important: Use GraphQL node ID, not REST API ID (they have different formats)
OWNER_ID=$(gh api graphql -f query='{ viewer { id } }' --jq '.data.viewer.id')

# Create project via GraphQL API
GRAPHQL_QUERY='mutation($ownerId:ID!, $name:String!) {
  createProjectV2(input: {ownerId: $ownerId, title: $name}) {
    projectV2 {
      id
      number
      title
      url
    }
  }
}'

PROJECT_RESPONSE=$(gh api graphql \
  -f query="$GRAPHQL_QUERY" \
  -f ownerId="$OWNER_ID" \
  -f name="$PROJECT_NAME")

PROJECT_ID=$(echo "$PROJECT_RESPONSE" | jq -r '.data.createProjectV2.projectV2.id')
PROJECT_NUMBER=$(echo "$PROJECT_RESPONSE" | jq -r '.data.createProjectV2.projectV2.number')
PROJECT_URL=$(echo "$PROJECT_RESPONSE" | jq -r '.data.createProjectV2.projectV2.url')

if [ -z "$PROJECT_ID" ] || [ "$PROJECT_ID" == "null" ]; then
    echo -e "${RED}❌ Failed to create project${NC}"
    exit 1
fi

echo -e "${GREEN}✓ Project created: #$PROJECT_NUMBER${NC}"
echo -e "${GREEN}✓ URL: $PROJECT_URL${NC}"
echo ""

# Add Status field (for kanban columns)
echo -e "${BLUE}📌 Adding Status field...${NC}"

STATUS_FIELD_RESPONSE=$(gh api graphql \
  -f query='mutation($projectId:ID!, $name:String!, $dataType:ProjectV2CustomFieldType!) {
    createProjectV2Field(input: {projectId: $projectId, dataType: $dataType, name: $name}) {
      projectV2Field {
        id
      }
    }
  }' \
  -f projectId="$PROJECT_ID" \
  -f name="Status" \
  -f dataType="SINGLE_SELECT")

STATUS_FIELD_ID=$(echo "$STATUS_FIELD_RESPONSE" | jq -r '.data.createProjectV2Field.projectV2Field.id')

if [ -z "$STATUS_FIELD_ID" ] || [ "$STATUS_FIELD_ID" == "null" ]; then
    echo -e "${YELLOW}⚠ Status field may already exist or couldn't be created${NC}"
else
    echo -e "${GREEN}✓ Status field created${NC}"
fi

echo ""

# Add Sprint field
echo -e "${BLUE}📌 Adding Sprint field...${NC}"

SPRINT_FIELD_RESPONSE=$(gh api graphql \
  -f query='mutation($projectId:ID!, $name:String!, $dataType:ProjectV2CustomFieldType!) {
    createProjectV2Field(input: {projectId: $projectId, dataType: $dataType, name: $name}) {
      projectV2Field {
        id

      }
    }
  }' \
  -f projectId="$PROJECT_ID" \
  -f name="Sprint" \
  -f dataType="SINGLE_SELECT")

SPRINT_FIELD_ID=$(echo "$SPRINT_FIELD_RESPONSE" | jq -r '.data.createProjectV2Field.projectV2Field.id')

if [ -z "$SPRINT_FIELD_ID" ] || [ "$SPRINT_FIELD_ID" == "null" ]; then
    echo -e "${YELLOW}⚠ Sprint field may already exist or couldn't be created${NC}"
else
    echo -e "${GREEN}✓ Sprint field created${NC}"
fi

echo ""

# Add Priority field
echo -e "${BLUE}📌 Adding Priority field...${NC}"

PRIORITY_FIELD_RESPONSE=$(gh api graphql -f query='
mutation($projectId:ID!, $name:String!, $dataType:ProjectV2CustomFieldType!) {
  createProjectV2Field(input: {projectId: $projectId, dataType: $dataType, name: $name}) {
    projectV2Field {
      id
    }
  }
}' -f projectId="$PROJECT_ID" -f name="Priority" -f dataType="SINGLE_SELECT")

PRIORITY_FIELD_ID=$(echo "$PRIORITY_FIELD_RESPONSE" | jq -r '.data.createProjectV2Field.projectV2Field.id')

if [ -z "$PRIORITY_FIELD_ID" ] || [ "$PRIORITY_FIELD_ID" == "null" ]; then
    echo -e "${YELLOW}⚠ Priority field may already exist or couldn't be created${NC}"
else
    echo -e "${GREEN}✓ Priority field created${NC}"
fi

echo ""

# Summary
echo -e "${GREEN}═══════════════════════════════════════════${NC}"
echo -e "${GREEN}✅ GitHub Project Dashboard Created!${NC}"
echo -e "${GREEN}═══════════════════════════════════════════${NC}"
echo ""
echo -e "Project: ${BLUE}$PROJECT_NAME${NC}"
echo -e "URL: ${BLUE}$PROJECT_URL${NC}"
echo ""
echo -e "${YELLOW}📝 Next Steps:${NC}"
echo "1. Open the project in your browser"
echo "2. Set up Kanban columns:"
echo "   • 📋 Backlog"
echo "   • 🔄 In Progress"
echo "   • 👀 In Review"
echo "   • ✅ Done"
echo "   • 🔴 Blocked"
echo ""
echo "3. Add Sprint values (S1, S2, S3, etc.)"
echo "4. Create GitHub issues for P0 components"
echo "5. Add issues to project and assign to sprints"
echo ""
echo -e "${BLUE}ℹ️  Useful gh commands:${NC}"
echo "  # List project items:"
echo "  gh project item-list $PROJECT_NUMBER --owner $(echo $REPO | cut -d/ -f1)"
echo ""
echo "  # Add an issue to project:"
echo "  gh project item-add <issue-number> --owner $(echo $REPO | cut -d/ -f1) --project $PROJECT_NUMBER"
echo ""
