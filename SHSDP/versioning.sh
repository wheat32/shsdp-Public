#!/bin/bash
set -e

versionFile="$(dirname "$0")/version.json"

# The first argument passed in (optional) will be the build configuration
buildConfig="$1"

# Read version.json into variables
major=$(jq '.MajorVersion' "$versionFile")
minor=$(jq '.MinorVersion' "$versionFile")
patch=$(jq '.PatchNumber' "$versionFile")
build=$(jq '.BuildNumber' "$versionFile")
lastCommitID=$(jq -r '.LastCommitID' "$versionFile")

# Get latest Git commit hash
latestCommit=$(git log --format="%H" -n 1)

# Always update LastCommitID, but only increment when in DEBUG
if [[ "$buildConfig" == "Debug" ]]; then
    # Increment patch if commit ID changed
    if [[ "$lastCommitID" != "$latestCommit" ]]; then
        patch=$((patch + 1))
    fi

    # Increment build number
    build=$((build + 1))
fi

# Create new version JSON using jq
jq -n \
    --argjson major "$major" \
    --argjson minor "$minor" \
    --argjson patch "$patch" \
    --argjson build "$build" \
    --arg lastCommit "$latestCommit" \
    '{
        MajorVersion: $major,
        MinorVersion: $minor,
        PatchNumber: $patch,
        BuildNumber: $build,
        LastCommitID: $lastCommit
    }' > "$versionFile"
