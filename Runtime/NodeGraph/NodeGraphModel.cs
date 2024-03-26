using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.NodeGraph
{
	public abstract class NodeGraphDataModel : SubAssetContainer
	{
		
	}
	
	public abstract class NodeGraphModel<T1, T2> : NodeGraphDataModel where T1 : NodeModel where T2 : TransitionModel
	{
		public List<T1> Nodes => _nodes;
		public List<T2> Transitions => _transitions;
		
		[SerializeField] private List<T1> _nodes = new ();
		[SerializeField] private List<T2> _transitions = new ();

		protected bool TryAddNode(T1 node)
		{
			if (node == null || _nodes.Contains(node)) return false;

			_nodes.Add(node);
			MarkDirty();
			return true;
		}

		protected bool TryRemoveNode(T1 node)
		{
			if (node == null || !_nodes.Contains(node)) return false;

			_nodes.Remove(node);
			MarkDirty();
			return true;
		}

		protected bool TryAddTransition(T2 transition)
		{
			var existingTransition = _transitions.FirstOrDefault(t => t.Equals(transition));
			if (existingTransition != null) return false;
			
			_transitions.Add(transition);

			#if UNITY_EDITOR
			{
				EditorUtility.SetDirty(this);
				EditorApplication.delayCall += () => AssetDatabase.SaveAssets();
			}
			#endif
			
			return true;
		}

		protected bool TryRemoveTransition(T2 transition)
		{
			if (!_transitions.Contains(transition)) return false;
			
			_transitions.Remove(transition);
			return true;
		}
		
		private void MarkDirty()
		{
			#if UNITY_EDITOR
			{
				EditorUtility.SetDirty(this);
			}
			#endif
		}

		protected override void ValidateSubAssets()
		{
			base.ValidateSubAssets();
			
			for(var i = _nodes.Count; i >= 0; i--)
			{
				var node = _nodes[i];
				if (node == null)
				{
					_nodes.RemoveAt(i);
				}
			}
			
			for(var i = _transitions.Count; i >= 0; i--)
			{
				var transition = _transitions[i];
				if (transition == null)
				{
					_transitions.RemoveAt(i);
				}
			}
		}
	}
}