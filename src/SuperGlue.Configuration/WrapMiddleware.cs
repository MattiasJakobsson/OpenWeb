﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperGlue.Configuration
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class WrapMiddleware<TMiddleware>
    {
        private readonly AppFunc _next;
        private readonly WrapMiddlewareOptions<TMiddleware> _options;

        public WrapMiddleware(AppFunc next, WrapMiddlewareOptions<TMiddleware> options)
        {
            if (next == null)
                throw new ArgumentNullException("next");

            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            foreach (var wrapper in _options.Wrappers)
                await wrapper.Before();

            await _next(environment);

            foreach (var wrapper in _options.Wrappers)
                await wrapper.After();
        }
    }

    public class WrapMiddlewareOptions<TMiddleware>
    {
        public WrapMiddlewareOptions(IEnumerable<IWrapMiddleware<TMiddleware>> wrappers)
        {
            Wrappers = wrappers;
        }

        public IEnumerable<IWrapMiddleware<TMiddleware>> Wrappers { get; private set; } 
    }
}