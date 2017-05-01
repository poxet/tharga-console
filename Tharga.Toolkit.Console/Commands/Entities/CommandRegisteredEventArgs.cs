using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Entities
{
    public class CommandRegisteredEventArgs
    {
        public ICommand Command { get; }

        public CommandRegisteredEventArgs(ICommand command)
        {
            Command = command;
        }
    }
}