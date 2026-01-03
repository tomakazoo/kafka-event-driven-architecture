# Security: SSL/TLS and SASL Authentication

This example demonstrates Kafka security configurations for SSL/TLS encryption and SASL authentication.

## 📚 Concepts

### SSL/TLS Encryption

**SSL/TLS** encrypts data in transit between clients and brokers.

**Configuration:**
- `SecurityProtocol = SecurityProtocol.Ssl`
- `SslCaLocation`: CA certificate path
- `SslCertificateLocation`: Client certificate path
- `SslKeyLocation`: Client private key path

**Use Cases:**
- Encrypt data in transit
- Prevent man-in-the-middle attacks
- Compliance requirements

### SASL Authentication

**SASL** (Simple Authentication and Security Layer) provides authentication.

**Mechanisms:**
- **PLAIN**: Username/password (simple, not encrypted without SSL)
- **SCRAM**: Salted Challenge Response Authentication (more secure)
- **GSSAPI**: Kerberos authentication
- **OAUTHBEARER**: OAuth 2.0 authentication

**Configuration:**
- `SecurityProtocol = SecurityProtocol.SaslSsl` (or `SaslPlaintext`)
- `SaslMechanism`: Authentication mechanism
- `SaslUsername`: Username
- `SaslPassword`: Password

**Use Cases:**
- Authenticate clients
- Control access to topics
- Multi-tenant environments

## 🏗️ Structure

```
05-security/
├── SSL/
│   ├── SslProducer.cs        # SSL producer example
│   ├── SslConsumer.cs         # SSL consumer example
│   ├── SSL.csproj
│   └── README.md              # SSL setup instructions
├── SASL/
│   ├── SaslProducer.cs       # SASL producer example
│   ├── SaslConsumer.cs       # SASL consumer example
│   ├── SASL.csproj
│   └── README.md              # SASL setup instructions
└── README.md
```

## ⚠️ Important Notes

**These examples require broker-side configuration:**

1. **SSL**: Requires certificates and broker SSL configuration
2. **SASL**: Requires JAAS configuration and broker SASL setup

**For local development:**
- SSL/SASL are typically not enabled
- These examples show the **client configuration** needed
- See individual README files for broker setup instructions

## 🚀 How to Run

### Prerequisites
- Kafka broker configured with SSL or SASL
- Certificates (for SSL) or credentials (for SASL)
- .NET 8.0 SDK

### Step 1: Configure Kafka Broker

See:
- `SSL/README.md` for SSL setup
- `SASL/README.md` for SASL setup

### Step 2: Update Client Configuration

Update certificate paths and credentials in:
- `SSL/SslProducer.cs` and `SSL/SslConsumer.cs`
- `SASL/SaslProducer.cs` and `SASL/SaslConsumer.cs`

### Step 3: Run Examples

```bash
cd examples/05-advanced-kafka/dotnet/05-security

# SSL examples
dotnet run --project SSL/SSL.csproj

# SASL examples
dotnet run --project SASL/SASL.csproj
```

## 🔍 What to Observe

1. **SSL**: Encrypted connections to Kafka
2. **SASL**: Authenticated access to Kafka
3. **Configuration**: Client-side security settings

## 📖 Key Takeaways

- **SSL/TLS**: Encrypts data in transit
- **SASL**: Provides authentication
- **Combined**: Use `SaslSsl` for both encryption and authentication
- **Production**: Always use SSL/SASL in production

## 🔗 Related Concepts

- Access Control Lists (ACLs)
- Certificate management
- Authentication mechanisms
- Security best practices

## 💡 Production Recommendations

1. **Always use SSL** in production
2. **Use SASL** for authentication
3. **Use SCRAM** instead of PLAIN (more secure)
4. **Rotate certificates** regularly
5. **Use ACLs** to control topic access

