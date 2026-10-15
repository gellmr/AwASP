using System.Security.Cryptography;
using System.Text;

namespace AngularWithASP.Server.Infrastructure
{
  public static class SecurePasswordEquals
  {
    // Helper method to compare password string for equality
    // prevents Non-Constant-Time String Comparison (Timing Attack)
    // Vulnerability: Standard string comparison like != short-circuits and terminates evaluation on the first non-matching character.
    // Impact: An attacker can send requests and measure microsecond response time differences to guess the password character-by-character.
    // Remediation: Use time-constant comparison: CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(_vipPassword))

    public static bool FixedTimeCompare(string? providedPassword, string? actualPassword)
    {
      if (string.IsNullOrEmpty(providedPassword) || string.IsNullOrEmpty(actualPassword))
      {
        return false;
      }

      byte[] providedBytes = Encoding.UTF8.GetBytes(providedPassword);
      byte[] actualBytes = Encoding.UTF8.GetBytes(actualPassword);

      // FixedTimeEquals requires byte arrays of equal length. 
      // To prevent timing attacks while handling length mismatches safely, 
      // we compare lengths first, but always compare the byte arrays using FixedTimeEquals 
      // to maintain a consistent code path (or fallback safely).
      if (providedBytes.Length != actualBytes.Length)
      {
        // Run a dummy comparison to help mitigate length timing leaks, 
        // or simply return false after ensuring constant time.
        CryptographicOperations.FixedTimeEquals(actualBytes, actualBytes);
        return false;
      }

      return CryptographicOperations.FixedTimeEquals(providedBytes, actualBytes);
    }
  }
}
