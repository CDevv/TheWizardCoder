using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.Data
{
    public class GameAction
    {
        private Global global;
        private Callable callable;

        public GameAction(Global global, string[] data) 
        {
            this.global = global;
            callable = new Callable();

            if (data != null && data.Length > 0)
            {
                string[] args = new string[data.Length-1];
                Array.Copy(data, 1, args, 0, args.Length);

                if (data[0] == "Display")
                {
                    Action displayAction = () => OnDisplay(args);
                    callable = Callable.From(displayAction);
                }
                else if (data[0] == "RoomMethod")
                {
                    Action roomMethodAction = () => OnRoomMethod(args);
                    callable = Callable.From(roomMethodAction);
                }
                else if (data[0] == "PlayerMethod")
                {
                    Action playerMethodAction = () => OnPlayerMethod(args);
                    callable = Callable.From(playerMethodAction);
                }
            }
        }

        public void Call()
        {
            callable.Call();
        }

        private void OnDisplay(string[] args)
        {
            string displayName = args[0];
            if (args.Length == 2)
            {
                global.CurrentRoom.Get(displayName).As<Node>().Call("ShowDisplay", args[1]);
            }
            else
            {
                global.CurrentRoom.GetNode(displayName).Call("ShowDisplay");
            }
        }

        private void OnRoomMethod(string[] args)
        {
            global.CurrentRoom.Call(args[0], args[1]);
        }

        private void OnPlayerMethod(string[] args)
        {
            global.CurrentRoom.Player.Call(args[0], args[1]);
        }
    }
}
