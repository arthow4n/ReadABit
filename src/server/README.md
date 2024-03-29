# ReadABit Server

## Development 

- Run `./scripts/init.sh` to install the dependencies
- Fix appsettings for dev environment
  ```
  cd ./ReadABit.Web
  cp appsettings.json appsettings.Development.json
  # Edit the file, use whatever editor you like.
  code -r appsettings.Development.json
  cd ..
  ```
- Setup DB secrets
  ```
  cd ./ReadABit.Web
  dotnet user-secrets set ConnectionStrings:CoreDbContext "Host=localhost;Username=...;Password=...;Database=readabit"
  cd ..
  
  cd ./ReadABit.Web.Test
  dotnet user-secrets set ConnectionStrings:CoreDbContext "Host=localhost;Username=...;Password=...;Database=readabit-test"
  cd ..
  ```
- Setup OpenIddict
  ```
  cd ./ReadABit.CliUtils
  dotnet run -- seed
  dotnet run -- cert
  
  cd ../ReadABit.Web
  # run the commands you got from the output of `dotnet run -- cert`
  ```
- Refer to `./scripts` or VS Code tasks (`ctrl-shift-p` -> Task) for some other snippets
