﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Experimental.UIElements;
using System.Linq;

namespace GraphProcessor
{
	public static class NodeProvider
	{
		static Dictionary< Type, Type >		nodeViewPerType = new Dictionary< Type, Type >();
		static Dictionary< string, Type >	nodePerMenuTitle = new Dictionary< string, Type >();
		static Dictionary< Type, string >	nodeViewScripts = new Dictionary< Type, string >();
		static Dictionary< Type, string >	nodeScripts = new Dictionary< Type, string >();

		static NodeProvider()
		{
			foreach (var type in AppDomain.CurrentDomain.GetAllTypes())
			{
				if (type.IsClass && !type.IsAbstract)
				{
					if (type.IsSubclassOf(typeof(BaseNode)))
						AddNodeType(type);
					if (type.IsSubclassOf(typeof(BaseNodeView)))
						AddNodeViewType(type);
				}
            }
		}

		static void AddNodeType(Type type)
		{
			var attrs = type.GetCustomAttributes(typeof(NodeMenuItemAttribute), false) as NodeMenuItemAttribute[];

			if (attrs != null && attrs.Length > 0)
				nodePerMenuTitle[attrs.First().menuTitle] = type;
				
			var nodeScriptAsset = FindScriptFromClassName(type.Name);
			if (nodeScriptAsset != null)
				nodeScripts[type] = nodeScriptAsset;
		}

		static void	AddNodeViewType(Type type)
		{
			var attrs = type.GetCustomAttributes(typeof(NodeCustomEditor), false) as NodeCustomEditor[];

			if (attrs != null && attrs.Length > 0)
			{
				Type nodeType = attrs.First().nodeType;
				nodeViewPerType[nodeType] = type;

				var nodeViewScriptAsset = FindScriptFromClassName(type.Name);

				if (nodeViewScriptAsset != null)
					nodeViewScripts[type] = nodeViewScriptAsset;
			}
		}

		static string FindScriptFromClassName(string className)
		{
			var scriptGUIDs = AssetDatabase.FindAssets(className);

			if (scriptGUIDs.Length == 0)
				return null;

			return AssetDatabase.GUIDToAssetPath(scriptGUIDs[0]);
		}

		public static Type GetNodeViewTypeFromType(Type nodeType)
		{
			Type	view;

			nodeViewPerType.TryGetValue(nodeType, out view);

			return view;
		}

		public static Dictionary< string, Type >	GetNodeMenuEntries()
		{
			return nodePerMenuTitle;
		}

		public static string GetNodeViewScript(Type type)
		{
			string scriptPath;

			nodeViewScripts.TryGetValue(type, out scriptPath);

			return scriptPath;
		}

		public static string GetNodeScript(Type type)
		{
			string scriptPath;

			nodeScripts.TryGetValue(type, out scriptPath);

			return scriptPath;
		}
	}
}
