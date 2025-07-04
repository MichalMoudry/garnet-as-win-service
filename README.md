# Garnet cache as a Windows service
A repository containing an implementation of a Windows service containing Garnet server.

Goal of this project is to package Garnet as a Windows service with additional functionality,
like Azure Key Vault integration.

Link to Garnet repository: [Garnet](https://github.com/microsoft/Garnet "Link to Garnet repository")

## Getting started
To get started with this service you will need .NET 9 SDK installed. To verify/work
with a secret/key vault, you will need a local instance or a running instance of
the Azure Key Vault service.

## Repository structure
- `/src` - a folder containing source code...
- `/test` - a folder with projects that are related to testing
- `/build` - a folder with scripts for building, installing and running the cache service

## Solution structure
### CacheService
The main project of the solution. This project has a [worker application model](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers "Link to .NET worker documentation").
Garnet cache is running as part of [GarnetService](./src/CacheService/GarnetService.cs "Link to GarnetService source file").
In this project there is also logic related to environment and configuration handling.
### CacheService.TestClient
A test project for working with/verifying local instance of the Garnet server.
This project uses NUnit testing framework.
### CacheService.UnitTests
A F# test project containing service's unit tests. This project uses NUnit testing framework
and NSubstitute library for mocking. Also, this project/library is marked as a friend assembly
by **CacheService** project, so **CacheService.UnitTests** is able to see its internal types.

## Deployment
Currently, there is only a manual option for deployment. The deployment process
consists of the following steps:
1. Create a release of the cache service. The command can be found here
[release script](./build/release.ps1).
2. (Optional) Zip all the release files.
3. Upload files to a target environment/machine.
4. Run [install script](./src/CacheService/install_garnet.ps1) that is
included in the release.

**Note**: to install the service, you can use the [uninstall script](./src/CacheService/remove_garnet.ps1)
or just remove it through sc.exe utility or GUI in Windows.

## Configuration
This service currently supports configuration of things like host address, port
or password (only for dev environment). Also, there is the integration with Azure
Key Vault service.

**Configuration sources**:
- JSON files (appsettings.json)
- Command-line arguments
### Configuration options
| Option                        | Description                                                                                                                                                                         |
|-------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Environment**               | Certain parts of the service work only in specific environments (like password configuration).<br/>It is possible to override this option through a supported configuration source. |
| **Host address**              | By default, host address is 0.0.0.0.                                                                                                                                                |
| **Port**                      | Default port is the same as a default port for Redis which is 6379.                                                                                                                 |
| **Password** (in dev)         |                                                                                                                                                                                     |
| **Password** (outside of dev) |                                                                                                                                                                                     |
### Azure Key Vault integration
TODO
