# Release Guide

This guide outlines the steps required to release new versions of the NorthernNerds.Aspire.Neo4j packages.

## Package Structure

We maintain two NuGet packages:
- `NorthernNerds.Aspire.Neo4j` - Client integration
- `NorthernNerds.Aspire.Hosting.Neo4j` - Hosting integration

## Steps to Release a New Version

1. **Update Version Numbers**:
   Update both project files with the same version number:
   ```bash
   src/Client/NorthernNerds.Aspire.Neo4j.csproj
   src/Hosting/NorthernNerds.Aspire.Hosting.Neo4j.csproj
   ```
   ```xml
   <PropertyGroup>
     <Version>2.0.0</Version>
   </PropertyGroup>
   ```

2. **Commit Version Updates**:
   ```bash
   git add src/Client/NorthernNerds.Aspire.Neo4j.csproj
   git add src/Hosting/NorthernNerds.Aspire.Hosting.Neo4j.csproj
   git commit -m "Update version to 2.0.0"
   ```
3. **Create and Push Tag**:
   ```bash
   git tag v2.0.0
   git push origin v2.0.0
   ```

4. **Monitor Release**:
 - The GitHub Actions workflow will build and publish both packages
 - Monitor progress in the "Actions" tab on GitHub

## Important Notes

- Both packages must share the same version number
- Tag pattern must be `v*.*.*` (e.g., v2.0.0)
- Versions follow SemVer (MAJOR.MINOR.PATCH)