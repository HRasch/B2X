#!/bin/bash
# ============================================================================
# B2Connect GitHub Project Board Setup Script
# ============================================================================
# This script creates a project board and configures a Status field
# using the GitHub CLI and GraphQL API
# ============================================================================

set -e  # Exit on error

# Configuration
REPO_OWNER="HRasch"
REPO_NAME="B2Connect"
PROJECT_TITLE="B2Connect Compliance Roadmap"
PROJECT_ID=""

echo "============================================"
echo "B2Connect GitHub Project Board Setup"
echo "============================================"

# Step 1: Get the repository node ID
echo ""
echo "Step 1: Getting repository node ID..."
REPO_ID=$(gh api graphql -f query='
query($owner: String!, $name: String!) {
  repository(owner: $owner, name: $name) {
    id
  }
}' -f owner="$REPO_OWNER" -f name="$REPO_NAME" --jq '.data.repository.id')

if [ -z "$REPO_ID" ]; then
    echo "❌ Error: Could not get repository ID"
    exit 1
fi
echo "✅ Repository ID: $REPO_ID"

# Step 2: Get the user node ID (for personal repos)
echo ""
echo "Step 2: Getting user node ID..."
OWNER_ID=$(gh api graphql -f query='
query {
  viewer {
    id
  }
}' --jq '.data.viewer.id')

if [ -z "$OWNER_ID" ]; then
    echo "❌ Error: Could not get owner ID"
    exit 1
fi
echo "✅ Owner ID: $OWNER_ID"

# Step 3: Check if project already exists
echo ""
echo "Step 3: Checking for existing project..."
EXISTING_PROJECT=$(gh api graphql -f query='
query($owner: String!) {
  user(login: $owner) {
    projectsV2(first: 20) {
      nodes {
        id
        title
      }
    }
  }
}' -f owner="$REPO_OWNER" --jq ".data.user.projectsV2.nodes[] | select(.title == \"$PROJECT_TITLE\") | .id" 2>/dev/null || echo "")

if [ -n "$EXISTING_PROJECT" ]; then
    echo "✅ Project already exists: $EXISTING_PROJECT"
    PROJECT_ID="$EXISTING_PROJECT"
else
    # Step 4: Create the project
    echo ""
    echo "Step 4: Creating project board..."
    PROJECT_ID=$(gh api graphql -f query='
    mutation($ownerId: ID!, $title: String!) {
      createProjectV2(input: {ownerId: $ownerId, title: $title}) {
        projectV2 {
          id
          title
          url
        }
      }
    }' -f ownerId="$OWNER_ID" -f title="$PROJECT_TITLE" --jq '.data.createProjectV2.projectV2.id')

    if [ -z "$PROJECT_ID" ]; then
        echo "❌ Error: Could not create project"
        exit 1
    fi
    echo "✅ Project created: $PROJECT_ID"
fi

# Step 5: Get the project URL
echo ""
echo "Step 5: Getting project details..."
PROJECT_URL=$(gh api graphql -f query='
query($id: ID!) {
  node(id: $id) {
    ... on ProjectV2 {
      url
      title
    }
  }
}' -f id="$PROJECT_ID" --jq '.data.node.url')
echo "✅ Project URL: $PROJECT_URL"

# Step 6: Link project to repository
echo ""
echo "Step 6: Linking project to repository..."
gh api graphql -f query='
mutation($projectId: ID!, $repositoryId: ID!) {
  linkProjectV2ToRepository(input: {projectId: $projectId, repositoryId: $repositoryId}) {
    repository {
      name
    }
  }
}' -f projectId="$PROJECT_ID" -f repositoryId="$REPO_ID" >/dev/null 2>&1 || echo "Project may already be linked"
echo "✅ Project linked to repository"

# Step 7: Check existing fields
echo ""
echo "Step 7: Checking existing fields..."
EXISTING_FIELDS=$(gh api graphql -f query='
query($id: ID!) {
  node(id: $id) {
    ... on ProjectV2 {
      fields(first: 20) {
        nodes {
          ... on ProjectV2Field {
            id
            name
          }
          ... on ProjectV2SingleSelectField {
            id
            name
            options {
              id
              name
            }
          }
        }
      }
    }
  }
}' -f id="$PROJECT_ID" --jq '.data.node.fields.nodes')
echo "Current fields:"
echo "$EXISTING_FIELDS" | jq -r '.[] | "  - \(.name)"'

# Check if Sprint field exists
SPRINT_FIELD_ID=$(echo "$EXISTING_FIELDS" | jq -r '.[] | select(.name == "Sprint") | .id' 2>/dev/null || echo "")

if [ -z "$SPRINT_FIELD_ID" ] || [ "$SPRINT_FIELD_ID" = "null" ]; then
    echo ""
    echo "Step 8: Creating Sprint field..."
    SPRINT_FIELD_ID=$(gh api graphql -f query='
    mutation($projectId: ID!, $name: String!, $dataType: ProjectV2CustomFieldType!, $options: [ProjectV2SingleSelectFieldOptionInput!]) {
      createProjectV2Field(input: {
        projectId: $projectId, 
        dataType: $dataType, 
        name: $name,
        singleSelectOptions: $options
      }) {
        projectV2Field {
          ... on ProjectV2SingleSelectField {
            id
            name
          }
        }
      }
    }' \
    -f projectId="$PROJECT_ID" \
    -f name="Sprint" \
    -f dataType="SINGLE_SELECT" \
    -f options='[{"name":"Sprint 1","color":"BLUE","description":"Week 1-2"},{"name":"Sprint 2","color":"GREEN","description":"Week 3-4"},{"name":"Sprint 3","color":"YELLOW","description":"Week 5-6"},{"name":"Sprint 4","color":"ORANGE","description":"Week 7-8"},{"name":"Sprint 5","color":"PURPLE","description":"Week 9-10"}]' \
    --jq '.data.createProjectV2Field.projectV2Field.id')
    echo "✅ Sprint field created: $SPRINT_FIELD_ID"
else
    echo "✅ Sprint field already exists: $SPRINT_FIELD_ID"
fi

# Check if Priority field exists
PRIORITY_FIELD_ID=$(echo "$EXISTING_FIELDS" | jq -r '.[] | select(.name == "Priority") | .id' 2>/dev/null || echo "")

if [ -z "$PRIORITY_FIELD_ID" ] || [ "$PRIORITY_FIELD_ID" = "null" ]; then
    echo ""
    echo "Step 9: Creating Priority field..."
    PRIORITY_FIELD_ID=$(gh api graphql -f query='
    mutation($projectId: ID!, $name: String!, $dataType: ProjectV2CustomFieldType!, $options: [ProjectV2SingleSelectFieldOptionInput!]) {
      createProjectV2Field(input: {
        projectId: $projectId, 
        dataType: $dataType, 
        name: $name,
        singleSelectOptions: $options
      }) {
        projectV2Field {
          ... on ProjectV2SingleSelectField {
            id
            name
          }
        }
      }
    }' \
    -f projectId="$PROJECT_ID" \
    -f name="Priority" \
    -f dataType="SINGLE_SELECT" \
    -f options='[{"name":"P0 - Critical","color":"RED","description":"Must complete before Phase 1"},{"name":"P1 - High","color":"ORANGE","description":"High priority"},{"name":"P2 - Medium","color":"YELLOW","description":"Medium priority"},{"name":"P3 - Low","color":"GREEN","description":"Low priority"}]' \
    --jq '.data.createProjectV2Field.projectV2Field.id')
    echo "✅ Priority field created: $PRIORITY_FIELD_ID"
else
    echo "✅ Priority field already exists: $PRIORITY_FIELD_ID"
fi

# Check if Component field exists
COMPONENT_FIELD_ID=$(echo "$EXISTING_FIELDS" | jq -r '.[] | select(.name == "Component") | .id' 2>/dev/null || echo "")

if [ -z "$COMPONENT_FIELD_ID" ] || [ "$COMPONENT_FIELD_ID" = "null" ]; then
    echo ""
    echo "Step 10: Creating Component field..."
    COMPONENT_FIELD_ID=$(gh api graphql -f query='
    mutation($projectId: ID!, $name: String!, $dataType: ProjectV2CustomFieldType!, $options: [ProjectV2SingleSelectFieldOptionInput!]) {
      createProjectV2Field(input: {
        projectId: $projectId, 
        dataType: $dataType, 
        name: $name,
        singleSelectOptions: $options
      }) {
        projectV2Field {
          ... on ProjectV2SingleSelectField {
            id
            name
          }
        }
      }
    }' \
    -f projectId="$PROJECT_ID" \
    -f name="Component" \
    -f dataType="SINGLE_SELECT" \
    -f options='[{"name":"P0.1 Audit Logging","color":"BLUE","description":"Immutable audit trail"},{"name":"P0.2 Encryption","color":"BLUE","description":"AES-256 at rest"},{"name":"P0.3 Incident Response","color":"BLUE","description":"NIS2 notifications"},{"name":"P0.4 Network","color":"BLUE","description":"Segmentation"},{"name":"P0.5 Key Management","color":"BLUE","description":"Azure KeyVault"},{"name":"P0.6 E-Commerce","color":"GREEN","description":"Legal compliance"},{"name":"P0.7 AI Act","color":"YELLOW","description":"AI governance"},{"name":"P0.8 BITV","color":"ORANGE","description":"Accessibility"},{"name":"P0.9 E-Rechnung","color":"PURPLE","description":"ZUGFeRD/UBL"}]' \
    --jq '.data.createProjectV2Field.projectV2Field.id')
    echo "✅ Component field created: $COMPONENT_FIELD_ID"
else
    echo "✅ Component field already exists: $COMPONENT_FIELD_ID"
fi

echo ""
echo "============================================"
echo "✅ Project Board Setup Complete!"
echo "============================================"
echo ""
echo "Project ID: $PROJECT_ID"
echo "Project URL: $PROJECT_URL"
echo ""
echo "Fields created:"
echo "  - Sprint (single select)"
echo "  - Priority (single select)"  
echo "  - Component (single select)"
echo ""
echo "Next steps:"
echo "  1. Open the project: $PROJECT_URL"
echo "  2. Add issues to the project"
echo "  3. Set up views (Board, Table, etc.)"
echo ""
