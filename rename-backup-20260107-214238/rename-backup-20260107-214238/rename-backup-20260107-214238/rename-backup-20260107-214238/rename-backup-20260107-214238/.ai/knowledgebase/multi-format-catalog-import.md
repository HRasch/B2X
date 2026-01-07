---
docid: KB-023
title: Multi-Format Catalog Import Architecture
owner: @Backend
status: Active
date: 2026-01-03
---

# Multi-Format Catalog Import Architecture

**DocID**: `KB-023`  
**Status**: ✅ Active  
**Maintainer**: @Backend  
**Last Updated**: 3. Januar 2026

## Overview

B2X supports import of product catalogs in multiple formats through a **plugin-based adapter architecture**. Each format has its own adapter that implements `IFormatAdapter`, enabling extensibility without modifying core logic.

### Supported Formats

| Format | File Types | Versions | Use Case |
|--------|-----------|----------|----------|
| **BMEcat** | `.xml` | 1.2, 2005, 2005.1, 2005.2 | International B2B product catalogs |
| **Datanorm** | `.txt`, `.dn`, `.datanorm` | All versions | German wholesale/retail trade (DIN) |
| **CSV** | `.csv`, `.txt` | - | Flexible spreadsheet imports |

---

## Architecture

### Plugin Pattern

```
HTTP Request (File Upload)
        ↓
Format Auto-Detection
        ↓
IFormatAdapter Implementation
        ├─ ValidateAsync (Schema + business rules)
        ├─ ParseAsync (Normalize to CatalogEntity)
        └─ DetectFormat (Content analysis)
        ↓
Canonical CatalogEntity Model
        ↓
Domain Logic (tenant, pricing, inventory)
```

### Key Interfaces

#### IFormatAdapter

Every format adapter implements this interface:

```csharp
public interface IFormatAdapter
{
    string FormatId { get; }                              // "bmecat", "datanorm", "csv"
    string FormatName { get; }                            // "BMEcat", "Datanorm", "CSV"
    IReadOnlyList<string> SupportedExtensions { get; }   // [".xml"], [".txt"], [".csv"]
    
    // Format detection (0.0 = no match, 1.0 = certain match)
    double DetectFormat(string content, string fileName);
    
    // Validation (returns errors and warnings)
    Task<ValidationResult> ValidateAsync(string content, CancellationToken ct);
    
    // Parse to canonical model
    Task<ParseResult> ParseAsync(string content, ImportMetadata metadata, CancellationToken ct);
}
```

#### CatalogEntity (Canonical Model)

All formats normalize to this:

```csharp
public record CatalogEntity(
    string ExternalId,              // SKU, EAN, or article number
    string SupplierId,              // Supplier identifier
    string Name,                    // Product name
    string? Description = null,     // Optional description
    string? Ean = null,            // EAN/GTIN barcode
    string? ManufacturerPartNumber = null,
    decimal? ListPrice = null,     // Optional price
    string? Currency = null,       // Currency code (EUR, USD, etc.)
    Dictionary<string, string>? Attributes = null  // Format-specific data
);
```

### FormatRegistry

Manages adapter lifecycle and auto-detection:

```csharp
public class FormatRegistry
{
    // Get adapter by format ID
    IFormatAdapter? GetAdapterById(string formatId);
    
    // Auto-detect format
    IFormatAdapter? DetectFormat(string content, string fileName);
    
    // List all adapters
    IReadOnlyList<IFormatAdapter> GetAllAdapters();
}
```

**Detection Priority** (most specific first):
1. BMEcat (100) - XML with BMECAT element
2. Datanorm (90) - Fixed-width records with type codes
3. CSV (10) - Delimiter-based with headers

---

## HTTP Endpoints

### POST /api/v2/catalog/import

Upload and auto-import a catalog file.

**Request:**
```http
POST /api/v2/catalog/import HTTP/1.1
X-Tenant-ID: 550e8400-e29b-41d4-a716-446655440000
Content-Type: multipart/form-data

------WebKitFormBoundary
Content-Disposition: form-data; name="file"; filename="catalog.xml"

[BMEcat XML content]
------WebKitFormBoundary
Content-Disposition: form-data; name="format"

bmecat                          // Optional: explicit format
------WebKitFormBoundary
Content-Disposition: form-data; name="supplierId"

ACME-001                        // Optional: supplier ID
------WebKitFormBoundary
```

**Response (Success):**
```json
{
  "success": true,
  "format": "bmecat",
  "formatName": "BMEcat",
  "statistics": {
    "totalItems": 1250,
    "validItems": 1240,
    "skippedItems": 10,
    "importedAt": "2026-01-03T14:30:00Z"
  },
  "entities": [
    {
      "externalId": "PROD-12345",
      "supplierId": "ACME-001",
      "name": "Industrial Widget",
      "description": "High-quality industrial widget",
      "ean": "5901234123457",
      "manufacturerPartNumber": "WID-500",
      "listPrice": 49.99,
      "currency": "EUR"
    }
  ],
  "warnings": [
    {
      "code": "BMECAT_MISSING_OPTIONAL_FIELD",
      "message": "Missing optional field: PRICE_AMOUNT for product PROD-100",
      "itemId": "PROD-100"
    }
  ]
}
```

**Response (Validation Error):**
```json
{
  "error": "Validation failed",
  "format": "csv",
  "errors": [
    {
      "code": "CSV_MISSING_SKU_COLUMN",
      "message": "CSV must have SKU/Article Number/Product ID column",
      "suggestion": null
    }
  ],
  "warnings": []
}
```

### GET /api/v2/catalog/import/formats

List all supported formats.

**Response:**
```json
{
  "supportedFormats": [
    {
      "id": "bmecat",
      "name": "BMEcat",
      "extensions": [".xml"],
      "description": "BMEcat XML standard for product catalogs (v1.2, 2005, 2005.1, 2005.2)"
    },
    {
      "id": "datanorm",
      "name": "Datanorm",
      "extensions": [".txt", ".dn", ".datanorm"],
      "description": "Datanorm text format for German wholesale/retail trade"
    },
    {
      "id": "csv",
      "name": "CSV",
      "extensions": [".csv", ".txt"],
      "description": "CSV format with flexible column headers"
    }
  ],
  "total": 3
}
```

### POST /api/v2/catalog/import/validate

Validate a file without importing.

**Response:**
```json
{
  "format": "csv",
  "isValid": false,
  "errors": [
    {
      "code": "CSV_NO_VALID_DATA",
      "message": "No valid data rows found (all rows failed validation)",
      "line": null
    }
  ],
  "warnings": [
    {
      "code": "CSV_FIELD_COUNT_MISMATCH",
      "message": "Row has 5 fields but header has 6",
      "line": 5
    }
  ]
}
```

---

## Format-Specific Documentation

### 1. BMEcat (XML)

**File**: `backend/Domain/Catalog/src/Catalog/ImportAdapters/Formats/BmecatImportAdapter.cs`  
**See**: [KB-025] BMEcat Format Details

#### Detection
- Content must contain `<BMECAT>` element
- Version attribute: `version="1.2"`, `version="2005"`, `version="2005.1"`, `version="2005.2"`
- Namespace: `bmecat/1.2/`, `bmecat/2005/`

#### Validation
- ✅ XSD schema validation (per version)
- ✅ Required elements: HEADER, SUPPLIER_ID, ARTICLE
- ✅ XXE attack prevention (DTD processing disabled)
- ⚠️ Warnings for missing optional fields

#### Schema Files
Located in `backend/Domain/Catalog/src/Infrastructure/Schemas/`:
- `BMEcat1200.xsd` - Version 1.2
- `BMEcat2005Base.xsd` - Version 2005 base
- `BMEcat20051.xsd` - Version 2005.1
- `BMEcat20052.xsd` - Version 2005.2

#### Mapped Fields
```
BMEcat Element          → CatalogEntity Field
SUPPLIER_AID           → ExternalId
DESCRIPTION_SHORT      → Name
DESCRIPTION_LONG       → Description
EAN                    → Ean
MANUFACTURER_AID       → ManufacturerPartNumber
PRICE_AMOUNT           → ListPrice
PRICE_CURRENCY         → Currency
```

---

### 2. Datanorm (Text-based)

**File**: `backend/Domain/Catalog/src/Catalog/ImportAdapters/Formats/DatanormImportAdapter.cs`

#### Detection
- File extension: `.txt`, `.dn`, `.datanorm`
- Content: Lines starting with digits (record type codes 0-9)
- Fixed-width fields per record type

#### Record Types
| Type | Purpose | Fields |
|------|---------|--------|
| 0 | Header | Document/supplier metadata |
| 1 | Article | SKU, name, article number |
| 2 | Pricing | Net price, currency |
| 3 | Availability | Stock levels, lead times |
| 4 | Description | Extended text fields |
| 8 | Notes | Comments, special info |
| 9 | Footer | End of document |

#### Validation
- ✅ Requires at least header (0) and articles (1)
- ⚠️ Warns if footer (9) missing
- ✅ Field width consistency

#### Mapped Fields (approx. positions)
```
Position 2-10:   EAN code              → Ean
Position 12-30:  Supplier article #    → ManufacturerPartNumber
Position 30-90:  Article name          → Name
Position 91-98:  Supplier code         → SupplierId
Type 2 Pos 3-14: Net price            → ListPrice
Type 2 Pos 15-16: Currency code       → Currency
Type 4 Pos 3+:   Description text     → Description
```

#### Advantages
- Efficient fixed-width format
- Widely used in German commerce
- Low bandwidth overhead

---

### 3. CSV (Comma/Semicolon/Tab-Separated)

**File**: `backend/Domain/Catalog/src/Catalog/ImportAdapters/Formats/CsvImportAdapter.cs`

#### Detection
- File extension: `.csv`, `.txt`
- Content: Headers row + delimiter-separated columns
- Auto-detects delimiter: `,` (comma), `;` (semicolon), `\t` (tab)

#### Required Columns (case-insensitive, any order)
```
SKU Column:  sku, article_number, product_id, product_code
Name Column: name, title, product_name
```

#### Optional Columns
```
description, long_description, details
ean, gtin, ean_code, barcode
mfg_part_number, manufacturer_part_number, mpn, part_number
price, list_price, msrp, retail_price
currency, currency_code, currency_iso
supplier, supplier_id
```

#### Validation
- ✅ Requires header row
- ✅ Requires at least one data row
- ✅ Field count consistency
- ✅ Required field values non-empty
- ⚠️ Warns for mismatched field counts

#### Example CSV
```csv
sku,name,description,ean,price,currency
WIDGET-001,Industrial Widget,Heavy-duty widget,5901234123457,49.99,EUR
WIDGET-002,Standard Widget,General purpose widget,5901234123458,29.99,EUR
WIDGET-003,Mini Widget,Compact widget,5901234123459,19.99,EUR
```

#### Advantages
- Simple format, easy to create
- Works with Excel, Google Sheets
- Flexible column order and naming
- Quoted field support for embedded delimiters

---

## Adding New Formats

### Step 1: Create Adapter Class

```csharp
namespace B2X.Catalog.ImportAdapters.Formats;

public class MyFormatImportAdapter : IFormatAdapter
{
    public string FormatId => "myformat";
    public string FormatName => "My Format";
    public IReadOnlyList<string> SupportedExtensions => new[] { ".myf" };
    
    public double DetectFormat(string content, string fileName)
    {
        // Return 0.0 to 1.0 confidence
    }
    
    public Task<ValidationResult> ValidateAsync(string content, CancellationToken ct)
    {
        // Validate content structure
    }
    
    public Task<ParseResult> ParseAsync(string content, ImportMetadata metadata, CancellationToken ct)
    {
        // Parse to CatalogEntity list
    }
}
```

### Step 2: Register in DI

Edit `FormatRegistry.cs`:

```csharp
public static IServiceCollection AddFormatAdapters(this IServiceCollection services)
{
    services.AddSingleton<IFormatAdapter, BmecatImportAdapter>();
    services.AddSingleton<IFormatAdapter, DatanormImportAdapter>();
    services.AddSingleton<IFormatAdapter, CsvImportAdapter>();
    services.AddSingleton<IFormatAdapter, MyFormatImportAdapter>();  // ← Add here
    services.AddSingleton<FormatRegistry>();
    return services;
}
```

### Step 3: Update Priority

In `FormatRegistry.GetAdapterPriority()`:

```csharp
private static int GetAdapterPriority(string formatId) =>
    formatId.ToLowerInvariant() switch
    {
        "bmecat" => 100,      // Most specific
        "datanorm" => 90,
        "myformat" => 85,     // ← Add here
        "csv" => 10,          // Most generic
        _ => 0
    };
```

### Step 4: Test

Create unit tests implementing `IFormatAdapterTests<MyFormatImportAdapter>`.

---

## Error Handling

### Validation Errors

Errors prevent import and must be fixed:

```csharp
public record ValidationError(
    string Code,                // Machine-readable code
    string Message,             // Human-friendly message
    string? ElementPath = null, // Where error occurred
    int? LineNumber = null,     // Line number (if applicable)
    string? Suggestion = null   // How to fix
);
```

**Example**: "BMECAT_VERSION_NOT_DETECTED" - user must specify version or fix XML.

### Validation Warnings

Warnings allow import to continue but highlight issues:

```csharp
public record ValidationWarning(
    string Code,
    string Message,
    string? ElementPath = null,
    int? LineNumber = null
);
```

**Example**: "CSV_FIELD_COUNT_MISMATCH" - row will be skipped but import continues.

### Parse Warnings

Warnings logged during parsing of valid content:

```csharp
public record ParseWarning(
    string Code,                // "DATANORM_MISSING_PRICE"
    string Message,             // "Price information missing for article ABC"
    string? ItemIdentifier = null,
    string? Suggestion = null   // "Price field is optional"
);
```

---

## Performance Considerations

### File Size Limits
- **Max file size**: 100 MB
- **Typical parsing**:
  - BMEcat 1.2 MB → ~100 ms
  - Datanorm 50 MB → ~500 ms
  - CSV 100 MB → ~2000 ms (limited by CSV parsing overhead)

### Memory Usage
- Small files (<10 MB): Load entire content in memory
- Large files (>100 MB): Should use streaming parser (future)
- Current implementation: Full load (suitable for catalogs <100 MB)

### Optimization Tips
1. **Compress files** - Use gzip for network transfer
2. **Batch imports** - Import once instead of incremental updates
3. **Validate first** - Use `/validate` endpoint to catch errors before parsing
4. **Async operations** - All operations are async for scalability

---

## Security

### XXE Protection

All XML parsing disables DTD processing:

```csharp
var settings = new XmlReaderSettings
{
    DtdProcessing = DtdProcessing.Prohibit,
    XmlResolver = null,
    ConformanceLevel = ConformanceLevel.Document
};
```

### Input Validation

- File size limit: 100 MB
- Character limits on text fields
- Schema validation where applicable

### Data Isolation

- Tenant ID required in header
- Imported data tagged with tenant
- No cross-tenant leakage

---

## Testing

### Test Data Structure
```
tests/Data/
├── BMEcat/
│   ├── Valid/
│   │   ├── bmecat-1.2-complete.xml
│   │   ├── bmecat-2005-minimal.xml
│   │   └── bmecat-2005.2-with-prices.xml
│   └── Invalid/
│       ├── xxe-attack.xml
│       └── missing-articles.xml
├── Datanorm/
│   ├── Valid/
│   │   ├── datanorm-complete.txt
│   │   └── datanorm-minimal.txt
│   └── Invalid/
│       └── malformed-record.txt
└── CSV/
    ├── Valid/
    │   ├── comma-delimited.csv
    │   ├── semicolon-delimited.csv
    │   └── with-quoted-fields.csv
    └── Invalid/
        ├── missing-sku-column.csv
        └── empty-file.csv
```

---

## Troubleshooting

### Format Not Detected

**Problem**: "Unable to detect format" error  
**Solutions**:
1. Specify format explicitly: `format=bmecat`
2. Check file extension and content type
3. Validate format with `/validate` endpoint

### Validation Failed

**Problem**: "Validation failed" with specific errors  
**Solutions**:
1. Read error code and suggestion
2. Check element/line number in file
3. Consult format-specific documentation
4. Inspect schema file if XSD error

### Import Hangs

**Problem**: Large file not processing  
**Solutions**:
1. Check file size (max 100 MB)
2. Monitor server logs for exceptions
3. Use `/validate` to test parsing
4. Contact support for files >50 MB

---

## Related Documentation

- [KB-025] - BMEcat Format Details
- [KB-026] - Datanorm Format Reference
- [KB-027] - CSV Import Patterns
- [ADR-026] - BMEcat Catalog Import Architecture

---

**Last Updated**: 3. Januar 2026  
**Next Review**: Q2 2026 (after MVP testing with customers)
