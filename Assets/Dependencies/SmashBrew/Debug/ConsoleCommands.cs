using System;
using HouraiTeahouse.Console;
using HouraiTeahouse.Events;
using HouraiTeahouse.Localization;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     Debug mode to turn on and off the rendering of hitboxes
    /// </summary>
    public class ConsoleCommands : MonoBehaviour {
        private void OnEnable() {
            GameConsole.RegisterCommand("hitbox", HitboxCommand);
            GameConsole.RegisterCommand("language", LanguageCommand);
            GameConsole.RegisterCommand("kill", KillCommand);
            GameConsole.RegisterCommand("damage", DamageCommand);
        }

        private void OnDisable() {
            GameConsole.UnregisterCommand("hitbox", HitboxCommand);
            GameConsole.UnregisterCommand("language", LanguageCommand);
            GameConsole.UnregisterCommand("kill", KillCommand);
            GameConsole.UnregisterCommand("damage", DamageCommand);
        }

        private void ChangeHitboxes(bool state) {
            Hitbox.DrawHitboxes = state;
            GameConsole.Log("Hitbox drawing: {0}", Hitbox.DrawHitboxes);
        }

        private bool ArgLengthCheck(int count, string[] args, string name) {
            var check = args.Length >= 1;
            if (!check) {
                GameConsole.Log("The command \"{0}\" requires at least {1} parameters.", name, count);
            }
            return check;
        }

        private int? IntParse(string src) {
            var val = 0;
            if (int.TryParse(src, out val))
                return val;
            return null;
        }

        private float? FloatParse(string src) {
            float val = 0;
            if (float.TryParse(src, out val))
                return val;
            return null;
        }

        private Player GetPlayer(string playerNumber) {
            var playerNum = IntParse(playerNumber);
            if (playerNum != null) {
                if (playerNum <= 0 || playerNum > Player.MaxPlayers) {
                    GameConsole.Log("There is no Player #{0}, try between 1 and {1}", playerNum, Player.MaxPlayers);
                }
                else {
                    return Player.GetPlayer(playerNum.Value - 1);
                }
            }
            else {
                GameConsole.Log("The term {0} cannot be converted to a player number.", playerNumber);
            }
            return null;
        }

        private void KillCommand(string[] args) {
            if (!ArgLengthCheck(1, args, "kill"))
                return;
            var player = GetPlayer(args[0]);
            if (player == null)
                return;
            GlobalMediator.Instance.Publish(new PlayerDieEvent {Player = player});
        }

        private void DamageCommand(string[] args) {
            if (!ArgLengthCheck(2, args, "damage"))
                return;
            var player = GetPlayer(args[0]);
            if (player == null)
                return;
            var damage = FloatParse(args[1]);
            if (damage == null) {
                GameConsole.Log("The term {0} cannot be converted into a damage value.", args[1]);
                return;
            }
            player.PlayerObject.Damage(this, damage.Value);
        }

        private void LanguageCommand(string[] args) {
            if (!ArgLengthCheck(1, args, "language"))
                return;
            try {
                LanguageManager.Instance.LoadLanguage(args[0]);
                GameConsole.Log("Game Langauge changed to {0}", args[0]);
            }
            catch (InvalidOperationException ae) {
                GameConsole.Log(ae.Message);
            }
        }

        private void HitboxCommand(string[] args) {
            if (!ArgLengthCheck(1, args, "hitbox"))
                return;
            switch (args[0].ToLower()) {
                case "enable":
                    ChangeHitboxes(true);
                    break;
                case "disable":
                    ChangeHitboxes(false);
                    break;
                case "toggle":
                    ChangeHitboxes(!Hitbox.DrawHitboxes);
                    break;
                default:
                    GameConsole.Log("Setting {0} not recognized.", args[0]);
                    break;
            }
        }
    }
}