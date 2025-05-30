﻿using DSharpPlus;
using LathBotBack.Base;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System.Threading;

namespace LathBotBack.Services
{
    public class GoodGuysService : BaseService
    {
        #region Singleton
        private static GoodGuysService instance;
        private static readonly Lock padlock = new();
        public static GoodGuysService Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new GoodGuysService();
                    return instance;
                }
            }
        }
        #endregion

        public int GoodGuysReactionCount
        {
            get
            {
                VariableRepository repo = new(ReadConfig.Config.ConnectionString);
                bool result = repo.Read(1, out Variable entity);
                if (result)
                {
                    return int.Parse(entity.Value);
                }
                return this._goodGuysReactionCount;
            }
            set
            {
                this._goodGuysReactionCount = value;
            }
        }
        private int _goodGuysReactionCount = 4;

        public bool GoodGuysStatus
        {
            get
            {
                VariableRepository repo = new(ReadConfig.Config.ConnectionString);
                bool result = repo.Read(4, out Variable entity);
                if (result)
                {
                    return bool.Parse(entity.Value);
                }
                return this._goodGuysStatus;
            }
            set
            {
                this._goodGuysStatus = value;
            }
        }
        private bool _goodGuysStatus = true;
        public override void Init(DiscordClient client) { }
    }
}
