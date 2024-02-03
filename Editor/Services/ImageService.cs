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
			switch (source)
			{
				case ResourceSource.Resources:
					return Resources.Load<Texture2D>(path);
				case ResourceSource.AssetPath:
					return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}