﻿using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FreeTrainSimulator.Models.Content;
using FreeTrainSimulator.Models.Handler;
using FreeTrainSimulator.Models.Imported.Shim;

using Orts.Formats.Msts.Files;

namespace FreeTrainSimulator.Models.Imported.ImportHandler.TrainSimulator
{
    internal sealed class PathModelImportHandler : ContentHandlerBase<PathModelCore>
    {
        internal const string SourceNameKey = "MstsSourcePath";

        // MSTS ships with 7 unfinished paths, which cannot be used as they reference tracks that do not exist.
        // MSTS checks for "broken path" before running the simulator and doesn't offer them in the list.
        // I.e. the first activity in Marias Pass is "Explore Longhale" which leads to a "Broken Path" message.
        // The message then confuses new users who have just started to play activities from MSTS,
        //private static readonly string[] brokenPaths = {
        //    @"ROUTES\USA1\PATHS\aftstrm(traffic03).pat",
        //    @"ROUTES\USA1\PATHS\aftstrmtraffic01.pat",
        //    @"ROUTES\USA1\PATHS\aiphwne2.pat",
        //    @"ROUTES\USA1\PATHS\aiwnphex.pat",
        //    @"ROUTES\USA1\PATHS\blizzard(traffic).pat",
        //    @"ROUTES\USA2\PATHS\longhale.pat",
        //    @"ROUTES\USA2\PATHS\long-haul west (blizzard).pat",
        //};

        public static async Task<FrozenSet<PathModelCore>> ExpandPathModels(RouteModelCore routeModel, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(routeModel, nameof(routeModel));

            ConcurrentBag<PathModelCore> results = new ConcurrentBag<PathModelCore>();

            string sourceFolder = routeModel.MstsRouteFolder().PathsFolder;
            if (Directory.Exists(sourceFolder))
            {
                // load existing MSTS files
                List<string> pathFiles = new List<string>(Directory.EnumerateFiles(sourceFolder, "*.pat"));

                foreach (IGrouping<string, string> item in pathFiles.GroupBy(f => Path.GetFileNameWithoutExtension(f).Trim(), StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
                {
                    foreach (string file in item.OrderBy(i => i.Length))
                    {
                        if (string.Equals(Path.GetFileNameWithoutExtension(file), item.Key, StringComparison.OrdinalIgnoreCase))
                            continue;
                        Trace.TraceWarning($"Found duplicate file \"{file}\" for same base file name ({item.Key}) differ by whitespace only. Ignoring this file.");
                        pathFiles.Remove(file);
                    }
                }

                await Parallel.ForEachAsync(pathFiles, cancellationToken, async (path, token) =>
                {
                    Task<PathModelCore> modelTask = Cast(Convert(path, routeModel, cancellationToken));

                    PathModelCore pathModel = await modelTask.ConfigureAwait(false);
                    string key = pathModel.Hierarchy();
                    results.Add(pathModel);
                    modelTaskCache[key] = modelTask;
                }).ConfigureAwait(false);
            }
            FrozenSet<PathModelCore> result = results.ToFrozenSet();
            string key = routeModel.Hierarchy();
            modelSetTaskCache[key] = Task.FromResult(result);
            _ = collectionUpdateRequired.TryRemove(key, out _);
            return result;
        }

        private static async Task<PathModel> Convert(string filePath, RouteModelCore routeModel, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(routeModel, nameof(routeModel));

            if (File.Exists(filePath))
            {
                PathFile patFile = new PathFile(filePath);

                PathModel pathModel = new PathModel()
                {
                    Name = string.IsNullOrEmpty(patFile.Name) ? $"unnamed (@ {Path.GetFileNameWithoutExtension(filePath)})" : patFile.Name.Trim(),
                    Id = patFile.PathID.Trim(),
                    PlayerPath = patFile.PlayerPath,
                    Start = string.IsNullOrEmpty(patFile.Start) ? $"unnamed (@ {Path.GetFileNameWithoutExtension(filePath)})" : patFile.Start.Trim(),
                    End = string.IsNullOrEmpty(patFile.End) ? $"unnamed (@ {Path.GetFileNameWithoutExtension(filePath)})" : patFile.End.Trim(),
                    Tags = new Dictionary<string, string> { { SourceNameKey, Path.GetFileNameWithoutExtension(filePath) } },
                };
                //this is the case where a file may have been renamed but not the path id, ie. in case of copy cloning, so adopting the filename as path id
                if (string.IsNullOrEmpty(pathModel.Id) || !string.Equals(pathModel.Tags[SourceNameKey].Trim(), pathModel.Id, StringComparison.OrdinalIgnoreCase))
                {
                    Trace.TraceWarning($"Path file {filePath} refers to path Id {pathModel.Id}. Renaming to {pathModel.Tags[SourceNameKey]}");
                    pathModel = pathModel with { Id = pathModel.Tags[SourceNameKey] };
                }
                await Create(pathModel, routeModel, cancellationToken).ConfigureAwait(false);
                return pathModel;
            }
            else
            {
                Trace.TraceWarning($"Path file {filePath} refers to non-existing file.");
                return null;
            }
        }
    }
}
