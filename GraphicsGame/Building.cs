using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsGame
{
    class Building
    {
        private int BuildingWidth;
        private int BuildingHeight;


        public int GetBuildingWidth
        {
            get
            {
                return BuildingWidth;
            }
            set
            {
                BuildingWidth = value;
            }
        }

        public int GetBuildingHeight
        {
            get
            {
                return BuildingHeight;
            }
            set
            {
                BuildingHeight = value;
            }
        }
    }
}
