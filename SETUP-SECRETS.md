# 🔐 Setting Up API Secrets

**IMPORTANT**: The API key is NOT stored in appsettings.json - you must configure it using one of the methods below.

## 🛠️ Setup Methods

### **Option 1: User Secrets (Recommended for Development)**

User Secrets stores sensitive data outside your project folder, preventing accidental commits.

#### Step 1: Initialize User Secrets
```powershell
cd src/Kotoba.Server
dotnet user-secrets init
```

#### Step 2: Set Your Gemini API Key
```powershell
dotnet user-secrets set "GoogleGemini:ApiKey" "YOUR_ACTUAL_GEMINI_API_KEY"
```

#### Step 3: (Optional) Override Model List
```powershell
dotnet user-secrets set "GoogleGemini:FallbackChain:0:ModelName" "gemini-1.5-pro"
dotnet user-secrets set "GoogleGemini:FallbackChain:0:MaxTokens" "150"
dotnet user-secrets set "GoogleGemini:FallbackChain:1:ModelName" "gemini-1.5-flash"
dotnet user-secrets set "GoogleGemini:FallbackChain:1:MaxTokens" "150"
```

#### View Current Secrets
```powershell
dotnet user-secrets list
```

---

### **Option 2: Environment Variables (Production/Docker)**

Set environment variables before running the app:

#### Windows PowerShell:
```powershell
$env:GoogleGemini__ApiKey = "YOUR_ACTUAL_GEMINI_API_KEY"
dotnet run --project src/Kotoba.Server
```

#### Linux/macOS:
```bash
export GoogleGemini__ApiKey="YOUR_ACTUAL_GEMINI_API_KEY"
dotnet run --project src/Kotoba.Server
```

#### Docker Compose:
```yaml
services:
  kotoba-server:
    environment:
      - GoogleGemini__ApiKey=${GEMINI_API_KEY}
```

---

### **Option 3: appsettings.Development.json (Local Only)**

⚠️ **Only for local development - already in .gitignore**

Create or edit `src/Kotoba.Server/appsettings.Development.json`:
```json
{
  "GoogleGemini": {
    "ApiKey": "YOUR_ACTUAL_GEMINI_API_KEY"
  }
}
```

This file is automatically ignored by Git, so it's safe for local development.

---

## 🔍 Configuration Priority

ASP.NET Core loads configuration in this order (later overrides earlier):
1. `appsettings.json` (base config)
2. `appsettings.{Environment}.json` (environment-specific)
3. **User Secrets** (Development only)
4. **Environment Variables** (all environments)
5. Command-line arguments

---

## ✅ Verify Setup

Run the server and check logs:
```powershell
cd src/Kotoba.Server
dotnet run
```

If API key is missing, you'll see:
```
[Error] Google Gemini API key is not configured.
```

If successful:
```
[Information] Starting AI suggestion generation...
```

---

## 🌐 Get Your Gemini API Key

1. Visit: https://makersuite.google.com/app/apikey
2. Create new API key
3. Copy the key
4. Use one of the setup methods above

---

## 📦 Production Deployment

For production, use:
- **Azure**: Azure Key Vault + Managed Identity
- **AWS**: AWS Secrets Manager + IAM
- **Docker**: Environment variables from secrets
- **Kubernetes**: Kubernetes Secrets

Never hardcode keys in appsettings.json!
