namespace ApiLib.Abstractions.Attributes;

public class GetAttribute : HttpAttribute
{
    public override string Method { get; } = "GET";
    
    public GetAttribute(string pathTemplate)
        : base(pathTemplate)
    {
    }
}

public abstract class HttpAttribute : Attribute
{
    public string PathTemplate { get; }
    public abstract string Method { get; }
    
    public HttpAttribute(string pathTemplate)
    {
        PathTemplate = pathTemplate;
    }
}