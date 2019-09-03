using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CSharpScript
{
    /// <summary>
    /// C# script expansion for string
    /// </summary>
    public static class CSharpScriptExpansion
    {
        /// <summary>
        /// Generate C# Result 
        /// </summary>
        /// <param name="csharpFragment">Csharp Fragment</param>
        /// <returns></returns>
        public static string GenerateResult(this string csharpFragment)
        {
            var sourceCodeText = $@"
using System;
using System.Collections.Generic;
using System.Text;

namespace RoslynDynamicGenerate
{{
    public class DynamicGenerateClass
    {{
        public string Generate()
        {{    
         {csharpFragment}
        }}       
    }}
}}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCodeText, new CSharpParseOptions(LanguageVersion.Latest));
            var assemblyName = $"RoslynDynamicGenerate";

            var compilation = CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(GetMetadataReference(typeof(object)))
                .AddSyntaxTrees(syntaxTree)
                ;
            var memory = new MemoryStream();
            var compilationResult = compilation.Emit(memory);
            if (compilationResult.Success)
            {
                try
                {
                    var assembly = Assembly.Load(memory.ToArray());
                    var type = assembly.GetType("RoslynDynamicGenerate.DynamicGenerateClass");
                    var obj = Activator.CreateInstance(type);
                    var methodInfo = type.GetMethod("Generate");
                    return methodInfo.Invoke(obj, null)?.ToString();
                }
                finally
                {
                    memory.Close();
                }
            }
            else
            {
                throw new ApplicationException($"this C# syntax is error:{sourceCodeText}");
            }
        }


        /// <summary>
        /// get Metadata Reference from type
        /// </summary>
        /// <param name="types">types</param>
        /// <returns></returns>
        static List<MetadataReference> GetMetadataReference(params Type[] types)
        {
            var list = new List<MetadataReference>();
            foreach (var type in types)
            {
                var metadateRef = MetadataReference.CreateFromFile(type.Assembly.Location);
                list.Add(metadateRef);
                foreach (var assembly in type.Assembly.GetReferencedAssemblies())
                {
                    list.Add(MetadataReference.CreateFromFile(Assembly.Load(assembly).Location));
                }
            }
            return list;
        }
    }
}
