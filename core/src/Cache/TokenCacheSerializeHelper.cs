﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using Microsoft.Identity.Core.Cache;
using Microsoft.Identity.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Core.Cache
{
    class TokenCacheSerializeHelper
    {
        private static readonly string AccessTokenKey = "access_tokens";
        private static readonly string RefreshTokenKey = "refresh_tokens";
        private static readonly string IdTokenKey = "id_tokens";
        private static readonly string AccountKey = "accounts";

        /// <summary>
        /// Deserializes the token cache from a serialization blob
        /// </summary>
        /// <param name="tokenCacheAccessor">Token cache accessor to perform cache write operations (to fill-in from the state)</param>
        /// <param name="state">Array of bytes containing serialized cache data</param>
        /// <remarks>
        /// <paramref name="state"/>Is a Json blob containing access tokens, refresh tokens, id tokens and accounts information
        /// </remarks>
        public static void DeserializeMsalCache(TokenCacheAccessor tokenCacheAccessor, byte[] state, RequestContext requestContext)
        {
            Dictionary<string, IEnumerable<string>> cacheDict = JsonHelper
                .DeserializeFromJson<Dictionary<string, IEnumerable<string>>>(state);

            if (cacheDict == null || cacheDict.Count == 0)
            {
                string msg = "Msal Cache is empty.";
                CoreLoggerBase.Default.Info(msg);
                CoreLoggerBase.Default.InfoPii(msg);
                return;
            }

            if (cacheDict.ContainsKey(AccessTokenKey))
            {
                foreach (var atItem in cacheDict[AccessTokenKey])
                {
                    var msalAccessTokenCacheItem = JsonHelper.TryToDeserializeFromJson<MsalAccessTokenCacheItem>(atItem, requestContext);
                    if (msalAccessTokenCacheItem != null)
                    {
                        tokenCacheAccessor.SaveAccessToken(msalAccessTokenCacheItem);
                    }
                }
            }

            if (cacheDict.ContainsKey(RefreshTokenKey))
            {
                foreach (var rtItem in cacheDict[RefreshTokenKey])
                {
                    var msalRefreshTokenCacheItem = JsonHelper.TryToDeserializeFromJson<MsalRefreshTokenCacheItem>(rtItem, requestContext);
                    if (msalRefreshTokenCacheItem != null)
                    {
                        tokenCacheAccessor.SaveRefreshToken(msalRefreshTokenCacheItem);
                    }
                }
            }

            if (cacheDict.ContainsKey(IdTokenKey))
            {
                foreach (var idItem in cacheDict[IdTokenKey])
                {
                    var msalIdTokenCacheItem = JsonHelper.TryToDeserializeFromJson<MsalIdTokenCacheItem>(idItem, requestContext);
                    if (msalIdTokenCacheItem != null)
                    {
                        tokenCacheAccessor.SaveIdToken(msalIdTokenCacheItem);
                    }
                }
            }

            if (cacheDict.ContainsKey(AccountKey))
            {
                foreach (var account in cacheDict[AccountKey])
                {
                    var msalAccountCacheItem = JsonHelper.TryToDeserializeFromJson<MsalIdTokenCacheItem>(account, requestContext);

                    tokenCacheAccessor.SaveAccount(JsonHelper.DeserializeFromJson<MsalAccountCacheItem>(account));
                }
            }
        }

        /// <summary>
        /// Serializes the entiere token cache
        /// </summary>
        /// <param name="tokenCacheAccessor">Token cache accessor to perform cache read operations</param>
        /// <returns>array of bytes containing the serialized cache</returns>
        public static byte[] SerializeMsalCache(TokenCacheAccessor tokenCacheAccessor)
        {
            // reads the underlying in-memory dictionary and dumps out the content as a JSON
            Dictionary<string, IEnumerable<string>> cacheDict = new Dictionary<string, IEnumerable<string>>
            {
                [AccessTokenKey] = tokenCacheAccessor.GetAllAccessTokensAsString(),
                [RefreshTokenKey] = tokenCacheAccessor.GetAllRefreshTokensAsString(),
                [IdTokenKey] = tokenCacheAccessor.GetAllIdTokensAsString(),
                [AccountKey] = tokenCacheAccessor.GetAllAccountsAsString()
            };

            return JsonHelper.SerializeToJson(cacheDict).ToByteArray();
        }
    }
}
