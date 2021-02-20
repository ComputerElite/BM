using DiscordRPC;
using System;

namespace DCRPManager
{
    public class PresenceManager
    {
        DiscordRpcClient client;
        Button[] buttons = new Button[0];
        bool enabled = true;

        public PresenceManager(String AppID, bool enabled = true)
        {
            this.enabled = enabled;
            if (!this.enabled) return;
            client = new DiscordRpcClient(AppID);
            client.Initialize();
        }

        public void SetActivity(String activity)
        {
            if (!enabled) return;
            RichPresence r = new RichPresence();
            r.Details = activity;
            r.Buttons = buttons;
            Assets a = new Assets();
            a.LargeImageKey = "logo";
            r.Assets = a;
            client.SetPresence(r);
        }

        public void SetOneButton(String label, String url)
        {
            if (!enabled) return;
            buttons = new Button[] { new Button() };
            buttons[0].Label = label;
            buttons[0].Url = url;
        }

        public void AddButton(String label, String url)
        {
            if (!enabled) return;
            Button[] tmp = new Button[buttons.Length + 1];
            int i = 0;
            foreach(Button b in buttons)
            {
                tmp[i] = b;
                i++;
            }
            buttons = tmp;
            buttons[buttons.Length - 1] = new Button();
            buttons[buttons.Length - 1].Label = label;
            buttons[buttons.Length - 1].Url = url;
        }

        public void RemoveButtons()
        {
            if (!enabled) return;
            buttons = new Button[0];
        }
    }

}