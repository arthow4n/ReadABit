#!/bin/sh
set -euo pipefail
cd "${0%/*}/../"

cd ReadABit.Web
dotnet ef database update "$1" --project ../ReadABit.Infrastructure
