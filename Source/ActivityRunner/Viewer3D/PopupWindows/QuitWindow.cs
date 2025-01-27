﻿using FreeTrainSimulator.Common.Info;
using FreeTrainSimulator.Common.Input;
using FreeTrainSimulator.Graphics;
using FreeTrainSimulator.Graphics.Window;
using FreeTrainSimulator.Graphics.Window.Controls;
using FreeTrainSimulator.Graphics.Window.Controls.Layout;

using GetText;

using Microsoft.Xna.Framework;

using Orts.ActivityRunner.Processes;
using Orts.Simulation.Multiplayer;

namespace Orts.ActivityRunner.Viewer3D.PopupWindows
{
    internal sealed class QuitWindow : WindowBase
    {
        private readonly UserCommandController<UserCommand> userCommandController;
        private readonly Viewer viewer;

        public QuitWindow(WindowManager owner, Point relativeLocation, Viewer viewer, Catalog catalog = null) :
            base(owner, (catalog ??= CatalogManager.Catalog).GetString("Pause Menu"), relativeLocation, new Point(320, 112), catalog)
        {
            Modal = true;
            ZOrder = 100;
            userCommandController = owner.UserCommandController as UserCommandController<UserCommand>;
            this.viewer = viewer;
            if (MultiPlayerManager.IsMultiPlayer())
                Resize(new Point(320, 95));
        }

        protected override ControlLayout Layout(ControlLayout layout, float headerScaling)
        {
            layout = base.Layout(layout, 1.5f);

            ControlLayout buttonLine = layout.AddLayoutHorizontal((int)(Owner.TextFontDefault.Height * 1.5));
            Label quitLabel = new Label(this, layout.RemainingWidth, Owner.TextFontDefault.Height, Catalog.GetString($"Quit {RuntimeInfo.ApplicationName} ({viewer.UserSettings.KeyboardSettings.UserCommands[UserCommand.GameQuit]})"), HorizontalAlignment.Center);
            quitLabel.OnClick += QuitLabel_OnClick;
            buttonLine.Add(quitLabel);
            layout.AddHorizontalSeparator();
            if (!MultiPlayerManager.IsMultiPlayer())
            {
                buttonLine = layout.AddLayoutHorizontal((int)(Owner.TextFontDefault.Height * 1.5));
                Label saveLabel = new Label(this, layout.RemainingWidth, Owner.TextFontDefault.Height, Catalog.GetString($"Save your game ({viewer.UserSettings.KeyboardSettings.UserCommands[UserCommand.GameSave]})"), HorizontalAlignment.Center);
                saveLabel.OnClick += SaveLabel_OnClick;
                buttonLine.Add(saveLabel);
                layout.AddHorizontalSeparator();
            }
            buttonLine = layout.AddLayoutHorizontal((int)(Owner.TextFontDefault.Height * 1.5));
            Label continueLabel = new Label(this, layout.RemainingWidth, Owner.TextFontDefault.Height, Catalog.GetString($"Continue playing ({viewer.UserSettings.KeyboardSettings.UserCommands[UserCommand.GamePauseMenu]})"), HorizontalAlignment.Center);
            continueLabel.OnClick += ContinueLabel_OnClick;
            buttonLine.Add(continueLabel);
            return layout;
        }

        private void ContinueLabel_OnClick(object sender, MouseClickEventArgs e)
        {
            _ = Close();
        }

        private async void SaveLabel_OnClick(object sender, MouseClickEventArgs e)
        {
            await viewer.Game.State.Save().ConfigureAwait(false);
        }

        private void QuitLabel_OnClick(object sender, MouseClickEventArgs e)
        {
            (Owner.Game as GameHost).PopState();
        }

        public override bool Open()
        {
            bool result = base.Open();
            if (result)
            {
                userCommandController.AddEvent(UserCommand.GamePauseMenu, KeyEventType.KeyPressed, QuitGame, true);
                userCommandController.AddEvent(UserCommand.GameSave, KeyEventType.KeyPressed, SaveGame, true);
            }
            return result;
        }

        public override bool Close()
        {
            userCommandController.RemoveEvent(UserCommand.GamePauseMenu, KeyEventType.KeyPressed, QuitGame);
            userCommandController.RemoveEvent(UserCommand.GameSave, KeyEventType.KeyPressed, SaveGame);
            Program.Viewer.ResumeReplaying();
            return base.Close();
        }

        private void QuitGame(UserCommandArgs args)
        {
            args.Handled = true;
            _ = Close();
        }

        private async void SaveGame(UserCommandArgs args)
        {
            args.Handled = true;
            await viewer.Game.State.Save().ConfigureAwait(false);
        }
    }
}
