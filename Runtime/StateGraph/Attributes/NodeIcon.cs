using System.IO;

namespace Nonatomic.VSM2.StateGraph.Attributes
{
	public abstract class NodeIcon
	{
		//Paths
		public const string IconPath = "Icons/Nodes/";
		public const string GuiPath = "Icons/GUI/";
		
		//Node Icons
		public const string Arc3d = "arc3d";
		public const string Arc3dCenterPoint = "arc3d-center-point";
		public const string Archery = "archery";
		public const string ArrowArchery = "arrow-archery";
		public const string Basketball = "basketball";
		public const string BeaconHaloLeft = "broadcast-halo-left";
		public const string BeaconHaloRight = "broadcast-halo-right";
		public const string BeaconLeft = "broadcast-left";
		public const string BeaconRight = "broadcast-right";
		public const string BoxingGlove = "boxing-glove";
		public const string Box3dCenter = "box3d-center";
		public const string Bridge3d = "bridge3d";
		public const string Center = "center";
		public const string CenterAlign = "center-align";
		public const string CenterSquare = "center-square";
		public const string CheckCircle = "check-circle";
		public const string CheckCircleSolid = "check-circle-solid";
		public const string Circle = "circle";
		public const string ClipboardCheck = "clipboard-check";
		public const string Clock = "clock";
		public const string Command = "command";
		public const string Cube = "cube";
		public const string CurveArray = "curve-array";
		public const string Download = "download";
		public const string DownloadCircle = "download-circle";
		public const string Eject = "eject";
		public const string Enter = "enter";
		public const string Erase = "erase";
		public const string Exit = "exit";
		public const string Extrude = "extrude";
		public const string Eye = "eye";
		public const string Fishing = "fishing";
		public const string FloppyDisk = "floppy-disk";
		public const string Flower = "flower";
		public const string Folder = "folder";
		public const string Golf = "golf";
		public const string Grid = "grid";
		public const string Help = "help";
		public const string HelpCircle = "help-circle";
		public const string Hexagon = "hexagon";
		public const string Hourglass = "hourglass";
		public const string InfoCircle = "info-circle";
		public const string LeaderboardStar = "leaderboard-star";
		public const string Menu = "menu";
		public const string Note = "note";
		public const string OnePointCircle = "one-point-circle";
		public const string Pencil = "pencil";
		public const string Pentagon = "pentagon";
		public const string Perspective = "grid-perspective";
		public const string Play = "play";
		public const string PlusCircle = "plus-circle";
		public const string PlusSquare = "plus-square";
		public const string Post = "post";
		public const string PostSolid = "post-solid";
		public const string Random = "random";
		public const string Report = "report";
		public const string RulerArrows = "ruler-arrows";
		public const string Share = "share";
		public const string Sparks = "sparks";
		public const string Spiral = "spiral";
		public const string Trash = "trash";
		public const string Triangle = "triangle";
		public const string Trophy = "trophy";
		public const string Warning = "warning";
		public const string XrayView = "xray-view";
		
		//GUI Icons
		public const string VsmGreen = "statemachine-green";
		public const string VsmRed = "statemachine-red";
		public const string VsmBlue= "statemachine-blue";
		public const string FolderGreen = "folder-green";
		public const string FolderRed = "folder-red";
		public const string FolderBlue = "folder-blue";

		public static string GetNodeIconPath(string icon) => Path.Combine(IconPath, icon);
		public static string GetGUIIconPath(string icon) => Path.Combine(GuiPath, icon);
	}
}