using System;
using System.Globalization;

namespace Asv.Tools
{

    /// <summary>
    /// WGS 84 (EPSG:4326)
    /// https://en.wikipedia.org/wiki/World_Geodetic_System
    /// </summary>

    public struct GeoPoint : IEquatable<GeoPoint>
    {
        // West-East
        public readonly double Longitude;
        // Nord-South
        public readonly double Latitude;
        public readonly double? Altitude;

        public double Lat => Latitude;
        public double Lng => Longitude;

        public static GeoPoint FromLatLon(double lat, double lon) { return new GeoPoint(lat, lon); }
        public static GeoPoint NordPole => new GeoPoint(90.0, 0.0);

        public static GeoPoint Zero => new GeoPoint(0.0, 0.0);
        public static GeoPoint ZeroWithAlt => new GeoPoint(0.0, 0.0, 0.0);

        public GeoPoint(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = null;
        }
        public GeoPoint(double latitude, double longitude, double altitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = altitude;
        }

        public void Deconstruct(out double lat, out double lon)
        {
            lat = this.Latitude;
            lon = this.Longitude;
        }


        public static GeoPoint operator +(GeoPoint x, GeoPoint y)
        {
            if (x.Altitude.HasValue && y.Altitude.HasValue)
            {
                return new GeoPoint(x.Latitude + y.Latitude, x.Longitude + y.Longitude,x.Altitude.Value + y.Altitude.Value);
            }
            else
            {
                return new GeoPoint(x.Latitude + y.Latitude, x.Longitude + y.Longitude);
            }
            
        }

        public static GeoPoint operator -(GeoPoint x, GeoPoint y)
        {
            if (x.Altitude.HasValue && y.Altitude.HasValue)
            {
                return new GeoPoint(x.Latitude - y.Latitude, x.Longitude - y.Longitude, x.Altitude.Value - y.Altitude.Value);
            }
            else
            {
                return new GeoPoint(x.Latitude - y.Latitude, x.Longitude - y.Longitude);
            }
                
        }

        // public void Offset(GeoPoint pos)
        // {
        //     Offset(pos.Latitude, pos.Longitude);
        // }
        //
        // public void Offset(double lat, double lng)
        // {
        //     Longitude += lng;
        //     Latitude -= lat;
        // }

        // public static bool operator <(GeoPoint p1, GeoPoint p2)
        // {
        //     return p1.Latitude < p2.Latitude || (p1.Latitude.EqualsWithTolerance(p2.Latitude) && p1.Longitude < p2.Longitude);
        // }
        //
        // public static bool operator >(GeoPoint p1, GeoPoint p2)
        // {
        //     return p1.Latitude > p2.Latitude || (p1.Latitude.EqualsWithTolerance(p2.Latitude) && p1.Longitude > p2.Longitude);
        // }
        //
        // public int CompareTo(GeoPoint other)
        // {
        //     return (this > other) ? -1 : ((this < other) ? 1 : 0);
        // }

       

        public override string ToString()
        {
            if (Altitude.HasValue)
            {
                return $"Lat:{Latitude:F7},Lon:{Longitude:F7},Alt:{Altitude:F1} m"; 
            }
            return $"Lat:{Latitude:F7},Lon:{Longitude:F7}";
        }

        public static GeoPoint Parse(string src)
        {
            var source = src.Split(CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator);
            switch (source.Length)
            {
                case 2:
                    return new GeoPoint(double.Parse(source[0]),double.Parse(source[1]));
                case 3:
                    return new GeoPoint(double.Parse(source[0]), double.Parse(source[1]), double.Parse(source[2]));
                default:
                    throw new Exception("Too many arguments for GeoPoint");
            }
        }

        public bool Equals(GeoPoint other)
        {
            return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude) && Nullable.Equals(Altitude, other.Altitude);
        }

        public override bool Equals(object obj)
        {
            return obj is GeoPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            if (Altitude.HasValue)
            {
                return Longitude.GetHashCode() ^ Latitude.GetHashCode() ^ Altitude.Value.GetHashCode();
            }
            else
            {
                return Longitude.GetHashCode() ^ Latitude.GetHashCode();
            }
            
        }
    }
}
