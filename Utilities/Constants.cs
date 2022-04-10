using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QCVault.Utilities
{
    public static class Constants
    {
        public static Dictionary<string, string> Categories = new Dictionary<string, string>()
        {
            { "coteries", "Groups of people self-organized by a shared label; sometimes political, sometimes historical, sometimes based on faith or things adjacent to it. \"Liberals\" and \"Conservatives\", \"Red\" and \"Blue\", any time people decide to love or hate based solely on self-identification. If we must talk about groups in terms of individuals, this is where it goes." },
            { "culture", "The beliefs, faiths, identity, and tendencies of people as a group, as described by trends rather than self-labeling. When we divide people based on action and statistics." },
            { "personal", "People interacting directly and individually, not en masse, not based on label. Romance, management, and the workplace, in no particular order." },
            { "knowledge", "The process of becoming better at things. Intelligence and education, science and engineering, rationality and logic." },
            { "economics", "Supply and demand. Game theory. People or organizations making decisions based on the hand of the market, regardless of what form that hand takes. Money, often, but not always." },
            { "civilization", "What keeps us together? What does it all mean? History, law, morality, and philosophy." },
            { "moloch", "Everything getting worse, despite the intentions of everyone. Records of inexorable erosion. A shrine for the things we, as a species, must fight, or be overwhelmed by." },
            { "media", "Writing, movies, music, and art. Fiction and non-fiction. Goblins and journalism." },
            { "flux", "Progress, positive and negative, perhaps intentional, perhaps accidental. The change of things over time." },
        };
    }
}
