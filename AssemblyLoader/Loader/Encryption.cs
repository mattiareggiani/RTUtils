using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
// https://gist.github.com/magicsih/be06c2f60288b54d9f52856feb96ce8c
class Aes
{
    private static RijndaelManaged rijndael = new RijndaelManaged();
    private static System.Text.UnicodeEncoding unicodeEncoding = new UnicodeEncoding();

    private const int CHUNK_SIZE = 128;

    private void InitializeRijndael()
    {
        rijndael.Mode = CipherMode.CBC;
        rijndael.Padding = PaddingMode.PKCS7;
    }

    public Aes()
    {
        InitializeRijndael();

        rijndael.KeySize = CHUNK_SIZE;
        rijndael.BlockSize = CHUNK_SIZE;

        rijndael.GenerateKey();
        rijndael.GenerateIV();
    }

    public Aes(String base64key, String base64iv)
    {
        InitializeRijndael();

        rijndael.Key = Convert.FromBase64String(base64key);
        rijndael.IV = Convert.FromBase64String(base64iv);
    }

    public Aes(byte[] key, byte[] iv)
    {
        InitializeRijndael();

        rijndael.Key = key;
        rijndael.IV = iv;
    }

    public byte[] Decrypt(byte[] cipher)
    {
        ICryptoTransform transform = rijndael.CreateDecryptor();
        byte[] decryptedValue = transform.TransformFinalBlock(cipher, 0, cipher.Length);
        return decryptedValue;
    }

    public byte[] DecryptFromBase64String(string base64cipher)
    {
        return Decrypt(Convert.FromBase64String(base64cipher));
    }

    public byte[] EncryptToByte(byte[] cipher)
    {
        ICryptoTransform encryptor = rijndael.CreateEncryptor();
        byte[] encryptedValue = encryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return encryptedValue;
    }

    public string EncryptToBase64String(byte[] cipher)
    {
        return Convert.ToBase64String(EncryptToByte(cipher));
    }

    public string GetKey()
    {
        return Convert.ToBase64String(rijndael.Key);
    }

    public string GetIV()
    {
        return Convert.ToBase64String(rijndael.IV);
    }

    public override string ToString()
    {
        return "KEY:" + GetKey() + Environment.NewLine + "IV:" + GetIV();
    }
}