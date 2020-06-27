using System.Linq;
using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace PipLib.Stub.Inject
{

    // credit where it's due to https://www.coengoedegebure.com/module-initializers-in-dotnet/ for helpign with this
    internal static class Injector
    {
        public const string INJECTION_CLASS = "ModuleInitializer";
        public const string INJECTION_METHOD = "Initialize";

        public static void Inject (string assemblyFile)
        {
            using (var assembly = AssemblyDefinition.ReadAssembly(assemblyFile, new ReaderParameters(){ ReadWrite = true }))
            {
                var method = FindInitializerMethod(assembly);
                if (method != null)
                {
                    InjectInitializer(assembly, method);
                    Console.WriteLine("successfully injected method: " + method.FullName);
                    WriteAssembly(assembly, assemblyFile);
                }
            }
        }

        private static MethodReference FindInitializerMethod (AssemblyDefinition assembly)
        {
            var initializerClass = assembly.MainModule.Types.FirstOrDefault(t => t.Name == INJECTION_CLASS);
            if (initializerClass == null)
            {
                Console.WriteLine("No ModuleInitializer class found in root namespace");
                return null;
            }

            var initializerMethod = initializerClass.Methods.FirstOrDefault(m => m.Name == INJECTION_METHOD);
            if (initializerMethod == null
                || !initializerMethod.IsPublic
                || initializerMethod.Parameters.Count > 0
                || !initializerMethod.IsStatic
                || !initializerMethod.ReturnType.FullName.Equals(typeof(void).FullName))
            {
                Console.WriteLine("No public, static, and void-returning Initialize method accepting no arguments found in ModuleInitializer");
                return null;
            }

            return initializerMethod;
        }

        private static void InjectInitializer (AssemblyDefinition assembly, MethodReference initializer)
        {
            var returnType = assembly.MainModule.ImportReference(initializer.ReturnType);
            var cctor = new MethodDefinition(".cctor", MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, returnType);
            var il = cctor.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Call, initializer));
            il.Append(il.Create(OpCodes.Ret));

            var moduleClass = assembly.MainModule.Types.FirstOrDefault(t => t.Name == "<Module>");
            if (moduleClass == null)
            {
                throw new InjectionException("No module class found");
            }

            moduleClass.Methods.Add(cctor);
        }

        private static void WriteAssembly(AssemblyDefinition assembly, string assemblyFile)
        {
            assembly.Write(assemblyFile + ".injected.dll");
        }
    }
}
