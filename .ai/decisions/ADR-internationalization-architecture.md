# ADR: Server-Side Internationalization Architecture

**Date:** January 1, 2026
**Status:** Accepted
**Owner:** @Architect
**Stakeholders:** @Backend, @Frontend, @Legal, @ProductOwner

## Context

B2Connect requires comprehensive internationalization support for a global user base. The system needs to handle multiple languages while maintaining performance, security, and user experience standards.

## Decision

**Server-side internationalization with client-side language preference management.**

### Architecture Principles

1. **Server-Side Localization**: All entity data is localized on the server before being sent to clients
2. **Client Language Preference**: Clients send language preferences via HTTP headers or user settings
3. **No Client-Side Translation**: Clients do not perform translation of entity data
4. **Cache-Friendly**: Localized data can be cached at appropriate levels
5. **Fallback Strategy**: Graceful degradation to default language when translations are missing

### Implementation Details

#### Backend (Domain Layer)
- **Localization Service**: `ILocalizationService` provides translation capabilities
- **Entity Localization**: Domain entities include localization metadata
- **Culture Context**: Request context carries current culture information
- **Translation Storage**: Translations stored in dedicated localization tables/collections

#### API Gateway Layer
- **Culture Middleware**: Extracts language preference from `Accept-Language` header
- **Culture Propagation**: Passes culture context through request pipeline
- **Localized DTOs**: API responses contain pre-localized data

#### Frontend
- **Language Detection**: Browser language detection with user override capability
- **Header Injection**: `Accept-Language` header sent with all API requests
- **Language Switching**: Triggers re-fetch of data with new language preference
- **No Translation Logic**: Frontend focuses on presentation, not translation

### Data Flow

```
User changes language → Frontend updates Accept-Language header → API request with new header → Server localizes data → Client receives localized entities → UI updates
```

### Benefits

1. **Performance**: Translation happens once on server, cached results served to multiple clients
2. **Consistency**: Single source of truth for translations
3. **Security**: Translation logic secured on server-side
4. **SEO**: Server-side rendering supports search engine indexing
5. **Caching**: CDN and application-level caching work effectively
6. **Maintainability**: Translation updates don't require client deployments

### Trade-offs

1. **Network Overhead**: Language changes require re-fetching data
2. **Server Load**: Translation processing on every request
3. **Latency**: Additional processing time for localization

### Alternatives Considered

#### Client-Side Translation
- **Rejected**: Would require shipping all translations to client, increasing bundle size
- **Rejected**: Translation logic exposed to client-side inspection
- **Rejected**: Inconsistent translation state across different client versions

#### Hybrid Approach
- **Rejected**: Would complicate caching and state management
- **Rejected**: Mixed responsibilities between client and server

## Consequences

### Positive
- ✅ Consistent translation across all clients
- ✅ Better performance through server-side caching
- ✅ Improved security (translations not exposed to client)
- ✅ Easier maintenance of translation data
- ✅ Better SEO support

### Negative
- ❌ Additional server processing for each request
- ❌ Language switches require data re-fetch
- ❌ Increased complexity in server-side localization logic

### Implementation Requirements

1. **Localization Service**: Implement `ILocalizationService` with caching
2. **Culture Middleware**: Add to API Gateway pipeline
3. **Translation Management**: Admin interface for translation updates
4. **Fallback Handling**: Default language fallback for missing translations
5. **Testing**: Localization testing across all supported languages

## Compliance

- **BITV 2.0**: Ensures accessibility through proper language support
- **GDPR**: Language preferences stored securely with user consent
- **Performance**: Localization caching to maintain response times

## Monitoring

- Translation coverage metrics
- Language preference distribution
- Localization performance monitoring
- Missing translation alerts

---

**Status:** ✅ Accepted - Server-side internationalization provides the best balance of performance, security, and maintainability for B2Connect's global requirements.</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/decisions/ADR-internationalization-architecture.md