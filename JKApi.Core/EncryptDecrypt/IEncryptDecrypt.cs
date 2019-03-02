using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Core
{
    public interface IEncryptDecrypt
    {
        /// <summary>
        /// Text Convert into Encrypt Value
        /// </summary>
        /// <param name="data">Text Value</param>
        /// <returns></returns>
        string Encrypt(string data);

        /// <summary>
        /// Encrypt value Convert into Actual text
        /// </summary>
        /// <param name="data">Encrypt value</param>
        /// <returns></returns>
        string Decrypt(string data);
    }
}
