using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Autodesk.Revit.Attributes;

namespace RpsRuntime
{
    /// <summary>
    /// The ExternalCommandAssemblyBuilder creates an assembly (.net dll) for
    /// a list of python scripts that can be used as IExternalCommand implementations
    /// in the Revit user interface (PushButtonData).
    /// </summary>
    public class ExternalCommandAssemblyBuilder
    {
        /// <summary>
        /// Build a new assembly and save it to disk as "pathToDll". Create a type (implementing IExternalCommand) for
        /// each class name in classNamesToScriptPaths that, when "Execute()" is called on it, will load the corresponding python script
        /// from disk and execute it.
        /// </summary>
        public void BuildExternalCommandAssembly(string pathToDll, IDictionary<string, string> classNamesToScriptPaths)
        {
            var dllName = Path.GetFileNameWithoutExtension(pathToDll);
            var dllFolder = Path.GetDirectoryName(pathToDll);
            var assemblyName = new AssemblyName { Name = dllName + ".dll", Version = new Version(1, 0, 0, 0) };
#if NET8_0
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(dllName + "Module");
#else
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(dllName + "Module", dllName + ".dll");
#endif

            foreach (var className in classNamesToScriptPaths.Keys)
            {
                var typebuilder = moduleBuilder.DefineType(className,
                                                        TypeAttributes.Class | TypeAttributes.Public,
                                                        typeof(RpsExternalCommandScriptBase));

                // add RegenerationAttribute to type
                var regenerationConstrutorInfo = typeof(RegenerationAttribute).GetConstructor(new Type[] { typeof(RegenerationOption) });
                var regenerationAttributeBuilder = new CustomAttributeBuilder(regenerationConstrutorInfo, new object[] { RegenerationOption.Manual });
                typebuilder.SetCustomAttribute(regenerationAttributeBuilder);

                // add TransactionAttribute to type
                var transactionConstructorInfo = typeof(TransactionAttribute).GetConstructor(new Type[] { typeof(TransactionMode) });
                var transactionAttributeBuilder = new CustomAttributeBuilder(transactionConstructorInfo, new object[] { TransactionMode.Manual });
                typebuilder.SetCustomAttribute(transactionAttributeBuilder);

                // call base constructor with script path
                var ci = typeof(RpsExternalCommandScriptBase).GetConstructor(new[] { typeof(string) });

                var constructorBuilder = typebuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
                var gen = constructorBuilder.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);                // Load "this" onto eval stack
                gen.Emit(OpCodes.Ldstr, classNamesToScriptPaths[className]);  // Load the path to the command as a string onto stack
                gen.Emit(OpCodes.Call, ci);               // call base constructor (consumes "this" and the string)
                gen.Emit(OpCodes.Nop);                    // Fill some space - this is how it is generated for equivalent C# code
                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Ret);                    // return from constructor
                typebuilder.CreateType();
            }
#if !NET8_0
            assemblyBuilder.Save(dllName + ".dll");
#endif
        }
    }
}