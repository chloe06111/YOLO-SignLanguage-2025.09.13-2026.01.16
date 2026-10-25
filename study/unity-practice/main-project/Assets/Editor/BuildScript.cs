using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildScript
{
    public static void Build()
    {
        // 1. Build Settings에서 활성화된 씬 목록을 가져옵니다.
        List<string> scenePaths = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenePaths.Add(scene.path);
            }
        }

        // 빌드 옵션 설정
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenePaths.ToArray(); // Build Settings에서 가져온 씬 목록을 사용
        buildPlayerOptions.locationPathName = "Builds/Windows/MyGame.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        // 빌드 실행
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
