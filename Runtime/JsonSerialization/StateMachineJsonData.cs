using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nonatomic.VSM2.JsonSerialization
{
	#region JSON Data Models

	[Serializable]
	public class StateMachineJsonData
	{
		public string name;
		public List<StateNodeJsonData> nodes = new();
		public List<StateTransitionJsonData> transitions = new();
	}

	[Serializable]
	public class StateNodeJsonData
	{
		public string id;
		public Vector2 position;
		public List<PortJsonData> inputPorts = new();
		public List<PortJsonData> outputPorts = new();
		public StateJsonData state;
	}

	[Serializable]
	public class PortJsonData
	{
		public string id;
		// Removed Name and AllowMultipleConnections as they don't exist in your PortModel
		// Add any other properties that actually exist in your PortModel
	}

	[Serializable]
	public class StateJsonData
	{
		public string typeName;
		public string assemblyQualifiedName;
		public Dictionary<string, object> serializedFields = new();
	}

	[Serializable]
	public class StateTransitionJsonData
	{
		public string originNodeId;
		public string destinationNodeId;
		public PortJsonData originPort;
		public PortJsonData destinationPort;
		public string typeName;
		public string assemblyQualifiedName;
		public Dictionary<string, object> serializedFields = new();
	}

	#endregion

	#region Custom JSON Converters

	public class Vector2Converter : JsonConverter<Vector2>
	{
		public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			var obj = JObject.Load(reader);
			return new((float)obj["x"], (float)obj["y"]);
		}

		public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WriteEndObject();
		}
	}

	#endregion

	public static class StateMachineJsonSerializer
	{
		private static JsonSerializerSettings GetSerializerSettings()
		{
			return new()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.Auto,
				Converters = new List<JsonConverter>
				{
					new Vector2Converter()
				}
			};
		}

		/// <summary>
		///     Serializes a state machine model to a JSON string
		/// </summary>
		public static string SerializeToJson(StateMachineModel stateMachine)
		{
			var jsonData = ConvertToJsonData(stateMachine);
			return JsonConvert.SerializeObject(jsonData, GetSerializerSettings());
		}

		/// <summary>
		///     Saves a state machine model to a JSON file
		/// </summary>
		public static void SaveToJsonFile(StateMachineModel stateMachine, string filePath)
		{
			var json = SerializeToJson(stateMachine);
			File.WriteAllText(filePath, json);
			Debug.Log($"State machine saved to {filePath}");
		}

		/// <summary>
		///     Deserializes a JSON string to a state machine model
		/// </summary>
		public static StateMachineModel DeserializeFromJson(string json)
		{
			var jsonData = JsonConvert.DeserializeObject<StateMachineJsonData>(json, GetSerializerSettings());
			return ConvertFromJsonData(jsonData);
		}

		/// <summary>
		///     Loads a state machine model from a JSON file
		/// </summary>
		public static StateMachineModel LoadFromJsonFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Debug.LogError($"File not found: {filePath}");
				return null;
			}

			var json = File.ReadAllText(filePath);
			var stateMachine = DeserializeFromJson(json);
			Debug.Log($"State machine loaded from {filePath}");
			return stateMachine;
		}

		/// <summary>
		///     Converts a StateMachineModel to a serializable JSON data structure
		/// </summary>
		private static StateMachineJsonData ConvertToJsonData(StateMachineModel stateMachine)
		{
			var jsonData = new StateMachineJsonData
			{
				name = stateMachine.name
			};

			// Serialize all state nodes
			foreach (var node in stateMachine.Nodes)
			{
				var nodeData = new StateNodeJsonData
				{
					id = node.Id,
					position = node.Position
				};

				// Serialize state data
				if (node.State != null)
				{
					var stateData = new StateJsonData
					{
						typeName = node.State.GetType().Name,
						assemblyQualifiedName = node.State.GetType().AssemblyQualifiedName
					};

					// Serialize state fields using reflection
					SerializeObjectFields(node.State, stateData.serializedFields);

					nodeData.state = stateData;
				}

				// Serialize input ports
				foreach (var port in node.InputPorts)
				{
					nodeData.inputPorts.Add(SerializePort(port));
				}

				// Serialize output ports
				foreach (var port in node.OutputPorts)
				{
					nodeData.outputPorts.Add(SerializePort(port));
				}

				jsonData.nodes.Add(nodeData);
			}

			// Serialize all transitions
			foreach (var transition in stateMachine.Transitions)
			{
				var transitionData = new StateTransitionJsonData
				{
					originNodeId = transition.OriginNodeId,
					destinationNodeId = transition.DestinationNodeId,
					typeName = transition.GetType().Name,
					assemblyQualifiedName = transition.GetType().AssemblyQualifiedName
				};

				// Serialize port data
				if (transition.OriginPort != null)
				{
					transitionData.originPort = SerializePort(transition.OriginPort);
				}

				if (transition.DestinationPort != null)
				{
					transitionData.destinationPort = SerializePort(transition.DestinationPort);
				}

				// Serialize transition fields using reflection
				SerializeObjectFields(transition, transitionData.serializedFields);

				jsonData.transitions.Add(transitionData);
			}

			return jsonData;
		}

		/// <summary>
		///     Serializes a PortModel to JSON data
		/// </summary>
		private static PortJsonData SerializePort(PortModel port)
		{
			if (port == null)
			{
				return null;
			}

			var portData = new PortJsonData
			{
				id = port.Id
				// Add other properties from PortModel that actually exist
			};

			return portData;
		}

		/// <summary>
		///     Serializes an object's fields using reflection
		/// </summary>
		private static void SerializeObjectFields(object obj, Dictionary<string, object> serializedFields)
		{
			if (obj == null)
			{
				return;
			}

			var fields = obj.GetType().GetFields(BindingFlags.Public |
												 BindingFlags.NonPublic |
												 BindingFlags.Instance);

			foreach (var field in fields)
			{
				// Skip fields marked with [NonSerialized] attribute
				if (field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length > 0)
				{
					continue;
				}

				// Check if field should be serialized (public or has [SerializeField] attribute)
				if (field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), true).Length > 0)
				{
					try
					{
						var value = field.GetValue(obj);

						// Skip null values and event delegates
						if (value == null || field.FieldType.IsSubclassOf(typeof(Delegate)))
						{
							continue;
						}

						// Handle Unity objects (avoid circular references)
						if (value is Object unityObj)
						{
							// For Unity Objects, store basic identification information
							serializedFields[field.Name] = new
							{
								IsUnityObject = true,
								Type = unityObj.GetType().AssemblyQualifiedName,
								Name = unityObj.name
							};
							continue;
						}

						// For everything else, store the value directly
						serializedFields[field.Name] = value;
					}
					catch (Exception ex)
					{
						Debug.LogWarning($"Could not serialize field {field.Name}: {ex.Message}");
					}
				}
			}
		}

		/// <summary>
		///     Converts a JSON data structure to a StateMachineModel
		/// </summary>
		private static StateMachineModel ConvertFromJsonData(StateMachineJsonData jsonData)
		{
			// Create a new state machine instance
			var stateMachine = ScriptableObject.CreateInstance<StateMachineModel>();
			stateMachine.name = jsonData.name;

			// Dictionary to track node IDs to models for linking transitions
			var nodesById = new Dictionary<string, StateNodeModel>();

			// Create all state nodes
			foreach (var nodeData in jsonData.nodes)
			{
				// Create the state if state data exists
				State stateInstance = null;
				if (nodeData.state != null)
				{
					var stateType = Type.GetType(nodeData.state.assemblyQualifiedName);
					if (stateType != null)
					{
						stateInstance = ScriptableObject.CreateInstance(stateType) as State;
						if (stateInstance != null)
						{
							// Restore serialized fields
							DeserializeObjectFields(stateInstance, nodeData.state.serializedFields);
						}
						else
						{
							Debug.LogWarning($"Failed to create state of type: {nodeData.state.typeName}");
						}
					}
					else
					{
						Debug.LogWarning($"Could not find state type: {nodeData.state.typeName}");
					}
				}

				// Create a new state node with the required constructor parameters
				var stateNode = new StateNodeModel(stateInstance, nodeData.position);
				stateNode.Id = nodeData.id;

				// State is already created and assigned in the constructor

				// Restore input ports
				foreach (var portData in nodeData.inputPorts)
				{
					var port = DeserializePort(portData);
					if (port != null)
					{
						stateNode.InputPorts.Add(port);
					}
				}

				// Restore output ports
				foreach (var portData in nodeData.outputPorts)
				{
					var port = DeserializePort(portData);
					if (port != null)
					{
						stateNode.OutputPorts.Add(port);
					}
				}

				// Add to dictionary for transition linking
				nodesById[stateNode.Id] = stateNode;

				// Add to state machine
				stateMachine.AddState(stateNode);
			}

			// Create all transitions
			foreach (var transitionData in jsonData.transitions)
			{
				// Since StateTransitionModel is not a ScriptableObject, we need to create it directly
				var transitionType = Type.GetType(transitionData.assemblyQualifiedName) ?? typeof(StateTransitionModel);

				// Create the transition using its constructor instead of CreateInstance
				StateTransitionModel transition;
				try
				{
					transition = (StateTransitionModel)Activator.CreateInstance(transitionType);
				}
				catch (Exception ex)
				{
					Debug.LogError($"Failed to create transition of type {transitionType.Name}: {ex.Message}");
					continue;
				}

				if (transition != null)
				{
					transition.OriginNodeId = transitionData.originNodeId;
					transition.DestinationNodeId = transitionData.destinationNodeId;

					// Restore origin and destination ports
					if (transitionData.originPort != null)
					{
						transition.OriginPort = DeserializePort(transitionData.originPort);
					}

					if (transitionData.destinationPort != null)
					{
						transition.DestinationPort = DeserializePort(transitionData.destinationPort);
					}

					// Restore serialized fields
					DeserializeObjectFields(transition, transitionData.serializedFields);

					// Add to state machine
					stateMachine.AddTransition(transition);
				}
			}

			return stateMachine;
		}

		/// <summary>
		///     Deserializes a PortJsonData to a PortModel
		/// </summary>
		private static PortModel DeserializePort(PortJsonData portData)
		{
			if (portData == null)
			{
				return null;
			}

			var port = new PortModel
			{
				Id = portData.id
				// Set any other properties that actually exist in PortModel
			};

			return port;
		}

		/// <summary>
		///     Deserializes fields onto an object using reflection
		/// </summary>
		private static void DeserializeObjectFields(object obj, Dictionary<string, object> serializedFields)
		{
			if (obj == null || serializedFields == null)
			{
				return;
			}

			var fields = obj.GetType().GetFields(BindingFlags.Public |
												 BindingFlags.NonPublic |
												 BindingFlags.Instance);

			foreach (var field in fields)
			{
				// Skip fields marked with [NonSerialized] attribute
				if (field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length > 0)
				{
					continue;
				}

				// Check if field should be deserialized (public or has [SerializeField] attribute)
				if (field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), true).Length > 0)
				{
					if (serializedFields.TryGetValue(field.Name, out var value))
					{
						try
						{
							// Handle JObject conversion for different field types
							if (value is JObject jObj)
							{
								// Check if this is a serialized Unity Object reference
								if (jObj.ContainsKey("IsUnityObject") && (bool)jObj["IsUnityObject"])
								{
									// We cannot restore Unity Object references in this context
									// They would need to be referenced by path/guid and loaded from assetDatabase
									continue;
								}

								// Convert JObject to the target field type
								var convertedValue = jObj.ToObject(field.FieldType);
								field.SetValue(obj, convertedValue);
							}
							else if (value is JArray jArray)
							{
								// Convert JArray to the target field type
								var convertedValue = jArray.ToObject(field.FieldType);
								field.SetValue(obj, convertedValue);
							}
							else
							{
								// For simple types, try direct conversion
								var convertedValue = Convert.ChangeType(value, field.FieldType);
								field.SetValue(obj, convertedValue);
							}
						}
						catch (Exception ex)
						{
							Debug.LogWarning($"Could not deserialize field {field.Name}: {ex.Message}");
						}
					}
				}
			}
		}
	}

	// Extension methods for easy use
	public static class StateMachineJsonExtensions
	{
		/// <summary>
		///     Exports the state machine to a JSON file
		/// </summary>
		public static void ExportToJson(this StateMachineModel stateMachine, string filePath)
		{
			StateMachineJsonSerializer.SaveToJsonFile(stateMachine, filePath);
		}

		/// <summary>
		///     Creates a runtime clone of this state machine from JSON serialization
		/// </summary>
		public static StateMachineModel CreateRuntimeCloneViaJson(this StateMachineModel stateMachine)
		{
			var json = StateMachineJsonSerializer.SerializeToJson(stateMachine);
			return StateMachineJsonSerializer.DeserializeFromJson(json);
		}
	}
}