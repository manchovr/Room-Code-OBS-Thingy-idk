using BepInEx;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilla;
using Utilla.Attributes;
using Utilla.Models;
using Utilla.Utils;

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
        bool inRoom;
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
            timer += Time.deltaTime;

            if (string.IsNullOrEmpty(PhotonNetwork.CurrentRoom?.Name))
            {
                actualtext = "Not Connected To Room";
            }
            else
            {
                actualtext = "Code: " + PhotonNetwork.CurrentRoom.Name + " | " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            }

            if (timer >= 1f)
            {
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "roomcode.txt"), actualtext);
                timer = 0f;
            }
        }
    }
}
