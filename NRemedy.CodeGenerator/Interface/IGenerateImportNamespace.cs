using System.CodeDom;

namespace NRemedy
{
    public interface IGenerateImportNamespace
    {
        void Create(CodeNamespace rootNamespace);
    }
}
