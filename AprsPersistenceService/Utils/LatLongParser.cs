using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprsPersistenceService.Utils
{
    public class LatLongParser
    {

        public static float ConvertLatitude(string latitude)
        {
            float retval = 0.0f;
            bool setNegative = false;
            latitude = latitude.TrimStart();

            string[] coords = latitude.Split(' ');

            if (latitude.Contains("S"))
            {
                setNegative = true;
            }

            if(coords.Length == 3)
            {
                int result = 0;
                bool success = int.TryParse(coords[1], out result);
                if (success)
                {
                    retval = (float)result;
                }

                float floatResult = 0.0f;
                bool floatSuccess = float.TryParse(coords[2], out floatResult);
                if (floatSuccess)
                {
                    retval += (floatResult / 60);
                }
            }

            if (setNegative)
            {
                retval = -retval;
            }

            return retval;
        }

        public static float ConvertLongitude(string longitude)
        {
            float retval = 0.0f;

            bool setNegative = false;

            longitude = longitude.TrimStart();

            string[] coords = longitude.Split(' ');

            if (longitude.Contains("W"))
            {
                setNegative = true;
            }

            if(coords.Length == 3)
            {
                int result = 0;
                bool success = int.TryParse(coords[1], out result);
                if(success)
                {
                    retval = (float)result;
                }

                float floatResult = 0.0f;
                bool floatSuccess = float.TryParse(coords[2], out floatResult);
                if (floatSuccess)
                {
                    retval += (floatResult / 60);
                }
            }

            if (setNegative)
            {
                retval = -retval;
            }

            return retval;
        }
    }
}
