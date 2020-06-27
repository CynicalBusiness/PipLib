using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace PipLib.Stub.Inject
{
    internal static class Program
    {
        public static void Main (string[] args)
        {
            // TODO be a little nicer about parsing
            Injector.Inject(args[0]);
        }
    }
}
