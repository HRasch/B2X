/**
 * HTTP Client for API requests
 * Re-exports the configured axios instance from api.ts
 */

import { api } from './api';

export const client = api;
export default api;
