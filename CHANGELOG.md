# Change Log

# [0.9.0-beta] - Sept 20, 2024
- BREAKING - State enter methods now require an [Enter] attribute.
- States now support types transitions
  - Example ```[Transition] public event Action<int> OnTransitionWithInt```
  - Example ```[Enter] public void OnEnterWithInt(int value){}```
- Abstracted validation methods to a separate class
- Added a CounterWithTargetState.
- Simplified the uss files used to style the basic states

# [0.8.5-beta] - Sept 17, 2024
- Fix for deleting a selection via the context menu

## [0.8.4-beta] - Sept 11, 2024
- SharedData.ClearData is now Obsolete and will be removed in a future release. Use SharedData.ClearAllData instead.
- Added SharedData.ClearAllData method to clear all shared data.
- Added SharedData.RemoveData(string key) method to remove a specific key from the shared data.
- Added SharedData.GetKeys() method to return all keys in the shared data.

## [0.8.3-beta] - Sept 10, 2024
Added support for switching SubStateMachine models at runtime

## [0.8.2-beta] - Sept 09, 2024
- Added TryGetData and HasData methods to the SharedData class

## [0.8.1-beta] - Aug 25, 2024
- Refactored the CommandState (Available when using the ScriptableCommands package) to better handle exceptions and clean up of cancellation tokens.

## [0.8.0-beta] - Aug 25, 2024
- Added support for shared data between states. The SharedData object is a generic data store.
  - States can access shared data via `this.SharedData`
- Shared data is exposed externally to the statemachine via StateMachineController.SharedData.

## [0.7.2-beta] - Aug 24, 2024
- Exposed the JumpTo method on the StateMachineController
  - This allows users to manually trigger a JumpIn node externally broadening the use-cases for the VSM
- Exposed the State member of the StateMachine through a public State member of the StateMachineController
  - This allows users to determine the current state and to directly call methods upon it externally.
  - I experimented with implementing a StatePattern with all States implementing a common interface.

## [0.7.1-beta] - Aug 21, 2024
- Amended the exception forwarding of StateMachine to throw the original exception rather than wrapping it. This improves debugging errors in states.

## [0.7.0-beta] - Aug 19, 2024
- Added support for copy & paste

## [0.6.5-beta] - Aug 16, 2024
- Updated frame delay of DelayState transition to 0
- Fix for add nodes multiple times when entering runtime
- Fix for unuseful error thrown when sub state-machines ref is missing

## [0.6.4-beta] - Aug 12, 2024
- Fix for rogue tilda
- Added relay states

## [0.6.3-beta] - Aug 10, 2024
- Added Counter State
- Fix: When scenes with multiple StateMachineControllers load the selected controller and the first controller in the scene were both loaded into the same StateMachineEditor

## [0.6.2-beta] - Aug 05, 2024
- Fix for the Entry node often missing from new StateMachineModels when first created 
- Fix for positioning new nodes correctly irrespective of zoom level
- 
## [0.6.1-beta] - Aug 04, 2024
- FrameDelay labels are hidden on ghost edges
- Added support for Sticky Notes

## [0.6.0-beta] - Aug 04, 2024
- EntryState OnEntry transition has a FrameDelay value of 0
- JumpInStates OnExit transition has a FrameDelay value of 0
- Fixed FrameDelay behaviour
- Enabled edge glow to be effected by FrameDelay duration
- Added FrameDelay labels to each transition edge
- Added additional icons

## [0.5.2-beta] - Jul 19, 2024
- Added guards to StateNodeModel to prevent Awake being called when already Awake, Exit being called having already exited and OnDestroy being called having already been destroyed.

## [0.5.1-beta] - Jul 18, 2024
- Added a protected setter to the Model accessor of BaseSubStateMachineState
    - This will allow for derived states to switch the model in the OnAwakeState

## [0.5.0-beta] - Jul 17, 2024
- Removal of the StateMachineController.Model setter.
  - Switching models requires the model to go through the Unity lifecycle which wasn't and should not occur via a setter.
  - Added a dedicated SwitchModel method which sets the model, creates a new instance and runs it through the unity life cycle. 
- Added method level comments to the StateMachineController to clarify it`s functionality.
- Updated the sample
  - Remove the traffic light sample
  - Fixed the animation example
  
## [0.4.11-beta] - Jul 09, 2024
- Fix for recentering the graph view at the appropriate time
- Fix for drawing the DelayUnscaledState with the wrong view
  - Added a BaseDelayState

## [0.4.10-beta] - Jul 03, 2024
- hotfix for asmdef file issues

## [0.4.9-beta] - Jul 03, 2024
- Extracted the scriptable command feature out into it's own package
- Added support for the Scriptable Command package when included in the project

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
