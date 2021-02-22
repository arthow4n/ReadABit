#!/bin/sh
set -euo pipefail
cd "${0%/*}/../"

dotnet nswag run ReadABit.Web/ReadABit.Web.nswag
