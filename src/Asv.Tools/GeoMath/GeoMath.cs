using System;
using Asv.Tools;
using Geodesy;

namespace Asv.Mavlink
{
    public static class GeoMath
    {
        private static readonly GeodeticCalculator Calculator = new GeodeticCalculator(Ellipsoid.WGS84);

        /// <summary>
        /// Calculates the initial azimuth (the angle measured clockwise from
        /// true north) at a point from that point to a second point.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <returns>
        /// The initial azimuth of the first point to the second point.
        /// </returns>
        /// <example>
        /// The azimuth from 0,0 to 1,0 is 0 degrees. From 0,0 to 0,1 is 90
        /// degrees (due east). The range of the result is [-180, 180].
        /// </example>
        public static double Azimuth(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var measurement = Calculator.CalculateGeodeticMeasurement(
                new GlobalPosition(new GlobalCoordinates(new Angle(latitude1), new Angle(longitude1))),
                new GlobalPosition(new GlobalCoordinates(new Angle(latitude2), new Angle(longitude2))));

            return measurement.Azimuth.Degrees;
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points.
        /// </summary>
        /// <param name="point1">The location of the first point.</param>
        /// <param name="point2">The location of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        /// <exception cref="ArgumentNullException">point1 or point2 is null.</exception>
        public static double Distance(GeoPoint point1, GeoPoint point2)
        {
            if ((point1.Altitude == null) || (point2.Altitude == null))
            {
                return Distance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude);
            }
            return Distance(point1.Latitude, point1.Longitude, point1.Altitude.Value, point2.Latitude, point2.Longitude, point2.Altitude.Value);
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points on
        /// the Earth's surface.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static double Distance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var measurement = Calculator.CalculateGeodeticMeasurement(
                new GlobalPosition(new GlobalCoordinates(new Angle(latitude1), new Angle(longitude1))),
                new GlobalPosition(new GlobalCoordinates(new Angle(latitude2), new Angle(longitude2))));

            return measurement.EllipsoidalDistance;
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points in
        /// all three dimensions.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="altitude1">The altitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <param name="altitude2">The altitude of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static double Distance(double latitude1, double longitude1, double altitude1, double latitude2, double longitude2, double altitude2)
        {
            var measurement = Calculator.CalculateGeodeticMeasurement(
                new GlobalPosition(new GlobalCoordinates(new Angle(latitude1), new Angle(longitude1)), altitude1),
                new GlobalPosition(new GlobalCoordinates(new Angle(latitude2), new Angle(longitude2)), altitude2));
            
            return measurement.PointToPointDistance;
        }

        public static double Elevation(GeoPoint p1, GeoPoint p2)
        {
            return Elevation(p1.Latitude, p1.Longitude, p1.Altitude ?? 0, p2.Latitude, p2.Longitude, p2.Altitude ?? 0);
        }

        /// <summary>
        /// Calculates the angle from the horizontal plane between the two altitudes.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="altitude1">The altitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <param name="altitude2">The altitude of the second point.</param>
        /// <returns>The angle from the horizontal plane in degrees.</returns>
        /// <remarks>
        /// This is a naive implementation accurate only over short distances
        /// and does not account for surface curvature. To use this as the value
        /// of KML's tilt, add 90 degrees (since in KML a tilt of 0 is vertical).
        /// </remarks>
        /// <example>
        /// The returned angle from (37.00, -121.98, 600) to a point about 1778
        /// meters west, 400 meters below at (37.00, -122.00, 200) is -12.7 degrees.
        /// </example>
        public static double Elevation(double latitude1, double longitude1, double altitude1, double latitude2, double longitude2, double altitude2)
        {
            double surfaceDistance = Distance(latitude1, longitude1, latitude2, longitude2);
            return RadiansToDegrees(Math.Atan2(altitude2 - altitude1, surfaceDistance));
        }

        /// <summary>
        /// Calculates the absolute distance between the ground point and the
        /// point directly under the end of the specified vector.
        /// </summary>
        /// <param name="range">The distance in meters.</param>
        /// <param name="elevation">
        /// The angle in degrees from the horizontal plane.
        /// </param>
        /// <returns>The absolute ground distance in meters.</returns>
        public static double GroundDistance(double range, double elevation)
        {
            return Math.Abs(Math.Cos(DegreesToRadians(elevation)) * range);
        }

        /// <summary>
        /// Высота точки, при заданном угле подъема и удалении
        /// </summary>
        /// <param name="range">The distance in meters.</param>
        /// <param name="elevation">
        /// The angle in degrees from the horizontal plane.
        /// </param>
        /// <returns>The absolute height in meters.</returns>
        public static double Height(double range, double elevation)
        {
            return Math.Abs(Math.Sin(DegreesToRadians(elevation)) * range);
        }

        /// <summary>
        /// Высота точки, при заданном угле подъема и удалении(по земле)
        /// </summary>
        /// <param name="groundRange">The distance in meters on the ground.</param>
        /// <param name="elevation">
        /// The angle in degrees from the horizontal plane.
        /// </param>
        /// <returns>The absolute height in meters.</returns>
        public static double HeightFromGroundRange(double groundRange, double elevation)
        {
            return Math.Abs(Math.Tan(DegreesToRadians(elevation)) * groundRange);
        }

        /// <summary>
        /// Удаление по земле, при заданном угле подъема и высоте
        /// </summary>
        /// <param name="height">The height in meters.</param>
        /// <param name="elevation">
        /// The angle in degrees from the horizontal plane.
        /// </param>
        /// <returns>The ground range in meters.</returns>
        public static double GroundRangeFromHeight(double height, double elevation)
        {
            return Math.Abs(height / Math.Tan(DegreesToRadians(elevation)));
        }

        /// <summary>
        /// Находит точку на заданной линии, являющуюся пересечением перпендикуляра опущенного из заданной точки к прямой (высота не учитывается)
        /// </summary>
        public static GeoPoint IntersectionLineAndPerpendicularFromPoint(GeoPoint lineX, GeoPoint lineY, GeoPoint p)
        {
            var azimuth = DegreesToRadians(lineX.Azimuth(lineY) - lineX.Azimuth(p));
            var d = Distance(lineX, p);
            var h = Math.Abs(d * Math.Cos(azimuth));
            return lineX.RadialPoint(h, lineX.Azimuth(lineY));
        }

        /// <summary>
        /// Находит точку, являющуюся пересечением перпендикуляра опущенного от заданной точки к прямой, проходящей на заданном углу (высота не учитывается)
        /// </summary>
        public static GeoPoint IntersectionLineAndPerpendicularFromPoint(GeoPoint lineX, GeoPoint lineY, double alpha)
        {
            var azimuth = lineX.Azimuth(lineY) + alpha;
            var b = Distance(lineX, lineY);
            var c = Math.Abs(b / Math.Cos(DegreesToRadians(alpha)));
            return lineX.RadialPoint(c, azimuth);
        }

        /// <summary>
        /// Находит кратчайшее расстояние (длина перпендикуляра) между точкой и заданной линией  (высота не учитывается)
        /// </summary>
        public static double PerpendicularLength(GeoPoint lineX, GeoPoint lineY, GeoPoint p)
        {
            var azimuth = lineX.Azimuth(lineY) - lineX.Azimuth(p);
            var d = Distance(lineX, p);
            return Math.Abs(d * Math.Sin(azimuth * Math.PI / 180));
        }

        /// <summary>
        /// Calculates a point at the specified distance along a radial from a
        /// center point.
        /// </summary>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="radial">
        /// The radial in degrees, measures clockwise from north.
        /// </param>
        /// <returns>
        /// A <see cref="GeoPoint"/> containing the Latitude and Longitude of the
        /// calculated point.
        /// </returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static GeoPoint RadialPoint(double latitude, double longitude, double distance, double radial)
        {
            radial = !double.IsNaN(radial) ? radial : 0;
            var coordinates = Calculator.CalculateEndingGlobalCoordinates(
                new GlobalCoordinates(new Angle(latitude), new Angle(longitude)), new Angle(radial), distance);

            return new GeoPoint(coordinates.Latitude.Degrees, coordinates.Longitude.Degrees);
        }

        /// <summary>Converts the specified value in radians to degrees.</summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The specified angle converted to degrees.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
        /// <summary>Converts the specified value in degrees to radians.</summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The specified angle converted to radians.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
