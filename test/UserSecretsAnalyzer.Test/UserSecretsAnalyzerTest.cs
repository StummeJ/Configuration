using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Microsoft.Extensions.Configuration.UserSecrets.Test
{
    public class UserSecretsAnalyzerTest
    {
        [Fact]
        public void NoDiagnosticsWhenMissingAdditionalFiles()
        {
            var source = @"
using System;
using Microsoft.Extensions.Configuration;

namespace UserSecretsSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddUserSecrets(""xyz"");
        }
    }
}";
            var projectId = ProjectId.CreateNewId("Whatever");
            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, "whatever", "Whatever", LanguageNames.CSharp);
            var documentId = DocumentId.CreateNewId(projectId, "test.cs");
            solution = solution.AddDocument(documentId, "test.cs", SourceText.From(source));

            var compilation = solution
                .GetProject(projectId)
                .GetCompilationAsync()
                .Result
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new UserSecretsAnalyzer()));

            var diags = compilation.GetAnalyzerDiagnosticsAsync().Result;
            Assert.Empty(diags);
        }
    }
}
