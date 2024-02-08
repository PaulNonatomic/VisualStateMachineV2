# VisualStateMachine V2

## Overview
Currently still a work in progress and subject to breaking changes.
VisualStateMachine is a Unity package designed to simplify the creation and management of state machines in Unity projects. It provides a visual editor for designing state machines, making it easier to create complex behaviors without writing extensive code.


https://github.com/PaulNonatomic/VisualStateMachineV2/assets/4581647/2b6d7d0e-f404-4c9d-a780-8b8a9fa6fdd0


## Features
- **Visual Editor**: Design state machines using a user-friendly graphical interface.
- **Unity Integration**: Seamlessly integrates with Unity, allowing for easy implementation in your game projects.
- **Custom State Support**: Create your own states to handle specific game behaviors.
- **Transition Management**: Easily manage transitions between states with intuitive controls.
[CHANGELOG.md](Assets%2FPackage%2FCHANGELOG.md)
## Installation
To install VisualStateMachine in your Unity project, follow these [package.json](Assets%2FPackage%2Fpackage.json)steps:
1. Via package manager add a package from git url https://github.com/PaulNonatomic/VisualStateMachine.git?path=/Assets/Package#master
    - To work with a specific version use <b><i>#0.3.6-alpha</b><i> or similar
    - And for the lastest and greatest word in progress use <b><i>#develop</b><i> at your own risk

## Usage
1. Create a state machine asset from the project panel. Right click -> Create -> State Machine -> State Machine
2. Either right click and select "Add State" or drag out from the Entry State

https://github.com/PaulNonatomic/VisualStateMachineV2/assets/4581647/8e7f1b99-b38e-417f-8d67-95d2af212a37

3. The State Selection window appears listing all available states.
    - States are grouped by namespace with the inbuilt states appearing at the top.
    - The group of states nearest to the location of the state machine asset will open by default but all states remain accessible.

![Unity_X0jYSx6JRS](https://github.com/PaulNonatomic/VisualStateMachineV2/assets/4581647/4eadac81-df9d-4793-943b-144a704d409a)

4. Create a custom state. Here's the built in DelayState as an example.
- Add a Transition attribute to an exposed event Action in order for it to appear upon the states node in the State Machine Editor
- Serialized and public properties are also exposed in the states node in the State Machine Editor. Note fields should be populated with value types and assets and not scene types.

```cs
[NodeWidth(width:190), NodeColor(NodeColor.Teal), NodeIcon(NodeIcon.V2_Clock)]
public class DelayState : State
{
    [Transition]
    public event Action OnComplete;
    
    [SerializeField, Tooltip("Duration in seconds")] 
    public float Duration = 1f;
    
    [NonSerialized]
    private float _elapsedTime;

    public override void Enter()
    {
        _elapsedTime = 0f;
    }
    
    public override void Update()
    {
        _elapsedTime += Time.deltaTime;
        
        if (_elapsedTime >= Duration)
        {
            OnComplete?.Invoke();
        }
    }

    public override void Exit()
    {
        
    }
}
```

5. Create a game object with a StateMachineController component upon it and assign it your new state machine asset.
6. Run the application with the StateMachineController selected to see the state of your state machine within the State Machine Editor window.

## Jump Nodes
Add JumpOutState state and set it's Id. Then create a JumpInState with the corresponding Id to jump from one node to another.

https://github.com/PaulNonatomic/VisualStateMachineV2/assets/4581647/8192f5ac-fa96-48e4-b44d-738fa2946f81



## Transition Delay
The process of transitioning between nodes originally incurred no delay at all but when wiring up a looping state machine
it could cause a stack overflow. To prevent this a delay of 1 frame has been added to all transitions by default, but this
can be configured on a per transition bases by passing a frameDelay value through the Transition attribute, but please use
with caution as a frameDelay of 0 can cause a stack overflow.

## Known Issues
- Renaming transition events will lead to the transition being removed.
    - I'm working on a fix for this were a combination of event name and order will be used to identify events.
- On occasions the nodes will loose there style.
- No way to follow the progress of parallel sub state machines at run time.
- The state selector window attempts to unfold the states in the namespace nearest to the stataemachine asset, but doesn't always get this right.

## Roadmap]()
- Support for sticky notes
- Grouping of nodes

## License
VisualStateMachineV2 is licensed under the MIT license

