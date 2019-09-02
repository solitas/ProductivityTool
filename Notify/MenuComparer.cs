using System;
using System.Collections.Generic;
using ProductivityTool.Notify.ViewModel;

namespace ProductivityTool.Notify
{
    public class MenuComparer : IComparer<INotifyMenu>
    {
        public int Compare(INotifyMenu app1, INotifyMenu app2) // IComparer에서 약속한 기능 구현
        {
            if (app1 == null || app2 == null)
            {
                throw new ApplicationException("Member 개체가 아닌 인자가 있습니다.");
            }

            if (app1 is ConfigMenu)
            {
                if (app2 is ExitM)
                {
                    return -1;
                }

                return 1;
            }

            if (app1 is ExitM)
            {
                if (app2 is ConfigMenu)
                {
                    return 1;
                }

                return -1;
            }

            return string.CompareOrdinal(app1.Label, app2.Label);
        }
    }
}