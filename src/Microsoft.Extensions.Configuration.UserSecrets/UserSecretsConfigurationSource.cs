using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration.UserSecrets
{
    using IOPath = Path;

    public class UserSecretsConfigurationSource : FileConfigurationSource
    {
        private const string Secrets_File_Name = "secrets.json";

        private readonly string _basePath;

        public UserSecretsConfigurationSource()
        {
            _basePath = ResolveBasePath();
            Path = Secrets_File_Name;
            ReloadOnChange = true;
        }

        public Predicate<string> KeyFilter { get; set; }
        public Func<string, string> KeyModifier { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            FileProvider = new PhysicalFileProvider(_basePath);

            return new UserSecretsConfigurationProvider(this);
        }

        private static string ResolveBasePath()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPDATA")))
            {
                return IOPath.Combine(Environment.GetEnvironmentVariable("APPDATA"), "Microsoft", "UserSecrets");
            }
            else
            {
                return IOPath.Combine(Environment.GetEnvironmentVariable("HOME"), ".microsoft", "usersecrets");
            }
        }
    }
}
