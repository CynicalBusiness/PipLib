namespace PipLib
{
    public static class Util
    {
        public enum TemperatureUnit
        {
            KELVIN,
            CELSIUS,
            FARENHEIT
        }

        public const float DELTA_KC = 273.15f;
        public const float DELTA_CF = 32f;
        public const float MULTI_CF = 9f / 5f;
        public static float ConvertTemperatureToKelvin(float temp, TemperatureUnit unit)
        {
            switch (unit)
            {
                case TemperatureUnit.KELVIN:
                default:
                    return temp;
                case TemperatureUnit.CELSIUS:
                    return temp - DELTA_KC;
                case TemperatureUnit.FARENHEIT:
                    return (temp - DELTA_KC) * MULTI_CF + DELTA_CF;
            }
        }
    }
}
