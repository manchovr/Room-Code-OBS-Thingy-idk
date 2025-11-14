using BepInEx;
using GorillaGameModes;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using Utilla;
using Utilla.Attributes;
using Utilla.Models;
using Utilla.Utils;
using Valve.VR;

namespace CodeLogThingOBS
{
	/// <summary>
	/// This is your mod's main class.
	/// </summary>

	/* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
	[ModdedGamemode]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
        private bool hiddencode = false;
		string actualtext = "Not Connected To Room";
		private float timer = 0f;

        void Start()
		{
			/* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */
			Events.GameInitialized += OnGameInitialized;
		}

		void OnEnable()
		{
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			HarmonyPatches.ApplyHarmonyPatches();
		}

		void OnDisable()
		{
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

			HarmonyPatches.RemoveHarmonyPatches();
		}

		void OnGameInitialized(object sender, EventArgs e)
		{
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "roomcode.txt"), "");
        }

		void Update()
		{

            bool leftStickClick;
            bool IsSteamVR = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";

            if (IsSteamVR) { leftStickClick = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetStateDown(SteamVR_Input_Sources.LeftHand); }
            else { ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out leftStickClick); }

            if (leftStickClick)
			{
				hiddencode = !hiddencode;
				Debug.Log(hiddencode);
			}

            timer += Time.deltaTime;

            if (string.IsNullOrEmpty(PhotonNetwork.CurrentRoom?.Name))
            {
                actualtext = "Not Connected To Room";
            }
			else if (hiddencode)
			{
                actualtext = "Code: " + PhotonNetwork.CurrentRoom.Name + " | " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            }
			else
            {
                actualtext = "Code: ??? | " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            }

            if (timer >= 1f)
            {
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "roomcode.txt"), actualtext);
                timer = 0f;
            }
        }
    }
}
