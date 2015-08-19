﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Races;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements race select window.
    /// </summary>
    public class DaggerfallRaceSelectWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "TMAP00I0.IMG";
        const string racePickerImgName = "TAMRIEL2.IMG";

        Texture2D nativeTexture;
        TextLabel promptLabel;
        DFBitmap racePickerBitmap;

        Dictionary<int, RaceTemplate> raceDict = new Dictionary<int, RaceTemplate>();

        public DaggerfallRaceSelectWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("RaceSelectWindow: Could not load native texture.");

            // Populates race dictionary
            PopulateRaceDict();

            // Load picker colours
            racePickerBitmap = GetImgBitmap(racePickerImgName);

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add "Please select your home province..." prompt
            DaggerfallUI ui = DaggerfallUI.Instance;
            promptLabel = AddTextLabel(ui.DefaultFont, new Vector2(0, 16), HardStrings.pleaseSelectYourHomeProvince);
            promptLabel.HorizontalAlignment = HorizontalAlignment.Center;
            promptLabel.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            promptLabel.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            promptLabel.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;

            // Handle clicks
            NativePanel.OnMouseClick += ClickHandler;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(exitKey))
                uiManager.PostMessage(WindowMessages.wmCloseWindow);
        }

        void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            int offset = (int)position.y * racePickerBitmap.Width + (int)position.x;
            if (offset < 0 || offset >= racePickerBitmap.Data.Length)
                return;

            int id = racePickerBitmap.Data[offset];
            if (raceDict.ContainsKey(id))
            {
                RaceTemplate raceTemplate = raceDict[id];
                TextFile.Token[] raceDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(raceTemplate.DescriptionID);

                promptLabel.Enabled = false;
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(raceDescription);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmRacePopup_OnButtonClick;
                messageBox.OnClose += ConfirmRacePopup_OnClose;
                uiManager.PushWindow(messageBox);
                AudioClip clip = DaggerfallUnity.Instance.SoundReader.GetAudioClip((uint)raceTemplate.ClipID);
                DaggerfallUI.Instance.AudioSource.PlayOneShot(clip);
            }
        }

        void ConfirmRacePopup_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                Debug.Log("Race confirmed.");
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.No)
                sender.CloseWindow();
        }

        void ConfirmRacePopup_OnClose()
        {
            promptLabel.Enabled = true;
        }

        #region Private Method

        // Manually populates race dictionary.
        // This is only temporary until loading race definitions from file is implemented.
        void PopulateRaceDict()
        {
            // Instantiate race templates
            Breton breton = new Breton();
            Redguard redguard = new Redguard();
            Nord nord = new Nord();
            DarkElf darkElf = new DarkElf();
            HighElf highElf = new HighElf();
            WoodElf woodElf = new WoodElf();
            Khajiit khajiit = new Khajiit();
            Argonian argonian = new Argonian();

            // Populate dictionary
            raceDict.Add(breton.ID, breton);
            raceDict.Add(redguard.ID, redguard);
            raceDict.Add(nord.ID, nord);
            raceDict.Add(darkElf.ID, darkElf);
            raceDict.Add(highElf.ID, highElf);
            raceDict.Add(woodElf.ID, woodElf);
            raceDict.Add(khajiit.ID, khajiit);
            raceDict.Add(argonian.ID, argonian);
        }

        #endregion
    }
}