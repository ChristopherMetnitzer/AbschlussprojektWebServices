using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using Abschlussprojekt.Models;

namespace Abschlussprojekt.Formatters
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type? type)
        {
            if (type == null) return false;

            // Student oder IEnumerable<Student>
            if (typeof(Student).IsAssignableFrom(type)) return true;
            if (typeof(IEnumerable<Student>).IsAssignableFrom(type)) return true;

            return false;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var sb = new StringBuilder();

            // CSV Header
            sb.AppendLine("id,name,matrikelnummer,semester");

            if (context.Object is Student s)
            {
                sb.AppendLine(ToCsvLine(s));
            }
            else if (context.Object is IEnumerable<Student> list)
            {
                foreach (var st in list)
                    sb.AppendLine(ToCsvLine(st));
            }

            await response.WriteAsync(sb.ToString(), selectedEncoding);
        }

        private static string ToCsvLine(Student s)
        {
            return $"{s.Id},{Escape(s.Name)},{Escape(s.Matrikelnummer)},{s.Semester}";
        }

        private static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            // Escape für CSV (Komma/Quotes/Newlines)
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
