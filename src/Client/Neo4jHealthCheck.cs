using Microsoft.Extensions.Diagnostics.HealthChecks;
using Neo4j.Driver;

namespace NorthernNerds.Aspire.Neo4j;

internal sealed class Neo4jHealthCheck(IDriver driver) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await driver.VerifyConnectivityAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
