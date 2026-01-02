using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;

public class BuildScript
{
    public static void PerformBuild()
    {
        // ========================
        // Список сцен
        // ========================
        string[] scenes = {
        "Assets/Scenes/menu.unity",
        "Assets/Scenes/rules.unity",
        "Assets/Scenes/first.unity",
        "Assets/Scenes/second.unity",
        "Assets/Scenes/third.unity",
        "Assets/Scenes/fourth.unity",

        };

        // ========================
        // Пути к файлам сборки
        // ========================
        string aabPath = "CrazyLab.aab";
        string apkPath = "CrazyLab.apk";

        // ========================
        // Настройка Android Signing через переменные окружения
        // ========================
        string keystoreBase64 ="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";
        string keystorePass = "crazylab";
        string keyAlias = "crazylab";
        string keyPass = "crazylab";

        string tempKeystorePath = null;

        if (!string.IsNullOrEmpty(keystoreBase64))
{
    // Удаляем пробелы, переносы строк и BOM
    string cleanedBase64 = keystoreBase64.Trim()
                                         .Replace("\r", "")
                                         .Replace("\n", "")
                                         .Trim('\uFEFF');

    // Создаем временный файл keystore
    tempKeystorePath = Path.Combine(Path.GetTempPath(), "TempKeystore.jks");
    File.WriteAllBytes(tempKeystorePath, Convert.FromBase64String(cleanedBase64));

    PlayerSettings.Android.useCustomKeystore = true;
    PlayerSettings.Android.keystoreName = tempKeystorePath;
    PlayerSettings.Android.keystorePass = keystorePass;
    PlayerSettings.Android.keyaliasName = keyAlias;
    PlayerSettings.Android.keyaliasPass = keyPass;

    Debug.Log("Android signing configured from Base64 keystore.");
}
        else
        {
            Debug.LogWarning("Keystore Base64 not set. APK/AAB will be unsigned.");
        }

        // ========================
        // Общие параметры сборки
        // ========================
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        // ========================
        // 1. Сборка AAB
        // ========================
        EditorUserBuildSettings.buildAppBundle = true;
        options.locationPathName = aabPath;

        Debug.Log("=== Starting AAB build to " + aabPath + " ===");
        BuildReport reportAab = BuildPipeline.BuildPlayer(options);
        if (reportAab.summary.result == BuildResult.Succeeded)
            Debug.Log("AAB build succeeded! File: " + aabPath);
        else
            Debug.LogError("AAB build failed!");

        // ========================
        // 2. Сборка APK
        // ========================
        EditorUserBuildSettings.buildAppBundle = false;
        options.locationPathName = apkPath;

        Debug.Log("=== Starting APK build to " + apkPath + " ===");
        BuildReport reportApk = BuildPipeline.BuildPlayer(options);
        if (reportApk.summary.result == BuildResult.Succeeded)
            Debug.Log("APK build succeeded! File: " + apkPath);
        else
            Debug.LogError("APK build failed!");

        Debug.Log("=== Build script finished ===");

        // ========================
        // Удаление временного keystore
        // ========================
        if (!string.IsNullOrEmpty(tempKeystorePath) && File.Exists(tempKeystorePath))
        {
            File.Delete(tempKeystorePath);
            Debug.Log("Temporary keystore deleted.");
        }
    }
}