using System;
using Microsoft.Extensions.Configuration;

namespace UserSecretsSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddUserSecrets("xyz");
        }
    }
}