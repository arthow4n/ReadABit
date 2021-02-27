#!/bin/sh
set -euo pipefail
cd "${0%/*}/../"

cd ReadABit.Web
if [[ -z "${1-}" ]]; then
    dotnet ef database update --project ../ReadABit.Infrastructure
else
    dotnet ef database update "$1" --project ../ReadABit.Infrastructure
fi
