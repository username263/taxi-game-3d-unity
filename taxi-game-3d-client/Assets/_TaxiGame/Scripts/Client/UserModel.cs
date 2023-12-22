using System.Collections.Generic;

namespace TaxiGame3D
{
    public class UserModel
    {
        public string Nickname { get; set; }
        public long Coin { get; set; }
        public List<CarTemplate> Cars { get; set; }
        public CarTemplate CurrentCar { get; set; }
        public StageTemplate CurrentStage { get; set; }
    }
}