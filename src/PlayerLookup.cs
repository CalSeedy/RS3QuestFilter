using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RS3QuestFilter.src
{
    public static class PlayerLookup
    {
        private static List<string> skills = new ();
        private static bool isInitialised = false;
        private static Player cached = null;
        private static DateTime lastChecked;
        private static readonly HttpClient client = new();
        private static int NO_OF_SKILLS = 28;

        public static void init()
        {
            if (!isInitialised)
            {
                skills.Add("Overall");
                skills.Add("Attack");
                skills.Add("Defence");
                skills.Add("Strength");
                skills.Add("Constitution");
                skills.Add("Ranged");
                skills.Add("Prayer");
                skills.Add("Magic");
                skills.Add("Cooking");
                skills.Add("Woodcutting");
                skills.Add("Fletching");
                skills.Add("Fishing");
                skills.Add("Firemaking");
                skills.Add("Crafting");
                skills.Add("Smithing");
                skills.Add("Mining");
                skills.Add("Herblore");
                skills.Add("Agility");
                skills.Add("Thieving");
                skills.Add("Slayer");
                skills.Add("Farming");
                skills.Add("Runecrafting");
                skills.Add("Hunter");
                skills.Add("Construction");
                skills.Add("Summoning");
                skills.Add("Dungeoneering");
                skills.Add("Divination");
                skills.Add("Invention");
                skills.Add("Archaeology");
            }                

            isInitialised = true;

        }

        public static async Task<Player> Lookup(string player)
        {
            if (cached != null)
                if (cached.Name.ToLower() == player.ToLower())
                    if (lastChecked != null)
                        if ((DateTime.UtcNow - lastChecked).TotalMinutes <= 10)
                            return cached;
            
            Player p =  ParsePage(await GetPage(player));
            p.Name = player;
            cached = p;
            lastChecked = DateTime.UtcNow;
            return p;
        }

        private static async Task<string> GetPage(string player)
        {
            try
            {
                string uri = $"https://secure.runescape.com/m=hiscore/index_lite.ws?player={System.Web.HttpUtility.UrlEncode(player)}";
                return await client.GetStringAsync(uri);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("\nHTTP Exception caught\nMessage:\n\t{0}", ex.Message);
                throw;
            }
        }

        private static Player ParsePage(string page)
        {
            string[] lines = page.Split('\n');
            List<string> splits = new();
            if (lines.Length > 0)
            {
                foreach (string l in lines)
                {
                    string[] split = l.Split(',');
                    foreach (string val in split)
                    {
                        splits.Add(val.Trim());
                    }
                }
            }
            Player p = new Player();
            for (int i = 1; i <= NO_OF_SKILLS; i++)
            {
                try 
                {
                    int x = Convert.ToInt32(splits[3 * i + 1]);
                    p.Skills[skills[i]].Level = x;
                }
                catch
                {
                    throw;
                }
            }
            return p;
        }
    }
}
