using System.Reflection;
using Argumenty.Attributes;
using Argumenty.Models;

namespace Argumenty;

public static class ArgumentParser
{
    
    public static ParsedArguments<T> Parse<T>(string[] args) where T : class
    {

        var model = Activator.CreateInstance<T>();
        

        var properties = typeof(T)
            .GetProperties()
            .Where(x => x.GetCustomAttributes<ArgumentAttribute>().Any())
            .ToArray();

        var arguments = new Dictionary<Argument, string>();
        var appliedArguments = new List<Argument>();
        var missingArguments = new List<Argument>();

        var flags = new List<string>();
        
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("-"))
            {
                
                var key = args[i].TrimStart('-');
                
                var dashes = (uint)args[i].TakeWhile(c => c == '-').Count();
                
                if (!(i + 1 < args.Length && !args[i + 1].StartsWith("-")) && !flags.Contains(key))
                {
                    flags.Add(key);
                }
                else
                {
                    if (arguments.FirstOrDefault(x => x.Key.Key == key && x.Key.NumberOfDashes == dashes).Key != null)
                        continue;
                    
                    arguments.Add(
                        new Argument()
                        {
                            Key = key, 
                            NumberOfDashes = dashes
                        }, 
                        ""
                        );
                }
            }
            else
            {
                if (arguments.Count == 0)
                    continue;
                
                var lastKey = arguments.Keys.Last();
                
                arguments[lastKey] += (!string.IsNullOrEmpty(arguments[lastKey]) ? " " : "") + args[i];
            }
        }

        foreach (var (arg, value) in arguments)
        {
            var key = arg.Key;
            var numDashes = arg.NumberOfDashes;
            
            var property = properties
                .FirstOrDefault(
                    x => 
                        (
                            x
                             .GetCustomAttributes<ArgumentAttribute>()
                             .First()
                             .PrimaryKey == key 
                            ||  x
                                .GetCustomAttributes<ArgumentAttribute>()
                                .First().KeyAliases.Contains(key)
                        )
                        
                         && x
                             .GetCustomAttributes<ArgumentAttribute>()
                             .First()
                             .NumberOfDashes == numDashes
                        
                );

            if (property == null)
                continue;
            
            property.SetValue(model, Convert.ChangeType(value, property.PropertyType));
            
            arguments.Remove(arg);
            
            appliedArguments.Add(arg);
        }

        foreach (var flag in flags.ToList())
        {
            var property = properties
                .FirstOrDefault(
                    x => x
                        .GetCustomAttributes<FlagAttribute>()
                        .First()
                        .Keys
                        .Contains(flag)
                    );
            
            if (property == null)
                continue;
            
            property.SetValue(model, Convert.ChangeType(true, property.PropertyType));
            
            flags.Remove(flag);
        }

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttributes<ArgumentAttribute>().First();

            if (attribute.IsRequired && !appliedArguments.Any(x => attribute.PrimaryKey == x.Key || attribute.KeyAliases.Contains(x.Key)))
            {
                missingArguments.Add(new Argument()
                {
                    Key = attribute.PrimaryKey,
                    NumberOfDashes = attribute.NumberOfDashes
                });
            }
        }
        
        
        var result = new ParsedArguments<T> (model, arguments, flags.ToArray(), missingArguments.ToArray());
        
        return result;
    }
    
}

public class ParsedArguments<T>
{
    public ParsedArguments(T item, Dictionary<Argument, string> unusedArguments, string[] unusedFlags, Argument[] missingArguments)
    {
        Item = item;
        UnusedArguments = unusedArguments;
        UnusedFlags = unusedFlags;
        MissingArguments = missingArguments;
    }

    public T Item { get; }
    
    public Dictionary<Argument, string> UnusedArguments { get; }
    
    public Argument[] MissingArguments { get; }
    
    public string[] UnusedFlags { get; }
}