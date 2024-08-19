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
1. Via package manager add a package from git url https://github.com/PaulNonatomic/VisualStateMachineV2.git

## Usage
1. Create a state machine asset from the project panel. Right click -> Create -> State Machine -> State Machine
2. Either right click and select "Add State" or drag out from the Entry State

https://github.com/user-attachments/assets/ff8fb491-fc7a-4658-8ee0-de0ddb2e8c0b

3. The State Selection window appears listing all available states.
    - States are grouped by namespace with the inbuilt states appearing at the top.
    - The group of states nearest to the location of the state machine asset will open by default but all states remain accessible.

![Unity_QcTKNF3bSr](https://github.com/user-attachments/assets/05eb3410-81ef-4085-860c-d95683d24b8b)

4. Create a custom state. Here's the built in DelayState as an example.
- Add a Transition attribute to an exposed event Action in order for it to appear upon the states node in the State Machine Editor
- Serialized and public properties are also exposed in the states node in the State Machine Editor. Note fields should be populated with value types and assets and not scene types.

```cs
[NodeWidth(width:190), NodeColor(NodeColor.Teal), NodeIcon(NodeIcon.Clock)]
public class DelayState : BaseDelayState
{
    [Transition(frameDelay:0)]
    public event Action OnComplete;
    
    [NonSerialized]
    private float _elapsedTime;

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

    public override void OnExitState()
    {
        //...
    }
}
```

5. Create a game object with a StateMachineController component upon it and assign it your new state machine asset.
6. Run the application with the StateMachineController selected to see the state of your state machine within the State Machine Editor window.

## Loops with Jump Nodes
Add JumpOutState state and set it's Id. Then create a JumpInState with the corresponding Id to jump from one node to another.

https://github.com/user-attachments/assets/35715234-e532-464b-9031-4b7780efd3ef

## Transition Delay
The process of transitioning between nodes originally incurred no delay at all but when wiring up a looping state machine
it could cause a stack overflow. To prevent this a delay of 1 frame has been added to all transitions by default, but this
can be configured on a per transition bases by passing a frameDelay value through the Transition attribute, but please use
with caution as a frameDelay of 0 can cause a stack overflow.

## License
VisualStateMachineV2 is licensed under the MIT license

