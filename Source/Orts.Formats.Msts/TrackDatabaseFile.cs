﻿// COPYRIGHT 2009, 2010, 2011, 2012, 2013, 2014 by the Open Rails project.
// 
// This file is part of Open Rails.
// 
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.Xna.Framework;
using Orts.Common;
using Orts.Common.Calc;
using Orts.Formats.Msts.Files;
using Orts.Formats.Msts.Models;
using Orts.Formats.Msts.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Orts.Formats.Msts
{
    /// <summary>
    /// TDBFile is a representation of the .tdb file, that contains the track data base.
    /// The database contains two kinds of items: TrackNodes and TrItems (Track Items).
    /// </summary>
    public class TrackDatabaseFile
    {
        /// <summary>
        /// Contains the Database with all the  tracks.
        /// </summary>
        public TrackDB TrackDB { get; private set; }

        /// <summary>
        /// Constructor from file
        /// </summary>
        /// <param name="filenamewithpath">Full file name of the .rdb file</param>
        public TrackDatabaseFile(string fileName)
        {
            using (STFReader stf = new STFReader(fileName, false))
                stf.ParseFile(new STFReader.TokenProcessor[] {
                    new STFReader.TokenProcessor("trackdb", ()=>{ TrackDB = new TrackDB(stf); }),
                });
        }

    }

    /// <summary>
    /// This class represents the Track Database.
    /// </summary>
    public class TrackDB
    {
        private readonly Dictionary<string, TrackJunctionNode> junctionNodes = new Dictionary<string, TrackJunctionNode>();
        
        /// <summary>
        /// Array of all TrackNodes in the track database
        /// Warning, the first TrackNode is always null.
        /// </summary>
        public TrackNode[] TrackNodes { get; private set; }
        
        /// <summary>
        /// Array of all Track Items (TrItem) in the track database
        /// </summary>
        public TrackItem[] TrackItems { get; private set; }

        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        public TrackDB(STFReader stf)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tracknodes", ()=>{
                    stf.MustMatch("(");
                    int numberOfTrackNodes = stf.ReadInt(null);
                    TrackNodes = new TrackNode[numberOfTrackNodes + 1];
                    int idx = 1;
                    stf.ParseBlock(new STFReader.TokenProcessor[] {
                        new STFReader.TokenProcessor("tracknode", ()=>{
                            TrackNodes[idx] = TrackNode.ReadTrackNode(stf, idx, numberOfTrackNodes);
                            if (TrackNodes[idx] is TrackJunctionNode junctionNode)
                            {
                                string key = $"{junctionNode.UiD.WorldId}-{junctionNode.UiD.Location.TileX}-{junctionNode.UiD.Location.TileZ}";
                                if (!junctionNodes.ContainsKey(key))
                                    junctionNodes.Add(key, junctionNode);
                                // only need any (first) junction node with that key here to relate back to ShapeIndex
                            }
                            ++idx;
                        }),
                    });
                }),
                new STFReader.TokenProcessor("tritemtable", ()=>{
                    stf.MustMatch("(");
                    int numberOfTrItems = stf.ReadInt(null);
                    TrackItems = new TrackItem[numberOfTrItems];
                    int idx = -1;
                    stf.ParseBlock(()=> ++idx == -1, new STFReader.TokenProcessor[] {
                        new STFReader.TokenProcessor("crossoveritem", ()=>{ TrackItems[idx] = new CrossoverItem(stf,idx); }),
                        new STFReader.TokenProcessor("signalitem", ()=>{ TrackItems[idx] = new SignalItem(stf,idx); }),
                        new STFReader.TokenProcessor("speedpostitem", ()=>{ TrackItems[idx] = new SpeedPostItem(stf,idx); }),
                        new STFReader.TokenProcessor("platformitem", ()=>{ TrackItems[idx] = new PlatformItem(stf,idx); }),
                        new STFReader.TokenProcessor("soundregionitem", ()=>{ TrackItems[idx] = new SoundRegionItem(stf,idx); }),
                        new STFReader.TokenProcessor("emptyitem", ()=>{ TrackItems[idx] = new EmptyItem(stf,idx); }),
                        new STFReader.TokenProcessor("levelcritem", ()=>{ TrackItems[idx] = new LevelCrItem(stf,idx); }),
                        new STFReader.TokenProcessor("sidingitem", ()=>{ TrackItems[idx] = new SidingItem(stf,idx); }),
                        new STFReader.TokenProcessor("hazzarditem", ()=>{ TrackItems[idx] = new HazzardItem(stf,idx); }),
                        new STFReader.TokenProcessor("pickupitem", ()=>{ TrackItems[idx] = new PickupItem(stf,idx); }),
                    });
                }),
            });
        }
        
        /// <summary>
        /// Add a number of TrItems (Track Items), created outside of the file, to the table of TrItems.
        /// This will also set the ID of the TrItems (since that gives the index in that array)
        /// </summary>
        /// <param name="trackItems">The array of new items.</param>
        public void AddTrackItems(TrackItem[] trackItems)
        {
            TrackItem[] tempTrackItems;

            if (TrackItems == null)
            {
                tempTrackItems = new TrackItem[trackItems.Length];
            }
            else
            {
                tempTrackItems = new TrackItem[TrackItems.Length + trackItems.Length];
                TrackItems.CopyTo(tempTrackItems, 0);
            }

            for (int i = 0; i < trackItems.Length; i++)
            {
                int newId = i + TrackItems.Length;
                trackItems[i].TrItemId = (uint)newId;
                tempTrackItems[newId] = trackItems[i];
            }

            TrackItems = tempTrackItems;
        }

        /// <summary>
        /// Provide a link to the TrJunctionNode for the switch track with 
        /// the specified UiD on the specified tile.
        /// 
        /// Called by switch track shapes to determine the correct position of the points.
        /// </summary>
        /// <param name="tileX">X-value of the current Tile</param>
        /// <param name="tileZ">Z-value of the current Tile</param>
        /// <param name="worldId">world ID as defined in world file</param>
        /// <returns>The TrackJunctionNode corresponding the the tile and worldID, null if not found</returns>
        public TrackJunctionNode GetJunctionNode(int tileX, int tileZ, int worldId)
        {
            if (!junctionNodes.TryGetValue($"{worldId}-{tileX}-{tileZ}", out TrackJunctionNode result))
                Trace.TraceWarning("{{TileX:{0} TileZ:{1}}} track node {2} could not be found in TDB", tileX, tileZ, worldId);
            return result;
        }
    }

    #region TrackItem
    /// <summary>
    /// Describes a Track Item, that is an item located on the track that interacts with the train or train operations
    /// This is a base class. 
    /// </summary>
    public abstract class TrackItem
    {
        protected WorldLocation location;
        /// <summary>
        /// The name of the item (used for the label shown by F6)
        /// </summary>
        public string ItemName { get; protected set; }

        public ref readonly WorldLocation Location => ref location;

        /// <summary>Id if track item</summary>
        public uint TrItemId { get; protected internal set; }
        /// <summary>Extra data 1, related to location along section</summary>
        public float SData1 { get; protected set; }
        /// <summary>Extra data 2</summary>
        public string SData2 { get; protected set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        protected TrackItem()
        {
        }

        /// <summary>
        /// Reads the ID from filestream
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="index">The index of this TrItem in the list of TrItems</param>
        protected void ParseTrackItemId(STFReader stf, int index)
        {
            stf.MustMatch("(");
            TrItemId = stf.ReadUInt(null);
            Debug.Assert(index == TrItemId, "Index Mismatch");
            stf.SkipRestOfBlock();
        }
        
        /// <summary>
        /// Reads the Rdata from filestream
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        protected void ParseTrackItemRData(STFReader stf)
        {
            stf.MustMatch("(");
            float x = stf.ReadFloat(null);
            float y = stf.ReadFloat(null);
            float z = stf.ReadFloat(null);
            location = new WorldLocation(stf.ReadInt(null), stf.ReadInt(null), x, y, z);
            stf.SkipRestOfBlock();
        }

        /// <summary>
        /// Reads the SData from filestream
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        protected void ParseTrackItemSData(STFReader stf)
        {
            stf.MustMatch("(");
            SData1 = stf.ReadFloat(STFReader.Units.None, null);
            SData2 = stf.ReadString();
            stf.SkipRestOfBlock();
        }
    } // TrItem

    /// <summary>
    /// Describes a cross-over track item
    /// <summary>A place where two tracks cross over each other</summary>
    /// </summary>
    public class CrossoverItem : TrackItem
    {
        /// <summary>Index to the tracknode</summary>
        public uint TrackNode { get; set; }
        /// <summary>Index to the shape ID</summary>
        public uint ShapeId { get; set; }
        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public CrossoverItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),

                new STFReader.TokenProcessor("crossovertritemdata", ()=>{
                    stf.MustMatch("(");
                    TrackNode = stf.ReadUInt(null);
                    ShapeId = stf.ReadUInt(null);
                    stf.SkipRestOfBlock();
                }),
            });
        }
    }

    /// <summary>
    /// Describes a signal item
    /// </summary>
    public class SignalItem : TrackItem
    {
        /// <summary>
        /// Struct to describe details of the signal for junctions
        /// </summary>
        public struct StrTrSignalDir
        {
            /// <summary>Index to the junction track node</summary>
            public uint TrackNode { get; set; }
            /// <summary>Used with junction signals, appears to be either 1 or 0</summary>
            public uint Sd1 { get; set; }
            /// <summary>Used with junction signals, appears to be either 1 or 0</summary>
            public uint LinkLRPath { get; set; }
            /// <summary>Used with junction signals, appears to be either 1 or 0</summary>
            public uint Sd3 { get; set; }
        }

        /// <summary>Set to  00000001 if junction link set</summary>
        public string Flags1 { get; set; }
        /// <summary>0 or 1 depending on which way signal is facing</summary>
        public uint Direction { get; set; }
        /// <summary>index to Sigal Object Table</summary>
        public int SigObj { get; set; }
        /// <summary>Signal Data 1</summary>
        public float SigData1 { get; set; }
        /// <summary>Type of signal</summary>
        public string SignalType { get; set; }
        /// <summary>Number of junction links</summary>
        public uint NoSigDirs { get; set; }
        /// <summary></summary>
        public StrTrSignalDir[] TrSignalDirs;

        /// <summary>Get the direction the signal is NOT facing</summary>
        public int ReverseDirection
        {
            get { return Direction == 0 ? 1 : 0; }
        }

        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public SignalItem(STFReader stf, int idx)
        {
            SigObj = -1;
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),

                new STFReader.TokenProcessor("trsignaltype", ()=>{
                    stf.MustMatch("(");
                    Flags1 = stf.ReadString();
                    Direction = stf.ReadUInt(null);
                    SigData1 = stf.ReadFloat(STFReader.Units.None, null);
                    SignalType = stf.ReadString().ToLowerInvariant();
                    // To do get index to Sigtypes table corresponding to this sigmal
                    stf.SkipRestOfBlock();
                }),
                new STFReader.TokenProcessor("trsignaldirs", ()=>{
                    stf.MustMatch("(");
                    NoSigDirs = stf.ReadUInt(null);
                    TrSignalDirs = new StrTrSignalDir[NoSigDirs];
                    int sigidx = 0;
                    stf.ParseBlock(new STFReader.TokenProcessor[] {
                        new STFReader.TokenProcessor("trsignaldir", ()=>{
                            if (sigidx >= NoSigDirs)
                                STFException.TraceWarning(stf, "Skipped extra TrSignalDirs");
                            else
                            {
                                TrSignalDirs[sigidx]=new StrTrSignalDir();
                                stf.MustMatch("(");
                                TrSignalDirs[sigidx].TrackNode = stf.ReadUInt(null);
                                TrSignalDirs[sigidx].Sd1 = stf.ReadUInt(null);
                                TrSignalDirs[sigidx].LinkLRPath = stf.ReadUInt(null);
                                TrSignalDirs[sigidx].Sd3 = stf.ReadUInt(null);
                                stf.SkipRestOfBlock();
                                sigidx++;
                            }
                        }),
                    });
                    if (sigidx < NoSigDirs)
                        STFException.TraceWarning(stf, (NoSigDirs - sigidx).ToString(System.Globalization.CultureInfo.CurrentCulture)
                            + " missing TrSignalDirs(s)");
                }),
            });
        }
    }

    /// <summary>
    /// Describes SpeedPost of MilePost (could be Kilometer post as well)
    /// </summary>
    public class SpeedPostItem : TrackItem
    {
        /// <summary>Flags from raw file describing exactly what this is.</summary>
        private uint Flags { get; set; }
        /// <summary>true to be milepost</summary>
        public bool IsMilePost { get; set; }
        /// <summary>speed warning</summary>
        public bool IsWarning { get; set; }
        /// <summary>speed limit</summary>
        public bool IsLimit { get; set; }
        /// <summary>speed resume sign (has no speed defined!)</summary>
        public bool IsResume { get; set; }
        /// <summary>is passenger speed limit</summary>
        public bool IsPassenger { get; set; }
        /// <summary>is freight speed limit</summary>
        public bool IsFreight { get; set; }
        /// <summary>is the digit in MPH or KPH</summary>
        public bool IsMPH { get; set; }
        /// <summary>show numbers instead of KPH, like 5 means 50KMH</summary>
        public bool ShowNumber { get; set; }
        /// <summary>if ShowNumber is true and this is set, will show 1.5 as for 15KMH</summary>
        public bool ShowDot { get; set; }
        /// <summary>Or distance if mile post.</summary>
        public float SpeedInd { get; set; }

        /// <summary>index to Signal Object Table</summary>
        public int SigObj { get; set; }
        /// <summary>speedpost (normalized) angle</summary>
        public float Angle { get; set; }
        /// <summary>derived direction relative to track</summary>
        public int Direction { get; set; }
        /// <summary>number to be displayed if ShowNumber is true</summary>
        public int DisplayNumber { get; set; }

        /// <summary>Get the direction the signal is NOT facing</summary>
        public int ReverseDirection
        {
            get { return Direction == 0 ? 1 : 0; }
        }

        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public SpeedPostItem(STFReader stf, int idx)
        {
            SigObj = -1;
            stf.MustMatch("(");
			stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),

                new STFReader.TokenProcessor("speedposttritemdata", ()=>{
                    stf.MustMatch("(");
                    Flags = stf.ReadUInt(null);
					if ((Flags & 1) != 0) IsWarning = true;
					if ((Flags & (1 << 1)) != 0) IsLimit = true;
					if (!IsWarning && !IsLimit) {
						IsMilePost = true;
					}
					else {
						if (IsWarning && IsLimit)
						{
							IsWarning = false;
							IsResume = true;
						}

						if ((Flags & (1 << 5)) != 0) IsPassenger = true;
						if ((Flags & (1 << 6)) != 0) IsFreight = true;
						if ((Flags & (1 << 7)) != 0) IsFreight = IsPassenger = true;
						if ((Flags & (1 << 8)) != 0) IsMPH = true;
						if ((Flags & (1 << 4)) != 0) {
							ShowNumber = true;
							if ((Flags & (1 << 9)) != 0) ShowDot = true;
						}
					}


                    //  The number of parameters depends on the flags seeting
                    //  To do: Check flags seetings and parse accordingly.
		            if (!IsResume)
		            {
                        //SpeedInd = stf.ReadFloat(STFReader.Units.None, null);
                        if (IsMilePost && ((Flags & (1 << 9)) == 0)) SpeedInd = (float)Math.Truncate(stf.ReadDouble(null));
                        else SpeedInd = stf.ReadFloat(STFReader.Units.None, null);
		            }

    		        if (ShowNumber)
		            {
			            DisplayNumber = stf.ReadInt(null);
		            }
                    
			        Angle = MathHelper.WrapAngle(stf.ReadFloat(STFReader.Units.None, null));

                    stf.SkipRestOfBlock();
                }),
            });
        }

        // used as base for TempSpeedPostItem
        public SpeedPostItem()
        { }
    }

    public class TempSpeedPostItem : SpeedPostItem
    {      
        /// <summary>
        /// Constructor for creating a speedpost from activity speed restriction zone
        /// </summary>
        /// <param name="routeFile">The routeFile with relevant data about speeds</param>
        /// <param name="position">Position/location of the speedposts</param>
        /// <param name="isStart">Is this the start of a speed zone?</param>
        /// 
        public WorldPosition WorldPosition;

        public TempSpeedPostItem(Tr_RouteFile routeFile, in WorldLocation location,  bool isStart, in WorldPosition worldPosition, bool isWarning)
        {
            // TrItemId needs to be set later
            WorldPosition = worldPosition;
            CreateRPData(location);

            IsMilePost = false;
            IsLimit = true;
            IsFreight = IsPassenger = true;
            IsWarning = isWarning;

            if (!isStart) { IsLimit = true; IsResume = true; }//end zone
            float speed = routeFile.TempRestrictedSpeed;
            if (speed < 0) speed = Speed.MeterPerSecond.FromKpH(25); //todo. Value is not used. Should it be used below instead of TempRestrictedSpeed? And if so, is the +0.01 then still needed?
            if (routeFile.MilepostUnitsMetric == true)
            {
                this.IsMPH = false;
                SpeedInd = (int)(Speed.MeterPerSecond.ToKpH(routeFile.TempRestrictedSpeed) + 0.1f); 
            }
            else
            {
                this.IsMPH = true;
                SpeedInd = (int)(Speed.MeterPerSecond.ToMpH(routeFile.TempRestrictedSpeed) + 0.1f);
            }

            Angle = 0;
        }

        /// <summary>
        /// Create the R P data from a position
        /// </summary>
        /// <param name="position">Position of the speedpost</param>
        private void CreateRPData(in WorldLocation location)
        {
            this.location = location;
        }

        public void Update(float y, float angle, in WorldPosition position)
        {
            this.location = location.SetElevation(y);
            Angle = angle;
            WorldPosition = position;
        }
    }

    /// <summary>
    /// Represents a region where a sound can be played.
    /// </summary>
    public class SoundRegionItem : TrackItem
    {
        /// <summary>Sound region data 1</summary>
        public uint SRData1 { get; set; }
        /// <summary>Sound region data 2</summary>
        public uint SRData2 { get; set; }
        /// <summary>Sound region data 3</summary>
        public float SRData3 { get; set; }

        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public SoundRegionItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),

                new STFReader.TokenProcessor("tritemsrdata", ()=>{
                    stf.MustMatch("(");
                    SRData1 = stf.ReadUInt(null);
                    SRData2 = stf.ReadUInt(null);
                    SRData3 = stf.ReadFloat(STFReader.Units.None, null);
                    stf.SkipRestOfBlock();
                }),
            });
        }
    }

    /// <summary>
    /// represent an empty item (which probably should only happen for badly defined routes?)
    /// </summary>
    public class EmptyItem : TrackItem
    {
        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public EmptyItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
            });
        }
    }

    /// <summary>
    /// Representa a level Crossing item (so track crossing road)
    /// </summary>
    public class LevelCrItem : TrackItem
    {
        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public LevelCrItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),
            });
        }
    }

    /// <summary>
    /// Represents either start or end of a siding.
    /// </summary>
    public class SidingItem : TrackItem
    {
        /// <summary>Flags 1 for a siding ???</summary>
        public string Flags1 { get; set; }
        /// <summary>Flags 2 for a siding, probably the index of the other end of the siding.</summary>
        public uint LinkedSidingId { get; set; }

        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public SidingItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),

                new STFReader.TokenProcessor("sidingname", ()=>{ ItemName = stf.ReadStringBlock(""); }),
                new STFReader.TokenProcessor("sidingtritemdata", ()=> {
                    stf.MustMatch("(");
                    Flags1 = stf.ReadString();
                    LinkedSidingId = stf.ReadUInt(null);
                    stf.SkipRestOfBlock();
                }),
            });
        }
    }
    
    /// <summary>
    /// Represents either start or end of a platform (a place where trains can stop).
    /// </summary>
    public class PlatformItem : TrackItem
    {

        /// <summary>Name of the station where the platform is</summary>
        public string Station { get; set; }
        /// <summary>Flags 1 for a platform ???</summary>
        public string Flags1 { get; set; }
        /// <summary>Minimum waiting time at the platform</summary>
        public uint PlatformMinWaitingTime { get; set; }
        /// <summary>Number of passengers waiting at the platform</summary>
        public uint PlatformNumPassengersWaiting { get; set; }
        /// <summary>TrItem Id of the other end of the platform</summary>
        public uint LinkedPlatformItemId { get; set; }

        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public PlatformItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),

                new STFReader.TokenProcessor("platformname", ()=>{ ItemName = stf.ReadStringBlock(""); }),
                new STFReader.TokenProcessor("station", ()=>{ Station = stf.ReadStringBlock(""); }),
                new STFReader.TokenProcessor("platformminwaitingtime", ()=>{ PlatformMinWaitingTime = stf.ReadUIntBlock(null); }),
                new STFReader.TokenProcessor("platformnumpassengerswaiting", ()=>{ PlatformNumPassengersWaiting = stf.ReadUIntBlock(null); }),
                new STFReader.TokenProcessor("platformtritemdata", ()=>{
                    stf.MustMatch("(");
                    Flags1 = stf.ReadString();
                    LinkedPlatformItemId = stf.ReadUInt(null);
                    stf.SkipRestOfBlock();
                }),
            });
        }

        /// <summary>
        /// Constructor to create Platform Item out of Siding Item
        /// </summary>
        /// <param name="thisSiding">The siding to use for a platform creation</param>
        public PlatformItem(SidingItem thisSiding)
        {
            TrItemId = thisSiding.TrItemId;
            SData1 = thisSiding.SData1;
            SData2 = thisSiding.SData2;
            ItemName = thisSiding.ItemName;
            Flags1 = thisSiding.Flags1;
            LinkedPlatformItemId = thisSiding.LinkedSidingId;
            Station = String.Copy(ItemName);
        }
    }

    /// <summary>
    /// Represends a hazard, a place where something more or less dangerous happens
    /// </summary>
    public class HazzardItem : TrackItem
    {
        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public HazzardItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),
            });
        }
    }

    /// <summary>
    /// Represents a pickup, a place to pickup fuel, water, ...
    /// </summary>
    public class PickupItem : TrackItem
    {
        /// <summary>
        /// Default constructor used during file parsing.
        /// </summary>
        /// <param name="stf">The STFreader containing the file stream</param>
        /// <param name="idx">The index of this TrItem in the list of TrItems</param>
        public PickupItem(STFReader stf, int idx)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("tritemid", ()=>{ ParseTrackItemId(stf, idx); }),
                new STFReader.TokenProcessor("tritemrdata", ()=>{ ParseTrackItemRData(stf); }),
                new STFReader.TokenProcessor("tritemsdata", ()=>{ ParseTrackItemSData(stf); }),
            });
        }
    }
    #endregion

}
