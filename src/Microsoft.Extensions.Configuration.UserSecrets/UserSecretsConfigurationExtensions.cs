// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Configuration extensions for adding user secrets configuration source.
    /// </summary>
    public static class UserSecretsConfigurationExtensions
    {
        private const string Secrets_File_Name = "secrets.json";

        /// <summary>
        /// Adds the user secrets configuration source with specified secrets id.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="userSecretsId"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, string userSecretsId)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (userSecretsId == null)
            {
                throw new ArgumentNullException(nameof(userSecretsId));
            }

            var prefix = userSecretsId + ":";
            return AddUserSecrets(configuration,
                k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase),
                k => k.Substring(prefix.Length));
        }

        public static IConfigurationBuilder AddUserSecrets(IConfigurationBuilder configuration,
            Predicate<string> keyFilter,
            Func<string, string> keyModifier,
            bool optional = true)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (keyFilter == null)
            {
                throw new ArgumentNullException(nameof(keyFilter));
            }

            if (keyModifier == null)
            {
                throw new ArgumentNullException(nameof(keyModifier));
            }

            return configuration.AddUserSecrets(source =>
            {
                source.KeyFilter = keyFilter;
                source.KeyModifier = keyModifier;
                source.Optional = optional;
            });
        }

        private static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder builder, Action<UserSecretsConfigurationSource> configSource)
            => builder.Add(configSource);
    }
}