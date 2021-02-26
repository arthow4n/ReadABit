#!/bin/sh
set -euo pipefail
REPO_ROOT="${0%/*}/../";

cd "${REPO_ROOT}"

dotnet restore

# TODO: Install external dependencies that are not covered by dotnet restore

mkdir -p tmp
cd tmp

curl -L --remote-name-all \
    # UDPipe binary & C# binding
    https://github.com/ufal/udpipe/releases/download/v1.2.0/udpipe-1.2.0-bin.zip \
    # UDPipe model
    https://lindat.mff.cuni.cz/repository/xmlui/bitstream/handle/11234/1-1659/udpipe-ud-1.2-160523.zip

unzip -oj udpipe-ud-1.2-160523.zip udpipe-ud-1.2-160523/swedish-ud-1.2-160523.udpipe
unzip -o ./udpipe-1.2.0-bin.zip 'udpipe-1.2.0-bin/bin-win64/csharp/**'
cp -r ../udpipe-1.2.0-bin/bin-win64/csharp "${REPO_ROOT}/ReadABit.Web/Integrations"
cd "${REPO_ROOT}/ReadABit.Web/Integrations"
mkdir -p Ufal/UDPipe/runtime
mv udpipe_csharp.dll Ufal/UDPipe/runtime
cp "${REPO_ROOT}/tmp/swedish-ud-1.2-160523.udpipe" Ufal/UDPipe/runtime

cd "${REPO_ROOT}"
