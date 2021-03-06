﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinBetting.Core.Interfaces
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(string uri);

        Task<TResult> PostAsync<TData, TResult>(string uri, TData data, List<KeyValuePair<string, string>> cookies = null);
        Task PostAsync(string uri);

        Task DeleteAsync(string uri);
    }
}
