
namespace NRemedy
{
    public interface IGenerateNameResolver
    {
        string DefaultClassNameResolver(string formName);

        string DefaultFieldNameResolver(string fieldName);

        string DefaultPropertyNameResolver(string fieldName);

        //Custom Selection Type Name for AR Selection Type
        string SelectionTypeResolver(string fieldName);
    }
}
