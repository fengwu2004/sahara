using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.Utils
{
    public static class HashGen
    {
        private static MD5 md5Hash = MD5.Create();

        public static string GenerateMd5(string content)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(content));
            StringBuilder sBuilder = new StringBuilder();    
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
