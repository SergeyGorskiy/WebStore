using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Store.Web.Models;

namespace Store.Web
{
    public static class SessionExtensions
    {
        private const string key = "Cart";
        public static void Set(this ISession session, Cart value)
        {
            if(value == null)
                return;
            using(var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {

            }

        }
    }
}
