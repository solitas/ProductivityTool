using System;
using System.Collections.Generic;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify
{
    public class MatchedApplicationComparer : IComparer<MatchedApplication>
    {
        public int Compare(MatchedApplication app1, MatchedApplication app2) // IComparer에서 약속한 기능 구현
        {
            if (app1 == null || app2 == null)
            {
                throw new ApplicationException("Member 개체가 아닌 인자가 있습니다.");
            }

            if (app1.Header == "Configuration")
            {
                if (app2.Header == "Exit")
                {
                    return -1;
                }

                return 1;
            }

            if (app1.Header == "Exit")
            {
                if (app2.Header == "Configuration")
                {
                    return 1;
                }

                return -1;
            }

            return string.CompareOrdinal(app1.Header, app2.Header);
        }
    }
}