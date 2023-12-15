using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiBuildAndRun
{
    [MenuItem("Tools/Run Multiplayer/1 Players")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }

    [MenuItem("Tools/Run Multiplayer/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    [MenuItem("Tools/Run Multiplayer/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }

    [MenuItem("Tools/Run Multiplayer/4 Players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        // 모바일이나 PC등 빌드 플랫폼 설정
        EditorUserBuildSettings.SwitchActiveBuildTarget( 
            BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows);

        // 정해진 갯수만큼 빌드
        for(int i = 1; i <= playerCount; i++)
        {
            // 윈64포맷으로 정해진 갯수만큼 빌드하고 경로와 파일명으로 만든 후 실행시킨다.
            BuildPipeline.BuildPlayer(GetScenePaths(),
                "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for(int i = 0; i < scenes.Length; i++)
        {
            scenes[i]= EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
