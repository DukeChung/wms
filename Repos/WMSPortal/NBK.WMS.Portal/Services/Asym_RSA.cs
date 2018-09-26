using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace NBK.WMS.Portal.Services
{
    public class Asym_RSA : ICrypto
    {
        private string m_RsaKey = "<RSAKeyValue><Modulus>wOq0QHjSnNmS6qAySWCYhWhMfWZHyCz+u2kTdFSboVoRgAH4T+wobLydGXUVdi2XccJwjvZcPHOZ5vZpYY9Hf9fkpJfxpOwaIB2IV+owq0EFyCdhE7vTFHiZm2cfCo+T8m224KHrMEFsoAhd11eQzyhXIU2K7XHiX5Xu2Jtnn1s=</Modulus><Exponent>AQAB</Exponent><P>9g91q1gltBev0vWlfdkElVXcV7TIu99/nHo5DE5wDDQPGO2Fmtfy02rWlc1G9pm67xcdCgPQ8wKbJ1JuYPY99Q==</P><Q>yLWuJ2/R5Levg3h8f2RZ2EhnyN3+ht7t0sFtdKSBOroU8Mgtvsu6FGkYQdihqN3+mbe3nICq/GuROvg3MUVGDw==</Q><DP>XlYTAPwsiF1EdZbkOdmIHlDqx11yUEUhwbZCROuVnbgfyajWvkTovhGJ76jh+g16U8wCwCIya9il7291DguaOQ==</DP><DQ>AfEQAD2qsCW+wuzVd34HCHqa1myfW7qoXlOUtX4p6eGG9lVZa/EYmb3yiCCKX9HV9rK6Sf9MqCh6PTHNhuJ+rQ==</DQ><InverseQ>yye+av62y1KJrvhUGtw5wqY0rBW8aCwywlqAy4+gOU8OsoNpzSW5j1rLNz7vZxCv/smrVDPm2hvJN745Ln3yow==</InverseQ><D>s/xFn8EZ/myfvXcoc31Dz3O3qWc7oW8ZWhB2rhoh+S/nE97CpQ5XyNtQVuf91fxDR0d5bGg9NclE1U8gknzy3prh4WoRsDv9ik44Dge8FvlFotAWuRJeSlla55m3mv9EcoKq9mxxDAMTin1Bnd70yE/HAnzybgSgFQUhNNxh4kE=</D></RSAKeyValue>";

        public string Decrypt(string encryptedBase64ConnectString)
        {
            byte[] rgb = Convert.FromBase64String(encryptedBase64ConnectString);
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
            provider.FromXmlString(this.m_RsaKey);
            byte[] bytes = provider.Decrypt(rgb, false);
            return Encoding.Unicode.GetString(bytes);
        }
        public string Encrypt(string plainConnectString)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(plainConnectString);
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
            provider.FromXmlString(this.m_RsaKey);
            byte[] inArray = provider.Encrypt(bytes, false);
            return Convert.ToBase64String(inArray, 0, inArray.Length);
        }

        public string JsDecrypt(string key)
        {
            var value = string.Empty;
            //密钥格式要生成pkcs#1格式的   而不是pkcs#8格式的
            string privateKey = @"MIICWgIBAAKBgGsXYGuspXPOoIcBdmLhgMIM+EOE3OwvNcw8FWfYRGeuhvv4ARFg
Pn8W/MRjyc/LbeceLux2h13/eaure3zPUfzzmvDjfKoTs+uTlH21R/vQF17nZJKw
+OJ/xztjILeGzMp2cJj8Yn4khOiuc8yVIZTSJt1O1Q6LSf+S61dD+oLfAgMBAAEC
gYAFia7twn7Hz06TzKqPkHO1FTCdZTh7ajGQ/yZoOVHIPskyLG/5sw1oSwsFKNfd
y/pB8lig17xGSxjCnNqE6L+nkNhDxDiTAF0aTx9odhj3ddNCd4ly6hh7U3AwJ1Mv
lxMhteDs5pptEKsqoZec1mrSrH2KdI1QcLoWHp4+c3VhmQJBAK3Xn3xYYjSpLLBf
z4qwNUvk+VNVnDZwJdXNXgXWb97KxBrKBbfnQVxNBOaXRnlFN5zypZSe9Btit2hO
xh/xHG0CQQCds9yowkpeZFvqQJJBJYTjn4ciAGzhw+1kcRhlkuD60bJIanaZNf5h
UT+Ar4vFnxhxsbD1rXRmfzORFwJ4k7T7AkB77F+WhnLLU2W1/Ta3iNEVXw6U50xK
SjvOY22I/8oTqbuN1UGqTUH8iDLcJi5ouHzidt6uSXl+JCrWQoFdt8UpAkAcUyDP
4s2utIHZyi06rD15EL8ZuU/VIPazcqi6Ha5w/lbyMMUKcas12Xz5ZQ5KuW+PhCXP
io78ld4yMhrMg2MfAkAhvgN/nuufbHjAr1FCTiHB0Yb1mT2H7qPu3CYEAv4lbQp/
JySMw9ym+04zNApecpu/qkr9OABcEOsK9RU+W8gA";

            try
            {
                RSACryptoServiceProvider rsaCryptoServiceProvider = CreateRsaProviderFromPrivateKey(privateKey);
                //把+号，再替换回来
                byte[] res = rsaCryptoServiceProvider.Decrypt(Convert.FromBase64String(key.Replace("%2B", "+")), false);
                value = Encoding.UTF8.GetString(res);
            }
            catch (Exception ex)
            {
                throw new Exception("用户名或密码异常请联系Support");
            }

            return value;
        }


        private RSACryptoServiceProvider CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = System.Convert.FromBase64String(privateKey);

            var RSA = new RSACryptoServiceProvider();
            var RSAparams = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            RSA.ImportParameters(RSAparams);
            return RSA;
        }
        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }


    }
}