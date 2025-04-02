
namespace Argumenty.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FlagAttribute : Attribute
{
    public string[] Keys { get; }
    
    public FlagAttribute(params string[] keys)
    {
        if (keys.Length == 0)
        {
            throw new ArgumentException("Flag Attributes must have at least one allowed name.");
        }
        
        Keys = keys;
    }


    
}