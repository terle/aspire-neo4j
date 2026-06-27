using System.Reflection;
using Neo4j.Driver;

namespace AspireApp1.Web.Movies;

/// <summary>
/// A small wrapper over the Neo4j <see cref="IDriver"/> that seeds and queries the canonical
/// Neo4j "Movies" sample graph (the dataset behind <c>:play movies</c> in Neo4j Browser).
/// </summary>
public sealed class MovieGraphService(IDriver driver, ILogger<MovieGraphService> logger)
{
    /// <summary>
    /// Loads the Movies sample graph if it has not been loaded yet. Safe to call repeatedly:
    /// the dataset uses <c>MERGE</c>, and an up-front count check avoids redundant writes.
    /// </summary>
    public async Task<int> EnsureSeededAsync(CancellationToken cancellationToken = default)
    {
        await using var session = driver.AsyncSession();

        var existing = await session.RunAsync("MATCH (m:Movie) RETURN count(m) AS count");
        var count = (await existing.SingleAsync())["count"].As<int>();
        if (count > 0)
        {
            return count;
        }

        logger.LogInformation("Seeding the Neo4j Movies sample graph...");

        // The dataset is a series of ';'-terminated statements. The .NET driver executes one
        // statement per call, so split and run each as its own autocommit query.
        foreach (var statement in ReadDatasetStatements())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await session.RunAsync(statement);
            await result.ConsumeAsync();
        }

        var seeded = await session.RunAsync("MATCH (m:Movie) RETURN count(m) AS count");
        return (await seeded.SingleAsync())["count"].As<int>();
    }

    /// <summary>Returns every movie with its release year and cast size, most recent first.</summary>
    public async Task<IReadOnlyList<MovieSummary>> GetMoviesAsync()
    {
        await using var session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read));

        var cursor = await session.RunAsync(
            """
            MATCH (m:Movie)
            OPTIONAL MATCH (m)<-[:ACTED_IN]-(p:Person)
            RETURN m.title AS title, m.released AS released, count(p) AS cast
            ORDER BY released DESC, title
            """);

        return await cursor.ToListAsync(r => new MovieSummary(
            r["title"].As<string>(),
            r["released"].As<int?>(),
            r["cast"].As<int>()));
    }

    /// <summary>Returns the cast (with roles) and directors for a single movie.</summary>
    public async Task<MovieDetail?> GetMovieAsync(string title)
    {
        await using var session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read));

        var cursor = await session.RunAsync(
            """
            MATCH (m:Movie {title: $title})
            OPTIONAL MATCH (actor:Person)-[role:ACTED_IN]->(m)
            WITH m, actor, role
            ORDER BY actor.name
            WITH m,
                 collect(DISTINCT {name: actor.name, roles: role.roles}) AS cast
            OPTIONAL MATCH (director:Person)-[:DIRECTED]->(m)
            RETURN m.title AS title, m.released AS released, m.tagline AS tagline,
                   cast, collect(DISTINCT director.name) AS directors
            """,
            new { title });

        if (!await cursor.FetchAsync())
        {
            return null;
        }

        var record = cursor.Current;
        var cast = record["cast"].As<List<IDictionary<string, object>>>()
            .Where(c => c["name"] is not null)
            .Select(c => new CastMember(
                c["name"].As<string>(),
                (c["roles"] as IEnumerable<object>)?.Select(x => x.ToString()!).ToArray() ?? []))
            .ToList();

        var directors = record["directors"].As<List<object>>()
            .Where(d => d is not null)
            .Select(d => d.ToString()!)
            .ToList();

        return new MovieDetail(
            record["title"].As<string>(),
            record["released"].As<int?>(),
            record["tagline"].As<string?>(),
            cast,
            directors);
    }

    /// <summary>
    /// Finds the shortest "co-star" chain between two actors — the classic "six degrees"
    /// query that graph databases make trivial. Returns the names along the path, or null
    /// if either actor is unknown or no chain exists.
    /// </summary>
    public async Task<IReadOnlyList<string>?> FindCoStarPathAsync(string fromActor, string toActor)
    {
        await using var session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read));

        var cursor = await session.RunAsync(
            """
            MATCH (from:Person {name: $fromActor}), (to:Person {name: $toActor})
            MATCH path = shortestPath((from)-[:ACTED_IN*..10]-(to))
            RETURN [n IN nodes(path) | coalesce(n.title, n.name)] AS hops
            """,
            new { fromActor, toActor });

        if (!await cursor.FetchAsync())
        {
            return null;
        }

        return cursor.Current["hops"].As<List<object>>().Select(o => o.ToString()!).ToList();
    }

    private static IEnumerable<string> ReadDatasetStatements()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Movies.movies.cypher";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded dataset '{resourceName}' was not found.");
        using var reader = new StreamReader(stream);
        var script = reader.ReadToEnd();

        return script
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => s.Length > 0);
    }
}

/// <summary>A movie with its release year and number of credited actors.</summary>
public sealed record MovieSummary(string Title, int? Released, int CastCount);

/// <summary>Full detail for a movie: cast (with roles) and directors.</summary>
public sealed record MovieDetail(
    string Title,
    int? Released,
    string? Tagline,
    IReadOnlyList<CastMember> Cast,
    IReadOnlyList<string> Directors);

/// <summary>An actor and the role(s) they played in a given movie.</summary>
public sealed record CastMember(string Name, IReadOnlyList<string> Roles);
