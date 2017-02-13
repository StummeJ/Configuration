using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.Extensions.Configuration.UserSecrets
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UserSecretsAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "USERSECRETS001",
            "User secret ID does not match ID in project",
            "The project contains a user secret ID for '{0}' but this call has '{1}'",
            "AspNetCore",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "error descriptoin");


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is InvocationExpressionSyntax exp))
            {
                return;
            }

            // TODO cache
            var expectedIds = context
                .Options
                .AdditionalFiles
                .FirstOrDefault(f => f.Path.EndsWith("usersecrets.g.txt"));

            TextLineCollection lines;
            if (expectedIds == null ||
                (lines = expectedIds.GetText().Lines).Count == 0)
            {
                return;
            }

            var memberAccess = exp.ChildNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .FirstOrDefault();

            var arguments = exp.ChildNodes()
                .OfType<ArgumentListSyntax>()
                .Where(a => a.Arguments.Count == 1)
                .FirstOrDefault();

            if (memberAccess == null || arguments == null)
            {
                return;
            }

            var hasUserSecrets = memberAccess.ChildNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(n => n.Identifier.ValueText == "AddUserSecrets");

            if (!hasUserSecrets)
            {
                return;
            }

            var argValue = arguments
                .Arguments
                .Single()
                .ChildNodes()
                .OfType<LiteralExpressionSyntax>()
                .FirstOrDefault();

            if (argValue == null)
            {
                return;
            }

            if (argValue.Token.Kind() != SyntaxKind.StringLiteralToken)
            {
                return;
            }

            if (!lines.Any(l => l.Equals(argValue.Token.ValueText)))
            {
                var diagnostic = Diagnostic.Create(Rule,
                    argValue.GetLocation(),
                    string.Join(" or ", lines),
                    argValue.Token.ValueText);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
