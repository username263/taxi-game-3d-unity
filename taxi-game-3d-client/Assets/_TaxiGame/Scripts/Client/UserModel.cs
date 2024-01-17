using System;
using System.Collections.Generic;

namespace TaxiGame3D
{
    public class UserModel
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
        public List<CarTemplate> Cars
        {
            get;
            set;
        }
        public CarTemplate CurrentCar
        {
            get;
            set;
        }
        public StageTemplate CurrentStage
        {
            get;
            set;
        }
        public Dictionary<int, CarTemplate> DailyCarRewards
        {
            get;
            set;
        }
        public DateTime DailyRewardedAtUtc
        {
            get;
            set;
        }
        public short NumberOfAttendance
        {
            get;
            set;
        }
        public DateTime CoinCollectedAtUtc
        {
            get;
            set;
        }
        public List<CarTemplate> RouletteCarRewards
        {
            get;
            set;
        }
        public DateTime RouletteSpunAtUtc
        {
            get;
            set;
        }
    }
}