# ReadABit Server

## Development 

- Run `./scripts/init.sh` to install the dependencies
- Setup DB secrets
```
cd ./ReadABit.Web
dotnet user-secrets set ConnectionStrings:CoreDbContext "Host=localhost;Username=...;Password=...;Database=readabit"
cd ..

cd ./ReadABit.Web.Test
dotnet user-secrets set ConnectionStrings:CoreDbContext "Host=localhost;Username=...;Password=...;Database=readabit-test"
cd ..
```
- Refer to `./scripts` or VS Code tasks (`ctrl-shift-p` -> Task) for some other snippets
