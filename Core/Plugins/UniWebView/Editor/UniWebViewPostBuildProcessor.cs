using System;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System.IO;
using System.Text;


class UniWebViewPostBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 1; } }
    public void OnPostGenerateGradleAndroidProject(string path) {
        Debug.Log("<UniWebView> UniWebView Post Build Scirpt is patching manifest file and gradle file...");
        PatchAndroidManifest(path);
        PatchBuildGradle(path);
        PatchGradleProperty(path);
    }

    private void PatchAndroidManifest(string root) {
        var manifestFilePath = GetManifestFilePath(root);
        var manifest = new UniWebViewAndroidManifest(manifestFilePath);
        
        var changed = false;
        
        Debug.Log("<UniWebView> Set hardware accelerated to enable smooth web view experience and HTML5 support like video and canvas.");
        changed = manifest.SetHardwareAccelerated() || changed;

        var settings = UniWebViewEditorSettings.GetOrCreateSettings();
        if (settings.usesCleartextTraffic) {
            changed = manifest.SetUsesCleartextTraffic() || changed;
        }
        if (settings.writeExternalStorage) {
            changed = manifest.AddWriteExternalStoragePermission() || changed;
        }
        if (settings.accessFineLocation) {
            changed = manifest.AddAccessFineLocationPermission() || changed;
        }
        if (settings.authCallbackUrls.Length > 0) {
            changed = manifest.AddAuthCallbacksIntentFilter(settings.authCallbackUrls) || changed;
        }

        if (settings.supportLINELogin) {
            changed = manifest.AddAuthCallbacksIntentFilter(new string[] { "lineauth://auth" }) || changed;
        }

        if (changed) {
            manifest.Save();
        }
    }

    private void PatchBuildGradle(string root) {
        var gradleFilePath = GetGradleFilePath(root);
        var config = new UniWebViewGradleConfig(gradleFilePath);

        var settings = UniWebViewEditorSettings.GetOrCreateSettings();
        
        var kotlinPrefix = "implementation 'org.jetbrains.kotlin:kotlin-stdlib-jdk7:";
        var kotlinVersion = String.IsNullOrWhiteSpace(settings.kotlinVersion) 
            ? UniWebViewEditorSettings.defaultKotlinVersion : settings.kotlinVersion;

        var browserPrefix = "implementation 'androidx.browser:browser:";
        var browserVersion = String.IsNullOrWhiteSpace(settings.androidBrowserVersion) 
            ? UniWebViewEditorSettings.defaultAndroidBrowserVersion : settings.androidBrowserVersion;

        var androidXCorePrefix = "implementation 'androidx.core:core:";
        var androidXCoreVersion = String.IsNullOrWhiteSpace(settings.androidXCoreVersion) 
            ? UniWebViewEditorSettings.defaultAndroidXCoreVersion : settings.androidXCoreVersion;
        
        var dependenciesNode = config.ROOT.FindChildNodeByName("dependencies");
        if (dependenciesNode != null) {
            // Add kotlin
            if (settings.addsKotlin) {
                dependenciesNode.ReplaceContenOrAddStartsWith(kotlinPrefix, kotlinPrefix + kotlinVersion + "'");
                Debug.Log("<UniWebView> Updated Kotlin dependency in build.gradle.");
            }

            // Add browser package
            if (settings.addsAndroidBrowser) {
                dependenciesNode.ReplaceContenOrAddStartsWith(browserPrefix, browserPrefix + browserVersion + "'");
                Debug.Log("<UniWebView> Updated Browser dependency in build.gradle.");
            }

            // Add Android X Core package
            if (!settings.addsAndroidBrowser && settings.addsAndroidXCore) {
                // When adding android browser to the project, we don't need to add Android X Core package, since gradle resolves for it.
                dependenciesNode.ReplaceContenOrAddStartsWith(androidXCorePrefix, androidXCorePrefix + androidXCoreVersion + "'");
                Debug.Log("<UniWebView> Updated Android X Core dependency in build.gradle.");
            }
        } else {
            Debug.LogError("UniWebViewPostBuildProcessor didn't find the `dependencies` field in build.gradle.");
            Debug.LogError("Although we can continue to add a `dependencies`, make sure you have setup Gradle and the template correctly.");

            var newNode = new UniWebViewGradleNode("dependencies", config.ROOT);
            if (settings.addsKotlin) {
                newNode.AppendContentNode(kotlinPrefix + kotlinVersion + "'");
            }
            if (settings.addsAndroidBrowser) {
                newNode.AppendContentNode(browserPrefix + browserVersion + "'");
            }

            if (settings.addsAndroidXCore) {
                newNode.AppendContentNode(androidXCorePrefix + androidXCoreVersion + "'");
            }
            newNode.AppendContentNode("implementation(name: 'UniWebView', ext:'aar')");
            config.ROOT.AppendChildNode(newNode);
        }
        config.Save();
    }

    private void PatchGradleProperty(string root) {
        var gradlePropertyFilePath = GetGradlePropertyFilePath(root);
        UniWebViewGradlePropertyPatcher.Patch(gradlePropertyFilePath);
    }

    private string CombinePaths(string[] paths) {
        var path = "";
        foreach (var item in paths) {
            path = Path.Combine(path, item);
        }
        return path;
    }

    private string GetManifestFilePath(string root) {
        string[] comps = {root, "src", "main", "AndroidManifest.xml"};
        return CombinePaths(comps);
    }

    private string GetGradleFilePath(string root) {
        string[] comps = {root, "build.gradle"};
        return CombinePaths(comps);
    }

    private string GetGradlePropertyFilePath(string root) {
        #if UNITY_2019_3_OR_NEWER
        string[] compos = {root, "..", "gradle.properties"};
        #else
        string[] compos = {root, "gradle.properties"};
        #endif
        return CombinePaths(compos);
    }
}