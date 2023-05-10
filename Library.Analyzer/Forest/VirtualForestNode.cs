﻿using System.Collections.Generic;
using Library.Analyzer.Grammars;
using Library.Analyzer.Charts;
using Library.Analyzer.Utilities;

namespace Library.Analyzer.Forest
{
    public class VirtualForestNode : InternalForestNode, ISymbolForestNode
    {
        private List<VirtualForestNodePath> _paths;

        private readonly int _hashCode;

        public override IReadOnlyList<IAndForestNode> Children
        {
            get
            {
                if (ShouldLoadChildren())
                    LazyLoadChildren();
                return _children;
            }
        }
                
        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Symbol; }
        }

        public ISymbol Symbol { get; private set; }
        
        public VirtualForestNode(
            int location,
            ITransitionState transitionState,
            IForestNode completedParseNode)
            : this(
                  location,
                  transitionState, 
                  completedParseNode,
                  transitionState.GetTargetState())
        {
        }

        protected VirtualForestNode(
            int location,
            ITransitionState transitionState,
            IForestNode completedParseNode,
            IState targetState)
            : base(targetState.Origin, location)
        {
            _paths = new List<VirtualForestNodePath>();
            
            Symbol = targetState.DottedRule.Production.LeftHandSide;
            _hashCode = ComputeHashCode();
            var path = new VirtualForestNodePath(transitionState, completedParseNode);
            AddUniquePath(path);
        }
                
        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
        
        public void AddUniquePath(VirtualForestNodePath path)
        {
            if (!IsUniquePath(path))
                return;
            if (IsUniqueChildSubTree(path))
                CloneUniqueChildSubTree(path.ForestNode as IInternalForestNode);
        
            _paths.Add(path);
        }

        private bool IsUniquePath(VirtualForestNodePath path)
        {
            for (int p = 0; p < _paths.Count; p++)
            {
                var otherPath = _paths[p];
                if(path.Equals(otherPath))
                    return false;
            }
            return true;
        }

        private static bool IsUniqueChildSubTree(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;

            return transitionState.Reduction.ParseNode != null
            && completedParseNode == transitionState.Reduction.ParseNode
            && (completedParseNode.NodeType == ForestNodeType.Intermediate
                || completedParseNode.NodeType == ForestNodeType.Symbol);
        }

        private void CloneUniqueChildSubTree(IInternalForestNode internalCompletedParseNode)
        {
            for (var a = 0; a < internalCompletedParseNode.Children.Count; a++)
            {
                var andNode = internalCompletedParseNode.Children[a];
                var newAndNode = new AndForestNode();
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    newAndNode.AddChild(child);
                }
                _children.Add(newAndNode);
            }
        }

        private bool ShouldLoadChildren()
        {
            return _children.Count == 0;
        }

        private void LazyLoadChildren()
        {
            for (int i = 0; i < _paths.Count; i++)
                LazyLoadPath(_paths[i]);
        }

        private void LazyLoadPath(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;
            if (transitionState.NextTransition != null)
            {
                var virtualNode = new VirtualForestNode(Location, transitionState.NextTransition, completedParseNode);

                if (transitionState.Reduction.ParseNode is null)
                    AddUniqueFamily(virtualNode);
                else
                    AddUniqueFamily(transitionState.Reduction.ParseNode, virtualNode);
            }
            else if (!(transitionState.Reduction.ParseNode is null))
            {
                AddUniqueFamily(transitionState.Reduction.ParseNode, completedParseNode);
            }
            else
            {
                AddUniqueFamily(completedParseNode);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is ISymbolForestNode symbolNode))
                return false;

            return Location == symbolNode.Location
                && NodeType == symbolNode.NodeType
                && Origin == symbolNode.Origin
                && Symbol.Equals(symbolNode.Symbol);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Symbol.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "ToString is not called in performance critical code")]
        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }
    }
}
