using Library.General.Utilities;
using System;

namespace Library.General.Project
{
    public class LogicModule : IModule
    {
        public string Name { get; private set; }
        
        public string Content { get; private set; }


        private int _hashCode;

        public LogicModule(string name, string content)
        {
            Name = name;
            Content = content;
            _hashCode = ComputeHashCode();
        }

        public LogicModule(string name)
        {
            Name = name;
            Content = default;
            _hashCode = ComputeHashCode();
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void SetContent(string content)
        {
            Content = content;
        }

        public void AddContent(string content)
        {
            Content += content;
        }

        private int ComputeHashCode() => Utilities.HashCode.Compute(Name.GetHashCode());

        public override int GetHashCode() => _hashCode;

        public int GetLevel()
        {
            if (Content == null)
                return -1;
            var parse = Name.Split(" ");
            if (parse.Length > 1)
                return Int32.Parse(parse[1]);
            else
                return Int32.Parse(parse[0]);
        }
    }
}
