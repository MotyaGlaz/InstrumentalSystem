using Library.Analyzer.Automata;
using Library.Analyzer.Forest;
using Library.General.Utilities;
using Library.General.Workspace;
using System.Collections.Generic;
using System.Text;

namespace Library.General.NameTable
{
    public class BaseNameElement : INameElement
    {
        public NameElementType NameElementType { get; private set; }

        public string ID { get; private set; }

        public IValue Value { get; private set; }

        public IPrefix Prefix { get; private set; }

        private int _hashCode;

        public BaseNameElement(NameElementType nameElementType, List<PrefixCouple> prefixs, string id, List<ITokenForestNode> value)

        {
            NameElementType = nameElementType;
            ID = id;
            Value = CreateValue(value);
            Prefix = CreatePrefix(prefixs);
            _hashCode = ComputeHashCode();
        }

        public bool IsNeedToDefine(string id)
        {
            return Prefix.IsNeedToDefine(id);
        }

        private IValue CreateValue(List<ITokenForestNode> value) => (NameElementType == NameElementType.AdditionalName) ?
            new AdditionalNameValue(value) : new MainNameValue(value);

        private IPrefix CreatePrefix(List<PrefixCouple> prefixs) => new Prefix(prefixs);

        private int ComputeHashCode() => HashCode.Compute(ID.GetHashCode(), Value.GetHashCode(), Prefix.GetHashCode());

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is BaseNameElement element))
                return false;
            return element.GetHashCode() == GetHashCode();
        }

        public override string ToString() => $"{NameElementType.ToString()} ==> {Prefix.ToString()} {ID} {Value.ToString()}";

    }
}
