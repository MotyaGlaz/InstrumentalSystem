using Library.Analyzer.Automata;
using Library.Analyzer.Collections;
using Library.Analyzer.Forest;
using Library.General.Workspace;
using System.Collections.Generic;
using System.Text;

namespace Library.General.NameTable
{
    public class ModuleNameTable : IModuleNameTable
    {
        public IReadOnlyList<BaseNameElement> Elements { get { return _elements; } }

        public string Name { get; private set; }

        private UniqueList<BaseNameElement> _elements;

        public ModuleNameTable(string name, ModuleNameTable moduleNameTable)
            : this(name, moduleNameTable.Elements)
        {
            
        }

        public ModuleNameTable(string name, IReadOnlyList<BaseNameElement> elements)
        {
            Name = name;
            _elements = new UniqueList<BaseNameElement>(elements);
        }

        public ModuleNameTable (string name)
        {
            Name = name;
            _elements = new UniqueList<BaseNameElement>();
        }

        public List<BaseNameElement> GetUndefidedNames()
        {
            var list = new List<BaseNameElement>();

            foreach (var element in _elements)
            {
                if (element.Value is MainNameValue mainNameValue)
                {
                    if (mainNameValue.GetUndefinedType() == UndefinedType.Set_String)
                        list.Add(element);
                    else if (mainNameValue.GetUndefinedType() == UndefinedType.Undefined_Sets
                        && element.Prefix.PrefixCouples.Count > 0)
                        list.Add(element);
                    else if (element.Prefix.PrefixCouples.Count > 0)
                        list.Add(element);
                }
            }

            return list;
        }

        public void AddNames(IReadOnlyList<BaseNameElement> elements)
        {
            foreach (var element in elements)
            {
                _elements.Add(element);
            }
        }

        public void AddNames(ModuleNameTable moduleNameTable)
        {
            foreach (var element in moduleNameTable.Elements)
            {
                _elements.Add(element);
            }
        }

        public void AddName(BaseNameElement element)
        {
            _elements.Add(element);
        }

        public void AddName(NameElementType nameElementType, List<PrefixCouple> prefixs, string id, List<ITokenForestNode> value)
        {
            _elements.Add(new BaseNameElement(nameElementType, prefixs, id, value));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Таблица ::: {Name} ::: Таблица");
            foreach (var element in _elements)
                builder.AppendLine(element.ToString());
            return builder.ToString();
        }

    }
}
