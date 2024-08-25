
   # Release Guide

   This guide outlines the steps required to release a new version of the `NorthernNerds.Aspire.Neo4j` NuGet package.

   ## Triggering a New Build and Release

   The release process is automated through GitHub Actions and is triggered by pushing a new Git tag that follows the pattern `v*.*.*` (e.g., `v1.0.0`, `v1.1.0`).

   ### Steps to Release a New Version

   1. **Update the Version Number**:
      - Open the `src/NorthernNerds.Aspire.Neo4j.csproj` file.
      - Update the `<Version>` element to reflect the new version number you want to release.

      ```xml
      <PropertyGroup>
        <Version>1.1.0</Version>
        <!-- Other properties -->
      </PropertyGroup>
      ```

   2. **Commit the Version Update**:
      - Commit your changes to the main branch.

      ```bash
      git add src/NorthernNerds.Aspire.Neo4j.csproj
      git commit -m "Update version to 1.1.0"
      ```

   3. **Create and Push a New Tag**:
      - Create a new tag with the version number.

      ```bash
      git tag v1.1.0
      git push origin v1.1.0
      ```

   4. **Monitor the GitHub Actions Workflow**:
      - After pushing the tag, the GitHub Actions workflow will automatically start. You can monitor the build and publish process in the "Actions" tab on GitHub.

   ### Important Notes

   - Ensure that the version number in the `.csproj` file matches the tag you create.
   - The GitHub Actions workflow will only trigger on tags that follow the `v*.*.*` pattern.

   For any further details, refer to the [nuget-publish.yml](../.github/workflows/nuget-publish.yml) file in the repository.
