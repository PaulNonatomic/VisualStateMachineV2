using System;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Services
{
	public class ImageService
	{
		public static Texture2D FetchTexture(string path, ResourceSource source = ResourceSource.Resources)
		{
			return source switch
			{
				ResourceSource.Resources => Resources.Load<Texture2D>(path),
				ResourceSource.AssetPath => AssetDatabase.LoadAssetAtPath<Texture2D>(path),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}