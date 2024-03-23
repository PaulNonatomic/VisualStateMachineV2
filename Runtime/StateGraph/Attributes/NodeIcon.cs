using System.IO;

namespace Nonatomic.VSM2.StateGraph.Attributes
{
	public abstract class NodeIcon
	{
		//Paths
		public const string IconPath = "Icons/Nodes/";
		public const string GuiPath = "Icons/GUI/";
		
		//Node Icons
		public const string BeaconRight = "broadcast-right";
		public const string BeaconLeft = "broadcast-left";
		public const string BeaconHaloLeft = "broadcast-halo-left";
		public const string BeaconHaloRight = "broadcast-halo-right";
		public const string Clock = "clock";
		public const string Enter = "enter";
		public const string Exit = "exit";
		public const string Note = "note";
		public const string Share = "share";
		public const string Folder = "folder";
		public const string Cube = "cube";
		public const string Grid = "grid";
		public const string Perspective = "grid-perspective";
		public const string Triangle = "triangle";
		public const string Pentagon = "pentagon";
		public const string Hexagon = "hexagon";
		public const string Circle = "circle";
		public const string Warning = "warning";
		public const string Report = "report";
		public const string Trophy = "trophy";
		public const string Help = "help";
		public const string Menu = "menu";
		public const string RulerArrows = "ruler-arrows";
		public const string Eject = "eject";
		public const string Center = "center";
		public const string CenterSquare = "center-square";
		public const string CenterAlign = "center-align";
		public const string Command = "command";
		public const string Random = "random";
		public const string Pencil = "pencil";
		
		//GUI Icons
		public const string VsmGreen = "Icons/statemachine-green";
		public const string VsmRed = "Icons/statemachine-red";
		public const string VsmBlue= "Icons/statemachine-blue";
		public const string FolderGreen = "Icons/folder-green";
		public const string FolderRed = "Icons/folder-red";
		public const string FolderBlue = "Icons/folder-blue";

		public static string GetNodeIconPath(string icon) => Path.Combine(IconPath, icon);
		public static string GetGUIIconPath(string icon) => Path.Combine(GuiPath, icon);
	}
}