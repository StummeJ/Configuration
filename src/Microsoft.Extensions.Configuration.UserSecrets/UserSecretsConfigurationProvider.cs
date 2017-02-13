using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration.Json;

namespace Microsoft.Extensions.Configuration.UserSecrets
{
    internal class UserSecretsConfigurationProvider : FileConfigurationProvider
    {
        public UserSecretsConfigurationProvider(UserSecretsConfigurationSource source)
            : base (source)
        {
        }

        public override void Load(Stream stream)
        {
            var source = ((UserSecretsConfigurationSource)Source);
            var parser = new JsonConfigurationFileParser();
            Data = parser.Parse(stream)
                .Where(k => source.KeyFilter(k.Key))
                .ToDictionary(k => source.KeyModifier(k.Key), v => v.Value);
        }
    }
}
