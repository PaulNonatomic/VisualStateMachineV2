# VisualStateMachine V2

## Overview
Currently still a work in progress and subject to breaking changes.
VisualStateMachine is a Unity package designed to simplify the creation and management of state machines in Unity projects. It provides a visual editor for designing state machines, making it easier to create complex behaviors without writing extensive code.

![Unity_ISB14HXGR3](https://github.com/PaulNonatomic/VisualStateMachineV2/assets/4581647/e06d7ba2-8e03-4635-9ce5-c1e7597f0f1d)

## Features
- **Visual Editor**: Design state machines using a user-friendly graphical interface.
- **Unity Integration**: Seamlessly integrates with Unity, allowing for easy implementation in your game projects.
- **Custom State Support**: Create your own states to handle specific game behaviors.
- **Transition Management**: Easily manage transitions between states with intuitive controls.

## Installation
To install VisualStateMachine in your Unity project, follow these steps:
1. Via package manager add a package from git url https://github.com/PaulNonatomic/VisualStateMachineV2.git

## Usage
1. Create a state machine asset from the project panel. Right click -> Create -> State Machine -> State Machine
2. Either right click and select "Add State" or drag out from the Entry State

![Unity_EQDqu8DdM8](https://github.com/PaulNonatomic/VisualStateMachineV2/assets/4581647/b8d9f18e-d168-49c1-9e02-f0df852ba086)

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

![Unity_6YHrBPVHBo](https://github.com/PaulNonatomic/VisualStateMachineV2/assets/4581647/9da52f3e-e28a-4475-9e7b-b646c9fa35d2)


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

