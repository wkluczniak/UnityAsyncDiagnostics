using System;
using ITnnovative.AOP.Attributes.Method;
using ITnnovative.AOP.Processing.Execution.Arguments;

namespace Nito.AsyncEx.AsyncDiagnostics
{
    [Serializable]
    public class AsyncDiagnosticAspect : Attribute, IMethodBoundaryAspect
    {
        public void OnMethodExit(MethodExecutionArguments args)
        {
            AsyncDiagnosticStack.Pop();
        }

        public void OnMethodEnter(MethodExecutionArguments args)
        {
            var name = args.method.DeclaringType?.Name + "." + args.method.Name;
            AsyncDiagnosticStack.Push(name);
        }
    }
}