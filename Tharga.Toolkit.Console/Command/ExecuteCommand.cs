using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ExecuteCommand : ContainerCommandBase
    {
        public ExecuteCommand(IConsole console, RootCommandBase rootCommand)
            : base(console, new [] { "exec" }, "Execute command features.", true)
        {
            RegisterCommand(new ExecuteFileCommand(console, rootCommand));
            RegisterCommand(new ExecuteSleepCommand(console));
        }
    }
}