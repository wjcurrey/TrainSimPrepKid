﻿using System;
using System.Collections.Generic;
using System.Linq;

using GetText;

using Microsoft.Xna.Framework;

using Orts.Common;
using Orts.Common.Calc;
using Orts.Common.Input;
using Orts.Graphics;
using Orts.Graphics.Window;
using Orts.Graphics.Window.Controls;
using Orts.Graphics.Window.Controls.Layout;
using Orts.Settings;
using Orts.Simulation;
using Orts.Simulation.RollingStocks;
using Orts.Simulation.RollingStocks.SubSystems.Brakes;

namespace Orts.ActivityRunner.Viewer3D.PopupWindows
{
    internal class DistributedPowerWindow : WindowBase
    {
        private const int monoColumnWidth = 48;
        private const int normalColumnWidth = 64;

        private enum WindowMode
        {
            Normal,
            NormalMono,
            Short,
            ShortMono,
        }

        private enum GroupDetail
        {
            GroupId,
            LocomotivesNumber,
            Throttle,
            Load,
            BrakePressure,
            Remote,
            EqualizerReservoir,
            BrakeCylinder,
            MainReservoir,
        }

        private readonly UserSettings settings;
        private readonly UserCommandController<UserCommand> userCommandController;
        private WindowMode windowMode;
        private Label labelExpandMono;
        private Label labelExpandDetails;
        private readonly EnumArray<ControlLayout, GroupDetail> groupDetails = new EnumArray<ControlLayout, GroupDetail>();
        private int groupCount;

        public DistributedPowerWindow(WindowManager owner, Point relativeLocation, UserSettings settings, Catalog catalog = null) :
            base(owner, (catalog ??= CatalogManager.Catalog).GetString("Distributed Power"), relativeLocation, new Point(160, 200), catalog)
        {
            userCommandController = Owner.UserCommandController as UserCommandController<UserCommand>;
            this.settings = settings;
            _ = EnumExtension.GetValue(settings.PopupSettings[ViewerWindowType.DistributedPowerWindow], out windowMode);
            UpdatePowerInformation();
            Resize();
        }

        protected override ControlLayout Layout(ControlLayout layout, float headerScaling = 1)
        {
            layout = base.Layout(layout, headerScaling).AddLayoutOffset(0);
            ControlLayout line = layout.AddLayoutHorizontal();
            line.HorizontalChildAlignment = HorizontalAlignment.Right;
            line.VerticalChildAlignment = VerticalAlignment.Top;
            line.Add(labelExpandMono = new Label(this, Owner.TextFontDefault.Height, Owner.TextFontDefault.Height, windowMode == WindowMode.ShortMono || windowMode == WindowMode.NormalMono ? Markers.ArrowRight : Markers.ArrowLeft, HorizontalAlignment.Center, Color.Yellow));
            labelExpandMono.OnClick += LabelExpandMono_OnClick;
            line.Add(labelExpandDetails = new Label(this, Owner.TextFontDefault.Height, Owner.TextFontDefault.Height, windowMode == WindowMode.Normal || windowMode == WindowMode.NormalMono ? Markers.ArrowUp : Markers.ArrowDown, HorizontalAlignment.Center, Color.Yellow));
            labelExpandDetails.OnClick += LabelExpandDetails_OnClick;
            labelExpandDetails.Visible = labelExpandMono.Visible = groupCount > 0;
            layout = layout.AddLayoutVertical();
            if (groupCount == 0)
            {
                layout.VerticalChildAlignment = VerticalAlignment.Center;
                layout.Add(new Label(this, layout.RemainingWidth, Owner.TextFontDefault.Height, Catalog.GetString("Distributed power management not available with this player train."), HorizontalAlignment.Center));
                Caption = Catalog.GetString("Distributed Power Info");
            }
            else
            {
                Caption = Catalog.GetString("DPU Info");

                void AddDetailLine(GroupDetail groupDetail, int width, string labelText, System.Drawing.Font font, HorizontalAlignment alignment = HorizontalAlignment.Right)
                {
                    line = layout.AddLayoutHorizontalLineOfText();
                    line.Add(new Label(this, width, font.Height, labelText, font));
                    for (int i = 0; i < groupCount; i++)
                    {
                        line.Add(new Label(this, 0, 0, width, font.Height, null, alignment, font, Color.White));
                        line.Add(new Label(this, 0, 0, 10, font.Height, null, HorizontalAlignment.Center, Owner.TextFontDefault, Color.Green));
                    }
                    groupDetails[groupDetail] = line;
                }

                if (windowMode == WindowMode.ShortMono || windowMode == WindowMode.NormalMono)
                {
                    int columnWidth = (int)(Owner.DpiScaling * monoColumnWidth);
                    AddDetailLine(GroupDetail.GroupId, columnWidth, FourCharAcronym.LocoGroup.GetLocalizedDescription(), Owner.TextFontMonoDefaultBold, HorizontalAlignment.Center);
                    layout.AddHorizontalSeparator(true);
                    AddDetailLine(GroupDetail.LocomotivesNumber, columnWidth, FourCharAcronym.Locomotives.GetLocalizedDescription(), Owner.TextFontMonoDefault);
                    AddDetailLine(GroupDetail.Throttle, columnWidth, FourCharAcronym.Throttle.GetLocalizedDescription(), Owner.TextFontMonoDefault);
                    AddDetailLine(GroupDetail.Load, columnWidth, FourCharAcronym.Load.GetLocalizedDescription(), Owner.TextFontMonoDefault);
                    AddDetailLine(GroupDetail.BrakePressure, columnWidth, FourCharAcronym.BrakePressure.GetLocalizedDescription(), Owner.TextFontMonoDefault);
                    AddDetailLine(GroupDetail.Remote, columnWidth, FourCharAcronym.Remote.GetLocalizedDescription(), Owner.TextFontMonoDefault);

                    if (windowMode == WindowMode.NormalMono)
                    {
                        AddDetailLine(GroupDetail.EqualizerReservoir, columnWidth, Catalog.GetString("ER"), Owner.TextFontMonoDefault);
                        AddDetailLine(GroupDetail.BrakeCylinder, columnWidth, Catalog.GetString("BC"), Owner.TextFontMonoDefault);
                        AddDetailLine(GroupDetail.MainReservoir, columnWidth, Catalog.GetString("MR"), Owner.TextFontMonoDefault);
                    }
                }
                else
                {
                    int columnWidth = (int)(Owner.DpiScaling * normalColumnWidth);
                    AddDetailLine(GroupDetail.GroupId, columnWidth, Catalog.GetString("Group"), Owner.TextFontDefault, HorizontalAlignment.Center);
                    layout.AddHorizontalSeparator(true);
                    AddDetailLine(GroupDetail.LocomotivesNumber, columnWidth, Catalog.GetString("Locos"), Owner.TextFontDefault);
                    AddDetailLine(GroupDetail.Throttle, columnWidth, Catalog.GetString("Throttle"), Owner.TextFontDefault);
                    AddDetailLine(GroupDetail.Load, columnWidth, Catalog.GetString("Load"), Owner.TextFontDefault);
                    AddDetailLine(GroupDetail.BrakePressure, columnWidth, Catalog.GetString("Brk Pres"), Owner.TextFontDefault);
                    AddDetailLine(GroupDetail.Remote, columnWidth, Catalog.GetString("Remote"), Owner.TextFontDefault);

                    if (windowMode == WindowMode.Normal)
                    {
                        AddDetailLine(GroupDetail.EqualizerReservoir, columnWidth, Catalog.GetString("ER"), Owner.TextFontDefault);
                        AddDetailLine(GroupDetail.BrakeCylinder, columnWidth, Catalog.GetString("BC"), Owner.TextFontDefault);
                        AddDetailLine(GroupDetail.MainReservoir, columnWidth, Catalog.GetString("MR"), Owner.TextFontDefault);
                    }
                }
            }
            return layout;
        }

        private void LabelExpandDetails_OnClick(object sender, MouseClickEventArgs e)
        {
            windowMode = windowMode.Next().Next();
            Resize();
        }

        private void LabelExpandMono_OnClick(object sender, MouseClickEventArgs e)
        {
            windowMode = windowMode == WindowMode.Normal || windowMode == WindowMode.Short ? windowMode.Next() : windowMode.Previous();
            Resize();
        }

        protected override void Update(GameTime gameTime, bool shouldUpdate)
        {
            base.Update(gameTime, shouldUpdate);
            if (shouldUpdate)
            {
                UpdatePowerInformation();
            }
        }

        public override bool Open()
        {
            userCommandController.AddEvent(UserCommand.DisplayDistributedPowerWindow, KeyEventType.KeyPressed, TabAction, true);
            return base.Open();
        }

        public override bool Close()
        {
            userCommandController.RemoveEvent(UserCommand.DisplayDistributedPowerWindow, KeyEventType.KeyPressed, TabAction);
            return base.Close();
        }

        private void TabAction(UserCommandArgs args)
        {
            if (groupCount > 0 && args is ModifiableKeyCommandArgs keyCommandArgs && (keyCommandArgs.AdditionalModifiers & settings.Input.WindowTabCommandModifier) == settings.Input.WindowTabCommandModifier)
            {
                windowMode = windowMode.Next();
                Resize();
            }
        }

        private void Resize()
        {
            if (groupCount == 0)
            {
                Resize(new Point(420, 60));
            }
            else
            {
                Point size = windowMode switch
                {
                    WindowMode.Normal => new Point((groupCount + 1) * (normalColumnWidth + 10), 170),
                    WindowMode.NormalMono => new Point((groupCount + 1) * (monoColumnWidth + 10), 170),
                    WindowMode.Short => new Point((groupCount + 1) * (normalColumnWidth + 10), 130),
                    WindowMode.ShortMono => new Point((groupCount + 1) * (monoColumnWidth + 10), 130),
                    _ => throw new InvalidOperationException(),
                };

                Resize(size);
            }

            settings.PopupSettings[ViewerWindowType.DistributedPowerWindow] = windowMode.ToString();
        }

        private void UpdatePowerInformation()
        {
            IEnumerable<IGrouping<int, MSTSDieselLocomotive>> distributedLocomotives = Simulator.Instance.PlayerLocomotive.Train.Cars.OfType<MSTSDieselLocomotive>().GroupBy((dieselLocomotive) => dieselLocomotive.DistributedPowerUnitId);
            int groups = distributedLocomotives.Count();

            if (groups != groupCount)
            {
                groupCount = groups;
                Resize();
            }

            int i = 1;
            RemoteControlGroup remoteControlGroup = RemoteControlGroup.FrontGroupSync;

            foreach (IGrouping<int, MSTSDieselLocomotive> item in distributedLocomotives)
            {
                MSTSDieselLocomotive groupLead = item.FirstOrDefault();
                bool fence = remoteControlGroup != (remoteControlGroup = groupLead.RemoteControlGroup);

                if (groupDetails[GroupDetail.GroupId]?.Controls[i] is Label groupLabel)
                    groupLabel.Text = $"{groupLead?.DistributedPowerUnitId}";
                if (i > 1) //fence is before the current group
                {
                    foreach (GroupDetail groupDetail in EnumExtension.GetValues<GroupDetail>())
                    {
                        if (groupDetails[groupDetail]?.Controls[i - 1] is Label label)
                        {
                            label.Text = fence ? Markers.Fence : null;
                            if (groupDetail == GroupDetail.GroupId)
                            {
                                if (!fence)
                                    label.Text = Markers.Dash;
                                label.TextColor = fence ? Color.Green : Color.White;
                            }
                        }
                    }
                }
                if (groupDetails[GroupDetail.LocomotivesNumber]?.Controls[i] is Label locoLabel)
                    locoLabel.Text = $"{item.Count()}";
                if (groupDetails[GroupDetail.Throttle]?.Controls[i] is Label throttleLabel)
                {
                    throttleLabel.Text = $"{groupLead.DistributedPowerThrottleInfo()}";
                    throttleLabel.TextColor = groupLead.DynamicBrakePercent >= 0 ? Color.Yellow : Color.White;
                }
                if (groupDetails[GroupDetail.BrakePressure]?.Controls[i] is Label brakeLabel)
                {
                    brakeLabel.Text = $"{FormatStrings.FormatPressure(groupLead.BrakeSystem.BrakeLine1PressurePSI, Pressure.Unit.PSI, groupLead.BrakeSystemPressureUnits[BrakeSystemComponent.BrakePipe], windowMode == WindowMode.Normal || windowMode == WindowMode.Short)}";
                }
                if (groupDetails[GroupDetail.Load]?.Controls[i] is Label loadLabel)
                {
                    loadLabel.Text = $"{groupLead.DistributedPowerLoadInfo():F0}{(windowMode == WindowMode.Normal || windowMode == WindowMode.Short ? $" {(Simulator.Instance.Route.MilepostUnitsMetric ? " A" : " K")}" : "")}";
                }
                if (groupDetails[GroupDetail.Remote]?.Controls[i] is Label remoteLabel)
                {
                    remoteLabel.Text = $"{(groupLead.IsLeadLocomotive() || groupLead.RemoteControlGroup < 0 ? "———" : groupLead.RemoteControlGroup == 0 ? Catalog.GetString("Sync") : Catalog.GetString("Async"))}";
                }
                if (windowMode == WindowMode.Normal || windowMode == WindowMode.NormalMono)
                {
                    TrainCar lastCar = groupLead.Train.Cars[^1];
                    if (lastCar == groupLead)
                        lastCar = groupLead.Train.Cars[0];

                    // EQ
                    if (groupDetails[GroupDetail.EqualizerReservoir]?.Controls[i] is Label eqLabel)
                        eqLabel.Text = $"{FormatStrings.FormatPressure(lastCar.Train.EqualReservoirPressurePSIorInHg, Pressure.Unit.PSI, groupLead.BrakeSystemPressureUnits[BrakeSystemComponent.EqualizingReservoir], windowMode == WindowMode.Normal)}";

                    // BC
                    if (groupDetails[GroupDetail.BrakeCylinder]?.Controls[i] is Label bcLabel)
                        bcLabel.Text = $"{FormatStrings.FormatPressure(groupLead.BrakeSystem.GetCylPressurePSI(), Pressure.Unit.PSI, groupLead.BrakeSystemPressureUnits[BrakeSystemComponent.MainReservoir], windowMode == WindowMode.Normal)}";

                    // MR
                    if (groupDetails[GroupDetail.MainReservoir]?.Controls[i] is Label mrLabel)
                        mrLabel.Text = $"{FormatStrings.FormatPressure(groupLead.MainResPressurePSI, Pressure.Unit.PSI, groupLead.BrakeSystemPressureUnits[BrakeSystemComponent.MainReservoir], windowMode == WindowMode.Normal)}";
                }
                i += 2;
            }

        }
    }
}
