namespace B2X.Shared.Monitoring.Enums;

/// <summary>
/// Enumeration of communication error types encountered during service integration.
/// </summary>
public enum CommunicationErrorType
{
    /// <summary>
    /// Network timeout during connection attempt
    /// </summary>
    Timeout = 0,

    /// <summary>
    /// Connection refused by remote host
    /// </summary>
    ConnectionRefused = 1,

    /// <summary>
    /// DNS resolution failed
    /// </summary>
    DnsError = 2,

    /// <summary>
    /// SSL/TLS certificate validation failed
    /// </summary>
    CertificateError = 3,

    /// <summary>
    /// Authentication/authorization failed (401, 403)
    /// </summary>
    AuthenticationError = 4,

    /// <summary>
    /// HTTP 5xx server error
    /// </summary>
    ServerError = 5,

    /// <summary>
    /// HTTP 4xx client error (non-auth related)
    /// </summary>
    ClientError = 6,

    /// <summary>
    /// Bad gateway or upstream service unavailable
    /// </summary>
    GatewayError = 7,

    /// <summary>
    /// Invalid or malformed response from service
    /// </summary>
    InvalidResponse = 8,

    /// <summary>
    /// Request body serialization failed
    /// </summary>
    SerializationError = 9,

    /// <summary>
    /// Response body deserialization failed
    /// </summary>
    DeserializationError = 10,

    /// <summary>
    /// Protocol error (not HTTP)
    /// </summary>
    ProtocolError = 11,

    /// <summary>
    /// Proxy connection error
    /// </summary>
    ProxyError = 12,

    /// <summary>
    /// Unknown or unclassified error
    /// </summary>
    Unknown = 99
}
