using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
	[CreateAssetMenu]
	public class BehaviourTreeGraph : NodeGraph
	{
		private BTBaseNode _rootNode;
		private State _currentState;

		public BTBaseNode RootNode
		{
			get
			{
				if(_rootNode == null)
				{
					FindRootNode();
				}
				return _rootNode;
			}
			set
			{
				_rootNode = value;
			}
		}

		public void InitBehaviourTree(BTAgent owner)
		{
			FindRootNode();
			InitNodes(owner);
		}

		public State Update()
		{
			if(_rootNode == null)
			{
				_currentState = State.Failure;
			}
			else if(_rootNode.state == State.Running || _rootNode.state == State.Success)
			{
				_currentState = _rootNode.Update();
			}
			return _currentState;
		}

    public void FindRootNode()
    {
      foreach(Node node in nodes)
			{
				if(node is RootNode)
				{
					_rootNode = node as RootNode;
					return;
				}
			}
    }

		public void InitNodes(BTAgent owner)
		{
      foreach (BTBaseNode node in nodes)
      {
        node.InitNode(owner);
        node.hasStarted = false;
      }
    }
  }
}