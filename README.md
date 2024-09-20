# Visual State Machine V2 (VSM2)

## Overview
Currently still a work in progress and subject to breaking changes.
Visual State Machine is a Unity package designed to simplify the creation and management of state machines in Unity projects. It provides a visual editor for designing state machines, making it easier to create complex behaviors without writing extensive code.

https://github.com/user-attachments/assets/61388bea-8bd4-4f92-9f12-c0f5768e1e8f

## Features
- **Visual Editor**: Design state machines using a user-friendly graphical interface.
- **Unity Integration**: Seamlessly integrates with Unity, allowing for easy implementation in your game projects.
- **Custom State Support**: Create your own states to handle specific game behaviors.
- **Transition Management**: Easily manage transitions between states with intuitive controls.

## Installation
To install Visual State Machine in your Unity project, follow these steps:
1. Open Unity and navigate to the **Package Manager**.
2. Click on the **+** button and select **Add package from git URL...**
3. Enter the following URL: `https://github.com/PaulNonatomic/VisualStateMachineV2.git` and press **Add**.

## Usage
1. **Create a State Machine Asset**
	- In the Project panel, right-click and select **Create -> State Machine -> State Machine**.
   
2. **Add States**
	- Either right-click and select **Add State** or drag out from the **Entry State**.

https://github.com/user-attachments/assets/ff8fb491-fc7a-4658-8ee0-de0ddb2e8c0b

3. **State Selection Window**
	- The State Selection window appears listing all available states.
		- States are grouped by namespace with the inbuilt states appearing at the top.
		- The group of states nearest to the location of the state machine asset will open by default, but all states remain accessible.

![Unity_QcTKNF3bSr](https://github.com/user-attachments/assets/05eb3410-81ef-4085-860c-d95683d24b8b)

4. **Create a Custom State**
	- Here's the built-in `DelayState` as an example:
	- Add a `Transition` attribute to an exposed event `Action` in order for it to appear upon the state's node in the State Machine Editor.
	- Serialized and public properties are also exposed in the state's node in the State Machine Editor. Note fields should be populated with value types and assets, not scene types.

```cs
[NodeWidth(width:190), NodeColor(NodeColor.Teal), NodeIcon(NodeIcon.Clock)]
public class DelayState : BaseDelayState
{
	[Transition(frameDelay:0)]
	public event Action OnComplete;
	
	[NonSerialized]
	private float _elapsedTime;

	[Enter]
	public override void OnEnterState()
	{
		_elapsedTime = 0f;
	}
	
	public override void OnUpdateState()
	{
		_elapsedTime += Time.deltaTime;

		if (_elapsedTime < Duration) return;
		OnComplete?.Invoke();
	}
}
```

5. **Assign the State Machine**
	- Create a GameObject with a StateMachineController component and assign your new state machine asset to it.
   
6. **Run the Application**
	- With the StateMachineController selected, you can see the state of your state machine within the State Machine Editor window.

## Typed Transitions
#### Passing Data Between States Using Typed Transitions

To pass data between states in the Visual State Machine, you utilize transitions. Transitions are defined by public event Actions that are decorated with a TransitionAttribute **[Transition]**. Here's how you can effectively use typed transitions:

### Defining Transitions with Parameters

* **Basic Transitions**
	- Use Action for transitions without parameters.

* **Typed Transitions**
	- Use Action<T> to pass data between states. Note: The package currently supports a maximum of one parameter per transition. Action<T1, T2> or more parameters are not supported at this time.

### Example: Passing an Integer Between States
#### Defining the Transition in the Source State

```csharp
public class SourceState : State
{
	[Transition]
	public event Action<int> OnTransitionWithInt;

	public void TriggerTransition()
	{
		int valueToPass = 42;
		OnTransitionWithInt?.Invoke(valueToPass);
	}

	[Enter]
	public void OnEnterState()
	{
		// Initialization logic
	}
}
```

#### Receiving the Transition in the Target State

```csharp
public class TargetState : State
{
	[Enter]
	public void OnEnterStateWithInt(int receivedValue)
	{
		Debug.Log($"Received value: {receivedValue}");
	}
}
```

#### Important Notes:
* **Single Parameter Limitation:** Currently, only one parameter is supported per transition. Ensure that your transitions use Action<T> with a single type parameter.<br><br>
* **Entry Method Naming Convention:**
	- It's recommended to name your entry methods as OnEnterState followed by the type, for example, OnEnterStateWithInt(int value). This enhances clarity and consistency.<br><br>
* **[Enter] Attribute Requirement:**
	- All entry methods must be decorated with the **[Enter]** attribute.
	- **Breaking Change:** In version 0.9.0-beta and above, the OnEnterState method is no longer abstract but virtual. If you do not override it and decorate it with the **[Enter]** attribute, it will not be called.<br><br>
* **Visual Indicators in the Editor:**
	- Any method decorated with the **[Enter]** attribute will appear as an input port at the bottom left of your state's node in the Visual State Machine graph, displaying the parameter type (e.g., <int>).
	- Transitions are listed on the bottom right of your state's node with the corresponding type description for typed transitions.<br><br>
* **Connection Rules:**
	- Typed output ports will only connect to matching typed input ports.
	- Transitions without types will only connect to input ports without types.
	- When dragging out from an output port, valid input ports will remain highlighted while invalid ports are greyed out.<br><br>
* **Troubleshooting:**
	- If your entry method does not appear in the state's input ports list:
		- Ensure you've added the **[Enter]** attribute.
		- Verify that you're only using one parameter in your Action<T>.<br><br>

### Example: Implementing Typed Transitions
#### Multiply State with Typed Transition

```csharp
public class MultiplyState : State
{
    [Transition] 
    public event Action<float> OnCompleteWithResult;

    [SerializeField] 
    private float _multiplyBy = 2;
    
    [Enter]
    public void OnEnterStateWithFloat(float value)
    {
        var result = value * _multiplyBy;
        OnCompleteWithResult?.Invoke(result);
    }
}
```

#### Result State Receiving Typed Transition

```csharp
public class ResultState : State
{
	[Enter]
	public void OnEnterStateWithResult(float result)
	{
		Debug.Log($"Received result: {result}");
	}
}
```

By following this approach, you can effectively pass data between states in your Visual State Machine, enhancing the flexibility and functionality of your state-driven behaviors.

![Unity_AqZI8MraYO](https://github.com/user-attachments/assets/a58aa26d-a207-4fce-bc3d-22d7f8ea0823)

## Loops with Jump Nodes
Add JumpOutState state and set it's Id. Then create a JumpInState with the corresponding Id to jump from one node to another.

https://github.com/user-attachments/assets/35715234-e532-464b-9031-4b7780efd3ef

## Transition Delay
The process of transitioning between nodes originally incurred no delay at all but when wiring up a looping state machine
it could cause a stack overflow. To prevent this a delay of 1 frame has been added to all transitions by default, but this
can be configured on a per transition bases by passing a frameDelay value through the Transition attribute, but please use
with caution as a frameDelay of 0 can cause a stack overflow.

## Copy & Paste
The State Machine Editor supports copy and paste within and between state machines.

https://github.com/user-attachments/assets/09c1b644-c519-4bfd-8c29-4b0771d8d8b6

## Shared Data
States have access to a shared data store

```cs
public class StateOne : State
{
	[Transition] public event Action OnComplete;

	[Enter]
	public override void OnEnterState()
	{
		SharedData.SetData("age", 42);
		OnComplete?.Invoke();
	}
}
	
public class StateTwo : State
{
	[Transition] public event Action OnComplete;
	
	[Enter]
	public override void OnEnterState()
	{
		var age = SharedData.GetData<int>("age");
		Debug.Log($"StateTwo - Age:{age}");
		
		OnComplete?.Invoke();
	}
}
```

This same data store is exposed externally to the state machine through the StateMachineController.SharedData

## License
VisualStateMachineV2 is licensed under the MIT license

