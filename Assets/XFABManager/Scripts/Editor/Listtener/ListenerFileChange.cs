using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ListenerFileChange : UnityEditor.AssetModificationProcessor {

	// 创建资源
	public static void OnWillCreateAsset(string path)
	{

		EditorApplication.delayCall += () => {
			if (path.EndsWith(".asset") || path.EndsWith(".asset.meta"))
			{
				XFABManager.XFABProjectManager.Instance.RefreshProjects();
			}
		};


	}

	public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
	{

		EditorApplication.delayCall += () =>
		{
			if (assetPath.EndsWith(".asset"))
			{
				XFABManager.XFABProjectManager.Instance.RefreshProjects();
			}
		};
		return AssetDeleteResult.DidNotDelete;
	}
}
