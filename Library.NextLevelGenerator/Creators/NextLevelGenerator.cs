using Library.Analyzer.Collections;
using Library.Analyzer.Forest;
using Library.Analyzer.Grammars;
using Library.General.NameTable;
using Library.InterfaceConnection.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.NextLevelGenerator.Creators
{
    public class NextLevelGenerator : INextLevelCreator, IForestNodeVisitor
    {
        public IModuleNameTable ModuleNameTable { get { return _generatedModuleNameTable; } }
        public UniqueList<BaseNameElement> TaskList { get; private set; }

        private IModuleNameTable _prevModuleNameTable;
        private IModuleNameTable? _generatedModuleNameTable;
        private ISymbolForestNode _root;
        private HashSet<IForestNode> _visited;
        private HashSet<ITokenForestNode> _params;
        private TextBoxWriter? _writer;
        private bool _isParam;

        public NextLevelGenerator(IModuleNameTable prevModuleNameTable, ISymbolForestNode root, TextBoxWriter? writer = null)
        {
            _prevModuleNameTable = prevModuleNameTable;
            _writer = writer;
            _root = root;
            _params = new HashSet<ITokenForestNode>();
            _generatedModuleNameTable = GenerateNextLevelNames();
            _visited = new HashSet<IForestNode>();
            _isParam = false;
            Visit(_root);
        }

        public IModuleNameTable? GenerateNextLevelNames()
        {

            TaskList = new UniqueList<BaseNameElement>();
            CheckNameTable();
            return null;
        }

        private void CheckNameTable()
        {
            foreach (var element in _prevModuleNameTable.Elements)
            {
                if (element.Prefix.PrefixCouples.Count != 0)
                {
                    foreach (var id in element.Prefix.PrefixCouples)
                    {
                        if (id.LeftPart.Equals(element.ID))
                        {
                            TaskList.Add(element);
                            //if (_writer != null)
                            //    _writer.WriteLine(element.ID + "::: УТОЧНИТЬ");
                        }
                    }
                }
                else
                {
                    if (element.NameElementType == NameElementType.MainName)
                    {
                        foreach (var value in element.Value.Value)
                        {
                            if (value.Token.TokenType.Id.Equals("STRING"))
                                TaskList.Add(element);
                            //if (_writer != null)
                            //    _writer.WriteLine(element.ID + "::: УТОЧНИТЬ");
                        }
                    }
                }
            }
        }

        public void Visit(ITerminalForestNode node)
        {
            CheckChild(node);
        }

        public void Visit(ISymbolForestNode node)
        {
            if (!_visited.Add(node))
                return;

            if (node.Symbol is INonTerminal nonTerminal)
                if (nonTerminal.Value.Equals("Params"))
                {
                    _isParam = true;
                } else if (nonTerminal.Value.Equals("Constructs"))
                {
                    _isParam = false;
                }

            for (var a = 0; a < node.Children.Count; a++)
            {
                var andNode = node.Children[a];
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    CheckChild(child);
                }
            }
            for (var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                Visit(child);
            }
        }

        public void Visit(IIntermediateForestNode node)
        {
            if (!_visited.Add(node))
                return;

            for (var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                Visit(child);
            }
        }

        public void Visit(IAndForestNode andNode)
        {
            for (var i = 0; i < andNode.Children.Count; i++)
            {
                var child = andNode.Children[i];
                child.Accept(this);
            }
        }

        public void Visit(ITokenForestNode tokenNode)
        {
            _visited.Add(tokenNode);
            return;
        }

        private static IList<IForestNode> GetFlattenedList(IIntermediateForestNode intermediate)
        {
            var children = new List<IForestNode>();
            for (var a = 0; a < intermediate.Children.Count; a++)
            {
                var andNode = intermediate.Children[a];
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    switch (child.NodeType)
                    {
                        case ForestNodeType.Intermediate:
                            var childList = GetFlattenedList(child as IIntermediateForestNode);
                            children.AddRange(childList);
                            break;
                        default:
                            children.Add(child);
                            break;
                    }
                }
            }
            return children;
        }

        private void CheckChild(IForestNode node)
        {
            switch (node.NodeType)
            {
                case ForestNodeType.Intermediate:
                    var intermediate = node as IIntermediateForestNode;
                    if (intermediate.Children.Count > 1)
                        throw new Exception("Intermediate node has more children than expected. ");
                    var flatList = GetFlattenedList(intermediate);
                    for (var i = 0; i < flatList.Count; i++)
                    {
                        CheckChild(flatList[i]);
                    }
                    break;

                case ForestNodeType.Symbol:
                    break;

                case ForestNodeType.Token:
                    if (_isParam)
                    {
                        var tokenForestNode = node as ITokenForestNode;
                        if (tokenForestNode.Token.TokenType.Id.Equals("ID"))
                            _params.Add(tokenForestNode);
                    }
                    break;
            }
        }
    }
}
