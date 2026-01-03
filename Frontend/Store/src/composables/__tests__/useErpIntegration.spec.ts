import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ref } from 'vue';
import { useErpIntegration } from '@/composables/useErpIntegration';

describe('useErpIntegration Composable', () => {
  let composable: ReturnType<typeof useErpIntegration>;

  beforeEach(() => {
    composable = useErpIntegration();
    // Mock fetch globally
    global.fetch = vi.fn();
  });

  describe('validateCustomerEmail', () => {
    it('should return error for empty email', async () => {
      const result = await composable.validateCustomerEmail('');

      expect(result.isValid).toBe(false);
      expect(result.error).toBe('INVALID_EMAIL');
      expect(result.customer).toBeNull();
    });

    it('should return error for invalid email format', async () => {
      const result = await composable.validateCustomerEmail('notanemail');

      expect(result.isValid).toBe(false);
      expect(result.error).toBe('INVALID_EMAIL');
    });

    it('should fetch customer on valid email', async () => {
      const mockCustomer = {
        customerNumber: 'CUST-001',
        customerName: 'Max Mustermann',
        email: 'max@example.com',
        country: 'DE',
        businessType: 'PRIVATE' as const,
        isActive: true,
      };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockCustomer,
      });

      const result = await composable.validateCustomerEmail('max@example.com');

      expect(result.isValid).toBe(true);
      expect(result.customer).toEqual(mockCustomer);
      expect(composable.customer.value).toEqual(mockCustomer);
      expect(global.fetch).toHaveBeenCalledWith(
        '/api/auth/erp/validate-email',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify({ email: 'max@example.com' }),
        })
      );
    });

    it('should handle network errors gracefully', async () => {
      (global.fetch as any).mockRejectedValueOnce(new Error('Network error'));

      const result = await composable.validateCustomerEmail('test@example.com');

      expect(result.isValid).toBe(false);
      expect(result.error).toBe('NETWORK_ERROR');
      expect(result.message).toContain('Verbindungsfehler');
      expect(composable.error.value).toContain('Network error');
    });

    it('should handle 404 response (customer not found)', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        json: async () => ({
          error: 'NOT_FOUND',
          message: 'Kunde nicht gefunden',
        }),
      });

      const result = await composable.validateCustomerEmail('unknown@example.com');

      expect(result.isValid).toBe(false);
      expect(result.error).toBe('NOT_FOUND');
      expect(result.message).toBe('Kunde nicht gefunden');
    });

    it('should set isLoading state during fetch', async () => {
      (global.fetch as any).mockImplementation(
        () =>
          new Promise(resolve => {
            expect(composable.isLoading.value).toBe(true);
            resolve({
              ok: true,
              json: async () => ({
                customerNumber: 'CUST-001',
                customerName: 'Test',
                email: 'test@example.com',
                country: 'DE',
                businessType: 'PRIVATE',
                isActive: true,
              }),
            });
          })
      );

      await composable.validateCustomerEmail('test@example.com');

      expect(composable.isLoading.value).toBe(false);
    });

    it('should record lookup time', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => ({
          customerNumber: 'CUST-001',
          customerName: 'Test',
          email: 'test@example.com',
          country: 'DE',
          businessType: 'PRIVATE',
          isActive: true,
        }),
      });

      const result = await composable.validateCustomerEmail('test@example.com');

      expect(result.loadingMs).toBeDefined();
      expect(result.loadingMs).toBeGreaterThanOrEqual(0);
      expect(composable.lastLookupTime.value).toBeDefined();
    });
  });

  describe('validateCustomerNumber', () => {
    it('should return error for empty customer number', async () => {
      const result = await composable.validateCustomerNumber('');

      expect(result.isValid).toBe(false);
      expect(result.error).toBe('INVALID_CUSTOMER_NUMBER');
    });

    it('should fetch customer by number', async () => {
      const mockCustomer = {
        customerNumber: 'CUST-100',
        customerName: 'TechCorp GmbH',
        email: 'info@techcorp.de',
        country: 'DE',
        businessType: 'BUSINESS' as const,
        isActive: true,
        creditLimit: 50000,
      };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockCustomer,
      });

      const result = await composable.validateCustomerNumber('CUST-100');

      expect(result.isValid).toBe(true);
      expect(result.customer).toEqual(mockCustomer);
      expect(global.fetch).toHaveBeenCalledWith(
        '/api/auth/erp/validate-number',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify({ customerNumber: 'CUST-100' }),
        })
      );
    });

    it('should handle invalid customer number response', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        json: async () => ({
          error: 'NOT_FOUND',
          message: 'Kunde nicht gefunden',
        }),
      });

      const result = await composable.validateCustomerNumber('INVALID-123');

      expect(result.isValid).toBe(false);
      expect(result.error).toBe('NOT_FOUND');
    });
  });

  describe('clearCustomer', () => {
    it('should clear all state', async () => {
      // Set some state first
      composable.customer.value = {
        customerNumber: 'CUST-001',
        customerName: 'Test',
        email: 'test@example.com',
        country: 'DE',
        businessType: 'PRIVATE',
        isActive: true,
      };
      composable.error.value = 'Some error';
      composable.lastLookupTime.value = 100;

      // Clear
      composable.clearCustomer();

      expect(composable.customer.value).toBeNull();
      expect(composable.error.value).toBeNull();
      expect(composable.lastLookupTime.value).toBeNull();
    });
  });

  describe('computed properties', () => {
    it('hasCustomer should be true when customer is set', () => {
      expect(composable.hasCustomer.value).toBe(false);

      composable.customer.value = {
        customerNumber: 'CUST-001',
        customerName: 'Test',
        email: 'test@example.com',
        country: 'DE',
        businessType: 'PRIVATE',
        isActive: true,
      };

      expect(composable.hasCustomer.value).toBe(true);
    });

    it('isPrivateCustomer should be true for PRIVATE business type', () => {
      composable.customer.value = {
        customerNumber: 'CUST-001',
        customerName: 'Max Mustermann',
        email: 'max@example.com',
        country: 'DE',
        businessType: 'PRIVATE',
        isActive: true,
      };

      expect(composable.isPrivateCustomer.value).toBe(true);
      expect(composable.isBusinessCustomer.value).toBe(false);
    });

    it('isBusinessCustomer should be true for BUSINESS business type', () => {
      composable.customer.value = {
        customerNumber: 'CUST-100',
        customerName: 'TechCorp GmbH',
        email: 'info@techcorp.de',
        country: 'DE',
        businessType: 'BUSINESS',
        isActive: true,
      };

      expect(composable.isBusinessCustomer.value).toBe(true);
      expect(composable.isPrivateCustomer.value).toBe(false);
    });
  });
});
