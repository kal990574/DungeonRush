using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DungeonRush.Stats.Repository.Csv
{
    public static class CsvParser
    {
        private enum State
        {
            FieldStart,
            UnquotedField,
            QuotedField,
            QuoteInQuoted
        }

        // RFC 4180 준수, BOM 처리, 상태 머신 기반 파서.
        public static List<string[]> Parse(string csvText)
        {
            if (string.IsNullOrEmpty(csvText))
            {
                return new List<string[]>();
            }

            // BOM 제거.
            if (csvText.Length > 0 && csvText[0] == '\uFEFF')
            {
                csvText = csvText.Substring(1);
            }

            var result = new List<string[]>();
            var fields = new List<string>();
            var field = new StringBuilder();
            var state = State.FieldStart;

            for (int i = 0; i < csvText.Length; i++)
            {
                char c = csvText[i];

                switch (state)
                {
                    case State.FieldStart:
                        if (c == '"')
                        {
                            state = State.QuotedField;
                        }
                        else if (c == ',')
                        {
                            fields.Add(field.ToString());
                            field.Clear();
                        }
                        else if (c == '\r')
                        {
                            // CR 무시, LF에서 처리.
                        }
                        else if (c == '\n')
                        {
                            fields.Add(field.ToString());
                            if (fields.Count > 0)
                            {
                                result.Add(fields.ToArray());
                            }

                            fields.Clear();
                            field.Clear();
                        }
                        else
                        {
                            field.Append(c);
                            state = State.UnquotedField;
                        }
                        break;

                    case State.UnquotedField:
                        if (c == ',')
                        {
                            fields.Add(field.ToString());
                            field.Clear();
                            state = State.FieldStart;
                        }
                        else if (c == '\r')
                        {
                            // CR 무시.
                        }
                        else if (c == '\n')
                        {
                            fields.Add(field.ToString());
                            result.Add(fields.ToArray());
                            fields.Clear();
                            field.Clear();
                            state = State.FieldStart;
                        }
                        else
                        {
                            field.Append(c);
                        }
                        break;

                    case State.QuotedField:
                        if (c == '"')
                        {
                            state = State.QuoteInQuoted;
                        }
                        else
                        {
                            field.Append(c);
                        }
                        break;

                    case State.QuoteInQuoted:
                        if (c == '"')
                        {
                            // 이스케이프된 따옴표.
                            field.Append('"');
                            state = State.QuotedField;
                        }
                        else if (c == ',')
                        {
                            fields.Add(field.ToString());
                            field.Clear();
                            state = State.FieldStart;
                        }
                        else if (c == '\r')
                        {
                            // CR 무시.
                        }
                        else if (c == '\n')
                        {
                            fields.Add(field.ToString());
                            result.Add(fields.ToArray());
                            fields.Clear();
                            field.Clear();
                            state = State.FieldStart;
                        }
                        else
                        {
                            field.Append(c);
                            state = State.UnquotedField;
                        }
                        break;
                }
            }

            // 마지막 필드/행 처리.
            if (field.Length > 0 || fields.Count > 0)
            {
                fields.Add(field.ToString());
                result.Add(fields.ToArray());
            }

            return result;
        }

        // 헤더 배열에서 컬럼 이름으로 인덱스를 검색.
        public static int FindColumn(string[] headers, string name)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        // 안전한 필드 접근. 인덱스 범위 밖이면 빈 문자열 반환.
        public static string GetField(string[] cols, int index)
        {
            if (index >= 0 && index < cols.Length)
            {
                return cols[index].Trim();
            }

            return string.Empty;
        }

        public static float ParseFloat(string value)
        {
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }

            return 0f;
        }

        public static int ParseInt(string value, int defaultValue = 0)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }

            return defaultValue;
        }

        public static bool ParseBool(string value)
        {
            return value.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
        }

        public static string[] ParseStringArray(string value, char separator)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<string>();
            }

            string[] parts = value.Split(separator);
            var result = new List<string>();
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    result.Add(trimmed);
                }
            }

            return result.ToArray();
        }

        // 필수 컬럼 검증. 누락 시 경고 로그 반환.
        public static bool ValidateRequiredColumns(string[] headers, string context, params string[] required)
        {
            bool allFound = true;
            foreach (string name in required)
            {
                if (FindColumn(headers, name) < 0)
                {
                    UnityEngine.Debug.LogWarning($"[CsvParser] {context}: 필수 컬럼 '{name}' 누락.");
                    allFound = false;
                }
            }

            return allFound;
        }
    }
}
