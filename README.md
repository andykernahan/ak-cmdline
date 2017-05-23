# Command Line Parser Library for .NET 4.6

A simple command line processing library which enables the public methods of an object to be invoked via the command line. The library also provides the following features:
* help information is generated using the program's xmldocs or by decorating members with the `System.ComponentModel.DescriptionAttribute`
* extensible type conversion built on top of `System.ComponentModel.TypeDescriptor`
* methods and parameters can be assigned a short name by decorating them with the `AK.CmdLine.ShortNameAttribute`
* method and paramteer names are treated case-insensitively
* supports positional arguments and multiple switch formats (`--name=value`, `/name=value`, `/name:value`, `/name value`)
* supports short form boolean switch (`--name[+]`, `--name-`)
* supports optional parameters
* supports `async` commands (`Task` and `Task<T>`)

## Example

Given the following program:

```c#
/// <summary>
/// Wake On LAN utility for .NET 4.6.
/// </summary>
public class WakeOnLan
{
    static WakeOnLan()
    {
        DefaultValueConverter.Register(x => new Regex(x));
        DefaultValueConverter.Register(x => PhysicalAddress.Parse(x.Replace(':', '-')));
    }

    public static void Main(string[] args)
    {
        new CmdLineDriver(new WakeOnLan(), Console.Out).TryProcess(args);
    }

    /// <summary>
    /// Wakes the machine with the specified name or MAC address.
    /// </summary>
    /// <param name="machine">The registered machine name or MAC address.</param>
    [ShortName("w")]
    public void Wake([ShortName("m")]string machine)
    {
        Console.WriteLine("Wake('{0}')", machine);
    }

    /// <summary>
    /// Wake all registered machines.
    /// </summary>
    /// <param name="pattern">The optional machine name pattern.</param>
    /// <param name="skip">Determines whether machines that match the pattern should be skipped.</param>
    public void WakeAll(Regex pattern = null, bool skip = false)
    {
        Console.WriteLine("WakeAll('{0}', {1})", pattern, skip);
    }

    /// <summary>
    /// Registers a machine and its MAC address.
    /// </summary>
    /// <param name="name">The machine name.</param>
    /// <param name="address">The MAC address of the machine.</param>        
    public void Register(string name, PhysicalAddress address)
    {
        Console.WriteLine("Register('{0}', '{1}')", name, address);
    }

    /// <summary>
    /// Unregisters the machine with the specified name.
    /// </summary>
    /// <param name="name">The name of the machine to unregister.</param>        
    public void Unregister(string name)
    {
        Console.WriteLine("Unregister('{0}')", name);
    }
}
```

Yields the following:

```
> WakeOnLan.exe
Wake On LAN utility for .NET 4. - v1.0.0.0
Copyright © Andy Kernahan 2011

usage: a command name is required

Wake[w] <--machine[-m]>
  - Wakes the machine with the specified name or MAC address.
      --machine:  The registered machine name or MAC address.

WakeAll [--pattern=] [--skip-]
  - Wake all registered machines.
      --pattern:    The optional machine name pattern.
      --skip[+|-]:  Determines whether machines that match the pattern should be skipped.

Register <--name> <--address>
  - Registers a machine and its MAC address.
      --name:     The machine name.
      --address:  The MAC address of the machine.

Unregister <--name>
  - Unregisters the machine with the specified name.
      --name:  The name of the machine to unregister.
  
> WakeOnLan.exe Wake a-machine
Wake('a-machine')

> WakeOnLan w --m=a-machine
Wake('a-machine')

> WakeOnLan.exe WakeAll
WakeAll('', False)

> WakeOnLan.exe WakeAll --pattern=[ab]-machine --skip
WakeAll('[ab]-machine', True)

> WakeOnLan Register /name:a-machine /address:00-00-00-00-00-00-00-00
Register('a-machine', '0000000000000000')

> WakeOnLan.exe Unregister /name a-machine
Unregister('a-machine')
```
