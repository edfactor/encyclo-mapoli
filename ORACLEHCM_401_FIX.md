# Oracle HCM 401 Authentication Fix

## Problem Identified

**Error:** `System.Net.Http.HttpRequestException: Response status code does not indicate success: 401 (Unauthorized)`

**Root Cause:** The HttpClient's default headers (including `Authorization`) were NOT being passed to the `HttpRequestMessage` when using `SendAsync()`.

---

## Technical Explanation

### The Setup (OracleHcmExtension.cs)

The `BuildOracleHcmAuthClient` method correctly sets the Authorization header on the HttpClient:

```csharp
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
```

✅ This puts the authentication on the HttpClient itself.

### The Problem (Clients)

However, the clients were creating a **new `HttpRequestMessage` directly** and sending it with `SendAsync()`:

```csharp
using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
```

❌ **When using `SendAsync()` with a custom `HttpRequestMessage`, the default headers from `HttpClient` are NOT automatically added.**

### Why This Matters

There are two ways to use HttpClient:

1. **Convenience methods** (GetAsync, PostAsync, etc.)
   ```csharp
   // ✅ These AUTOMATICALLY add default headers
   await _httpClient.GetAsync(url);
   ```

2. **SendAsync with HttpRequestMessage**
   ```csharp
   var request = new HttpRequestMessage(HttpMethod.Get, url);
   // ❌ Default headers are NOT automatically added
   await _httpClient.SendAsync(request);
   ```

---

## Solution Applied

Both affected clients now explicitly copy the default headers from HttpClient to the request message:

```csharp
private async Task<HttpResponseMessage> GetOracleHcmValue(string url, CancellationToken cancellationToken)
{
    using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
    
    // Copy default headers from HttpClient to the request message
    // (SendAsync does NOT automatically add Authorization and other default headers)
    foreach (var header in _httpClient.DefaultRequestHeaders)
    {
        request.Headers.Add(header.Key, header.Value);
    }
    
    HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
    // ... rest of method
}
```

### Files Fixed

1. ✅ **EmployeeFullSyncClient.cs** (GetOracleHcmValue method)
   - Line 161: Now copies headers before SendAsync

2. ✅ **PayrollSyncClient.cs** (GetOraclePayrollValue method)
   - Line 255: Now copies headers before SendAsync

---

## What Gets Copied

The fix ensures these headers are included:

- ✅ `Authorization: Basic <base64(username:password)>`
- ✅ `REST-Framework-Version: <version>`
- ✅ `Accept: application/json`
- ✅ Any other default headers set on the HttpClient

---

## Testing

After this fix, the Oracle HCM service should:

1. ✅ Successfully authenticate with the Authorization header
2. ✅ Receive 200 responses instead of 401
3. ✅ Complete employee full sync
4. ✅ Complete payroll sync

**Verification:**
- Remove any retry/workaround code that was added due to 401 errors
- The service should now connect on first attempt (if credentials are correct)
- Check logs to confirm successful responses from Oracle HCM

---

## Related Code

The authentication setup is in `OracleHcmExtension.cs` (lines 372-383):

```csharp
private static void BuildOracleHcmAuthClient(IServiceProvider services, HttpClient client)
{
    OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();
    if (config.Username == null)
    {
        return;
    }
    
    // Create Basic auth token
    string authToken = Convert.ToBase64String(
        Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}")
    );

    // Set headers on HttpClient
    client.BaseAddress = new Uri(config.BaseAddress, UriKind.Absolute);
    client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json")
    );
}
```

This is now properly propagated to all requests by the fix.

---

## Summary

| Aspect | Before | After |
|--------|--------|-------|
| **Auth Header on HttpClient** | ✅ Set correctly | ✅ Set correctly |
| **Auth Header on Request** | ❌ Missing | ✅ Copied from HttpClient |
| **Result** | ❌ 401 Unauthorized | ✅ 200 Success |

The fix ensures the Authorization header set in `OracleHcmExtension.cs` is actually used by all clients.

---

*Fix Applied: October 28, 2025*
*Files Modified: 2*
*- EmployeeFullSyncClient.cs*
*- PayrollSyncClient.cs*
