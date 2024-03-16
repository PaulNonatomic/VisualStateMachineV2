# Change Log

## [0.3.1-beta] - Mar 16, 2024
- hotfix for JumpIn nodes changing ids at runtime because the node comparison method would fail at runtime.
- substate and parallel substate machine nodes now destroy substates on exit. This is more important for parallel substates.

## [0.3.0-beta] - Mar 16, 2024
- Added Parallel Sub State Machine State
- Added custom property drawer for StateMachineModel to include an open button
- Removed the now redundant Open button from the SubStateMachine node
- Added Random nodes for 2 and 3 outputs
- Fixed typo in name of DelayUnscaledState (possible breaking change)

## [0.2.1-beta] - Mar 11, 2024
- Fix for graph position label not updating when the graph is moved
- Fix for the graph state label not updating when in active mode

## [0.2.0-beta] - Mar 09, 2024
- SubStateMachine now has a button on the Node to open the sub state
- Moved away from using Selection.activeObject to populate the StateMachineEditor opting for a custom solution.
- Added breadcrumb trail to the StateMachineEditor to show the current state machine and parent state machines.
- Added current state label to the StateMachineEditor to show the current state of the state machine. (Still needs some work)
- Added a hue shift to the jump nodes so they don't start on red which I think confuses the jump nodes with exit nodes.

## [0.1.1-beta] - Feb 25, 2024
- Hotfix for editor script dependencies in runtime code

## [0.1.0-beta] - Feb 25, 2024
- Breaking change. Renamed the state methods because States are ScriptableObjects the Unity messages Awake and OnDestroy
behave inconsistently between Unity Editor versions. Renaming provides greater control over the state lifecycle.
When renaming these methods I considered just adding State on the end but this caused a clash with some State names.
I also considered just adding On before the method name but this causes clashes with some transition names. 

## [0.0.10-beta] - Feb 24, 2024
- Fix for duplication of state method calls for awake and destroy

## [0.0.9-beta] - Feb 17, 2024
- Removed the temporary fix for the BaseSubStateMachine that switched focus to the substatemachine on update

## [0.0.8-beta] - Feb 17, 2024
- Fix for states updating after exiting

## [0.0.7-beta] - Feb 16, 2024
- Fix for BaseSubsStateMachine not clearing down the complete handler on Exit

## [0.0.6-beta] - Feb 16, 2024
- BaseSubStateMachine will set the activeObject to the substate when updating. Not this is a temporary fix and will be replaced with a more robust solution in the future.

## [0.0.5-beta] - Feb 16, 2024
- Fix for the unavailability of Event.Current in StateGraphView when opening the state selection window

## [0.0.4-beta] - Feb 15, 2024
- Made a BaseStateMachineState for SubStateMachineState

## [0.0.3-beta] - Feb 15, 2024
- Made the SubStateMachineState more extensible.

## [0.0.2-beta] - Feb 14, 2024
- Added a call to the current states Exit method when a state machine is destroyed. Allowing the state an opportunity to be cleaned up as it usually would when exiting.

## [0.0.1-beta] - Feb 11, 2024
- Fixed issue with the `StateMachineEditorWindow` not loading the last model when Unity opened.
- Added custom inspector for the `StateMachineController` to show the create and open state machines.

## [0.0.0-beta] - Feb 08, 2024
- First tagged release

## [0.0.0-beta] - Feb 03, 2024
- First release of the Visual State Machine V2