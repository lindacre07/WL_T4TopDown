using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration.Demo
{
    /// <summary>Self-contained local multiplayer targeting demonstration for up to four gamepads.</summary>
    public sealed class GamepadVibrationMultiplayerDemo : MonoBehaviour
    {
        private sealed class PlayerSlot
        {
            public Gamepad Gamepad;
            public GamepadVibrationHandle Continuous;
            public readonly List<GamepadVibrationHandle> OwnedHandles = new List<GamepadVibrationHandle>();
            public string LastEffect = "None";
        }

        private readonly PlayerSlot[] players =
        {
            new PlayerSlot(), new PlayerSlot(), new PlayerSlot(), new PlayerSlot()
        };
        private readonly HashSet<Gamepad> assigned = new HashSet<Gamepad>();
        private int selectedPlayer;
        private GUIStyle titleStyle;
        private GUIStyle headingStyle;
        private GUIStyle statusStyle;
        private GUIStyle codeStyle;

        private void Update()
        {
            ReleaseDisconnectedPlayers();
            JoinPlayersFromGamepads();
            for (int i = 0; i < players.Length; i++)
            {
                players[i].OwnedHandles.RemoveAll(handle => handle == null || !handle.IsPlaying);
                ProcessPlayerInput(i);
            }
        }

        private void ReleaseDisconnectedPlayers()
        {
            for (int i = 0; i < players.Length; i++)
            {
                PlayerSlot slot = players[i];
                if (slot.Gamepad == null || slot.Gamepad.added) continue;
                Gamepad disconnected = slot.Gamepad;
                StopOwned(slot);
                assigned.Remove(disconnected);
                slot.Gamepad = null;
                slot.LastEffect = "Waiting for replacement";
            }
        }

        private void JoinPlayersFromGamepads()
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (assigned.Contains(gamepad) || !gamepad.buttonSouth.wasPressedThisFrame) continue;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].Gamepad != null) continue;
                    players[i].Gamepad = gamepad;
                    players[i].LastEffect = "Joined";
                    assigned.Add(gamepad);
                    selectedPlayer = i;
                    break;
                }
            }
        }

        private void ProcessPlayerInput(int playerIndex)
        {
            PlayerSlot slot = players[playerIndex];
            Gamepad gamepad = slot.Gamepad;
            if (gamepad == null || !gamepad.added) return;

            if (gamepad.buttonSouth.wasPressedThisFrame) Play(playerIndex, GamepadVibrationPresets.LightHit, "Light Hit");
            if (gamepad.buttonEast.wasPressedThisFrame) Play(playerIndex, GamepadVibrationPresets.Explosion, "Explosion");
            if (gamepad.startButton.wasPressedThisFrame) Play(playerIndex, GamepadVibrationPresets.Alert, "Alert");
            if (gamepad.rightTrigger.wasPressedThisFrame) StartContinuous(playerIndex);
            if (gamepad.rightTrigger.wasReleasedThisFrame) StopContinuous(playerIndex);
        }

        private void Play(int playerIndex, GamepadVibrationSettings settings, string effect)
        {
            PlayerSlot slot = players[playerIndex];
            if (!IsAvailable(slot)) return;
            slot.OwnedHandles.Add(settings.Play(slot.Gamepad));
            slot.LastEffect = effect;
            selectedPlayer = playerIndex;
        }

        private void StartContinuous(int playerIndex)
        {
            PlayerSlot slot = players[playerIndex];
            if (!IsAvailable(slot)) return;
            if (slot.Continuous != null)
            {
                slot.Continuous.Stop();
                slot.OwnedHandles.Remove(slot.Continuous);
            }
            slot.Continuous = GamepadVibrationManager.PlayContinuous(0.35f, 0.20f, slot.Gamepad);
            slot.OwnedHandles.Add(slot.Continuous);
            slot.LastEffect = "Continuous ON";
            selectedPlayer = playerIndex;
        }

        private void StopContinuous(int playerIndex)
        {
            PlayerSlot slot = players[playerIndex];
            slot.Continuous?.Stop();
            slot.OwnedHandles.Remove(slot.Continuous);
            slot.Continuous = null;
            slot.LastEffect = "Continuous OFF";
        }

        private void StopPlayer(int playerIndex)
        {
            PlayerSlot slot = players[playerIndex];
            StopOwned(slot);
            slot.LastEffect = "Stopped";
        }

        private static void StopOwned(PlayerSlot slot)
        {
            for (int i = slot.OwnedHandles.Count - 1; i >= 0; i--)
                slot.OwnedHandles[i]?.Stop();
            slot.OwnedHandles.Clear();
            slot.Continuous = null;
        }

        private static bool IsAvailable(PlayerSlot slot)
        {
            return slot.Gamepad != null && slot.Gamepad.added;
        }

        private void OnGUI()
        {
            BuildStyles();
            float width = Mathf.Min(Screen.width - 40f, 1180f);
            float x = (Screen.width - width) * 0.5f;
            GUILayout.BeginArea(new Rect(x, 18f, width, Screen.height - 36f));
            GUILayout.Label("LOCAL MULTIPLAYER GAMEPAD TARGETING", titleStyle);
            GUILayout.Label("Press A / Cross on each unassigned gamepad to join • Click a panel to select a player", statusStyle);
            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            for (int i = 0; i < players.Length; i++) DrawPlayerPanel(i, width / 4f - 8f);
            GUILayout.EndHorizontal();

            GUILayout.Space(12f);
            GUILayout.BeginHorizontal();
            GUI.enabled = IsAvailable(players[selectedPlayer]);
            if (GUILayout.Button("LIGHT HIT", GUILayout.Height(36f))) Play(selectedPlayer, GamepadVibrationPresets.LightHit, "Light Hit");
            if (GUILayout.Button("EXPLOSION", GUILayout.Height(36f))) Play(selectedPlayer, GamepadVibrationPresets.Explosion, "Explosion");
            if (GUILayout.Button("ALERT", GUILayout.Height(36f))) Play(selectedPlayer, GamepadVibrationPresets.Alert, "Alert");
            if (GUILayout.Button("START CONTINUOUS", GUILayout.Height(36f))) StartContinuous(selectedPlayer);
            if (GUILayout.Button("STOP SELECTED", GUILayout.Height(36f))) StopPlayer(selectedPlayer);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("STOP ALL GAMEPADS", GUILayout.Height(42f)))
            {
                for (int i = 0; i < players.Length; i++)
                {
                    StopOwned(players[i]);
                    players[i].LastEffect = "Stopped";
                }
            }

            GUILayout.Space(14f);
            GUILayout.Label("CONTROLLER INPUT", headingStyle);
            GUILayout.Label("A / Cross: Light Hit   •   B / Circle: Explosion   •   Start: Alert   •   Hold Right Trigger: Continuous", statusStyle);
            GUILayout.Space(10f);
            GUILayout.Label("CODE USED FOR THE SELECTED PLAYER", headingStyle);
            GUILayout.Label("GamepadVibrationPresets.Explosion.Play(playerGamepad);\nGamepadVibrationPresets.Alert.Play(playerGamepad);\nownedHandle.Stop();", codeStyle);
            GUILayout.EndArea();
        }

        private void DrawPlayerPanel(int index, float width)
        {
            PlayerSlot slot = players[index];
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = index == selectedPlayer ? new Color(0.25f, 0.65f, 1f) : new Color(0.28f, 0.30f, 0.36f);
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.Height(180f));
            if (GUILayout.Button($"PLAYER {index + 1}", headingStyle, GUILayout.Height(34f))) selectedPlayer = index;
            GUI.backgroundColor = previous;

            if (slot.Gamepad == null)
            {
                GUILayout.Label("WAITING FOR GAMEPAD", statusStyle);
                GUILayout.Label("Press A / Cross to join", statusStyle);
            }
            else if (!slot.Gamepad.added)
            {
                GUILayout.Label("DISCONNECTED", statusStyle);
                GUILayout.Label(slot.Gamepad.displayName, statusStyle);
            }
            else
            {
                GUILayout.Label("CONNECTED", statusStyle);
                GUILayout.Label(slot.Gamepad.displayName, statusStyle);
                GUILayout.Label($"Device ID: {slot.Gamepad.deviceId}", statusStyle);
                GUILayout.Space(6f);
                GUILayout.Label($"Last: {slot.LastEffect}", statusStyle);
            }
            GUILayout.EndVertical();
        }

        private void BuildStyles()
        {
            if (titleStyle != null) return;
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            headingStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            statusStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            codeStyle = new GUIStyle(GUI.skin.box) { fontSize = 15, alignment = TextAnchor.MiddleLeft, wordWrap = true, padding = new RectOffset(14, 14, 10, 10) };
        }

        private void OnDisable()
        {
            for (int i = 0; i < players.Length; i++) StopOwned(players[i]);
        }
    }
}
