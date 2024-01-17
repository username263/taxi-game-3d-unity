using System;
using System.Collections.Generic;

namespace TaxiGame3D
{
    public class UserResponse
    {
        public string Nickname
        {
            get;
            set;
        }
        public long Coin
        {
            get;
            set;
        }
        public List<string> Cars
        {
            get;
            set;
        }
        public string CurrentCar
        {
            get;
            set;
        }
        public int CurrentStage
        {
            get;
            set;
        }
        public Dictionary<string, string> DailyCarRewards
        {
            get;
            set;
        }
        public DateTime DailyRewardedAt
        {
            get;
            set;
        }
        public short NumberOfAttendance
        {
            get;
            set;
        }
        public DateTime CoinCollectedAt
        {
            get;
            set;
        }
        public List<string> RouletteCarRewards
        {
            get;
            set;
        }
        public DateTime RouletteSpunAt
        {
            get;
            set;
        }
    }
}