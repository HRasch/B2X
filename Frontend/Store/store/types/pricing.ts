export interface PriceBreakdown {
  productPrice: number;
  vatRate: number;
  vatAmount: number;
  totalWithVat: number;
  currencyCode: string;
  shippingCost: number;
  finalTotal: number;
  destinationCountry: string;
}

export interface PriceBreakdownResponse {
  success: boolean;
  breakdown?: PriceBreakdown;
  error?: string;
  message?: string;
}

export interface CalculatePriceCommand {
  productPrice: number;
  destinationCountry: string;
  shippingCost?: number;
  currencyCode?: string;
}
