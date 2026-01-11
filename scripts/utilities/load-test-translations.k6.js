import http from 'k6/http';
import { check, sleep } from 'k6';

// Test configuration
export const options = {
  stages: [
    { duration: '2m', target: 100 }, // Ramp up to 100 users over 2 minutes
    { duration: '5m', target: 100 }, // Stay at 100 users for 5 minutes
    { duration: '2m', target: 500 }, // Ramp up to 500 users over 2 minutes
    { duration: '5m', target: 500 }, // Stay at 500 users for 5 minutes
    { duration: '2m', target: 1000 }, // Ramp up to 1000 users over 2 minutes
    { duration: '5m', target: 1000 }, // Stay at 1000 users for 5 minutes
    { duration: '2m', target: 100 }, // Ramp down to 100 users
    { duration: '1m', target: 0 }, // Ramp down to 0
  ],
  thresholds: {
    http_req_duration: ['p(99)<100'], // 99% of requests should be below 100ms
    http_req_failed: ['rate<0.01'], // Error rate should be below 1%
  },
};

// Test data
const tenants = ['tenant1', 'tenant2', 'tenant3', 'tenant4', 'tenant5'];
const languages = ['en', 'de', 'fr', 'es', 'it', 'pt', 'nl', 'pl'];

export default function () {
  // Random tenant and language selection
  const tenantId = tenants[Math.floor(Math.random() * tenants.length)];
  const language = languages[Math.floor(Math.random() * languages.length)];

  // Test client-side API endpoint
  const clientResponse = http.get(
    `http://localhost:8000/api/v1/localization/${tenantId}/${language}`,
    {
      headers: {
        'Accept': 'application/json',
        'User-Agent': 'k6-load-test',
      },
    }
  );

  check(clientResponse, {
    'client API status is 200': (r) => r.status === 200,
    'client API response time < 100ms': (r) => r.timings.duration < 100,
    'client API has translations': (r) => r.json().length > 0,
  });

  // Test SSR endpoint (simulate server-side loading)
  const ssrResponse = http.get(
    `http://localhost:8000/api/v1/localization/ssr/${tenantId}/${language}`,
    {
      headers: {
        'Accept': 'application/json',
        'User-Agent': 'k6-ssr-test',
      },
    }
  );

  check(ssrResponse, {
    'SSR API status is 200': (r) => r.status === 200,
    'SSR API response time < 500ms': (r) => r.timings.duration < 500,
    'SSR API has translations': (r) => r.json().length > 0,
  });

  // Simulate realistic user behavior
  sleep(Math.random() * 2 + 1); // Sleep 1-3 seconds between requests
}