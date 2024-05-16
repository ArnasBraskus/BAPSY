using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace JsonSerializationClass
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class JsonSerializationClassAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "JsonSerializationClass";

        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor RuleFoundField = new DiagnosticDescriptor(DiagnosticId, "The JSON serialization class has a public field that will not be serialized", "The JSON serialization class '{0}' has a public field '{1}' that will not be serialized", Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Serializable properties should be auto properties");

        private static readonly DiagnosticDescriptor RuleRequiredModifier = new DiagnosticDescriptor(DiagnosticId, "The JSON serialization class has a property that does not have the 'required' modifier", "The JSON serialization class '{0}' has a property '{1}' that does not have the 'required' modifier", Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Serializable properties should have 'required' modifiers");

        private static readonly DiagnosticDescriptor RuleNaming = new DiagnosticDescriptor(DiagnosticId, "The JSON serialization class name should have 'Json' at the end", "The JSON serialization class name '{0}' should have 'Json' at the end", Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "JSON serialization classes have to be specifally named.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(RuleFoundField, RuleRequiredModifier, RuleNaming); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(Analyze);
        }

        static void Analyze(CompilationStartAnalysisContext startContext) {
            var symbolsToCheck = new List<ITypeSymbol>();

            startContext.RegisterSyntaxNodeAction(context => {
                var invocation = (InvocationExpressionSyntax)context.Node;

                var methodSymbol = context
                    .SemanticModel
                    .GetSymbolInfo(invocation, context.CancellationToken)
                    .Symbol as IMethodSymbol;

                if (methodSymbol == null || methodSymbol.Name != "ReadJson")
                    return;

                var symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node);
                var calledMethod = symbolInfo.Symbol as IMethodSymbol;

                var argument = calledMethod.TypeArguments.First();

                symbolsToCheck.Add(argument);
            }, SyntaxKind.InvocationExpression);

            startContext.RegisterCompilationEndAction(context => {
                foreach (var symbol in symbolsToCheck) {
                    if (!symbol.Name.EndsWith("Json")) {
                        var diagnostic = Diagnostic.Create(RuleNaming, symbol.Locations[0], symbol.Name);

                        context.ReportDiagnostic(diagnostic);
                    }

                    foreach (var member in symbol.GetMembers().OfType<IPropertySymbol>()) {
                        bool containsRequiredModifier = false;

                        foreach (var syntaxRef in member.DeclaringSyntaxReferences) {
                            var syntax = syntaxRef.GetSyntax() as PropertyDeclarationSyntax;

                            if (syntax.Modifiers.Any(x => x.ToString() == "required")) {
                                containsRequiredModifier = true;
                                break;
                            }
                        }

                        if (containsRequiredModifier)
                            continue;

                        var diagnostic = Diagnostic.Create(RuleRequiredModifier, member.Locations[0], symbol.Name, member.Name);

                        context.ReportDiagnostic(diagnostic);

                    }

                    foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>()) {
                        if (member.DeclaredAccessibility != Accessibility.Public)
                            continue;

                        var diagnostic = Diagnostic.Create(RuleFoundField, member.Locations[0], symbol.Name, member.Name);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            });
        }
    }
}
