using UnityEngine;
using UnityEditor;
 
public class CreateGameObjectLocal : Editor
{
	[MenuItem ("GameObject/Create Empty Local #&n")]
	static void CreateEmptyLocal()
	{
		if(Selection.activeTransform != null)
		{
 
			// Check if the selected object is a prefab instance and display a warning
			PrefabType type = EditorUtility.GetPrefabType( Selection.activeGameObject );
			if(type == PrefabType.PrefabInstance)
			{
				if(!EditorUtility.DisplayDialog("Losing prefab", 
				                               "This action will lose the prefab connection. Are you sure you wish to continue?", 
				                               "Continue", "Cancel"))
				{
					return; // The user does not want to break the prefab connection so do nothing.
				}
			}
		}
 
		// Make this action undoable
		Undo.RegisterSceneUndo("Create Empty Local");
 
		// Create our new GameObject
		GameObject newGameObject = new GameObject();
		newGameObject.name = "GameObject";
 
		// If there is a selected object in the scene then make the new object its child.
		if(Selection.activeTransform != null)
		{
			newGameObject.transform.parent = Selection.activeTransform;
			newGameObject.name = Selection.activeTransform.gameObject.name + "Child";
 
			// Place the new GameObject at the same position as the parent.
			newGameObject.transform.localPosition = Vector3.zero;
			newGameObject.transform.localRotation = Quaternion.identity;
			newGameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
		}
 
		// Select our newly created GameObject
		Selection.activeTransform = newGameObject.transform;
	}
}