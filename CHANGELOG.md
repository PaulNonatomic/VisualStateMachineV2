# Change Log

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
[package.json](package.json)
## [0.0.0-beta] - Feb 08, 2024
- First tagged release

## [0.0.0-beta] - Feb 03, 2024
- First release of the Visual State Machine V2[package.json](package.json)