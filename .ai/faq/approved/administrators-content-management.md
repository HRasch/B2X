# Administrator FAQs - Content Management

## How do I add a new product to the catalog?
**Answer:** As an administrator, you can add products through the B2Connect Admin portal:

1. Log in to the Admin portal at `/admin`
2. Navigate to "Catalog" → "Products" → "Add Product"
3. Fill in the product details:
   - Basic information (name, SKU, description)
   - Pricing (regular price, sale price, tax settings)
   - Inventory (stock levels, variants, attributes)
   - Media (product images, videos)
   - Categories and tags
4. Configure SEO settings (meta title, description, URL slug)
5. Set visibility and publication status
6. Click "Save Product"

The product will be immediately available in the store if published, or saved as draft for later review.

**Related FAQs:** How do I manage product categories?, How do I bulk import products?

**Source:** CMS Domain Architecture, Admin Portal Requirements
**Last Updated:** 2026-01-01
**Access Count:** 0
**Satisfaction Score:** N/A

---

## How do I manage customer orders?
**Answer:** To manage customer orders in the Admin portal:

1. Access the Admin portal and go to "Orders" → "All Orders"
2. Use filters to find specific orders (by status, date, customer)
3. Click on an order to view details:
   - Customer information and order history
   - Order items with prices and quantities
   - Shipping and billing addresses
   - Payment status and transaction details
4. Update order status as needed:
   - Processing → Shipped → Delivered
   - Handle returns or cancellations
5. Add internal notes for order tracking
6. Generate invoices or shipping labels

For bulk operations, use the order export/import features for reporting and analysis.

**Related FAQs:** How do I process refunds?, How do I track order fulfillment?

**Source:** Order Management Domain, Admin Workflow Requirements
**Last Updated:** 2026-01-01
**Access Count:** 0
**Satisfaction Score:** N/A

---

## How do I configure shipping settings?
**Answer:** Shipping configuration is managed in the Admin portal under Settings:

1. Navigate to "Settings" → "Shipping"
2. Set up shipping zones (geographic regions)
3. Configure shipping methods for each zone:
   - Flat rate shipping
   - Weight-based rates
   - Free shipping thresholds
   - Table rate shipping
4. Define shipping classes for different product types
5. Set up tax calculations per region
6. Configure shipping restrictions (weight limits, prohibited items)

Test your shipping calculations with sample orders before going live. Use the shipping calculator in product settings to verify rates.

**Related FAQs:** How do I set up tax rates?, How do I configure payment methods?

**Source:** E-commerce Domain Requirements, Admin Configuration Guide
**Last Updated:** 2026-01-01
**Access Count:** 0
**Satisfaction Score:** N/A