# API Documentation Template

**API:** [API Name]  
**Version:** [Version]  
**Last Updated:** [Date]  
**Owner:** [Team/Individual]

## Overview
[Brief description of what this API does and its purpose]

## Base URL
```
[Production/Staging URL]
```

## Authentication
[Describe authentication method - JWT, API Key, etc.]

## Endpoints

### [Endpoint Name]
**Method:** [GET/POST/PUT/DELETE]  
**Path:** `/api/[path]`  
**Description:** [What this endpoint does]

#### Parameters
| Name | Type | Required | Description |
|------|------|----------|-------------|
| param1 | string | Yes | Description of param1 |

#### Request Body
```json
{
  "example": "data"
}
```

#### Response
**Status:** 200 OK
```json
{
  "success": true,
  "data": {
    "example": "response"
  }
}
```

**Status:** 400 Bad Request
```json
{
  "error": "Validation failed",
  "details": ["Field is required"]
}
```

#### Example Usage
```bash
curl -X GET "https://api.example.com/api/endpoint" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json"
```

## Error Codes
| Code | Description |
|------|-------------|
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 500 | Internal Server Error |

## Rate Limits
- [Describe rate limiting if applicable]

## Changelog
- v1.0.0 (2026-01-01): Initial release
- [Add version history]

## Support
For questions or issues, contact [support email/team]