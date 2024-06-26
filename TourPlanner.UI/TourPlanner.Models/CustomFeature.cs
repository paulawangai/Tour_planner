using System;
using System.Collections.Generic;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;
using NetTopologySuite.Geometries;

namespace Tour_planner.TourPlanner.UI.TourPlanner.Models
{
    public class CustomFeature : IFeature
    {
        public Geometry Geometry { get; set; }
        public MRect? Extent => Geometry?.EnvelopeInternal.ToMRect();
        public IDictionary<IStyle, object> RenderedGeometry { get; set; } = new Dictionary<IStyle, object>();

        // Change Fields to implement IEnumerable<string>
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public IEnumerable<string> Fields => _fields.Keys;

        public ICollection<IStyle> Styles { get; } = new List<IStyle>();

        public object this[string key]
        {
            get => _fields[key];
            set => _fields[key] = value;
        }

        public CustomFeature()
        {
        }

        public CustomFeature(Geometry geometry)
        {
            Geometry = geometry;
        }

        public void Dispose()
        {
            // Implement IDisposable if necessary
        }

        public void CoordinateVisitor(Action<double, double, CoordinateSetter> visit)
        {
            if (Geometry == null) return;

            foreach (var coordinate in Geometry.Coordinates)
            {
                visit(coordinate.X, coordinate.Y, (x, y) => {
                    coordinate.X = x;
                    coordinate.Y = y;
                });
            }
        }
    }
}
