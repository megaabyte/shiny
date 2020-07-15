﻿using System;
using System.Linq;
using Uno.RoslynHelpers;
using Uno.SourceGeneration;


namespace Shiny.Generators.Generators.iOS
{
    public static class AppDelegateBoilerplateGenerator
    {
        public static void Execute(SourceGeneratorContext context)
        {
            var appDelegateClass = context.Compilation.GetTypeByMetadataName("UIKit.UIApplicationDelegate");
            if (appDelegateClass == null)
                return;
            
            var appDelegates = context
                .GetAllDerivedClassesForType("UIKit.UIApplicationDelegate")
                .WhereNotSystem()
                .ToList();

            //System.Diagnostics.Debugger.Launch();
            foreach (var appDelegate in appDelegates)
            {
                // TODO: make sure it is partial
                var builder = new IndentedStringBuilder();
                builder.AppendNamespaces("Foundation", "UIKit");

                using (builder.BlockInvariant($"namespace {appDelegate.ContainingNamespace}"))
                {
                    using (builder.BlockInvariant($"public partial class {appDelegate.Name} : {appDelegate.BaseType.ToDisplayString()}"))
                    {
                        builder.AppendLineInvariant(
                            "public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo) => this.ShinyDidReceiveRemoteNotification(userInfo, null);");
                        builder.AppendLineInvariant(
                            "public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler) => this.ShinyDidReceiveRemoteNotification(userInfo, completionHandler);");
                        builder.AppendLineInvariant(
                            "public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken) => this.ShinyRegisteredForRemoteNotifications(deviceToken);");
                        builder.AppendLineInvariant(
                            "public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error) => this.ShinyFailedToRegisterForRemoteNotifications(error);");
                        builder.AppendLineInvariant(
                            "public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler) => this.ShinyPerformFetch(completionHandler);");
                        builder.AppendLineInvariant(
                            "public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler) => this.ShinyHandleEventsForBackgroundUrl(sessionIdentifier, completionHandler);");
                    }
                }
                context.AddCompilationUnit(appDelegate.Name, builder.ToString());
            }
        }
    }
}