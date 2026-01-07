# BMEcat - Business Messaging e-Catalog Format

**Last Updated**: 3. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current

---

## Official Resources

- **BME Downloads Page**: https://www.bme.de/services/bmecat/downloads_BMEcat/
- **BME Organization**: Bundesverband Materialwirtschaft, Einkauf und Logistik e.V. (German Association for Materials Management, Purchasing and Logistics)

---

## Supported Versions

### BMEcat 1.2 (Legacy)
- **XSD Schema**: `BMEcat1200.xsd`
- **Source**: https://a.storyblok.com/f/104752/x/b7ed0193f6/bmecat_12_xsd_all_in_one.zip
- **Last Updated**: 2003-01-10
- **Status**: Legacy, still supported for backward compatibility
- **Usage**: `<BMECAT version="1.2">`

### BMEcat 2005 (Base)
- **XSD Schema**: `BMEcat2005Base.xsd`
- **Source**: https://a.storyblok.com/f/104752/x/6a10e4022d/bmecat_2005_xsd.zip
- **Last Updated**: 2005-11-04
- **Status**: Supported (older 2005 standard)
- **Usage**: `<BMECAT version="2005">`

### BMEcat 2005.1
- **XSD Schema**: `BMEcat20051.xsd`
- **Source**: https://a.storyblok.com/f/104752/x/063fbced32/bmecat_2005_1-xsd.zip
- **Last Updated**: 2015-07-18
- **Status**: Supported
- **Usage**: `<BMECAT version="2005.1">`

### BMEcat 2005.2 (Latest)
- **XSD Schema**: `BMEcat20052.xsd`
- **Source**: https://a.storyblok.com/f/104752/x/13da94f38b/bmecat_2005-2.xsd
- **Last Updated**: 2022-09-10
- **Status**: ✅ Current recommended version
- **Usage**: `<BMECAT version="2005.2">`

---

## Schema Locations

All official BMEcat XSD schemas are stored in:
```
backend/Domain/Catalog/src/Infrastructure/Schemas/
├── BMEcat1200.xsd         (1.2 - legacy)
├── BMEcat2005Base.xsd     (2005 - base version)
├── BMEcat20051.xsd        (2005.1)
└── BMEcat20052.xsd        (2005.2 - latest)
```

---

## B2X Implementation

### Version Detection

The import adapter (`BmecatImportAdapter.cs`) automatically detects the BMEcat version by:

1. **Parsing version attribute** from BMECAT root element: `<BMECAT version="2005.2">`
2. **Fallback to namespace** detection: `xmlns="...bmecat/2005/..."`

Supported version strings:
- `"1.2"` → Uses `BMEcat1200.xsd`
- `"2005"` → Uses `BMEcat2005Base.xsd`
- `"2005.1"` → Uses `BMEcat20051.xsd`
- `"2005.2"` → Uses `BMEcat20052.xsd`

### XSD Validation

```csharp
// Schema validation with safe DTD processing
using var reader = XmlReader.Create(schemaStream, new XmlReaderSettings
{
    DtdProcessing = DtdProcessing.Prohibit,  // Security: prevent XXE attacks
    XmlResolver = null                        // Security: prevent external entity resolution
});
var schema = XmlSchema.Read(reader, validationHandler);
```

### Custom Schema Support

Users can provide custom schemas via the `CustomSchemaPath` property in `CatalogMetadata`:

```csharp
var metadata = new CatalogMetadata
{
    CustomSchemaPath = "/path/to/custom-bmecat.xsd"
};
```

The adapter will use the custom schema if provided, otherwise fall back to the built-in schema matching the detected version.

---

## Key BMEcat Elements

### Root Element
- `<BMECAT version="...">` - Catalog container with version attribute

### Main Sections
- `<HEADER>` - Metadata about the catalog
  - `<CATALOG>` - Language, currency, etc.
  - `<BUYER>` - Buyer information
  - `<SUPPLIER>` - Supplier information
- `<T_NEW_CATALOG>` - New product catalog (BMEcat 1.2)
- `<T_PRODUCT>` or `<PRODUCT>` - Product definition (context-dependent)
- `<ARTICLE>` - Article/SKU details

### Product Details
- `<PRODUCT_DETAILS>` - Description, features
- `<PRODUCT_ORDER_DETAILS>` - Order units, quantities
- `<PRODUCT_PRICE_DETAILS>` - Pricing information
  - `<PRODUCT_PRICE>` - Price entry with currency and type

---

## Common BMEcat Attributes

| Attribute | Values | Purpose |
|-----------|--------|---------|
| `version` | 1.2, 2005, 2005.1, 2005.2 | Catalog format version |
| `price_type` | net_customer, gross_customer, net_supplier, etc. | Price calculation basis |
| `currency` | EUR, USD, GBP, etc. | Currency code (ISO 4217) |
| `unit` | C62 (piece), MTR (meter), KGM (kilogram), etc. | Order unit |

---

## File Size Reference

| Version | Schema File | Size | Release Date |
|---------|------------|------|--------------|
| 1.2 | BMEcat1200.xsd | 154 KB | 2003-01-10 |
| 2005 | BMEcat2005Base.xsd | 245 KB | 2005-11-04 |
| 2005.1 | BMEcat20051.xsd | 239 KB | 2015-07-18 |
| 2005.2 | BMEcat20052.xsd | 249 KB | 2022-09-10 |

---

## Validation Errors

Common XSD validation errors and causes:

| Error | Cause | Solution |
|-------|-------|----------|
| `Element X not found in schema` | Wrong version detected | Check BMECAT version attribute |
| `Required attribute missing` | Malformed XML | Validate XML structure against spec |
| `Invalid price type` | Unsupported price calculation method | Use standard price_type values |
| `Invalid unit code` | Non-standard order unit | Use ISO standard unit codes (C62, MTR, etc.) |

---

## B2X Test Data

Sample BMEcat 1.2 test file:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<BMECAT version="1.2">
  <HEADER>
    <CATALOG>
      <LANGUAGE>deu</LANGUAGE>
      <CURRENCY>EUR</CURRENCY>
    </CATALOG>
    <BUYER><NAME>Test Buyer</NAME></BUYER>
    <SUPPLIER><NAME>Test Supplier</NAME></SUPPLIER>
  </HEADER>
  <T_NEW_CATALOG>
    <PRODUCT>
      <SUPPLIER_PID>TEST-001</SUPPLIER_PID>
      <PRODUCT_DETAILS>
        <DESCRIPTION_SHORT>Test Product</DESCRIPTION_SHORT>
      </PRODUCT_DETAILS>
      <PRODUCT_PRICE_DETAILS>
        <PRODUCT_PRICE price_type="net_customer">
          <PRICE_AMOUNT>100.00</PRICE_AMOUNT>
          <PRICE_CURRENCY>EUR</PRICE_CURRENCY>
        </PRODUCT_PRICE>
      </PRODUCT_PRICE_DETAILS>
    </PRODUCT>
  </T_NEW_CATALOG>
</BMECAT>
```

---

## Performance Considerations

- **Schema Loading**: XSD schemas are loaded once per import operation
- **Validation**: Full schema validation happens during XML parsing
- **Memory**: Large BMEcat files (1000+ products) use ~100-200 MB during import
- **Speed**: XSD validation adds ~10-20% overhead compared to structure validation

---

## Security Notes

✅ **DTD Processing Disabled**: Prevents XML External Entity (XXE) attacks  
✅ **External Entity Resolution Disabled**: Prevents SSRF attacks  
✅ **Input Validation**: All element values are validated against schema  
✅ **Secure Schema Loading**: Uses safe XmlReader with restricted settings  

---

## Migration Path

### From BMEcat 1.2 to 2005.x
- Element names may change (e.g., `<PRODUCT>` vs `<T_PRODUCT>`)
- Additional attributes and elements introduced
- New pricing model with more flexibility
- Namespace changes (if applicable)

No automatic migration tool available; manual mapping required.

---

## Related Documentation

- [BMEcat Official Specification](https://www.bme.de/services/bmecat/) (German)
- [B2X Catalog Import Architecture](../../decisions/ADR-026-bmecat-catalog-import-architecture.md)
- [Adapter Pattern Implementation](./bmecat-import-adapter.md)

---

## Troubleshooting

### Schema Not Found
```
Error: Unable to locate schema file for version 1.2
```
**Solution**: Verify `BMEcat1200.xsd` exists in `Infrastructure/Schemas/`

### Version Detection Failed
```
Error: Unable to detect BMEcat version from XML
```
**Solution**: Ensure BMECAT root element has version attribute or valid namespace

### Validation Failed
```
Error: The element 'PRODUCT' has invalid child element 'DESCRIPTION'
```
**Solution**: Check element spelling and nesting against XSD schema requirements

---

**Next Review**: 3. April 2026  
**Maintained By**: GitHub Copilot  
**Last Updated**: 3. Januar 2026
