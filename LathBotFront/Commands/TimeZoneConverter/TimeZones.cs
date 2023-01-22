using DSharpPlus.Entities;

namespace LathBotBack.Commands.TimeZoneConverter
{
    public class TimeZones
    {
        public static DiscordEmbed UTC(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]);
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusOne(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 1;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusTwo(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 2;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusThree(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 3;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusFour(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 4;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusFive(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 5;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusSix(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 6;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusSeven(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 7;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusEight(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 8;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusNine(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 9;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusTen(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 10;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusEleven(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 11;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCPlusTwelve(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 12;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusTwelve(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 12;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusEleven(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 11;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusTen(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 10;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusNine(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 9;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusEight(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 8;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusSeven(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 7;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusSix(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 6;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusFive(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 5;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusFour(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 4;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusThree(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) + 3;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusTwo(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 2;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }

        public static DiscordEmbed UTCMinusOne(string time)
        {
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]) - 1;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Brown,
                Title = "UTC±0/WEZ/GMT/AZODT/IST/EGST/SLT:",
                Description = ((hours > 24) ? (hours - 24) : hours) + ":" + timeParts[1]
            };

            #region UTCPlus
            if (hours + 1 > 24)
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+1/MEZ/CET/WAT/WESZ/WEST/BST/IST:", (hours + 1) + ":" + timeParts[1]);
            if (hours + 2 > 24)
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+2/EET/OEZ/CEST/CEDT/MESZ/CAT/SAST/Egypt Standart Time/Israel Standart Time/USZ1/WAST:", (hours + 2) + ":" + timeParts[1]);
            if (hours + 3 > 24)
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+3/AST/EAT/EEST/IDT/MSK/SYST:", (hours + 3) + ":" + timeParts[1]);
            if (hours + 4 > 24)
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+4/AMST/AZT/GET/GST/ICT/MUT/RET/SCT:", (hours + 4) + ":" + timeParts[1]);
            if (hours + 5 > 24)
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+5/CAST/TFT/HMT/MVT/PKT/TJT/TMT/UZT/WKST:", (hours + 5) + ":" + timeParts[1]);
            if (hours + 6 > 24)
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+6/BDT/BTT/EKST/BIOT/MAWT/OMSTVOST:", (hours + 6) + ":" + timeParts[1]);
            if (hours + 7 > 24)
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+7/KRAT/NOVT/ICT/WIB/DAVT/KOVT/CXT/BST:", (hours + 7) + ":" + timeParts[1]);
            if (hours + 8 > 24)
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+8/PST/CIST/AKDT:", (hours + 8) + ":" + timeParts[1]);
            if (hours + 9 > 24)
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+9/WIT/JST/KST:", (hours + 9) + ":" + timeParts[1]);
            if (hours + 10 > 24)
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+10/DTAT/AEST/TRUT/PRT/VLAT/ChST/YAPT:", (hours + 10) + ":" + timeParts[1]);
            if (hours + 11 > 24)
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+11/KOST/SRET/NCT/PONT/SBT/VUT/NFT/AEDT/LHDT", (hours + 11) + ":" + timeParts[1]);
            if (hours + 12 > 24)
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12 - 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC+12/WFT/TVT/FJT/GILT/NRT/MHT/PETT/NZST", (hours + 12) + ":" + timeParts[1]);
            #endregion
            #region UTCMinus
            if (hours - 12 < 0)
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-12/IDLW/BIT", (hours - 12) + ":" + timeParts[1]);
            if (hours - 11 < 0)
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-11/WST/WSST/NUT", (hours - 11) + ":" + timeParts[1]);
            if (hours - 10 < 0)
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-10/HAST/TAHT", (hours - 10) + ":" + timeParts[1]);
            if (hours - 9 < 0)
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-9/HADT/GAMT/AKST/YST", (hours - 9) + ":" + timeParts[1]);
            if (hours - 8 < 0)
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-8/PST/CIST/AKDT", (hours - 8) + ":" + timeParts[1]);
            if (hours - 7 < 0)
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-7/MST/PDT", (hours - 7) + ":" + timeParts[1]);
            if (hours - 6 < 0)
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-6/CST/GALT/PIT/MDT", (hours - 6) + ":" + timeParts[1]);
            if (hours - 5 < 0)
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-5/EAST/EST/ECT/ACT/COT/PET/CDT", (hours - 5) + ":" + timeParts[1]);
            if (hours - 4 < 0)
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-4/BOT/FKST/AST/PYT/BWST/SLT/GYT/JFST/EDT", (hours - 4) + ":" + timeParts[1]);
            if (hours - 3 < 0)
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-3/ART/BRT/CLST/WGT/GFT/PMST/ROTT/SRT/UYT/ADT/BWDT/FKDT/JFDT/PYST/SLST", (hours - 3) + ":" + timeParts[1]);
            if (hours - 2 < 0)
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-2/GST/BEST/PNDT/CGST/UYST/ARDT/BRST", (hours - 2) + ":" + timeParts[1]);
            if (hours - 1 < 0)
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1 + 24) + ":" + timeParts[1]);
            else
                embedBuilder.AddField("UTC-1/AZOST/CVT", (hours - 1) + ":" + timeParts[1]);
            #endregion

            return embedBuilder.Build();
        }
    }
}
