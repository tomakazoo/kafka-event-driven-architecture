# How to Run: Security Examples

Step-by-step guide to running SSL and SASL security examples.

## ⚠️ Important Note

**These examples require broker-side configuration.**

For local development, SSL/SASL are typically **not enabled**. These examples show the **client configuration** needed, but won't work without broker setup.

## Prerequisites

- Kafka broker configured with SSL or SASL
- Certificates (for SSL) or credentials (for SASL)
- .NET 8.0 SDK

## SSL/TLS Example

### Step 1: Configure Kafka Broker for SSL

See `SSL/README.md` for detailed broker setup instructions.

**Quick summary:**
1. Generate CA certificate
2. Generate broker certificates
3. Generate client certificates
4. Configure broker with SSL listeners
5. Update `server.properties`

### Step 2: Update Client Configuration

Edit `SSL/SslProducer.cs` and `SSL/SslConsumer.cs`:

```csharp
SslCaLocation = "/path/to/ca-cert",           // Update path
SslCertificateLocation = "/path/to/client-cert", // Update path
SslKeyLocation = "/path/to/client-key",        // Update path
```

### Step 3: Run SSL Producer

```bash
cd examples/05-advanced-kafka/dotnet/05-security
dotnet run --project SSL/SSL.csproj
```

**Expected Output:**
```
🔒 SSL/TLS Producer Example
⚠️  NOTE: Requires SSL-enabled Kafka broker

📋 SSL Configuration:
   Security Protocol: Ssl
   CA Location: /path/to/ca-cert
   Certificate Location: /path/to/client-cert
   Key Location: /path/to/client-key

✅ SSL Producer created
```

### Step 4: Run SSL Consumer

**Modify `SSL.csproj` to use `SslConsumer`:**

```xml
<StartupObject>Security.SSL.SslConsumer</StartupObject>
```

Then run:

```bash
dotnet run --project SSL/SSL.csproj
```

## SASL Example

### Step 1: Configure Kafka Broker for SASL

See `SASL/README.md` for detailed broker setup instructions.

**Quick summary:**
1. Create JAAS configuration file
2. Configure broker with SASL listeners
3. Update `server.properties`
4. Create users (for PLAIN mechanism)

### Step 2: Update Client Configuration

Edit `SASL/SaslProducer.cs` and `SASL/SaslConsumer.cs`:

```csharp
SaslUsername = "alice",              // Update username
SaslPassword = "password",          // Update password
SslCaLocation = "/path/to/ca-cert", // Update if using SaslSsl
```

### Step 3: Run SASL Producer

```bash
dotnet run --project SASL/SASL.csproj
```

**Expected Output:**
```
🔐 SASL Producer Example (PLAIN)
⚠️  NOTE: Requires SASL-enabled Kafka broker

📋 SASL Configuration:
   Security Protocol: SaslSsl
   SASL Mechanism: Plain
   Username: alice
   Password: ********

✅ SASL Producer created
```

### Step 4: Run SASL Consumer

**Modify `SASL.csproj` to use `SaslConsumer`:**

```xml
<StartupObject>Security.SASL.SaslConsumer</StartupObject>
```

Then run:

```bash
dotnet run --project SASL/SASL.csproj
```

## 🔍 Understanding Security

### SSL/TLS

- **Encrypts** data in transit
- **Prevents** man-in-the-middle attacks
- **Required** for production

### SASL

- **Authenticates** clients
- **Controls** access to topics
- **Works with** SSL for secure authentication

### Combined (SaslSsl)

- **Both** encryption and authentication
- **Recommended** for production
- **Most secure** option

## 🐛 Troubleshooting

### SSL Connection Failed

- Verify broker SSL is enabled
- Check certificate paths are correct
- Verify certificates are valid
- Check broker SSL port (default: 9093)

### SASL Authentication Failed

- Verify broker SASL is enabled
- Check username/password are correct
- Verify JAAS configuration
- Check broker SASL port

### Certificates Not Found

- Verify certificate paths
- Check file permissions
- Ensure certificates are readable

## 📚 Next Steps

- Learn multi-DC replication (06-multi-dc-replication)
- Explore delivery semantics (01-delivery-semantics)
- Understand performance tuning (04-performance-tuning)

## 💡 Production Checklist

- [ ] SSL/TLS enabled
- [ ] SASL authentication configured
- [ ] Certificates properly managed
- [ ] ACLs configured for topic access
- [ ] Credentials stored securely (not hardcoded)
- [ ] Certificate rotation process in place


