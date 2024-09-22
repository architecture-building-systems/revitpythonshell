using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RpsRuntime;

public static class DynamicAssemblyCompiler
{
    [UnconditionalSuppressMessage("SingleFile", "IL3000:Avoid accessing Assembly file path when publishing as a single file", Justification = "<Pending>")]
    public static void CompileAndSave(string sourceCode, string outputDllPath, ResourceDescription[] resources = null)
    {
        // 1. Create a syntax tree from the source code
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        // 2. Reference required assemblies, including Revit's
        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),  // mscorlib
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location), // System.Console
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Linq
            MetadataReference.CreateFromFile(Assembly.Load("Autodesk.Revit.Attributes").Location), // Revit attributes
            MetadataReference.CreateFromFile(Assembly.Load("Autodesk.Revit.DB").Location), // Revit DB
            MetadataReference.CreateFromFile(Assembly.Load("Autodesk.Revit.UI").Location)  // Revit UI
        };

        // 3. Compile the assembly
        var compilation = CSharpCompilation.Create(
            Path.GetFileNameWithoutExtension(outputDllPath),
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // 4. Emit the DLL to disk
        var result = compilation.Emit(outputDllPath, manifestResources: resources);

        // 5. Check for compilation errors
        if (!result.Success)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                Console.WriteLine(diagnostic.ToString());
            }
        }
        else
        {
            Console.WriteLine(@"Compilation successful. Assembly saved to " + outputDllPath);
        }
    }
}