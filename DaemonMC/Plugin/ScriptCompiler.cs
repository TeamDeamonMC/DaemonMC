using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace DaemonMC.Plugin
{
    public static class ScriptCompiler
    {
        private static readonly object _refLock = new();
        private static readonly Dictionary<string, MetadataReference> _referenceCache = new(StringComparer.OrdinalIgnoreCase);

        private const string GlobalUsings = """
            global using System;
            global using System.Collections.Generic;
            global using System.IO;
            global using System.Linq;
            global using System.Net.Http;
            global using System.Threading;
            global using System.Threading.Tasks;
            """;

        public sealed class Result
        {
            public byte[]? Assembly { get; init; }
            public byte[]? Pdb { get; init; }
            public List<string> Errors { get; } = new();
            public bool Success => Assembly != null;
        }

        public static Result Compile(string filePath)
        {
            var result = new Result();
            var fullPath = Path.GetFullPath(filePath);

            string source;
            try
            {
                source = ReadAllTextWithRetry(fullPath);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Couldn't read {filePath}: {ex.Message}");
                return result;
            }

            var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);
            var sourceText = SourceText.From(source, Encoding.UTF8);
            var trees = new[]
            {
                CSharpSyntaxTree.ParseText(sourceText, parseOptions, fullPath),
                CSharpSyntaxTree.ParseText(SourceText.From(GlobalUsings, Encoding.UTF8), parseOptions, "GlobalUsings.g.cs")
            };

            var assemblyName = $"{Path.GetFileNameWithoutExtension(fullPath)}_{Guid.NewGuid():N}";

            var compilation = CSharpCompilation.Create(
                assemblyName,
                trees,
                GetReferences(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Debug)
                    .WithNullableContextOptions(NullableContextOptions.Enable)
                    .WithAllowUnsafe(true));

            using var dllStream = new MemoryStream();
            using var pdbStream = new MemoryStream();

            var emitResult = compilation.Emit(
                dllStream,
                pdbStream,
                embeddedTexts: new[] { EmbeddedText.FromSource(fullPath, sourceText) },
                options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb));

            if (!emitResult.Success)
            {
                foreach (var diag in emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                {
                    var pos = diag.Location.GetLineSpan().StartLinePosition;
                    result.Errors.Add($"{Path.GetFileName(fullPath)}({pos.Line + 1},{pos.Character + 1}): {diag.Id}: {diag.GetMessage()}");
                }
                return result;
            }

            return new Result { Assembly = dllStream.ToArray(), Pdb = pdbStream.ToArray() };
        }

        private static List<MetadataReference> GetReferences()
        {
            var references = new Dictionary<string, MetadataReference>(StringComparer.OrdinalIgnoreCase);

            lock (_refLock)
            {
                foreach (var asm in AssemblyLoadContext.Default.Assemblies)
                {
                    if (asm.IsDynamic) continue;
                    var name = asm.GetName().Name;
                    if (name == null || references.ContainsKey(name)) continue;

                    var key = "asm:" + asm.FullName;
                    if (!_referenceCache.TryGetValue(key, out var reference))
                    {
                        reference = CreateFromLoadedAssembly(asm);
                        if (reference == null) continue;
                        _referenceCache[key] = reference;
                    }
                    references[name] = reference;
                }

                if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpa)
                {
                    foreach (var path in tpa.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var name = Path.GetFileNameWithoutExtension(path);
                        if (references.ContainsKey(name)) continue;

                        if (!_referenceCache.TryGetValue(path, out var reference))
                        {
                            try
                            {
                                reference = MetadataReference.CreateFromFile(path);
                            }
                            catch
                            {
                                continue;
                            }
                            _referenceCache[path] = reference;
                        }
                        references[name] = reference;
                    }
                }
            }

            return references.Values.ToList();
        }

        private static unsafe MetadataReference? CreateFromLoadedAssembly(System.Reflection.Assembly asm)
        {
            if (!System.Reflection.Metadata.AssemblyExtensions.TryGetRawMetadata(asm, out var blob, out var length)) return null;
            var module = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
            return AssemblyMetadata.Create(module).GetReference(display: asm.GetName().Name);
        }

        private static string ReadAllTextWithRetry(string path)
        {
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                    return reader.ReadToEnd();
                }
                catch (IOException) when (attempt < 10)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
