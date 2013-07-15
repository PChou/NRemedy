using System.CodeDom;
using System.Collections.Generic;

namespace NRemedy.CodeGenerator
{
    public interface IGenerateImportNamespace
    {
        void Create(CodeNamespace rootNamespace, List<NamespaceStructure> namespaceImports);
    }
}
