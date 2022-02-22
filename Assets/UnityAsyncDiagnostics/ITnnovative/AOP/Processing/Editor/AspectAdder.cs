using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Nito.AsyncEx.AsyncDiagnostics;

namespace ITnnovative.AOP.Processing.Editor
{
    public static class AspectAdder
    {
        public static void AddAspects(AssemblyDefinition assemblyDefinition)
        {
            foreach (var module in assemblyDefinition.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (!CodeProcessor.IncludeTypeCondition(type))
                    {
                        continue;
                    }

                    for (var index = 0; index < type.Methods.Count; index++)
                    {
                        var method = type.Methods[index];
                        if (method.IsConstructor)
                        {
                            continue;
                        }

                        if (type.HasGenericParameters)
                        {
                            continue;
                        }

                        if (method.Name.Contains(AOPProcessor.APPENDIX))
                        {
                            continue;
                        }

                        if (!CodeProcessor.HasAttributeOfType<AsyncDiagnosticAspect>(method))
                        {
                            AddAspect<AsyncDiagnosticAspect>(module, method);
                        }
                    }
                }
            }
        }

        private static void AddAspect<T>(ModuleDefinition module, IMemberDefinition obj) where T : Attribute
        {
            if (module.HasType(typeof(T)))
            {
                var attribute = module.GetType(typeof(T))
                    .GetConstructors().First();

                obj.CustomAttributes.Add(new CustomAttribute(attribute));
            }
            else
            {
                var attribute = module.ImportReference(
                    typeof(T).GetConstructors((BindingFlags) int.MaxValue)
                        .First());

                obj.CustomAttributes.Add(new CustomAttribute(attribute));
            }
        }
    }
}