using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Workspace
{
    public class Prefix : IPrefix
    {
        public IReadOnlyList<PrefixCouple> PrefixCouples { get { return _prefixCouples; } }

        private List<PrefixCouple> _prefixCouples;

        public Prefix (List<PrefixCouple> couples)
        {
            _prefixCouples = new List<PrefixCouple> (couples);
        }

        public Prefix(PrefixCouple couple)
        {
            _prefixCouples = new List<PrefixCouple>();
            _prefixCouples.Add (couple);
        }

        public bool IsNeedToDefine(string id)
        {
            if (_prefixCouples.Count == 0)
                return false;
            foreach(var couple in _prefixCouples)
            {
                foreach (var value in couple.RightPart.Value)
                {
                    if (value.Token.Capture.ToString().Equals(id))
                        return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder =  new StringBuilder();
            foreach (var couple in _prefixCouples)
            {
                stringBuilder.Append(couple.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}
