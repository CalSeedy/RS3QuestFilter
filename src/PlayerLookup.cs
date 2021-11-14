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

            Player p = await GetPlayer(player);

            cached = p;
            lastChecked = DateTime.UtcNow;
            return p;
        }

        private static async Task<Player> GetPlayer(string player)
        {
            string safePlayer = System.Web.HttpUtility.UrlEncode(player);
            string response;
            string baseuri = $"https://secure.runescape.com/m=hiscore/index_lite.ws?player={safePlayer}";
            string ironmanuri = $"https://secure.runescape.com/m=hiscore_ironman/index_lite.ws?player={safePlayer}";
            string hardcoreuri = $"https://secure.runescape.com/m=hiscore_hardcore_ironman/index_lite.ws?player={safePlayer}";

            PlayerFlags flags = PlayerFlags.Standard;
            List<int> levels;
            try
            {
                response = await client.GetStringAsync(hardcoreuri);
                string ironresponse = await client.GetStringAsync(ironmanuri);
                (List<int> ranks, List<int> lvls, List<int> xps) = ParsePage(response);
                (List<int> ranks_iron, List<int> lvls_iron, List<int> xps_iron) = ParsePage(ironresponse);
                flags |= PlayerFlags.Ironman;
                bool isHC = true;
                for (int i = 0; i < xps.Count; i++)
                {
                    if (xps[i] != xps_iron[i])
                    {
                        isHC = false;
                        break;
                    }
                }
                if (isHC)
                    flags |= PlayerFlags.Hardcore;
                levels = lvls_iron;
            }
            catch (HttpRequestException)
            {
                try
                {
                    response = await client.GetStringAsync(ironmanuri);
                    (List<int> ranks, List<int> lvls, List<int> xps) = ParsePage(response);
                    levels = lvls;
                    flags |= PlayerFlags.Ironman;
                }
                catch (HttpRequestException)
                {
                    try
                    {
                        response = await client.GetStringAsync(baseuri);
                        (List<int> ranks, List<int> lvls, List<int> xps) = ParsePage(response);
                        levels = lvls;
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine($"\nHTTP Exception caught\nMessage:\n\t{ex.Message}");
                        throw new HttpRequestException($"Player data for \"{player}\" could not be found. Check your spelling, or their privacy settings are blocking.");
                    }
                }
            }

            Player p = new();
            p.Name = player;
            p.Flags = flags;

            for (int i = 0; i < levels.Count; i++)
            {
                p.Skills[skills[i+1]].Level = levels[i];
            }

            return p;     
        }

        private static (List<int>, List<int>, List<int>) ParsePage(string page)
        {

            List<int> ranks = new(), levels = new(), xps = new();
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
            for (int i = 1; i <= NO_OF_SKILLS; i++)
            {
                int x = Convert.ToInt32(splits[3 * i + 0]);
                int y = Convert.ToInt32(splits[3 * i + 1]);
                int z = Convert.ToInt32(splits[3 * i + 2]);
                ranks.Add(x);
                levels.Add(y);
                xps.Add(z);
            }
            return (ranks, levels, xps);
        }
    }
}
