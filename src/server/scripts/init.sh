#!/bin/sh
set -euo pipefail
REPO_ROOT="${0%/*}/../";

cd "${REPO_ROOT}"

dotnet restore

# TODO: Install external dependencies that are not covered by dotnet restore
