using System;
using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateSelectorWindow : EditorWindow
	{
		public Action<Type> OnTypeSelected;
		
		private Dictionary<Button, Type> _buttons = new ();
		private int _currentSelectedIndex = -1;
		private NodeGraphDataModel _model;
		private const string HighlightClass = "highlighted-button-style";
		private const string BuildInStateNamespace = "Nonatomic.VSM2.StateGraph.States";
		private const string BuildInStateDirectoryName = "Built-in States";
		
		public static void Open(NodeGraphDataModel model, 
								Vector2 position, 
								Action<Type> typeSelectionCallback, 
								List<Type> filterOut = null)
		{
			var window = GetWindow<StateSelectorWindow>("Select State Type");
			window.position = new Rect(position, window.position.size);
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateSelectorWindow"));
			window.OnTypeSelected = typeSelectionCallback;
			window.SearchStates(model, string.Empty, filterOut);
		}
		
		private void OnGUI()
		{
			var evt = Event.current;

			if (evt.type == EventType.KeyDown)
			{
				HandleKeyDown(evt.keyCode);
			}
		}
		
		public void CreateGUI()
		{
			var root = rootVisualElement;

			var searchField = new ToolbarSearchField();
			searchField.name = "search-field";
			searchField.AddToClassList("search-bar");
			searchField.RegisterValueChangedCallback(evt =>
			{
				SearchStates(_model, evt.newValue);
			});
			searchField.RegisterCallback<GeometryChangedEvent>(evt =>
			{
				searchField.Q<TextField>().Q("unity-text-input").Focus();
			}); 
			searchField.RegisterCallback<KeyDownEvent>(evt =>
			{
				HandleKeyDown(evt.keyCode);
			});
			root.Add(searchField);
			
			var results = new ScrollView();
			root.Add(results);
			results.Clear();
		}
		
		private void OnLostFocus()
		{
			//this?.Close();
		}
		
		private void OnFocus()
		{
			Focus();
		}
		
		private void HandleKeyDown(KeyCode keyCode)
		{
			switch (keyCode)
			{
				case KeyCode.DownArrow:
					SelectNextButton();
					break;
				case KeyCode.UpArrow:
					SelectPreviousButton();
					break;
				case KeyCode.Return:
					ActivateSelectedButton();
					break;
			}
		}
		
		private void SelectPreviousButton()
		{
			if (_currentSelectedIndex > 0)
			{
				_currentSelectedIndex--;
			}
			else if (_currentSelectedIndex == 0)
			{
				_currentSelectedIndex = -1;
			}

			HighlightButton(_currentSelectedIndex);
			ScrollToButton(_currentSelectedIndex);
		}
		
		private void ScrollToButton(int index)
		{
			if (index < 0 || index >= _buttons.Count) return;

			var button = _buttons.Keys.ElementAt(index);
			var scrollView = rootVisualElement.Q<ScrollView>(); // Replace with your ScrollView's name if different

			if (scrollView == null || button == null)
				return;

			// Calculate the position of the button within the ScrollView
			var buttonWorldPos = button.worldBound;
			var scrollViewWorldPos = scrollView.worldBound;

			// Check if the button is above or below the visible area of the ScrollView
			if (buttonWorldPos.yMin < scrollViewWorldPos.yMin)
			{
				// Scroll up to the button
				scrollView.scrollOffset = new Vector2(scrollView.scrollOffset.x, scrollView.scrollOffset.y - (scrollViewWorldPos.yMin - buttonWorldPos.yMin));
			}
			else if (buttonWorldPos.yMax > scrollViewWorldPos.yMax)
			{
				// Scroll down to the button
				scrollView.scrollOffset = new Vector2(scrollView.scrollOffset.x, scrollView.scrollOffset.y + (buttonWorldPos.yMax - scrollViewWorldPos.yMax));
			}
		}
		
		private void HighlightButton(int index)
		{
			if (index >= 0 && index < _buttons.Count)
			{
				// Reset all button styles
				foreach (var kvp in _buttons)
				{
					// Apply normal style
					kvp.Key.RemoveFromClassList(HighlightClass);
				}

				// Apply highlighted style
				// You can change the style as per your UI design
				var button = _buttons.Keys.ElementAt(index);
				button.AddToClassList(HighlightClass);

				var foldout = button.parent as Foldout;
				foldout.value = true;
			}
		}
		
		private void SelectNextButton()
		{
			// If no button is currently selected, start from the first button.
			if (_currentSelectedIndex == -1 && _buttons.Count > 0)
			{
				_currentSelectedIndex = 0;
			}
			else if (_currentSelectedIndex < _buttons.Count - 1)
			{
				// Move to the next button in the list.
				_currentSelectedIndex++;
			}

			HighlightButton(_currentSelectedIndex);
			ScrollToButton(_currentSelectedIndex);
		}
		
		private void ActivateSelectedButton()
		{
			if (_currentSelectedIndex >= 0 && _currentSelectedIndex < _buttons.Count)
			{
				var state = _buttons.Values.ElementAt(_currentSelectedIndex);
				SelectState(state);
			}
		}

		private void SelectState(Type stateType)
		{
			OnTypeSelected?.Invoke(stateType);
			Close();
		}
		
		private List<List<Type>> GetGroupedStates(List<Type> filterOut = null)
		{
			var derivedTypes = GetFilteredListOfStates(filterOut);
			var filterOutAbstractStates = derivedTypes.Where(type => !type.IsAbstract).ToList();
			var filterOutHiddenStates = filterOutAbstractStates.Where(type => !Attribute.IsDefined(type, typeof(HideInStateSelectorAttribute))).ToList();
			var groupedStates = filterOutHiddenStates
				.GroupBy(state => state.Namespace)
				.Select(group => group.ToList())
				.ToList();

			MoveListWithNamespaceToFront(groupedStates, BuildInStateNamespace);
			
			return groupedStates;
		}

		public List<Type> GetFilteredListOfStates(List<Type> filterOut = null)
		{
			var derivedTypes = AssetUtils.GetAllDerivedTypes<State>();
			if (filterOut == null) return derivedTypes;
			
			derivedTypes.RemoveAll(item => filterOut.Contains(item));
			return derivedTypes;
		}
		
		public static void MoveListWithNamespaceToFront(List<List<Type>> lists, string namespaceName)
		{
			var matchingList = lists.FirstOrDefault(list => list.Any() && list[0].Namespace == namespaceName);
			if (matchingList == null) return;
			
			lists.Remove(matchingList);
			lists.Insert(0, matchingList);
		}
		
		private int FindNearestNamespaceToSO(ScriptableObject so, IReadOnlyList<List<Type>> groupedStates)
		{
			if (so == null) return -1;
			
			var closestMatch = 0;
			var smallestDistance = int.MaxValue;
			var soPath = AssetDatabase.GetAssetPath(so);

			for (var index = 0; index < groupedStates.Count; index++)
			{
				var group = groupedStates[index];
				var groupLocation = GetScriptPath(group[0]);
				
				var distance = Utils.StringUtils.FindLevenshteinDistance(soPath, groupLocation);
				if (distance >= smallestDistance) continue;
				
				smallestDistance = distance;
				closestMatch = index;
			}

			return closestMatch;
		}
		
		public static string GetScriptPath(Type type)
		{
			var monoScript = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(type));
			var path = AssetDatabase.GetAssetPath(monoScript);
			
			return path;
		}
		
		private Foldout MakeGroupFoldout(int groupIndex, string groupName, string icon, bool foldedState = false)
		{
			var groupBody = new Foldout();
			groupBody.value = foldedState;
			groupBody.name = "group-body";
			if(groupIndex == 0) groupBody.AddToClassList("first-group");
			
			var checkMark = groupBody.Q("unity-checkmark");
			checkMark.parent.name = "group-header";
			checkMark.parent.Insert(0, MakeGroupIcon(icon));
				
			var label = new Label(groupName);
			checkMark.parent.Insert(1, label);

			return groupBody;
		}
		
		private Image MakeGroupIcon(string icon)
		{
			return new Image()
			{
				image = Resources.Load<Texture2D>(icon),
				scaleMode = ScaleMode.ScaleToFit,
				name = "group-icon"
			};
		}
		
		private void SearchStates(NodeGraphDataModel model, string searchQuery, List<Type> filterOut = null)
		{
			_model = model;
			
			var emptySearchQuery = string.IsNullOrEmpty(searchQuery);
			
			_buttons.Clear();
			var container = rootVisualElement.Q<ScrollView>();
			if (container == null) return;
			
			container.Clear();

			var groupedStates = GetGroupedStates(filterOut);
			var nearestGroupToStateMachine = FindNearestNamespaceToSO(model, groupedStates);
			
			for (var groupIndex = 0; groupIndex < groupedStates.Count; groupIndex++)
			{
				var group = groupedStates[groupIndex];

				var firstGroup = groupIndex == 0;
				var open = firstGroup || nearestGroupToStateMachine == groupIndex || !emptySearchQuery;

				var buttonIcon = firstGroup ? NodeIcon.VsmBlue : NodeIcon.VsmGreen;
				var folderName = firstGroup ? BuildInStateDirectoryName : group[0].Namespace;
				var folderIcon = firstGroup ? NodeIcon.FolderBlue : NodeIcon.FolderGreen;
				
				Foldout groupBody = null;
				for (var stateIndex = 0; stateIndex < group.Count; stateIndex++)
				{
					var stateType = group[stateIndex];
					if (!stateType.Name.ToLower().Contains(searchQuery.ToLower())) continue;

					//Prevent empty groups from being created
					if (groupBody == null)
					{
						groupBody = MakeGroupFoldout(groupIndex, folderName, folderIcon, open);
						container.Add(groupBody);
					}

					var button = MakeStateButton(stateType, stateIndex, buttonIcon);
					_buttons.Add(button, stateType);
					groupBody.Add(button);
				}
			}
		}
		
		private Button MakeStateButton(Type stateType, int index, string icon)
		{
			var isEven = index % 2 == 0;
			var button = new Button(() => SelectState(stateType)) { };
			button.name = "state-button";

			if (isEven) button.AddToClassList("even");

			button.Insert(0, new Image()
			{
				image = Resources.Load<Texture2D>(icon),
				scaleMode = ScaleMode.ScaleToFit
			});

			var stateNamespace = stateType.Namespace;
			var stateName = ProcessStateName(stateType.Name);

			button.Add(new Label()
			{
				text = stateName
			});

			return button;
		}
		
		private string ProcessStateName(string stateName)
		{
			stateName = Utils.StringUtils.PascalCaseToTitleCase(stateName);
			stateName = Utils.StringUtils.RemoveStateSuffix(stateName);
			stateName = Utils.StringUtils.ApplyEllipsis(stateName, 30);
			
			return stateName;
		}
	}
}