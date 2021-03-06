﻿using Bit.Core.Implementations;
using Bit.IdentityServer.Contracts;
using IdentityServer3.Core.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Bit.IdentityServer.Implementations
{
    public class DefaultCustomLoginDataProvider : ICustomLoginDataProvider
    {
        public virtual dynamic GetCustomData(SignInMessage signInMessage)
        {
            if (signInMessage == null)
                throw new ArgumentNullException(nameof(signInMessage));

            return JsonConvert.DeserializeObject<dynamic>(new Uri(signInMessage.ReturnUrl).ParseQueryString()["state"], DefaultJsonContentFormatter.DeSerializeSettings());
        }
    }
}
