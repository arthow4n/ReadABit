#!/bin/sh
set -euo pipefail
cd "${0%/*}/../"

dotnet restore

# TODO: Install external dependencies that are not covered by dotnet restore
