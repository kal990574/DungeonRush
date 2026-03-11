using System.Collections.Generic;
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
    }
}
