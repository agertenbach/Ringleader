﻿using Microsoft.Extensions.Options;
using Ringleader.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Ringleader.HttpClientFactory
{
    public class DefaultActionedPrimaryHandlerFactoryOptions
    {
        /// <summary>
        /// Function to optionally resolve an <see cref="HttpMessageHandler"/> based on a specified client and/or context
        /// </summary>
        public Func<TypedClientSignature, string, HttpMessageHandler?> HandlerFactory { get; set; } 
            = (client, context) => null;
    }

    /// <summary>
    /// Resolves a primary handler based on a registered function
    /// </summary>
    public class DefaultActionedPrimaryHandlerFactory : IPrimaryHandlerFactory
    {
        private readonly IOptionsMonitor<DefaultActionedPrimaryHandlerFactoryOptions> _options;

        public DefaultActionedPrimaryHandlerFactory(IOptionsMonitor<DefaultActionedPrimaryHandlerFactoryOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public HttpMessageHandler? CreateHandler(TypedClientSignature clientName, string handlerContext)
            => _options.CurrentValue.HandlerFactory.Invoke(clientName, handlerContext);
    }
}
