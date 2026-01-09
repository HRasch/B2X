#!/bin/bash

# Script to fix project reference paths in all test projects
# Changes paths like "../../shared/..." to "../../../src/shared/..."
# and "../../Store/..." to "../../../src/Store/..."

echo "Fixing project reference paths in all test projects..."

# Find all .csproj files in tests directory
find /Users/holger/Documents/Projekte/B2X/tests -name "*.Tests.csproj" -type f | while read -r file; do
    echo "Processing: $file"

    # Use sed to replace the incorrect paths
    # Pattern: ../../shared/ -> ../../../src/shared/
    sed -i '' 's|../../shared/|../../../src/shared/|g' "$file"

    # Pattern: ../../Store/ -> ../../../src/Store/
    sed -i '' 's|../../Store/|../../../src/Store/|g' "$file"

    # Pattern: ../../../shared/ -> ../../../src/shared/ (for kernel, etc.)
    sed -i '' 's|../../../shared/|../../../src/shared/|g' "$file"

    # Pattern: ../../Admin/ -> ../../../src/Admin/
    sed -i '' 's|../../Admin/|../../../src/Admin/|g' "$file"

    # Note: ../B2X.*.API.csproj patterns are handled manually due to regex complexity

    echo "Fixed: $file"
done

echo "All test project reference paths have been updated."