using UnityEditor;
using System;
using UnityEditor.SceneManagement;

public class MyBuildPipeline {

    // constants
    const string WINDOWS_FILENAME = "Spamocalypse_Windows";
    const string LINUX_FILENAME = "Spamocalypse_Linux";
    const string MAC_FILENAME = "Spamocalypse_Mac";
    const string SCENE_FOLDER = "Assets/_Scenes/";
    const string BUILD_PATH = "Player Files/";

    // TODO: find how to combine these
    static BuildOptions devOptions = BuildOptions.AllowDebugging;

    static BuildOptions releaseOptions = BuildOptions.None;


    static void Build(BuildOptions options)
    {
        int sceneCount = EditorSceneManager.sceneCountInBuildSettings;
        GameTagManager.LogMessage("Creating new builds with {0} levels", sceneCount);

        var levels = new string[sceneCount];
        levels[0] = SCENE_FOLDER + "Preloading.unity";
        levels[1] = SCENE_FOLDER + "Main Menu.unity";
        levels[2] = SCENE_FOLDER + "Sound Test.unity";
        levels[3] = SCENE_FOLDER + "City Inbound.unity";
        levels[4] = SCENE_FOLDER + "Inner City.unity";
        levels[5] = SCENE_FOLDER + "Museum Model.unity";
        levels[6] = SCENE_FOLDER + "Escape Level.unity";
        levels[7] = SCENE_FOLDER + "End.unity";

        // calculate the build date
        DateTime date = DateTime.Now;
        string buildDate = string.Format("_{0}.{1}.{2}",date.Day, date.Month, date.Year);
        GameTagManager.LogMessage("Creating new builds on {0}. Levels are {1}", buildDate, levels);

        MakeWindowsBuild(levels, buildDate, BUILD_PATH, options);
        MakeMacBuild(levels, buildDate, BUILD_PATH, options );
        MakeLinuxBuild(levels, buildDate, BUILD_PATH, options);
    }

    [MenuItem("Tools/Aceade/Build Dev")]
    public static void BuildDev()
    {
        Build(devOptions);
    }

    [MenuItem("Tools/Aceade/Build Release")]
    public static void BuildRelease()
    {
        Build(releaseOptions);
    }

    /// <summary>
    /// Makes a Linux build.
    /// </summary>
    /// <param name="levelsToBuild">Levels to build.</param>
    /// <param name="dateString">Date string.</param>
    /// <param name="path">Path.</param>
    /// <param name = "buildingOptions"></param>
    static void MakeLinuxBuild(string[] levelsToBuild, string dateString, string path, BuildOptions buildingOptions)
    {
        GameTagManager.LogMessage("Creating Linux build");
        string fileName = LINUX_FILENAME + dateString + ".x86_64";
        BuildTarget target = BuildTarget.StandaloneLinux64;
        BuildPipeline.BuildPlayer(levelsToBuild, path + fileName, target, buildingOptions);
    }

    /// <summary>
    /// Makes the Windows build.
    /// </summary>
    /// <param name="levelsToBuild">Levels to build.</param>
    /// <param name="dateString">Date string.</param>
    /// <param name="path">Path.</param>
    /// <param name = "buildingOptions"></param>
    static void MakeWindowsBuild(string[] levelsToBuild, string dateString, string path, BuildOptions buildingOptions)
    {
        GameTagManager.LogMessage("Creating Windows build");
        string fileName = WINDOWS_FILENAME + dateString + ".exe";
        BuildTarget target = BuildTarget.StandaloneWindows64;
        BuildPipeline.BuildPlayer(levelsToBuild, path + fileName, target, buildingOptions);
    }

    /// <summary>
    /// Makes a build for Mac devices.
    /// </summary>
    /// <param name="levelsToBuild">Levels to build.</param>
    /// <param name="dateString">Date string.</param>
    /// <param name="path">Path.</param>
    /// <param name = "buildingOptions"></param>
    static void MakeMacBuild(string[] levelsToBuild, string dateString, string path, BuildOptions buildingOptions)
    {
        GameTagManager.LogMessage("Creating Mac OSX build");
        string fileName = MAC_FILENAME + dateString + ".app";
        BuildTarget target = BuildTarget.StandaloneOSX;
        BuildPipeline.BuildPlayer(levelsToBuild, path + fileName, target, buildingOptions);
    }
}
