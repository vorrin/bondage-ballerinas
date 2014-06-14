using UnityEngine;
using UnityEditor;
using System;

public class FBXScaleOverride : AssetPostprocessor {
   void OnPreprocessModel() {
       ModelImporter importer = assetImporter as ModelImporter;
       String name = importer.assetPath.ToLower();
       if ((name.Substring(name.Length - 4, 4)==".fbx")) {
           if (name.Contains("ANIM") == false){    
               importer.globalScale = 1.0F;
           }
           else{
               importer.globalScale = 1.0F;
                 importer.generateAnimations = ModelImporterGenerateAnimations.None;
               importer.animationType = ModelImporterAnimationType.None;
           }
       }
   }
}