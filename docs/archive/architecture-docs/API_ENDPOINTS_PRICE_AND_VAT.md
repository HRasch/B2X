# API Endpoints: Price Calculation & VAT Validation

**Version**: 1.0  
**Last Updated**: 29. Dezember 2025  
**Status**: Sprint 1 Complete  
**Related Issues**: #30 (B2C Price Transparency), #31 (B2B VAT-ID Validation)

---

## Overview

B2Connect provides two core API endpoints for price and VAT management:

1. **Price Calculation API** - Calculate VAT-transparent pricing for B2C customers (19 EU countries)
2. **VAT-ID Validation API** - Validate B2B customer VAT-IDs and determine reverse charge applicability

Both endpoints follow Wolverine HTTP handler conventions and return standardized JSON responses.

---

## 1. Price Calculation API

### Endpoint

**POST** `/calculateprice`

**Port**: 7005 (Catalog Service)

**Protocol**: HTTP/JSON

**Handler**: `PriceCalculationHandler` (Wolverine auto-discovered)

### Purpose

Calculate product price with transparent VAT breakdown for B2C customers across 19 EU countries.

**Compliance**: PAngV (Price Indication Ordinance) requires showing:
- Product price (net)
- VAT rate (%)
- VAT amount (€)
- Final price (gross)

### Request Schema

```json
{
  "productPrice": 99.99,
  "countryCode": "DE",
  "discountAmount": 10.00,
  "discountPercentage": null
}
```

**Parameters**:
- `productPrice` (decimal, required): Base product price in EUR (2 decimal places)
- `countryCode` (string, required): ISO 3166-1 alpha-2 country code (e.g., "DE", "FR", "IT")
- `discountAmount` (decimal, optional): Fixed discount in EUR. Null if not applicable.
- `discountPercentage` (decimal, optional): Percentage discount (0-100%). Null if not applicable.

**Validation Rules**:
- productPrice: > 0
- countryCode: Exactly 2 uppercase letters, must be in 19 supported EU countries
- discountAmount: If provided, must be > 0 and < productPrice
- discountPercentage: If provided, must be 0-100

### Response Schema (Success)

```json
{
  "success": true,
  "data": {
    "productPrice": 99.99,
    "vatRate": 19.0,
    "vatAmount": 19.00,
    "priceIncludingVat": 118.99,
    "discountAmount": 10.00,
    "finalPrice": 108.99,
    "countryCode": "DE"
  },
  "message": "Price calculated successfully"
}
```

**Fields**:
- `productPrice`: Original net price in EUR
- `vatRate`: VAT percentage for the country (e.g., 19.0 = 19%)
- `vatAmount`: VAT amount in EUR (calculated and rounded to 2 decimals)
- `priceIncludingVat`: Net price + VAT (before discount)
- `discountAmount`: Applied discount in EUR
- `finalPrice`: Final gross price = (productPrice + vatAmount) - discountAmount
- `countryCode`: ISO country code used for calculation

### Response Schema (Error)

```json
{
  "success": false,
  "error": "INVALID_COUNTRY",
  "message": "Country code 'XX' is not supported. Supported countries: DE, AT, FR, ...",
  "data": null
}
```

**Error Codes**:
- `INVALID_COUNTRY` - Country code not in supported 19 EU countries
- `INVALID_PRICE` - Product price <= 0
- `INVALID_DISCOUNT` - Discount amount/percentage invalid
- `INTERNAL_ERROR` - Server error during calculation

### Supported Countries (19)

| Country | Code | VAT Rate | Example Currency |
|---------|------|----------|------------------|
| Germany | DE | 19% | 1,99€ |
| Austria | AT | 20% | 1,99€ |
| France | FR | 20% | 1,99€ |
| Belgium | BE | 21% | 1,99€ |
| Bulgaria | BG | 20% | 1,99 лв |
| Croatia | HR | 25% | 1,99€ |
| Cyprus | CY | 19% | 1,99€ |
| Czech Republic | CZ | 21% | 1,99 Kč |
| Denmark | DK | 25% | 1,99 kr |
| Estonia | EE | 20% | 1,99€ |
| Finland | FI | 24% | 1,99€ |
| Greece | GR | 24% | 1,99€ |
| Hungary | HU | 27% | 1,99 Ft |
| Ireland | IE | 23% | €1.99 |
| Italy | IT | 22% | 1,99€ |
| Latvia | LV | 21% | 1,99€ |
| Lithuania | LT | 21% | 1,99€ |
| Luxembourg | LU | 17% | 1,99€ |
| Malta | MT | 18% | €1.99 |
| Netherlands | NL | 21% | €1,99 |
| Poland | PL | 23% | 1,99 zł |
| Portugal | PT | 23% | 1,99€ |
| Romania | RO | 19% | 1,99 lei |
| Slovakia | SK | 20% | 1,99€ |
| Slovenia | SI | 22% | 1,99€ |
| Spain | ES | 21% | 1,99€ |
| Sweden | SE | 25% | 1,99 kr |

### Example Requests

#### Example 1: Germany, €99.99, No Discount

```bash
curl -X POST http://localhost:7005/calculateprice \
  -H "Content-Type: application/json" \
  -d '{
    "productPrice": 99.99,
    "countryCode": "DE"
  }'
```

**Response**:
```json
{
  "success": true,
  "data": {
    "productPrice": 99.99,
    "vatRate": 19.0,
    "vatAmount": 19.00,
    "priceIncludingVat": 118.99,
    "discountAmount": 0.00,
    "finalPrice": 118.99,
    "countryCode": "DE"
  },
  "message": "Price calculated successfully"
}
```

#### Example 2: Austria, €50.00, €5 Fixed Discount

```bash
curl -X POST http://localhost:7005/calculateprice \
  -H "Content-Type: application/json" \
  -d '{
    "productPrice": 50.00,
    "countryCode": "AT",
    "discountAmount": 5.00
  }'
```

**Response**:
```json
{
  "success": true,
  "data": {
    "productPrice": 50.00,
    "vatRate": 20.0,
    "vatAmount": 10.00,
    "priceIncludingVat": 60.00,
    "discountAmount": 5.00,
    "finalPrice": 55.00,
    "countryCode": "AT"
  },
  "message": "Price calculated successfully"
}
```

#### Example 3: France, €100.00, 10% Percentage Discount

```bash
curl -X POST http://localhost:7005/calculateprice \
  -H "Content-Type: application/json" \
  -d '{
    "productPrice": 100.00,
    "countryCode": "FR",
    "discountPercentage": 10
  }'
```

**Response**:
```json
{
  "success": true,
  "data": {
    "productPrice": 100.00,
    "vatRate": 20.0,
    "vatAmount": 20.00,
    "priceIncludingVat": 120.00,
    "discountAmount": 12.00,
    "finalPrice": 108.00,
    "countryCode": "FR"
  },
  "message": "Price calculated successfully"
}
```

### Performance

- **Response Time**: < 100ms (local calculation, no I/O)
- **Caching**: No caching required (deterministic, country rates static)
- **Throughput**: > 1000 req/sec

---

## 2. VAT-ID Validation API

### Endpoint

**POST** `/validatevatid`

**Port**: 7005 (Catalog Service)

**Protocol**: HTTP/JSON

**Handler**: `VatIdValidationHandler` (Wolverine auto-discovered)

### Purpose

Validate B2B customer VAT-IDs against the EU VIES system and determine if reverse charge (0% VAT) applies.

**Compliance**: AStV (Umsatzsteuer-Durchführungsverordnung) reverse charge requires:
- Valid VAT-ID per VIES
- Intra-EU transaction (both buyer and seller in EU)
- Result: Seller applies 0% VAT

**Caching Strategy**:
- Valid IDs: 365-day TTL (EU regulations recommend annual validation)
- Invalid IDs: 24-hour TTL (enables retry if API recovers)
- Key format: `vat:{countryCode}:{vatNumber}`

### Request Schema

```json
{
  "countryCode": "AT",
  "vatNumber": "U99999999",
  "buyerCountryCode": "DE"
}
```

**Parameters**:
- `countryCode` (string, required): VAT-ID issuing country (ISO 3166-1 alpha-2, e.g., "AT", "DE", "FR")
- `vatNumber` (string, required): VAT number without country prefix (1-17 alphanumeric characters)
- `buyerCountryCode` (string, optional): Buyer's country for reverse charge determination. If omitted, only validates ID.

**Validation Rules**:
- countryCode: Exactly 2 uppercase letters, must be EU country (27 supported)
- vatNumber: 1-17 alphanumeric characters, no spaces/special chars
- buyerCountryCode: If provided, must be 2 uppercase letters, must be EU country

### Response Schema (Success)

```json
{
  "success": true,
  "data": {
    "isValid": true,
    "companyName": "Example GmbH",
    "companyAddress": "Musterstraße 1, 1010 Wien",
    "shouldApplyReverseCharge": true,
    "validatedAt": "2025-12-29T10:30:00Z",
    "expiresAt": "2026-12-29T10:30:00Z",
    "countryCode": "AT",
    "vatNumber": "U99999999"
  },
  "message": "VAT-ID validated successfully"
}
```

**Fields**:
- `isValid`: Boolean - VAT-ID is registered in VIES
- `companyName`: Company name from VIES (if valid)
- `companyAddress`: Company address from VIES (if valid)
- `shouldApplyReverseCharge`: Boolean - whether seller should apply 0% VAT (isValid=true AND buyer in different EU country)
- `validatedAt`: ISO 8601 timestamp when validation occurred (or cache retrieval time)
- `expiresAt`: ISO 8601 timestamp when cache expires (valid: +365 days, invalid: +24 hours)
- `countryCode`: VAT-ID country code
- `vatNumber`: VAT number used for lookup

### Response Schema (Invalid VAT-ID)

```json
{
  "success": true,
  "data": {
    "isValid": false,
    "companyName": null,
    "companyAddress": null,
    "shouldApplyReverseCharge": false,
    "validatedAt": "2025-12-29T10:30:00Z",
    "expiresAt": "2025-12-30T10:30:00Z",
    "countryCode": "AT",
    "vatNumber": "INVALID12345"
  },
  "message": "VAT-ID is not registered in VIES"
}
```

Note: Invalid IDs cache with 24-hour TTL for retry capability.

### Response Schema (Error)

```json
{
  "success": false,
  "error": "INVALID_COUNTRY_CODE",
  "message": "Country code 'XX' must be exactly 2 uppercase letters and must be an EU country",
  "data": null
}
```

**Error Codes**:
- `INVALID_COUNTRY_CODE` - Country code format invalid or not EU
- `INVALID_VAT_NUMBER` - VAT number format invalid
- `VIES_API_ERROR` - VIES API call failed (returns isValid=false as safe default)
- `INTERNAL_ERROR` - Server error during validation

### Supported EU Countries (27)

Austria (AT), Belgium (BE), Bulgaria (BG), Croatia (HR), Cyprus (CY), Czech Republic (CZ), Denmark (DK), Estonia (EE), Finland (FI), France (FR), Germany (DE), Greece (GR), Hungary (HU), Ireland (IE), Italy (IT), Latvia (LV), Lithuania (LT), Luxembourg (LU), Malta (MT), Netherlands (NL), Poland (PL), Portugal (PT), Romania (RO), Slovakia (SK), Slovenia (SI), Spain (ES), Sweden (SE)

### Example Requests

#### Example 1: Valid Austrian VAT-ID, Reverse Charge Applicable

```bash
curl -X POST http://localhost:7005/validatevatid \
  -H "Content-Type: application/json" \
  -d '{
    "countryCode": "AT",
    "vatNumber": "U12345678",
    "buyerCountryCode": "DE"
  }'
```

**Response** (assuming VAT-ID registered):
```json
{
  "success": true,
  "data": {
    "isValid": true,
    "companyName": "Example GmbH",
    "companyAddress": "Musterstraße 1, 1010 Wien, AT",
    "shouldApplyReverseCharge": true,
    "validatedAt": "2025-12-29T10:30:00Z",
    "expiresAt": "2026-12-29T10:30:00Z",
    "countryCode": "AT",
    "vatNumber": "U12345678"
  },
  "message": "VAT-ID validated successfully. Reverse charge applies."
}
```

#### Example 2: Same Country (No Reverse Charge)

```bash
curl -X POST http://localhost:7005/validatevatid \
  -H "Content-Type: application/json" \
  -d '{
    "countryCode": "DE",
    "vatNumber": "123456789",
    "buyerCountryCode": "DE"
  }'
```

**Response**:
```json
{
  "success": true,
  "data": {
    "isValid": true,
    "companyName": "Sample AG",
    "companyAddress": "Hauptstraße 10, 10115 Berlin, DE",
    "shouldApplyReverseCharge": false,
    "validatedAt": "2025-12-29T10:30:00Z",
    "expiresAt": "2026-12-29T10:30:00Z",
    "countryCode": "DE",
    "vatNumber": "123456789"
  },
  "message": "VAT-ID validated successfully. Reverse charge does NOT apply (same country)."
}
```

#### Example 3: Invalid VAT-ID

```bash
curl -X POST http://localhost:7005/validatevatid \
  -H "Content-Type: application/json" \
  -d '{
    "countryCode": "FR",
    "vatNumber": "INVALID123"
  }'
```

**Response**:
```json
{
  "success": true,
  "data": {
    "isValid": false,
    "companyName": null,
    "companyAddress": null,
    "shouldApplyReverseCharge": false,
    "validatedAt": "2025-12-29T10:30:00Z",
    "expiresAt": "2025-12-30T10:30:00Z",
    "countryCode": "FR",
    "vatNumber": "INVALID123"
  },
  "message": "VAT-ID is not registered in VIES system"
}
```

### Performance & Reliability

- **Cache Hit Response**: < 5ms
- **Cache Miss (VIES Call)**: 500-2000ms (EU VIES API latency)
- **Timeout**: 10 seconds per VIES attempt
- **Retry Strategy**: 3 attempts with exponential backoff (1s, 2s, 4s)
- **Safe Fallback**: On VIES API failure → isValid=false, 24h cache (enables retry later)
- **Uptime**: VIES API availability typically > 99.5%

### Test VAT-IDs

For staging/testing, use official VIES test numbers:

```
DE100000000  → Always INVALID (test purposes)
AT100000000  → Always INVALID (test purposes)
FR100000000  → Always INVALID (test purposes)
```

**Note**: Production integration uses real VIES API. Testing in staging uses test IDs above.

---

## Integration Notes

### Multi-Tenant Support

Both endpoints require `X-Tenant-ID` header for multi-tenant isolation:

```bash
curl -X POST http://localhost:7005/calculateprice \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: 550e8400-e29b-41d4-a716-446655440000" \
  -d '{
    "productPrice": 99.99,
    "countryCode": "DE"
  }'
```

### Error Handling

Both endpoints return:
- HTTP 200 with `success: true/false` for business logic errors
- HTTP 400 for validation errors
- HTTP 500 for server errors

Never throw exceptions; always return JSON responses.

### Rate Limiting

- **Price Calculation**: No rate limit (stateless, fast)
- **VAT-ID Validation**: 100 requests/minute per tenant (VIES API rate limits)

Exceeding limits returns HTTP 429 (Too Many Requests).

### Security

- All requests require authentication (JWT token in Authorization header)
- TenantId verified from JWT claims
- VIES API calls use 10-second timeout to prevent hanging
- All inputs validated server-side (no trust of client input)

---

## Status Codes

### Price Calculation

| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success (success=true) | All valid inputs |
| 200 | Validation Error (success=false) | Invalid country/price |
| 400 | Malformed JSON | Missing required field |
| 401 | Unauthorized | No JWT token |
| 500 | Server Error | Unexpected exception |

### VAT-ID Validation

| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success (success=true) | Valid request, valid or invalid VAT-ID |
| 200 | Validation Error (success=false) | Invalid country code |
| 400 | Malformed JSON | Missing required field |
| 401 | Unauthorized | No JWT token |
| 429 | Rate Limited | > 100 req/min per tenant |
| 500 | Server Error | VIES API timeout (but cached) |

---

## Changelog

### Version 1.0 (Sprint 1, 29.12.2025)
- ✅ Price Calculation API (POST /calculateprice) - 19 EU countries
- ✅ VAT-ID Validation API (POST /validatevatid) - 27 EU countries
- ✅ VIES integration with 3x retry + exponential backoff
- ✅ Caching: 365-day valid, 24-hour invalid
- ✅ Reverse charge logic: valid ID + different EU countries = 0% VAT
- ✅ FluentValidation for all request validation
- ✅ Wolverine HTTP handler pattern
- ✅ 100% test coverage (49 tests, all passing)

---

**See Also**:
- [ARCHITECTURE_PRICE_AND_VAT_VALIDATION.md](ARCHITECTURE_PRICE_AND_VAT_VALIDATION.md) - Component diagrams, flow diagrams
- [DATABASE_SCHEMA_VAT_VALIDATION.md](DATABASE_SCHEMA_VAT_VALIDATION.md) - Database design
- [TESTING_PRICE_AND_VAT.md](TESTING_PRICE_AND_VAT.md) - Test coverage details
- [DEPLOYMENT_PRICE_AND_VAT.md](DEPLOYMENT_PRICE_AND_VAT.md) - Deployment guide
- [COMPLIANCE_PRICE_AND_VAT.md](COMPLIANCE_PRICE_AND_VAT.md) - Regulatory compliance details
