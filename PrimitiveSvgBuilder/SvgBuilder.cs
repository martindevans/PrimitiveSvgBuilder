using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;

namespace PrimitiveSvgBuilder
{
    public class SvgBuilder
    {
        public float Scale { get; }

        private Vector2 _min = new Vector2(float.MaxValue);
        private Vector2 _max = new Vector2(float.MinValue);

        private readonly List<string> _parts = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale">All positions will be multiplied by this value</param>
        public SvgBuilder(float scale = 1)
        {
            Scale = scale;
        }

        public SvgBuilder Outline(IReadOnlyList<Vector2> shape, string stroke = "blue", string fill = "none", bool closed = true)
        {
            _parts.Add(ToSvgPath(shape, Scale, stroke, fill, closed));

            UpdateMinMax(shape);

            return this;
        }

        public SvgBuilder Circle(Vector2 center, float radius, string fill = "blue")
        {
            _parts.Add($"<circle cx=\"{center.X * Scale}\" cy=\"{center.Y * Scale}\" r=\"{radius * Scale}\" fill=\"{fill}\"></circle>");

            UpdateMinMax(center - new Vector2(radius));
            UpdateMinMax(center + new Vector2(radius));

            return this;
        }

        public SvgBuilder Line(Vector2 start, Vector2 end, float width, string stroke)
        {
            _parts.Add($"<line x1=\"{start.X * Scale}\" y1=\"{start.Y * Scale}\" x2=\"{end.X * Scale}\" y2=\"{end.Y * Scale}\" stroke=\"{stroke}\" stroke-width=\"{width}\" />");

            UpdateMinMax(start, end);

            return this;
        }

        private static string ToSvgPath(IReadOnlyList<Vector2> shape, float scale, string stroke, string fill, bool closed)
        {
            var builder = new StringBuilder($"<path fill=\"{fill}\" stroke=\"{stroke}\" d=\"");

            builder.Append($"M {shape[0].X * scale} {shape[0].Y * scale} ");
            for (var i = 1; i < shape.Count; i++)
                builder.Append($"L {shape[i].X * scale} {shape[i].Y * scale} ");

            if (closed)
                builder.Append("Z");
            builder.Append("\"></path>");

            return builder.ToString();
        }

        public SvgBuilder Text(string text, Vector2 position, string color = "green", string fontFamily = "verdana", int fontSize = 25)
        {
            _parts.Add($"<text x=\"{position.X * Scale}\" y=\"{position.Y * Scale}\" font-family=\"{fontFamily}\" font-size=\"{fontSize}\">{WebUtility.HtmlEncode(text)}</text>");

            return this;
        }

        private void UpdateMinMax(Vector2 v)
        {
            _min = Vector2.Min(_min, v);
            _max = Vector2.Max(_max, v);
        }

        private void UpdateMinMax(params Vector2[] vs)
        {
            UpdateMinMax((IEnumerable<Vector2>)vs);
        }

        private void UpdateMinMax(IEnumerable<Vector2> vs)
        {
            _min = Vector2.Min(_min, vs.Aggregate(Vector2.Min));
            _max = Vector2.Max(_max, vs.Aggregate(Vector2.Max));
        }

        public override string ToString()
        {
            var extent = (_max - _min) * Scale;

            return $"<svg width=\"{extent.X}\" height=\"{extent.Y}\"><g transform=\"translate({-_min.X * Scale}, {-_min.Y * Scale})\">{string.Join("", _parts)}</g></svg>";
        }
    }
}
