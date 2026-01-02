using System.Security.Cryptography;
using System.Text;

namespace Proyecto_alcaldia.Application.Services;

public interface IImageEncryptionService
{
    string EncryptImage(byte[] imageBytes);
    byte[] DecryptImage(string encryptedData);
}

public class ImageEncryptionService : IImageEncryptionService
{
    // Clave de encriptación AES de 256 bits (32 bytes)
    // En producción, esta clave debería estar en configuración segura
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public ImageEncryptionService(IConfiguration configuration)
    {
        // Obtener la clave desde configuración o generar una por defecto
        var keyString = configuration["Security:ImageEncryptionKey"] ?? "ProyectoAlcaldia2026ImageKey!";
        var ivString = configuration["Security:ImageEncryptionIV"] ?? "AlcaldiaIV2026!!";
        
        // Asegurar que la clave tenga exactamente 32 bytes (256 bits)
        _key = DeriveKeyFromString(keyString, 32);
        // El IV debe tener exactamente 16 bytes (128 bits)
        _iv = DeriveKeyFromString(ivString, 16);
    }

    public string EncryptImage(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
            throw new ArgumentException("Los datos de la imagen no pueden estar vacíos", nameof(imageBytes));

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            csEncrypt.Write(imageBytes, 0, imageBytes.Length);
            csEncrypt.FlushFinalBlock();
        }

        var encrypted = msEncrypt.ToArray();
        // Convertir a Base64 para almacenar como string en la BD
        return Convert.ToBase64String(encrypted);
    }

    public byte[] DecryptImage(string encryptedData)
    {
        if (string.IsNullOrEmpty(encryptedData))
            throw new ArgumentException("Los datos encriptados no pueden estar vacíos", nameof(encryptedData));

        try
        {
            var encryptedBytes = Convert.FromBase64String(encryptedData);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(encryptedBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var msResult = new MemoryStream();
            
            csDecrypt.CopyTo(msResult);
            return msResult.ToArray();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al desencriptar la imagen", ex);
        }
    }

    private static byte[] DeriveKeyFromString(string key, int length)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        var result = new byte[length];
        Array.Copy(hash, result, length);
        return result;
    }
}
