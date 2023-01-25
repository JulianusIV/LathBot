//using DSharpPlus.Entities;
//using LathBotBack.Config;
//using LathBotBack.Models;
//using LathBotBack.Repos;
//using LathBotBack.Services;
//using System;
//using System.Collections.Generic;

//namespace LathBotFront.Commands.Events
//{
//    public class EventParams
//    {
//        #region Singleton
//        private static EventParams instance = null;
//        private static readonly object padlock = new();
//        public static EventParams Instance
//        {
//            get
//            {
//                lock (padlock)
//                {
//                    instance ??= new EventParams();
//                    return instance;
//                }
//            }
//        }
//        #endregion

//        public bool? IsInEventMode
//        {
//            get
//            {
//                VariableRepository repo = new(ReadConfig.Config.ConnectionString);
//                if (repo.Read(5, out Variable variable))
//                {
//                    _isInEventMode = Convert.ToBoolean(variable.Value);
//                    return _isInEventMode;
//                }
//                else
//                {
//                    SystemService.Instance.Logger.Log("Failed to read variable from database!");
//                    return null;
//                }
//            }
//            set
//            {
//                if (value is null)
//                    return;
//                VariableRepository repo = new(ReadConfig.Config.ConnectionString);
//                if (repo.Read(5, out Variable variable))
//                {
//                    variable.Value = value.ToString();
//                    if (repo.Update(variable))
//                    {
//                        _isInEventMode = value;
//                        return;
//                    }
//                }
//                SystemService.Instance.Logger.Log("Error reading or updating a variable in the database!");
//            }
//        }
//        public ulong SubmissionsChannelId = 898705551792439377;
//        public Dictionary<ulong, DiscordMessage> Submissions = new();

//        private bool? _isInEventMode;
//    }
//}
