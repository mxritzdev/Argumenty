namespace Argumenty.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ArgumentAttribute : Attribute
{
    public string PrimaryKey { get; }
    
    public string[] KeyAliases { get; }
    public bool IsRequired { get; }
    public string HelpText { get; }
    
    public uint NumberOfDashes { get; }

    public ArgumentAttribute(string primaryKey, string helpText, string[]? keyAliases = null, bool isRequired = false, uint numberOfDashes = 1)
    {
        PrimaryKey = primaryKey;
        HelpText = helpText;
        
        NumberOfDashes = numberOfDashes == 0 ? 1 : numberOfDashes;
        
        KeyAliases = keyAliases ?? [];
        
        IsRequired = isRequired;
        NumberOfDashes = numberOfDashes;
    }
}