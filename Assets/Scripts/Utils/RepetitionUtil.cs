using System.Collections.Generic;

namespace Utils
{
    public static class RepetitionUtil
    {
        public static int GetRepetitionCount(List<string> strings)
        {
            if (strings.Count > 1)
            {
                var last = strings[^1];
                var lastIndexOf = strings.GetRange(0, strings.Count - 1).LastIndexOf(last);

                if (lastIndexOf != -1 && lastIndexOf > 0)
                {
                    for (var i = 1; i < strings.Count - lastIndexOf - 1; i++)
                    {
                        if (!strings[strings.Count - 1 - i].Equals(strings[lastIndexOf - i]))
                        {
                            return -1;
                        }
                    }

                    return strings.Count - lastIndexOf - 1;
                }
            }

            return -1;
        }
    }
}