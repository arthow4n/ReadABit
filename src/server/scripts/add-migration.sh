#!/bin/sh
set -euo pipefail
cd "${0%/*}/../"

cd ReadABit.Web
dotnet ef migrations add "$1" --project ../ReadABit.Infrastructure
