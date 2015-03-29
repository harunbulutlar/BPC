using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utility
{
    public interface IGenericEncryption
    {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] cipher);
    }

    public class GenericEncryption<T> : IGenericEncryption where T : SymmetricAlgorithm, new()
    {
        internal const string Inputkey = "560A18CD-6346-4CF0-A2E8-671F9B6B9EA9";
        internal const string Salt = "f676537e-1afe-432e-b614-4785d3b558f2";

        public byte[] Encrypt(byte[] data)
        {
            T algorithm = GetSymmetricAlgorithm();
            ICryptoTransform encryptor = algorithm.CreateEncryptor();
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new BinaryWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }
            var trimmedType = (string) EncryptionFactory.typesTrimmedReverse[typeof (T).ToString()];
            byte[] encryptedBytes = msEncrypt.ToArray();
            byte[] trimmedTypeBytes = Encoding.ASCII.GetBytes(trimmedType);
            int totalLength = 2 + 2 + 2 + trimmedTypeBytes.Length + algorithm.Key.Length + algorithm.IV.Length +
                              encryptedBytes.Length;

            var byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes((short) totalLength));
            byteList.AddRange(trimmedTypeBytes);
            byteList.AddRange(BitConverter.GetBytes((short) algorithm.Key.Length));
            byteList.AddRange(BitConverter.GetBytes((short) algorithm.IV.Length));
            byteList.AddRange(algorithm.Key);
            byteList.AddRange(algorithm.IV);
            byteList.AddRange(encryptedBytes);

            return byteList.ToArray();
        }

        public byte[] Decrypt(byte[] cipher)
        {
            var totalLengthBytes = new byte[2];
            int offset = 0;

            Buffer.BlockCopy(cipher, offset, totalLengthBytes, 0, 2);
            short totalLength = BitConverter.ToInt16(totalLengthBytes, 0);

            var trimmedTypeBytes = new byte[2];
            Buffer.BlockCopy(cipher, offset += 2, trimmedTypeBytes, 0, 2);
            //var trimmedType = Encoding.Default.GetString(trimmedTypeBytes);

            var keyLengthBytes = new byte[2];
            Buffer.BlockCopy(cipher, offset += 2, keyLengthBytes, 0, 2);
            short keyLength = BitConverter.ToInt16(keyLengthBytes, 0);

            var ivLengthBytes = new byte[2];
            Buffer.BlockCopy(cipher, offset += 2, ivLengthBytes, 0, 2);
            short ivLength = BitConverter.ToInt16(ivLengthBytes, 0);

            var keyByte = new byte[keyLength];
            Buffer.BlockCopy(cipher, offset += 2, keyByte, 0, keyLength);

            var ivByte = new byte[ivLength];
            Buffer.BlockCopy(cipher, offset += keyLength, ivByte, 0, ivLength);
            offset += ivLength;

            int dataLength = totalLength - offset;
            var data = new byte[dataLength];
            Buffer.BlockCopy(cipher, offset, data, 0, dataLength);

            T aesAlg = GetSymmetricAlgorithm();
            aesAlg.Key = keyByte;
            aesAlg.IV = ivByte;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor();
            byte[] decrypted;
            using (var msDecrypt = new MemoryStream(data))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new BinaryReader(csDecrypt))
                    {
                        decrypted = srDecrypt.ReadBytes(dataLength);
                    }
                }
            }
            return decrypted;
        }

        private static T GetSymmetricAlgorithm()
        {
            var aesAlg = new T();

            return aesAlg;
        }
    }

    public class EncryptionFactory
    {
        public static readonly OrderedDictionary typesTrimmedForward = new OrderedDictionary
        {
            {"TD", "System.Security.Cryptography.TripleDESCryptoServiceProvider"},
            {"DE", "System.Security.Cryptography.DESCryptoServiceProvider"},
            {"RM", "System.Security.Cryptography.RijndaelManaged"},
            {"RC", "System.Security.Cryptography.RC2CryptoServiceProvider"},
        };

        public static readonly OrderedDictionary typesTrimmedReverse = new OrderedDictionary
        {
            {"System.Security.Cryptography.TripleDESCryptoServiceProvider", "TD"},
            {"System.Security.Cryptography.DESCryptoServiceProvider", "DE"},
            {"System.Security.Cryptography.RijndaelManaged", "RM"},
            {"System.Security.Cryptography.RC2CryptoServiceProvider", "RC"}
        };

        public static IGenericEncryption GetEncryption(string typeIdentifier)
        {
            var typeName = (String) typesTrimmedForward[typeIdentifier];
            Type typeArgument = Type.GetType(typeName);
            Type genericClass = typeof (GenericEncryption<>);
            // MakeGenericType is badly named
            Type constructedClass = genericClass.MakeGenericType(typeArgument);

            return (IGenericEncryption) Activator.CreateInstance(constructedClass);
        }

        public static string GetRandomTrimmedEncryption()
        {
            var rnd = new Random();
            return (String) typesTrimmedReverse[rnd.Next(0, typesTrimmedReverse.Count)];
        }
    }
}