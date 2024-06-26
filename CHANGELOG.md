# Change Log

## [0.4.8-beta] - Jul 01, 2024
- Hot fix for grid position changing when entering/exiting runtime.

## [0.4.7-beta] - Jul 01, 2024
- Added support for flashing edges as transitions are executed

## [0.4.6-beta] - Jun 30, 2024
- Changed the style of the jump nodes to better identify the draggable area of the node.
- Added a tool for flagging all StateMachineModels as dirty when the project is saved.

## [0.4.6-beta] - Jun 28, 2024
- Fix for State Machine Editor title
- Grid position label now scales to fit content and is reduced in precision to a single decimal place.

## [0.4.5-beta] - Jun 27, 2024
- Fix for overtly recentering
- Fix for double deletion

## [0.4.4-beta] - Jun 26, 2024
- Code refactor

## [0.4.3-beta] - Jun 25, 2024
- hotfix to allow opening the State Machine Editor via the StateMachineModelPropertyDrawer open button regardless of state.

## [0.4.2-beta] - Jun 17, 2024
- hotfix to add preprocessors to enable VSM2 in builds
 
## [0.4.1-beta] - Mar 23, 2024
- hotfix for broken icon path

## [0.4.0-beta] - Mar 23, 2024
- Added an edit button to custom states to enable quicker access to edit state scripts
- Refactored node icons (Breaking change for anyone using node icons in their custom states)

## [0.3.4-beta] - Mar 20, 2024
- Fix for StateMachineModelPropertyDrawer in SubStateMachines not opening the live instance of statemachines at runtime.

## [0.3.3-beta] - Mar 20, 2024
- Renamed the StateMachineModel instances produced

## [0.3.2-beta] - Mar 16, 2024
- Fix for parallel substate lifespan
- Adding more resilience against the error SerializedObject target has been destroyed
- Refactored the substatemachines to make them easier to extend
- Removed redundant custom editor for StateMachineController
- Fix for the life span of substates in both SubStateMachine and ParallelSubState machines we now exit the last node state machine and set the statemachine to complete when the parent node completes

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