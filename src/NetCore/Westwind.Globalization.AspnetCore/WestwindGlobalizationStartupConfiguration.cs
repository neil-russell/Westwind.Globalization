﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;


namespace Westwind.Globalization.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWestwindGlobalization(this IApplicationBuilder app)
        {
            return app;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWestwindGlobalization(this IServiceCollection services,
            Action<DbResourceConfiguration> setOptionsAction = null)
        {
            // we allow configuration via AppSettings so make sure that's loaded
            services.AddOptions();

            // initialize the static instance from DbResourceConfiguration.json if it exists 
            // But you can override with a new customized instance if desired
            DbResourceConfiguration.Current.Initialize();
            var config = DbResourceConfiguration.Current;

            var provider = services.BuildServiceProvider();
            var serviceConfiguration = provider.GetService<IConfiguration>();

            // read settings from DbResourceConfiguration in Appsettings.json
            services.Configure<DbResourceConfiguration>(serviceConfiguration.GetSection("DbResourceConfiguration"));           
            var configData = provider.GetRequiredService<IOptions<DbResourceConfiguration>>();
            if (configData != null && configData.Value != null && !configData.Value.ConnectionString.StartsWith("***"))
            {
                config = configData.Value;
                DbResourceConfiguration.Current = config;
            }
            setOptionsAction?.Invoke(config);

            // register with DI
            services.AddSingleton(config);

            return services;
        }
    }
}