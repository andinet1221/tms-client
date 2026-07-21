# TMS API Versioning Policy

## Purpose
This document defines how the TMS API evolves over time, what changes require
a new version, and how we communicate those changes to API consumers. Any
engineer proposing a change to a public endpoint should be able to check this
page and know, within a few minutes, whether the change is safe to ship
directly or requires a new version.

## What Counts as a Breaking Change
A change is breaking if an existing client, written against the current
contract, could fail or behave incorrectly after the change ships without
any code changes on their end. This includes:

- Removing a field from a response
- Renaming a field
- Changing a field's data type (e.g. string to integer)
- Changing a status code returned for an existing scenario
- Tightening request validation (making previously-valid input invalid)
- Changing a default sort order or default page size
- Changing the meaning of an existing field
- Removing or renaming an endpoint

Any of the above requires a new API version (a new URL segment, e.g. `/api/v3/`).

## What Counts as Non-Breaking (Additive)
These changes are safe to ship into an existing version without bumping it,
because well-behaved clients ignore fields and parameters they don't
recognize:

- Adding a new optional field to a response
- Adding a new endpoint
- Adding a new optional query parameter
- Adding a new optional field to a request body (with a sensible default)
- Loosening validation (accepting input that was previously rejected)

## Sunset Window
Once a new major version ships, the previous version stays live for a
**minimum of 6 months** before it is shut down. This window exists because
some of our clients (e.g. rural training centres) only apply updates on a
quarterly maintenance cycle, so they need at least two maintenance windows to
migrate safely. The sunset date is fixed at the time the new version ships
and is not moved up, even under pressure to deprecate old code sooner.

## Communication Plan
From day one that a new version ships, the following happens automatically
or as a checklist item:

1. **HTTP headers on every old-version response**: `Deprecation: true`,
   `Sunset: <date>`, and `Link: <new-version-url>; rel="successor-version"`.
   These are the source of truth — any client inspecting its own traffic can
   discover the migration path without reading documentation.
2. **CHANGELOG entry**: added to the repo the same day the new version
   ships, describing what changed and why.
3. **Direct email**: sent to every team or partner holding an active API key
   for the old version, linking to the CHANGELOG and the new version's docs.
4. **Calendar invite**: sent for the exact sunset/shutdown date, so the
   deadline is on record and not just buried in an email thread.

## Version Skipping
Clients are never required to upgrade through every intermediate version.
A client on v1 is free to skip directly to v3 once v3 ships — they do not
need to first integrate with v2. Each version's contract is documented
independently so a client can jump straight to whichever version fits their
needs.