# Tharga Console

Tharga Toolkit Console is used to simplify construction of advanced console applications.

Perfect for hosting local services where you want to be able to perform some extra features.

The development platform is .NET C#.

## NuGet

To get started you can download the prebuilt [NuGet package](https://www.nuget.org/packages/Tharga.Toolkit.Console/).

## Engine, Command and Console

Tharga console has an engine that runs the program. The engine needs a root command and a console to run.
Here are some basic examples on how to get started.

#### Simplest core application
```
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var console = new ClientConsole())
            {
                var command = new RootCommand(console);
                var engine = new CommandEngine(command);
                engine.Start(args);
            }
        }
    }
```

#### Adding stuff that should start automatically and run in background
```
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var console = new ClientConsole())
            {
                var command = new RootCommand(console);
                var engine = new CommandEngine(command)
                {
                    TaskRunners = new[]
                    {
                        new TaskRunner(e =>
                        {
                            while (!e.IsCancellationRequested)
                            {
                                console.WriteLine("Running...", OutputLevel.Information);
                                Thread.Sleep(3000);
                            }
                        }),
                    },
                };

                engine.Start(args);
            }
        }
    }
```


#### Adding custom commands
```
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var console = new ClientConsole())
            {
                var command = new RootCommand(console);
                command.RegisterCommand(new FooCommand());
                var engine = new CommandEngine(command);
                engine.Start(args);
            }
        }
    }

    public class FooCommand : ContainerCommandBase
    {
        public FooCommand()
            : base("Foo")
        {
            RegisterCommand(new BarCommand());
        }
    }

    public class BarCommand : ActionCommandBase
    {
        public BarCommand()
            : base("Bar")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            OutputInformation("Foo Bar command executed successfully.");
            return true;
        }
    }
```

## Sample

A good sample to look at can be found here.
[Sample code](https://github.com/poxet/tharga-console/blob/master/Samples/SampleConsole/Program.cs)

## Arguments

You can send arguments to the console application having it execute commands right when it starts. Each *command* is sent using quotation marks (ie "some list" "some item A"). When sending parameters the console will exit automatically.

#### /c
If you want the console to stay open send the switch /c.

#### /e
The switch /e will make the console stay open if something goes wrong, otherwise it will close.

#### /r
Resets configuration.

### Theese examples will make the console stay open when completed
- "status success" /c
- "status fail" /c
- "status exception" /c
- "status fail" /e
- "status exception" /e

### Theese examples will make the console close when completed
- "status success"
- "status fail"
- "status exception"
- "status success" /e

It is also possible to provide commands using a textfile. Use the command "exec file myCommandFile.txt" as a parameter and the console will execute each line in the file as a separate command. I usually use this method during development when I want to set up data or testing.

## Clients
There are several different type of consoles that can be used.
- ClientConsole - Regular console used for normal console applications.
- EventConsole - Fires off an event on each console output.
- ActionConsole - Fires off a function on each console output.
- NullConsole - Swallows all inputs and outputs
- AggregateConsole - Merge serveral consoles together and use them all. For instance *ClientConsole* and *EventConsole* in combination.
- VoiceConsole - Use voice commands to control the application. (*Under development*)

### Client inheritance tree
```
IOutputConsole
	IConsole
		ConsoleBase
			ClientConsole
				VoiceConsole (*Under development*)
			NullConsole
			EventConsole
			ActionConsole
			AggregateConsole
	OutputConsoleBase (*Planned*)
		EventLogConsole (*Planned*)
		FileLogConsole (*Planned*)
```

## Commands
There are two types of command classes; container commands and action commands. The container commands is used to group other commands together and the action commands to execute stuff.
When executing commands from the console the names are to be typed in one flow. Say for instance that you have a container command named "some" and an action command named "item".
The command is executed by typing *some item*.

### Sending parapeters
Many times you want to query the user for input. This can be done inside a command like this.
Ex: *var id = QueryParam<string>("Some Id", GetParam(paramList, 0));*
When executing the command by typing *some item*, the user will be queried for *Some Id*.

You can also send the parameter directly by typing *some item A*. This will automatically send the parameter value A as the first parameter for the command. (The part *GetParam(paramList, 0)* will feed the *QueryParam<T>* function with the fist value provided)

### Query input in different ways
The simplest way of querying is just to use the generic *QueryParam<T>* function. The parameter *param* is an enumerable string. The *QueryParam* function will pick the first value from *parm* the first time it is called, and the second value next time and so on. If there are not enough values in *param* the user will be queried for the input.
Ex: *var id = QueryParam<string>("Some Id", param);*

If you want the user to have options to choose from you can provide a list of possible selections as a dictionary. The key is what will be returned and the value (string) what will be displayed.
Ex: *var answer = QueryParam<bool>("Are you sure?", param, new Dictionary<bool, string> { { true, "Yes" }, { false, "No" } });*

There are also async versions that takes functions of possible selections, when using the base class *ActionAsyncCommandBase*.

## Help texts
Type your command followed by -? to get help. Or just use the keyword help.

Override the command classes you crate with the property *HelpText* and write your own help text for each command you create.

## Color
There are four types of output, the colors for theese can be configured using the appSettings part of the config file
- InformationColor - Default Green
- WarningColor - Default Yellow
- ErrorColor - Default Red
- EventColor - Default Cyan

## Log4net appender
Add the nuget package *Tharga.Toolkit.Console.Log4Net* to get *Tharga Console* and a *log4net appender*.

Remember to add ```[assembly: log4net.Config.XmlConfigurator(Watch = true)]``` to your *AssemblyInfo.cs*-file for the configuration to load.

##### App.config example
```
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
  </configSections>
  <log4net debug="false">
    <appender name="ThargaConsoleAppender" type="Tharga.Toolkit.Console.Log4Net.ThargaConsoleAppender, Tharga.Toolkit.Console.Log4Net">
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%d [%t] %-5p %c %m" />
      </layout>
    </appender>
    <root>
      <level value="DEBUG" />
      <appender-ref ref="ThargaConsoleAppender" />
    </root>
  </log4net>
</configuration>
```

## License
Tharga Console goes under The MIT License (MIT).