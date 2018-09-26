using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMSLog.Portal.Services
{
    public interface ICrypto
    {
        string Decrypt(string encryptedBase64ConnectString);
        string Encrypt(string plainConnectString);

        string JsDecrypt(string key);
    }
}