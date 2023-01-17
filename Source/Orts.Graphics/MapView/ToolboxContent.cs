﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Orts.Common;
using Orts.Common.DebugInfo;
using Orts.Common.Position;
using Orts.Formats.Msts;
using Orts.Formats.Msts.Files;
using Orts.Formats.Msts.Models;
using Orts.Graphics.DrawableComponents;
using Orts.Graphics.MapView.Widgets;
using Orts.Graphics.Xna;
using Orts.Models.Track;

namespace Orts.Graphics.MapView
{
    public class ToolboxContent : ContentBase
    {
        private (double distance, INameValueInformationProvider statusItem) nearestSegmentForStatus;
        private (double distance, INameValueInformationProvider statusItem) nearestItemForStatus;

        private TileIndexedList<TrackItemBase, Tile> trackItemsByTile;

        private readonly InsetComponent insetComponent;

        private EditorTrainPath currentPath;

        public TrainPath TrainPath => currentPath?.TrainPathModel;
        private TrackModel trackModel;
        private TrackModel roadTrackModel;

        public INameValueInformationProvider TrackNodeInfo { get; } = new DetailInfoProxy();

        public INameValueInformationProvider TrackItemInfo { get; } = new DetailInfoProxy();


        public ToolboxContent(Game game) :
            base(game)
        {
            FormattingOptions.Add("Route Information", FormatOption.Bold);
            DetailInfo.Add("Route Information", null);
            DetailInfo["Route Name"] = RuntimeData.GameInstance(game).RouteName;
            insetComponent = ContentArea.Game.Components.OfType<InsetComponent>().FirstOrDefault();
        }

        public override async Task Initialize()
        {
            await Task.Run(() => AddTrackSegments()).ConfigureAwait(false);
            await Task.Run(() => AddTrackItems()).ConfigureAwait(false);

            ContentArea.Initialize();
            //just put an empty list so the draw method does not skip the paths
            contentItems[MapViewItemSettings.Paths] = new TileIndexedList<Widgets.EditorTrainPath, Tile>(new List<Widgets.EditorTrainPath>() { });

            DetailInfo["Metric Scale"] = RuntimeData.GameInstance(game).UseMetricUnits.ToString();
            DetailInfo["Track Nodes"] = $"{trackModel.SegmentSections.Count}";
            DetailInfo["Track Segments"] = $"{contentItems[MapViewItemSettings.Tracks].ItemCount}";
            DetailInfo["Track End Segments"] = $"{contentItems[MapViewItemSettings.EndNodes].ItemCount}";
            DetailInfo["Junction Segments"] = $"{contentItems[MapViewItemSettings.JunctionNodes].ItemCount}";
            DetailInfo["Road Nodes"] = $"{TrackModel.Instance<RoadTrackModel>(game).SegmentSections.Count}";
            DetailInfo["Road Segments"] = $"{contentItems[MapViewItemSettings.Roads].ItemCount}";
            DetailInfo["Road End Segments"] = $"{contentItems[MapViewItemSettings.RoadEndNodes].ItemCount}";
            DetailInfo["Tiles"] = $"{contentItems[MapViewItemSettings.Grid].Count}";
        }

        public void UpdateWidgetColorSettings(EnumArray<string, ColorSetting> colorPreferences)
        {
            if (null == colorPreferences)
                throw new ArgumentNullException(nameof(colorPreferences));

            foreach (ColorSetting setting in EnumExtension.GetValues<ColorSetting>())
            {
                ContentArea.UpdateColor(setting, ColorExtension.FromName(colorPreferences[setting]));
            }
        }

        internal override void UpdatePointerLocation(in PointD position, ITile bottomLeft, ITile topRight)
        {
            nearestSegmentForStatus = (float.MaxValue, null);
            nearestItemForStatus = (float.MaxValue, null);
            GridTile nearestGridTile = contentItems[MapViewItemSettings.Grid].FindNearest(position, bottomLeft, topRight).First() as GridTile;
            if (nearestGridTile != nearestItems[MapViewItemSettings.Grid])
            {
                nearestItems[MapViewItemSettings.Grid] = nearestGridTile;
            }

            foreach (MapViewItemSettings viewItem in EnumExtension.GetValues<MapViewItemSettings>())
            {
                double distanceSquared = double.MaxValue;
                if (viewItem == MapViewItemSettings.Grid)
                    //already drawn above
                    continue;
                if (viewSettings[viewItem] && contentItems[viewItem] != null)
                {
                    foreach (ITileCoordinate<Tile> item in contentItems[viewItem].BoundingBox(bottomLeft, topRight))
                    {
                        if (item is VectorPrimitive vectorPrimitive)
                        {
                            double itemDistance = vectorPrimitive.DistanceSquared(position);
                            if (itemDistance < distanceSquared)
                            {
                                nearestItems[viewItem] = vectorPrimitive;
                                distanceSquared = itemDistance;
                            }
                        }
                        else if (item is PointPrimitive pointPrimitive)
                        {
                            double itemDistance = pointPrimitive.Location.DistanceSquared(position);
                            if (itemDistance < distanceSquared)
                            {
                                nearestItems[viewItem] = pointPrimitive;
                                distanceSquared = itemDistance;
                            }
                        }
                    }
                }
                if (distanceSquared < 100)
                {
                    switch (viewItem)
                    {
                        case MapViewItemSettings.Tracks:
                        case MapViewItemSettings.JunctionNodes:
                        case MapViewItemSettings.EndNodes:
                        case MapViewItemSettings.Roads:
                        case MapViewItemSettings.RoadCrossings:
                        case MapViewItemSettings.RoadEndNodes:
                            if (distanceSquared < 1 || distanceSquared < nearestSegmentForStatus.distance)
                                nearestSegmentForStatus = (distanceSquared, nearestItems[viewItem] as INameValueInformationProvider);
                            break;
                        default:
                            if (distanceSquared < 1 || distanceSquared < nearestItemForStatus.distance)
                                nearestItemForStatus = (distanceSquared, nearestItems[viewItem] as INameValueInformationProvider);
                            break;
                    }
                }
                else
                    nearestItems[viewItem] = null;
            }

            //distanceSquared = double.MaxValue;
            //ITileCoordinate<Tile> nearest = null;
            //foreach (ITileCoordinate<Tile> item in trackItemsByTile.BoundingBox(bottomLeft, topRight))
            //{
            //    double itemDistance = (item as PointPrimitive).Location.DistanceSquared(position);
            //    if (itemDistance < distanceSquared)
            //    {
            //        nearest = item;
            //        distanceSquared = itemDistance;
            //    }
            //}
            
            (TrackNodeInfo as DetailInfoProxy).Source = nearestSegmentForStatus.statusItem;
            (TrackItemInfo as DetailInfoProxy).Source = nearestItemForStatus.statusItem;
        }

        internal override void Draw(ITile bottomLeft, ITile topRight)
        {
            foreach (MapViewItemSettings viewItemSetting in EnumExtension.GetValues<MapViewItemSettings>())
            {
                if (viewSettings[viewItemSetting] && contentItems[viewItemSetting] != null)
                {
                    if (viewItemSetting == MapViewItemSettings.Paths)
                    {
                        currentPath?.Draw(ContentArea);
                    }
                    else
                    {
                        foreach (ITileCoordinate<Tile> item in contentItems[viewItemSetting].BoundingBox(bottomLeft, topRight))
                        {
                            // this could also be resolved otherwise also if rather vectorwidget & pointwidget implement InsideScreenArea() function
                            // but the performance impact/overhead seems invariant
                            if (item is VectorPrimitive vectorPrimitive && ContentArea.InsideScreenArea(vectorPrimitive))
                            {
                                (item as IDrawable<VectorPrimitive>).Draw(ContentArea);
                            }
                            else if (item is PointPrimitive pointPrimitive && ContentArea.InsideScreenArea(pointPrimitive))
                            {
                                (item as IDrawable<PointPrimitive>).Draw(ContentArea);
                            }
                        }
                    }
                }
            }
            // skip highlighting closest track items if a path is loaded
            if (currentPath != null)
            {

            }
            else
            {
                if (null != nearestItems[MapViewItemSettings.Tracks])
                {
                    foreach (TrackSegmentBase segment in trackModel.SegmentSections[(nearestItems[MapViewItemSettings.Tracks] as TrackSegmentBase).TrackNodeIndex].SectionSegments)
                    {
                        (segment as IDrawable<VectorPrimitive>).Draw(ContentArea, ColorVariation.ComplementHighlight);
                    }
                }
                if (null != nearestItems[MapViewItemSettings.Roads])
                {
                    TrackModel roadTrackModel = TrackModel.Instance<RoadTrackModel>(game);
                    foreach (TrackSegmentBase segment in roadTrackModel.SegmentSections[(nearestItems[MapViewItemSettings.Roads] as TrackSegmentBase).TrackNodeIndex].SectionSegments)
                    {
                        (segment as IDrawable<VectorPrimitive>).Draw(ContentArea, ColorVariation.ComplementHighlight);
                    }
                }

                foreach (MapViewItemSettings viewItemSettings in EnumExtension.GetValues<MapViewItemSettings>())
                {
                    if (viewSettings[viewItemSettings] && nearestItems[viewItemSettings] != null)
                    {
                        if (nearestItems[viewItemSettings] is VectorPrimitive vectorPrimitive && ContentArea.InsideScreenArea(vectorPrimitive))
                            (vectorPrimitive as IDrawable<VectorPrimitive>).Draw(ContentArea, ColorVariation.Complement);
                        else if (nearestItems[viewItemSettings] is PointPrimitive pointPrimitive && ContentArea.InsideScreenArea(pointPrimitive))
                            (pointPrimitive as IDrawable<PointPrimitive>).Draw(ContentArea, ColorVariation.Complement);
                    }
                }
            }
        }

        #region additional content (Paths)
        public void InitializePath(PathFile path, string filePath)
        {
            currentPath = path != null ? new EditorTrainPath(path, filePath, game) : null;
            if (currentPath != null && currentPath.TopLeftBound != PointD.None && currentPath.BottomRightBound != PointD.None)
            {
                ContentArea?.UpdateScaleToFit(currentPath.TopLeftBound, currentPath.BottomRightBound);
                ContentArea?.SetTrackingPosition(currentPath.MidPoint);
            }
        }

        public void HighlightPathItem(int index)
        {
            currentPath.SelectedNodeIndex = index;
            Widgets.EditorPathItem item = currentPath.SelectedNode;
            if (item != null)
                ContentArea.SetTrackingPosition(item.Location);
        }
        #endregion

        #region build content database
        private void AddTrackSegments()
        {
            RuntimeData runtimeData = RuntimeData.GameInstance(game);
            TrackDB trackDB = runtimeData.TrackDB;
            RoadTrackDB roadTrackDB = runtimeData.RoadTrackDB;
            TrackSectionsFile trackSectionsFile = runtimeData.TSectionDat;
            if (null == trackSectionsFile)
                throw new ArgumentNullException(nameof(trackSectionsFile));

            ConcurrentBag<TrackSegment> trackSegments = new ConcurrentBag<TrackSegment>();
            ConcurrentBag<EndNode> endSegments = new ConcurrentBag<EndNode>();
            ConcurrentBag<JunctionNode> junctionSegments = new ConcurrentBag<JunctionNode>();
            ConcurrentBag<RoadSegment> roadSegments = new ConcurrentBag<RoadSegment>();
            ConcurrentBag<RoadEndSegment> roadEndSegments = new ConcurrentBag<RoadEndSegment>();

            Parallel.ForEach(trackDB?.TrackNodes ?? Enumerable.Empty<TrackNode>(), trackNode =>
            {
                switch (trackNode)
                {
                    case TrackEndNode trackEndNode:
                        TrackVectorNode connectedVectorNode = trackDB.TrackNodes.VectorNodes[trackEndNode.TrackPins[0].Link];
                        endSegments.Add(new EndNode(trackEndNode, connectedVectorNode, trackSectionsFile.TrackSections));
                        break;
                    case TrackVectorNode trackVectorNode:
                        int i = 0;
                        foreach (TrackVectorSection trackVectorSection in trackVectorNode.TrackVectorSections)
                        {
                            trackSegments.Add(new TrackSegment(trackVectorSection, trackSectionsFile.TrackSections, trackVectorNode.Index, i++));
                        }
                        break;
                    case TrackJunctionNode trackJunctionNode:
                        List<TrackVectorNode> vectorNodes = new List<TrackVectorNode>();
                        foreach (TrackPin pin in trackJunctionNode.TrackPins)
                        {
                            vectorNodes.Add(trackDB.TrackNodes.VectorNodes[pin.Link]);
                        }
                        junctionSegments.Add(new JunctionNode(trackJunctionNode, trackSectionsFile.TrackShapes[trackJunctionNode.ShapeIndex].MainRoute, vectorNodes, trackSectionsFile.TrackSections));
                        break;
                }
            });

            insetComponent?.SetTrackSegments(trackSegments);

            trackModel = TrackModel.Initialize<RailTrackModel>(game, runtimeData, trackSegments, junctionSegments, endSegments);
            contentItems[MapViewItemSettings.Tracks] = trackModel.ByTile.Segments;
            contentItems[MapViewItemSettings.JunctionNodes] = trackModel.ByTile.JunctionNodes;
            contentItems[MapViewItemSettings.EndNodes] = trackModel.ByTile.EndNodes;

            Parallel.ForEach(roadTrackDB?.TrackNodes ?? Enumerable.Empty<TrackNode>(), trackNode =>
            {
                switch (trackNode)
                {
                    case TrackEndNode trackEndNode:
                        TrackVectorNode connectedVectorNode = roadTrackDB.TrackNodes[trackEndNode.TrackPins[0].Link] as TrackVectorNode;
                        roadEndSegments.Add(new RoadEndSegment(trackEndNode, connectedVectorNode, trackSectionsFile.TrackSections));
                        break;
                    case TrackVectorNode trackVectorNode:
                        int i = 0;
                        foreach (TrackVectorSection trackVectorSection in trackVectorNode.TrackVectorSections)
                        {
                            roadSegments.Add(new RoadSegment(trackVectorSection, trackSectionsFile.TrackSections, trackVectorNode.Index, i++));
                        }
                        break;
                }
            });

            roadTrackModel = TrackModel.Initialize<RoadTrackModel>(game, runtimeData, roadSegments, Enumerable.Empty<JunctionNodeBase>(), roadEndSegments);
            contentItems[MapViewItemSettings.Roads] = roadTrackModel.ByTile.Segments;
            contentItems[MapViewItemSettings.RoadEndNodes] = roadTrackModel.ByTile.EndNodes;

            // identify all tiles by looking at tracks and roads and their respective end segments
            contentItems[MapViewItemSettings.Grid] = new TileIndexedList<GridTile, Tile>(
                contentItems[MapViewItemSettings.Tracks].Select(d => d.Tile as ITile).Distinct()
                .Union(contentItems[MapViewItemSettings.EndNodes].Select(d => d.Tile as ITile).Distinct())
                .Union(contentItems[MapViewItemSettings.Roads].Select(d => d.Tile as ITile).Distinct())
                .Union(contentItems[MapViewItemSettings.RoadEndNodes].Select(d => d.Tile as ITile).Distinct())
                .Select(t => new GridTile(t)));

            InitializeBounds();
        }

        private void AddTrackItems()
        {
            RuntimeData runtimeData = RuntimeData.GameInstance(game);

            IEnumerable<TrackItemBase> trackItems = TrackItemBase.CreateTrackItems(
                runtimeData.TrackDB?.TrackItems,
                runtimeData.SignalConfigFile,
                runtimeData.TrackDB, trackModel.SegmentSections).Concat(TrackItemBase.CreateRoadItems(runtimeData.RoadTrackDB?.TrackItems));

            IEnumerable<PlatformPath> platforms = PlatformPath.CreatePlatforms(trackModel, trackItems.OfType<PlatformTrackItem>());
            contentItems[MapViewItemSettings.Platforms] = new TileIndexedList<PlatformPath, Tile>(platforms);

            IEnumerable<SidingPath> sidings = SidingPath.CreateSidings(trackModel, trackItems.OfType<SidingTrackItem>());
            contentItems[MapViewItemSettings.Sidings] = new TileIndexedList<SidingPath, Tile>(sidings);

            IEnumerable<SignalTrackItem> signals = trackItems.OfType<SignalTrackItem>();

            contentItems[MapViewItemSettings.Signals] = new TileIndexedList<SignalTrackItem, Tile>(trackItems.OfType<SignalTrackItem>().Where(s => s.Normal));
            contentItems[MapViewItemSettings.OtherSignals] = new TileIndexedList<SignalTrackItem, Tile>(trackItems.OfType<SignalTrackItem>().Where(s => !s.Normal));
            contentItems[MapViewItemSettings.MilePosts] = new TileIndexedList<SpeedPostTrackItem, Tile>(trackItems.OfType<SpeedPostTrackItem>().Where(s => s.MilePost));
            contentItems[MapViewItemSettings.SpeedPosts] = new TileIndexedList<SpeedPostTrackItem, Tile>(trackItems.OfType<SpeedPostTrackItem>().Where(s => !s.MilePost));
            contentItems[MapViewItemSettings.CrossOvers] = new TileIndexedList<CrossOverTrackItem, Tile>(trackItems.OfType<CrossOverTrackItem>());
            contentItems[MapViewItemSettings.RoadCrossings] = new TileIndexedList<LevelCrossingTrackItem, Tile>(trackItems.OfType<LevelCrossingTrackItem>().Where(s => s.RoadLevelCrossing));
            contentItems[MapViewItemSettings.LevelCrossings] = new TileIndexedList<LevelCrossingTrackItem, Tile>(trackItems.OfType<LevelCrossingTrackItem>().Where(s => !s.RoadLevelCrossing));
            contentItems[MapViewItemSettings.Hazards] = new TileIndexedList<HazardTrackItem, Tile>(trackItems.OfType<HazardTrackItem>());
            contentItems[MapViewItemSettings.Pickups] = new TileIndexedList<PickupTrackItem, Tile>(trackItems.OfType<PickupTrackItem>());
            contentItems[MapViewItemSettings.SoundRegions] = new TileIndexedList<SoundRegionTrackItem, Tile>(trackItems.OfType<SoundRegionTrackItem>());
            contentItems[MapViewItemSettings.CarSpawners] = new TileIndexedList<CarSpawnerTrackItem, Tile>(trackItems.OfType<CarSpawnerTrackItem>());
            contentItems[MapViewItemSettings.Empty] = new TileIndexedList<EmptyTrackItem, Tile>(trackItems.OfType<EmptyTrackItem>());

            IEnumerable<IGrouping<string, PlatformPath>> stations = platforms.GroupBy(p => p.StationName);
            contentItems[MapViewItemSettings.StationNames] = new TileIndexedList<StationNameItem, Tile>(StationNameItem.CreateStationItems(stations));
            contentItems[MapViewItemSettings.PlatformNames] = new TileIndexedList<PlatformNameItem, Tile>(platforms.Select(p => new PlatformNameItem(p)));
            contentItems[MapViewItemSettings.SidingNames] = new TileIndexedList<SidingNameItem, Tile>(sidings.Select(p => new SidingNameItem(p)));

            trackItemsByTile = new TileIndexedList<TrackItemBase, Tile>(trackItems);
        }
        #endregion

        private protected class DetailInfoProxy : DetailInfoProxyBase
        {
            internal INameValueInformationProvider Source;

            public override InformationDictionary DetailInfo => Source?.DetailInfo;

            public override Dictionary<string, FormatOption> FormattingOptions => Source?.FormattingOptions;
        }
    }
}
