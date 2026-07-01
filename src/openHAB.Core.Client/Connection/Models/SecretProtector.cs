using System;
using System.Security.Cryptography;
using System.Text;

namespace openHAB.Core.Client.Connection.Models;

/// <summary>
/// Protects small secrets (credentials) for on-disk storage using Windows DPAPI, scoped to the
/// current user. Values are encrypted with the user's login key and stored as base64 in the
/// connection file, so they are unreadable by other users or offline disk access.
/// </summary>
/// <remarks>
/// ponytail: DPAPI is Windows-only. If a non-Windows UI head is ever put on this client, swap the
/// two calls below for that platform's keystore (Keychain/libsecret) — the seam is these two methods.
/// </remarks>
internal static class SecretProtector
{
    /// <summary>
    /// Encrypts a plaintext secret for the current user, returning a base64 string. Null/empty passes through.
    /// </summary>
    /// <param name="plaintext">The value to protect.</param>
    /// <returns>The DPAPI-protected value as base64, or the original value if null/empty.</returns>
    public static string Protect(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            return plaintext;
        }

        byte[] encrypted = ProtectedData.Protect(Encoding.UTF8.GetBytes(plaintext), null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// Decrypts a value produced by <see cref="Protect"/>. If the value is not valid protected data
    /// (e.g. a legacy plaintext credential being migrated), it is returned unchanged.
    /// </summary>
    /// <param name="protectedValue">The base64 protected value.</param>
    /// <returns>The decrypted plaintext, or the input unchanged if it was not protected data.</returns>
    public static string Unprotect(string protectedValue)
    {
        if (string.IsNullOrEmpty(protectedValue))
        {
            return protectedValue;
        }

        try
        {
            byte[] decrypted = ProtectedData.Unprotect(Convert.FromBase64String(protectedValue), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
        catch (FormatException)
        {
            return protectedValue;
        }
        catch (CryptographicException)
        {
            return protectedValue;
        }
    }
}
