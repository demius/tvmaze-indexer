# TV Maze Indexer

This is a solution to a coding assignment which entails the creation of a service to ingest basic TV show & cast information from the [TV Maze](https://www.tvmaze.com/) database (using their API).
The stated requirements are to:
- create an ingestion service
- expose a REST API to query ingested data (with some specific rules around ordering)

## Design considerations

The idea was to split the solution into two distinct applications, namely the _Ingester_ which is responsible for seeding the index
and then enriching the TV shows with more information (e.g. cast, crew etc.). The second application is naturally the REST API which exposes the indexed
data.

I wanted to create a service that could run on a single host with _minimal_ dependencies, 
thus the solution relies mostly on standard .NET libraries, with a few mature NuGet dependencies to meet specific challenges, namely:

[Polly](https://github.com/App-vNext/Polly)
Allows for easy implementation of specific communication patterns for HTTP API consumers (e.g. circuit-breaker, retry, request caching etc.). Polly allows us to easily deal with TV Maze's rate-limits.

[Flurl](https://flurl.dev/docs/fluent-http/)
A really elegent fluent abstraction over the built-in .NET web client which takes care of some of the (annoying) problems of client instantiation and caching.

[Quartz](https://www.quartz-scheduler.net/)
A **super** mature scheduler framework for .NET which we use to schedule indexing runs within the ingester with high precision.

## TODO

Unfortunately, given the time constraints I wasn't able to finalise the solution to a production standard.
If I were to do it again, I'd probably just use Azure Functions in a fan-out pattern, but alas, I decided to take the "hard" road.
Here's a list of (some) things I didn't get around to:

- [ ] Unit-tests
- [ ] Splitting enrichment workers into a separate application
- [ ] Competing consumers (task workers)
- [ ] Character information enrichment
- [ ] Docker support

# Closing thoughts

This was a really fun project and I really enjoyed it! :)
