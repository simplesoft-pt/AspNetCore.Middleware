#region License
// The MIT License (MIT)
// 
// Copyright (c) 2018 Simplesoft.pt
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    internal class HealthCheckBuilder : IHealthCheckBuilder
    {
        private readonly IServiceCollection _services;

        public HealthCheckBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public void Add(IHealthCheck healthCheck)
        {
            if (healthCheck == null) throw new ArgumentNullException(nameof(healthCheck));
            _services.AddSingleton(healthCheck);
        }

        public void Add<T>() where T : class, IHealthCheck
        {
            _services.AddScoped<IHealthCheck, T>();
        }

        public void Add(Func<IServiceProvider, IHealthCheck> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _services.AddScoped(factory);
        }

        public void Register(Action<IServiceCollection> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder(_services);
        }
    }
}