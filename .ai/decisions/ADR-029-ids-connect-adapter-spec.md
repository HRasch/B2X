---
docid: ADR-069
title: ADR 029 Ids Connect Adapter Spec
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# IDS Connect Adapter - Technische Spezifikation

**Status:** Spezifikation  
**Datum:** 3. Januar 2026  
**Owner:** @Backend  
**Referenz:** [ADR-029](.ai/decisions/ADR-029-multi-format-punchout-integration.md)

---

## Übersicht

Der IDS Connect Adapter implementiert das **Shop-System-Interface** für direkten Katalogzugriff gemäß dem ITEK IDS Connect Standard. Dieser Adapter ermöglicht Shop-Systemen den direkten Zugriff auf B2X's Produktkatalog für Suchen, Anzeigen und Bestellungen.

### Zweck
- Direkter Produktkatalog-Zugriff für Shop-Systeme
- Integration mit bestehender Handwerkersoftware
- Unterstützung von Such- und Bestellprozessen

### Architekturelle Einordnung
```
Shop-System (Taifun, MSoft, etc.)
    ↓ HTTP/XML
IDS Connect Adapter
    ↓ Unified Gateway
B2X Core Services (Catalog, Search, Orders)
```

---

## Technische Anforderungen

### Protokoll & Kommunikation
- **HTTP Methoden**: GET, POST
- **Content-Type**: `application/xml`
- **Encoding**: UTF-8
- **Authentication**: API Key oder OAuth2
- **Timeout**: 30 Sekunden für alle Requests

### Datenformate
- **Request/Response**: IDS XML Schema
- **Fehlermeldungen**: Standardisierte XML Error Responses
- **Pagination**: Cursor-basierte Paginierung für große Resultsets

### Sicherheit
- **Transport**: HTTPS erforderlich
- **Authentication**: Tenant-spezifische API Keys
- **Authorization**: Granulare Berechtigungen (Read, Search, Order)
- **Audit**: Vollständiges Logging aller Zugriffe

---

## API-Endpunkte

### Base URL
```
/api/ids/{tenant-id}/
```

### 1. Katalogsuche
**Endpoint:** `GET /api/ids/{tenant-id}/catalog/search`

**Query Parameters:**
- `q` (string, required): Suchbegriff
- `category` (string, optional): Kategorie-Filter
- `page` (int, optional): Seitennummer (default: 1)
- `size` (int, optional): Ergebnisse pro Seite (default: 20, max: 100)

**Headers:**
- `Authorization`: `Bearer {api-key}`
- `Accept-Language`: Sprache für Lokalisierung (default: de)

**Response (XML):**
```xml
<IdsSearchResponse>
  <Products>
    <Product>
      <Id>{product-id}</Id>
      <Sku>{product-sku}</Sku>
      <Name>{localized-name}</Name>
      <Description>{localized-description}</Description>
      <Price>{price}</Price>
      <Currency>EUR</Currency>
      <StockLevel>{stock-level}</StockLevel>
      <Category>{category}</Category>
      <Manufacturer>{manufacturer}</Manufacturer>
    </Product>
    <!-- ... weitere Produkte ... -->
  </Products>
  <Pagination>
    <Page>1</Page>
    <Size>20</Size>
    <TotalResults>150</TotalResults>
    <TotalPages>8</TotalPages>
  </Pagination>
</IdsSearchResponse>
```

### 2. Produktdetails
**Endpoint:** `GET /api/ids/{tenant-id}/catalog/products/{sku}`

**Path Parameters:**
- `tenant-id` (string): Tenant-Identifier
- `sku` (string): Product SKU

**Headers:**
- `Authorization`: `Bearer {api-key}`
- `Accept-Language`: Sprache für Lokalisierung

**Response (XML):**
```xml
<IdsProductResponse>
  <Product>
    <Id>{product-id}</Id>
    <Sku>{product-sku}</Sku>
    <Name>{localized-name}</Name>
    <Description>{localized-description}</Description>
    <LongDescription>{detailed-description}</LongDescription>
    <Price>{price}</Price>
    <Currency>EUR</Currency>
    <StockLevel>{stock-level}</StockLevel>
    <Category>{category}</Category>
    <Manufacturer>{manufacturer}</Manufacturer>
    <TechnicalData>
      <Weight>{weight-kg}</Weight>
      <Dimensions>{dimensions}</Dimensions>
      <Material>{material}</Material>
    </TechnicalData>
    <Images>
      <Image>
        <Url>{image-url}</Url>
        <AltText>{alt-text}</AltText>
        <Type>primary</Type>
      </Image>
    </Images>
  </Product>
</IdsProductResponse>
```

### 3. Kategorien auflisten
**Endpoint:** `GET /api/ids/{tenant-id}/catalog/categories`

**Query Parameters:**
- `parent` (string, optional): Parent-Kategorie-ID für hierarchische Navigation

**Headers:**
- `Authorization`: `Bearer {api-key}`
- `Accept-Language`: Sprache für Lokalisierung

**Response (XML):**
```xml
<IdsCategoriesResponse>
  <Categories>
    <Category>
      <Id>{category-id}</Id>
      <Name>{localized-name}</Name>
      <ParentId>{parent-category-id}</ParentId>
      <ProductCount>{product-count}</ProductCount>
      <Subcategories>
        <Category>
          <Id>{subcategory-id}</Id>
          <Name>{localized-name}</Name>
        </Category>
      </Subcategories>
    </Category>
  </Categories>
</IdsCategoriesResponse>
```

### 4. Bestellung erstellen
**Endpoint:** `POST /api/ids/{tenant-id}/orders`

**Headers:**
- `Authorization`: `Bearer {api-key}`
- `Content-Type`: `application/xml`

**Request Body (XML):**
```xml
<IdsOrderRequest>
  <Customer>
    <Id>{customer-id}</Id>
    <Name>{customer-name}</Name>
    <Email>{customer-email}</Email>
  </Customer>
  <Items>
    <Item>
      <ProductSku>{product-sku}</ProductSku>
      <Quantity>{quantity}</Quantity>
      <UnitPrice>{unit-price}</UnitPrice>
    </Item>
  </Items>
  <ShippingAddress>
    <Street>{street}</Street>
    <City>{city}</City>
    <PostalCode>{postal-code}</PostalCode>
    <Country>DE</Country>
  </ShippingAddress>
  <Notes>{order-notes}</Notes>
</IdsOrderRequest>
```

**Response (XML):**
```xml
<IdsOrderResponse>
  <Order>
    <Id>{order-id}</Id>
    <OrderNumber>{order-number}</OrderNumber>
    <Status>confirmed</Status>
    <TotalAmount>{total-amount}</TotalAmount>
    <Currency>EUR</Currency>
    <EstimatedDelivery>{delivery-date}</EstimatedDelivery>
  </Order>
</IdsOrderResponse>
```

---

## Fehlerbehandlung

### Standard Error Response
```xml
<IdsErrorResponse>
  <Error>
    <Code>{error-code}</Code>
    <Message>{error-message}</Message>
    <Details>{additional-details}</Details>
  </Error>
</IdsErrorResponse>
```

### Error Codes
- `AUTH_001`: Ungültige API Key
- `AUTH_002`: Unzureichende Berechtigungen
- `TENANT_001`: Tenant nicht gefunden
- `PRODUCT_001`: Produkt nicht gefunden
- `ORDER_001`: Bestellung fehlgeschlagen
- `VALIDATION_001`: Ungültige Request-Daten

---

## Integration mit B2X Services

### Abhängige Services
- **Catalog Service**: Produkt-Daten und -Suche
- **Search Service**: Volltextsuche und Filter
- **Customer Service**: Kundendaten für Bestellungen
- **Order Service**: Bestell-Erstellung und -Verwaltung
- **Localization Service**: Mehrsprachige Inhalte

### Service Client Integration
```csharp
public class IdsConnectAdapter : IIdsConnectAdapter
{
    private readonly ICatalogServiceClient _catalogService;
    private readonly ISearchServiceClient _searchService;
    private readonly ICustomerServiceClient _customerService;
    private readonly IOrderServiceClient _orderService;
    private readonly ILocalizationServiceClient _localizationService;

    // Constructor injection der Service Clients
}
```

### Tenant-Kontext
- Alle Requests enthalten Tenant-ID im URL-Pfad
- Automatische Tenant-Isolation in allen Service-Calls
- Tenant-spezifische Konfigurationen (Preise, Verfügbarkeit, etc.)

---

## Authentifizierung & Autorisierung

### API Key Management
- Tenant-spezifische API Keys
- Granulare Berechtigungen pro Key:
  - `ids:read`: Katalog lesen
  - `ids:search`: Produkte suchen
  - `ids:order`: Bestellungen erstellen

### Beispiel API Key Konfiguration
```json
{
  "tenantId": "tenant-123",
  "apiKey": "ids-abc123def456",
  "permissions": ["ids:read", "ids:search", "ids:order"],
  "rateLimit": 1000,
  "validUntil": "2026-12-31"
}
```

---

## Performance & Skalierbarkeit

### Caching Strategie
- Produkt-Daten: 15 Minuten Cache
- Suchergebnisse: 5 Minuten Cache
- Kategorie-Hierarchie: 1 Stunde Cache

### Rate Limiting
- 1000 Requests pro Stunde pro API Key (konfigurierbar)
- Burst-Limit: 100 Requests pro Minute

### Asynchrone Verarbeitung
- Such-Requests: Synchrone Verarbeitung (< 500ms)
- Bestell-Erstellung: Asynchrone Verarbeitung mit Status-Tracking
- Bulk-Operations: Queue-basierte Verarbeitung

---

## Monitoring & Logging

### Metriken
- Request/Response Latenz
- Error Rate pro Endpoint
- Cache Hit Rate
- Tenant-spezifische Nutzungsstatistiken

### Logging
- Structured Logging für alle Requests
- Audit Trail für Bestellungen
- Error Tracking mit Correlation IDs

### Health Checks
- Service-Verfügbarkeit
- Datenbank-Konnektivität
- Cache-Performance

---

## Teststrategie

### Unit Tests
- XML Serialization/Deserialization
- Business Logic Validation
- Error Handling

### Integration Tests
- End-to-End mit Mock Services
- Tenant-Isolation Tests
- Authentication Tests

### Performance Tests
- Load Testing (1000 concurrent users)
- Memory Usage Tests
- Cache Performance Tests

---

## Deployment & Konfiguration

### Environment Variables
```bash
IDS_ADAPTER_PORT=8081
IDS_CACHE_TTL_PRODUCTS=900
IDS_CACHE_TTL_SEARCH=300
IDS_RATE_LIMIT_REQUESTS=1000
IDS_RATE_LIMIT_WINDOW=3600
```

### Docker Configuration
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081
```

### Kubernetes Deployment
- Horizontal Pod Autoscaling
- ConfigMaps für Tenant-Konfigurationen
- Secrets für API Keys

---

## Erweiterte Features

### Webhooks (Optional)
- Bestell-Status-Updates
- Produkt-Verfügbarkeitsänderungen
- Preis-Updates

### Analytics Integration
- Nutzungsstatistiken pro Tenant
- Beliebteste Suchbegriffe
- Konversionsraten

### Multi-Language Support
- Automatische Lokalisierung basierend auf Accept-Language Header
- Fallback auf Deutsch bei fehlenden Übersetzungen

---

## Risiken & Mitigation

### XML Parsing Security
- **Risiko**: XML External Entity (XXE) Attacks
- **Mitigation**: Sichere XML Parser Konfiguration, Input Validation

### Performance bei großen Katalogen
- **Risiko**: Langsame Suchen bei 100k+ Produkten
- **Mitigation**: Elasticsearch Integration, effiziente Indizierung

### Tenant Data Isolation
- **Risiko**: Datenleckage zwischen Tenants
- **Mitigation**: Strenge Tenant-Kontext-Prüfung in allen Service-Calls

---

## Implementierungsplan

### Phase 1: Core Infrastructure (1 Woche)
- Basis-Adapter-Framework
- XML Serialization/Deserialization
- Authentication Middleware
- Basic Logging & Monitoring

### Phase 2: Katalog-Funktionalität (1 Woche)
- Produkt-Suche implementieren
- Produktdetails implementieren
- Kategorie-Navigation implementieren

### Phase 3: Bestell-Funktionalität (1 Woche)
- Bestell-Erstellung implementieren
- Validierung und Fehlerbehandlung
- Integration Tests

### Phase 4: Optimierung & Tests (1 Woche)
- Performance-Optimierung
- Caching implementieren
- Vollständige Test-Suite
- Dokumentation

---

## Abhängigkeiten

### Externe Libraries
- `System.Xml` (.NET Core)
- `Microsoft.AspNetCore.Mvc` (Web API)
- `Microsoft.Extensions.Caching` (Caching)
- `Serilog` (Logging)

### Interne Services
- B2X.Catalog.API
- B2X.Search.API
- B2X.Customer.API
- B2X.Order.API
- B2X.Localization.API

---

## Open Questions

1. **XML Schema Version**: Welche IDS Connect XML Schema Version soll unterstützt werden?
2. **Custom Fields**: Sollen tenant-spezifische Custom Fields unterstützt werden?
3. **File Uploads**: Integration mit Produktbildern und Dokumenten?
4. **Real-time Updates**: WebSocket Support für Live-Preise/Verfügbarkeit?

---

**Nächste Schritte:**
1. XML Schema Definition finalisieren
2. Mock Service für Entwicklung erstellen
3. Basis-Adapter-Framework implementieren
4. Erste Integrationstests mit Catalog Service

**Referenzen:**
- [ADR-029](.ai/decisions/ADR-029-multi-format-punchout-integration.md)
- [ADR-029-BPA](.ai/decisions/ADR-029-business-process-analysis.md)
- ITEK IDS Connect Standard Dokumentation</content>
<parameter name="filePath">c:\Users\Holge\repos\B2X\.ai\decisions\ADR-029-ids-connect-adapter-spec.md